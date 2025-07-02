using System.Threading.Tasks;
using Communication.Domain.Entities;

namespace Communication.Domain.Interfaces
{
    /// <summary>
    /// Domain-level abstraction for publishing reviews to the AI evaluation service
    /// </summary>
    public interface IReviewEvaluationPublisher
    {
        Task PublishForEvaluationAsync(PublishReview review);
    }
} 