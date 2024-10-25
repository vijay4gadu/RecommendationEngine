
using RecommendationEngine.Shared.Models;
using RecommendationEngine.Shared.Responses;
using System.Threading.Tasks;

namespace RecommendationEngine.Application.Services
{
    public interface IRecommendationService
    {
        Task<Response> ProcessRecommendationAsync(RecommendationRequest request);
    }
}

