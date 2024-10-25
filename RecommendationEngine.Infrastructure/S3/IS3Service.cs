
using System.IO;
using System.Threading.Tasks;

namespace RecommendationEngine.Infrastructure.S3
{
    public interface IS3Service
    {
        Task<Stream> RetrieveFileAsync(string filePath);
    }
}

