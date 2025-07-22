using Core.Result;
using MediatR;
using Microsoft.Extensions.Logging;
using Payments.Domain.Entities;
using Payments.Domain.Repositories;
using Payments.Domain.Interfaces;
using Core.Interfaces;
using Shared.Contracts.Commands;
using Shared.Contracts.DTOs.Blockchain;
using Shared.Contracts.Queries;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace Payments.Application.Commands.ProcessPayment;

public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, Result<Payment>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentMethodRepository _paymentMethodRepository;
    private readonly IPaymentStatusRepository _paymentStatusRepository;
    private readonly IMediator _mediator;
    private readonly ILogger<ProcessPaymentCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderPaymentBlockchainPublisher _blockchainPublisher;

    public ProcessPaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IPaymentMethodRepository paymentMethodRepository,
        IPaymentStatusRepository paymentStatusRepository,
        IMediator mediator,
        ILogger<ProcessPaymentCommandHandler> logger,
        IUnitOfWork unitOfWork,
        IOrderPaymentBlockchainPublisher blockchainPublisher)
    {
        _paymentRepository = paymentRepository;
        _paymentMethodRepository = paymentMethodRepository;
        _paymentStatusRepository = paymentStatusRepository;
        _mediator = mediator;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _blockchainPublisher = blockchainPublisher;
    }

    public async Task<Result<Payment>> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate payment method
            var paymentMethod = await _paymentMethodRepository.GetByIdAsync(request.PaymentMethodId);
            if (paymentMethod == null)
            {
                return Result<Payment>.Fail(
                    message: "طريقة الدفع غير موجودة",
                    errorType: "PaymentMethodNotFound",
                    resultStatus: ResultStatus.NotFound);
            }

            if (!paymentMethod.IsActive)
            {
                return Result<Payment>.Fail(
                    message: "طريقة الدفع غير متاحة حالياً",
                    errorType: "PaymentMethodInactive",
                    resultStatus: ResultStatus.ValidationError);
            }

            // Begin transaction
            await _unitOfWork.BeginTransaction();

            try
            {
                // Get initial status (Pending)
                var statuses = await _paymentStatusRepository.GetAllAsync();
                var pendingStatus = statuses.FirstOrDefault(s => s.Name == "قيد الانتظار");
                if (pendingStatus == null)
                {
                    return Result<Payment>.Fail(
                        message: "حالة الدفع غير موجودة",
                        errorType: "PaymentStatusNotFound",
                        resultStatus: ResultStatus.Failed);
                }

                // Create payment record with pending status
                var payment = new Payment
                {
                    Id = Guid.NewGuid(),
                    OrderId = request.OrderId,
                    Amount = request.Amount,
                    PaymentMethodId = request.PaymentMethodId,
                    StatusId = pendingStatus.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Save the initial payment record
                await _paymentRepository.AddAsync(payment);

                // Process payment based on method
                var processResult = await ProcessPaymentByMethod(paymentMethod, request.PaymentDetails);
                if (!processResult.Success)
                {
                    await _unitOfWork.RollbackTransaction();
                    return Result<Payment>.Fail(
                        message: processResult.Message,
                        errorType: processResult.ErrorType,
                        resultStatus: processResult.ResultStatus);
                }

                // Get completed status
                var completedStatus = statuses.FirstOrDefault(s => s.Name == "مكتمل");
                if (completedStatus != null)
                {
                    // Update payment status directly in the database
                    payment.StatusId = completedStatus.Id;
                    payment.UpdatedAt = DateTime.UtcNow;
                    
                    // Use the repository to update only the necessary fields
                    await _paymentRepository.UpdateFields(payment.Id, new Dictionary<string, object>
                    {
                        { nameof(Payment.StatusId), completedStatus.Id },
                        { nameof(Payment.UpdatedAt), DateTime.UtcNow }
                    });
                }

                // Update order status to paid
                var orderResult = await _mediator.Send(new PayOrderCommand(request.OrderId), cancellationToken);
                if (!orderResult.Success)
                {
                    await _unitOfWork.RollbackTransaction();
                    return Result<Payment>.Fail(
                        message: orderResult.Message,
                        errorType: orderResult.ErrorType,
                        resultStatus: orderResult.ResultStatus);
                }

                // Commit all changes
                await _unitOfWork.CommitTransaction();

                // Fetch the updated payment to return
                var updatedPayment = await _paymentRepository.GetByIdAsync(payment.Id);

                // Publish blockchain data synchronously to avoid service provider disposal
                try
                {
                    await PublishBlockchainDataAsync(updatedPayment);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error publishing blockchain data for order {OrderId} and payment {PaymentId}", 
                        request.OrderId, updatedPayment.Id);
                    // Don't fail the payment if blockchain publishing fails
                }

                return Result<Payment>.Ok(
                    data: updatedPayment,
                    message: "تم الدفع بنجاح",
                    resultStatus: ResultStatus.Success);
            }
            catch
            {
                await _unitOfWork.RollbackTransaction();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment for order {OrderId}", request.OrderId);
            return Result<Payment>.Fail(
                message: "حدث خطأ أثناء معالجة الدفع",
                errorType: "ProcessPaymentFailed",
                resultStatus: ResultStatus.Failed);
        }
    }

    private async Task<Result> ProcessPaymentByMethod(PaymentMethod paymentMethod, string? paymentDetails)
    {
        try
        {
            // Here we simulate payment processing based on the method
            switch (paymentMethod.Name)
            {
                case "الدفع عند الاستلام": // Cash on Delivery
                    return Result.Ok(
                        message: "تم تأكيد الدفع عند الاستلام",
                        resultStatus: ResultStatus.Success);

                case "بطاقة ائتمان": // Credit Card
                    return Result.Ok(
                        message: "تم معالجة الدفع بالبطاقة بنجاح",
                        resultStatus: ResultStatus.Success);
                case "بطاقة خصم": // Debit Card
                    //if (string.IsNullOrEmpty(paymentDetails))
                    //{
                    //    return Result.Fail(
                    //        message: "تفاصيل البطاقة مطلوبة",
                    //        errorType: "PaymentDetailsRequired",
                    //        resultStatus: ResultStatus.ValidationError);
                    //}
                    // Here you would integrate with a payment gateway
                    // For now, we'll simulate success
                    return Result.Ok(
                        message: "تم معالجة الدفع بالبطاقة بنجاح",
                        resultStatus: ResultStatus.Success);

                case "باي بال": // PayPal
                    //if (string.IsNullOrEmpty(paymentDetails))
                    //{
                    //        return Result.Fail(
                    //            message: "معرف معاملة PayPal مطلوب",
                    //            errorType: "PaymentDetailsRequired",
                    //            resultStatus: ResultStatus.ValidationError);
                    //}
                    // Here you would verify with PayPal API
                    return Result.Ok(
                        message: "تم التحقق من دفع PayPal",
                        resultStatus: ResultStatus.Success);

                case "تحويل بنكي": // Bank Transfer
                    //if (string.IsNullOrEmpty(paymentDetails))
                    //{
                    //    return Result.Fail(
                    //        message: "تفاصيل التحويل البنكي مطلوبة",
                    //        errorType: "PaymentDetailsRequired",
                    //        resultStatus: ResultStatus.ValidationError);
                    //}
                    // Here you would verify bank transfer details
                    return Result.Ok(
                        message: "تم التحقق من التحويل البنكي",
                        resultStatus: ResultStatus.Success);

                default:
                    return Result.Fail(
                        message: "طريقة الدفع غير مدعومة",
                        errorType: "UnsupportedPaymentMethod",
                        resultStatus: ResultStatus.ValidationError);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment method {MethodName}", paymentMethod.Name);
            return Result.Fail(
                message: "حدث خطأ أثناء معالجة طريقة الدفع",
                errorType: "PaymentMethodProcessingError",
                resultStatus: ResultStatus.Failed);
        }
    }

    private async Task PublishBlockchainDataAsync(Payment payment)
    {
        try
        {
            // Get payment blockchain details
            var paymentBlockchainQuery = new GetPaymentBlockchainDetailsQuery(payment.Id);
            var paymentBlockchainResult = await _mediator.Send(paymentBlockchainQuery);

            // Get order blockchain details
            var orderBlockchainQuery = new GetOrderBlockchainDetailsQuery(payment.OrderId);
            var orderBlockchainResult = await _mediator.Send(orderBlockchainQuery);

            // Only publish if both requests are successful
            if (paymentBlockchainResult.Success && orderBlockchainResult.Success)
            {
                var publishBlockChain = new PublishBlockChain
                {
                    // Map Order Data
                    OrderId = orderBlockchainResult.Data.OrderId,
                    OrderDate = orderBlockchainResult.Data.OrderDate,
                    OrderStatus = orderBlockchainResult.Data.OrderStatus,
                    TotalAmount = orderBlockchainResult.Data.TotalAmount,
                    ShippingAddress = orderBlockchainResult.Data.ShippingAddress ?? "Unknown",
                    Items = orderBlockchainResult.Data.Items?.Select(item => new OrderItemBlockchain
                    {
                        ItemId = item.ItemId,
                        ItemName = item.ItemName,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        SerialNumber=item.SerialNumber,
                        TotalPrice = item.TotalPrice
                    }).ToList() ?? new List<OrderItemBlockchain>(),
                    
                    // Map Payment Data
                    PaymentId = paymentBlockchainResult.Data.PaymentId,
                    PaymentAmount = paymentBlockchainResult.Data.Amount,
                    PaymentMethod = paymentBlockchainResult.Data.PaymentMethod,
                    PaymentStatus = paymentBlockchainResult.Data.PaymentStatus,
                    TransactionDate = paymentBlockchainResult.Data.TransactionDate,
                    TransactionHash = paymentBlockchainResult.Data.TransactionHash,
                    PaymentDetails = paymentBlockchainResult.Data.PaymentDetails,
                    
                    // Map User Data
                    SellerName = orderBlockchainResult.Data.Seller?.Username ?? "Unknown Seller",
                    SellerPhone = orderBlockchainResult.Data.Seller?.PhoneNumber ?? "Unknown",
                    CustomerName = orderBlockchainResult.Data.Customer?.Username ?? "Unknown Customer", 
                    CustomerPhone = orderBlockchainResult.Data.Customer?.PhoneNumber ?? "Unknown",
                    
                    // Transaction Metadata
                    TransactionTimestamp = DateTime.UtcNow,
                    TransactionType = "ORDER_PAYMENT_COMPLETION"
                };

                await _blockchainPublisher.PublishOrderPaymentBlockchainAsync(publishBlockChain);

                _logger.LogInformation(
                    "Successfully published combined blockchain data for order {OrderId} and payment {PaymentId}", 
                    payment.OrderId, payment.Id);
            }
            else
            {
                _logger.LogWarning(
                    "Failed to get blockchain details - Order: {OrderSuccess}, Payment: {PaymentSuccess} for order {OrderId} and payment {PaymentId}",
                    orderBlockchainResult.Success, paymentBlockchainResult.Success, payment.OrderId, payment.Id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Error publishing blockchain data for order {OrderId} and payment {PaymentId}", 
                payment.OrderId, payment.Id);
            // Don't throw - this is not critical for payment processing
        }
    }
}