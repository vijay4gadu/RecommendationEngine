

namespace RecommendationEngine.Shared.Responses
{
    public class Response
    {
        public int Status { get; set; }
        public Error Error { get; set; }
        public object Data { get; set; }
    }

    public class Error
    {
        public string ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
    }
}
