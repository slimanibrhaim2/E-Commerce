using System;
using System.Collections.Generic;

namespace Payments.Domain.Entities
{
    public class PublishBlockChain
    {
        // Order Information
        public Guid OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; } = null!;
        public double TotalAmount { get; set; }
        public string ShippingAddress { get; set; } = null!;
        public List<OrderItemBlockchain> Items { get; set; } = new List<OrderItemBlockchain>();
        
        // Payment Information
        public Guid PaymentId { get; set; }
        public double PaymentAmount { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string PaymentStatus { get; set; } = null!;
        public DateTime TransactionDate { get; set; }
        public string TransactionHash { get; set; } = null!;
        public string? PaymentDetails { get; set; }

        // User Information
        public string SellerName { get; set; } = null!;
        public string SellerPhone { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public string CustomerPhone { get; set; } = null!;

        // Transaction Metadata
        public DateTime TransactionTimestamp { get; set; }
        public string TransactionType { get; set; } = "ORDER_PAYMENT_COMPLETION";
    }

    public class OrderItemBlockchain
    {
        public Guid ItemId { get; set; }
        public string ItemName { get; set; } = null!;
        public double Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double TotalPrice { get; set; }
    }
}