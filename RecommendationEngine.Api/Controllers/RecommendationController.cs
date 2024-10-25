using Microsoft.AspNetCore.Mvc;
using RecommendationEngine.Shared.Models;
using RecommendationEngine.Application.Services;

namespace RecommendationEngine.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecommendationController : ControllerBase
    {
        private readonly IRecommendationService _recommendationService;

        public RecommendationController(IRecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }

        [HttpPost]
        public async Task<IActionResult> GenerateRecommendation([FromBody] RecommendationRequest request)
        {
            var result = await _recommendationService.ProcessRecommendationAsync(request);
            return Ok(result);
        }
    }
}
