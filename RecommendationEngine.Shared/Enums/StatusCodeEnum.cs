
namespace RecommendationEngine.Shared.Enums
{
    public enum StatusCodeEnum
    {
        Success = 200,
        BadRequest = 400,
        Unauthorized = 401,
        NotFound = 404,
        InternalServerError = 500,
        CustomError = 1000,
        HeaderValidationFailed = 1001,
        RegexValidationFailed = 1002
    }

}
