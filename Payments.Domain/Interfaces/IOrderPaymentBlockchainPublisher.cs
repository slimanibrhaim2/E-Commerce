using System.Threading.Tasks;
using Payments.Domain.Entities;

namespace Payments.Domain.Interfaces
{
    /// <summary>
    /// Interface for publishing combined order and payment blockchain data to workers
    /// </summary>
    public interface IOrderPaymentBlockchainPublisher
    {
        Task PublishOrderPaymentBlockchainAsync(PublishBlockChain publishBlockChain);
    }
} 