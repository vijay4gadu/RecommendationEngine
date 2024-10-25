using System.Text.RegularExpressions;
using RecommendationEngine.Shared.Models;

namespace RecommendationEngine.Application.Validators
{
    public static class RecommendationValidator
    {
        // Regex for alphanumeric with special characters
        private static readonly Regex AlphanumericWithSpecialChars = new Regex(@"^[a-zA-Z0-9\\-_]+$", RegexOptions.Compiled);

        public static bool Validate(RecommendationRequest request)
        {
            // Validate mandatory fields
            if (string.IsNullOrWhiteSpace(request.EntityID)) return false;
            if (string.IsNullOrWhiteSpace(request.ProgramID)) return false;
            if (string.IsNullOrWhiteSpace(request.Region)) return false;
            if (string.IsNullOrWhiteSpace(request.Language)) return false;
            if (string.IsNullOrWhiteSpace(request.FilePath)) return false;
            if (string.IsNullOrWhiteSpace(request.FileName)) return false;

            // Validate alphanumeric with special characters for EntityID and ProgramID
            if (!AlphanumericWithSpecialChars.IsMatch(request.EntityID)) return false;
            if (!AlphanumericWithSpecialChars.IsMatch(request.ProgramID)) return false;

            // Validate the file is an xlsx by magic byte detection
            if (!IsExcelFile(request.FilePath)) return false;

            return true;
        }

        private static bool IsXlsxFile(string filePath)
        {
            // Read the first 4 bytes of the file to check for ZIP magic numbers
            byte[] buffer = new byte[4];

            using (var fileStream = File.OpenRead(filePath))
            {
                if (fileStream.Length < 4)
                    return false; // File is too small to be an xlsx file

                // Read the first 4 bytes
                fileStream.Read(buffer, 0, buffer.Length);
            }

            // Check for ZIP magic numbers for .xlsx files (PKZIP format)
            return buffer[0] == 0x50 && buffer[1] == 0x4B && buffer[2] == 0x03 && buffer[3] == 0x04;
        }

        private static bool IsExcelFile(string filePath)
        {
            // Read the first 8 bytes of the file for checking file signatures
            byte[] buffer = new byte[8];

            using (var fileStream = File.OpenRead(filePath))
            {
                if (fileStream.Length < 8)
                    return false; // File is too small to be a valid Excel file

                // Read the first 8 bytes
                fileStream.Read(buffer, 0, buffer.Length);
            }

            // Check for ZIP magic numbers for .xlsx files (PKZIP format)
            bool isXlsx = buffer[0] == 0x50 && buffer[1] == 0x4B && buffer[2] == 0x03 && buffer[3] == 0x04;

            // Check for the .xls file magic number (D0 CF 11 E0 for Microsoft Compound File Binary Format)
            bool isXls = buffer[0] == 0xD0 && buffer[1] == 0xCF && buffer[2] == 0x11 && buffer[3] == 0xE0;

            return isXlsx || isXls;
        }

    }
}


