using RecommendationEngine.Shared.Models;
using RecommendationEngine.Infrastructure.S3;
using RecommendationEngine.Shared.Enums;
using RecommendationEngine.Domain.Entities;
using RecommendationEngine.Application.Validators;
using RecommendationEngine.Shared.Responses;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System.Text.RegularExpressions;
namespace RecommendationEngine.Application.Services
{
    public class RecommendationService : IRecommendationService
    {
        private readonly IS3Service _s3Service;
        private readonly IColumnConfigurationService _columnConfigurationService;
        private readonly ILogger<RecommendationService> _logger;
        public RecommendationService(IS3Service s3Service, IColumnConfigurationService columnConfigurationService, ILogger<RecommendationService> logger)
        {
            _s3Service = s3Service;
            _columnConfigurationService = columnConfigurationService;
            _logger = logger;
        }


        public async Task<Response> ProcessRecommendationAsync(RecommendationRequest request)
        {
            _logger.LogInformation("Method {MethodName} started", nameof(ProcessRecommendationAsync));

            // Validate input


            if (!ValidateInput(request))
            {
                return new Response
                {
                    Status = (int)StatusCodeEnum.HeaderValidationFailed,
                    Error = new Error { ErrorCode = "1001", ErrorDescription = "Header validation failed." }
                };
            }

            // Retrieve file from S3
            //Stream fileStream = await _s3Service.RetrieveFileAsync(request.FilePath);
            //Stream fileStream = File.OpenRead(request.FilePath);

            string path = "C:\\Users\\P7167409\\OneDrive - Ness Digital Engineering\\vijay\\RecommondationEngine\\Recommondation2.xlsx";
            Stream fileStream = File.OpenRead(path);

            // Get configuration based on the template type
            var columnConfig = await _columnConfigurationService.GetColumnConfigurations(
                    request.TemplateType, request.Region, request.Language);

            if (columnConfig != null)
            {
                columnConfig = columnConfig.OrderBy(c => c.Index).ToList();
            }

            // Read and validate Excel headers
            var excelHeaders = ReadExcelHeaders(fileStream); // Extract headers

            if (!ValidateHeaders(excelHeaders, columnConfig))
            {
                return new Response
                {
                    Status = (int)StatusCodeEnum.HeaderValidationFailed,
                    Error = new Error { ErrorCode = "1001", ErrorDescription = "Header validation failed." }
                };
            }

            // Further validation of rows
            var successList = new List<Dictionary<string, string>>();
            var failureList = new List<Dictionary<string, string>>();

            var rows = ReadExcelRows(fileStream);

            foreach (var row in rows)
            {
                var validatedRow = ValidateRow(row, columnConfig);

                // Check if the row contains errors (check for the "Error" key in the dictionary)
                if (validatedRow.ContainsKey("Error"))
                {
                    failureList.Add(validatedRow); // Add to failure list if the row has validation issues
                }
                else
                {
                    successList.Add(validatedRow); // Add to success list if the row is valid
                }
            }

            _logger.LogInformation("Method {MethodName} completed", nameof(ProcessRecommendationAsync));

            return new Response
            {
                Status = (int)StatusCodeEnum.Success,
                Data = new { SuccessList = successList, FailureList = failureList }
            };

        }

        private bool ValidateInput(RecommendationRequest request)
        {
            // Implement validation for mandatory fields, enums, and data types
            bool isvalid = false;
            if (request != null)
            {
                isvalid = RecommendationValidator.Validate(request);
            }

            return isvalid;
        }
        private List<string> ReadExcelHeaders(Stream fileStream)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Add this line
            using (var package = new ExcelPackage(fileStream))
            {
                var worksheet = package.Workbook.Worksheets[0]; // Assuming you are working with the first sheet
                int colCount = worksheet.Dimension.End.Column; // Get number of columns

                var headers = new List<string>();
                for (int col = 1; col <= colCount; col++)
                {
                    headers.Add(worksheet.Cells[1, col].Text); // Reading the first row for headers
                }

                return headers;
            }
        }
        private bool ValidateHeaders(List<string> excelHeaders, List<ColumnConfiguration> columnConfig)
        {
            // Get the expected headers from the column configuration, ordered by Index
            var expectedHeaders = columnConfig
                .OrderBy(c => c.Index) // Order by Index to ensure the correct order
                .Select(c => c.ColumnHeader)
                .ToList();

            // Check if the count of headers matches
            if (excelHeaders.Count != expectedHeaders.Count)
            {
                _logger.LogError("Header count mismatch: Expected {ExpectedCount}, but got {ActualCount}",
                                 expectedHeaders.Count, excelHeaders.Count);
                return false; // Validation fails if the counts do not match
            }

            // Check if all headers match in order
            for (int i = 0; i < expectedHeaders.Count; i++)
            {
                if (!string.Equals(expectedHeaders[i], excelHeaders[i], StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogError("Header mismatch at position {Position}: Expected '{ExpectedHeader}', but found '{ActualHeader}'",
                                     i + 1, expectedHeaders[i], excelHeaders[i]);
                    return false; // Validation fails if any header does not match
                }
            }

            _logger.LogInformation("Header validation passed");
            return true; // Validation passes if all headers match in content and order
        }


        //private List<ExcelRangeBase[]> ReadExcelRows(Stream fileStream)
        //{
        //    ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Add this line
        //    var rows = new List<ExcelRangeBase[]>();

        //    using (var package = new ExcelPackage(fileStream))
        //    {
        //        var worksheet = package.Workbook.Worksheets[0]; // Assuming we're working with the first worksheet
        //        int rowCount = worksheet.Dimension.End.Row;     // Get total number of rows
        //        int colCount = worksheet.Dimension.End.Column;  // Get total number of columns

        //        // Iterate through each row (starting from the second row, after the headers)
        //        for (int row = 2; row <= rowCount; row++)
        //        {
        //            // Create an array of cells in this row
        //            var rowCells = new ExcelRangeBase[colCount];

        //            for (int col = 1; col <= colCount; col++)
        //            {
        //                rowCells[col - 1] = worksheet.Cells[row, col]; // Store each cell in the array
        //            }

        //            rows.Add(rowCells); // Add this row's cells to the list
        //        }
        //    }

        //    return rows;
        //}

        private List<Dictionary<string, string>> ReadExcelRows(Stream fileStream)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var rows = new List<Dictionary<string, string>>();

            using (var package = new ExcelPackage(fileStream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.End.Row;
                int colCount = worksheet.Dimension.End.Column;

                // Iterate through each row (starting from the second row)
                for (int row = 2; row <= rowCount; row++)
                {
                    var rowData = new Dictionary<string, string>();

                    for (int col = 1; col <= colCount; col++)
                    {
                        rowData[$"Column{col}"] = worksheet.Cells[row, col].Text;
                    }

                    rows.Add(rowData); // Add this row's data to the list
                }
            }

            return rows;
        }

        private Dictionary<string, string> ValidateRow(Dictionary<string, string> row, List<ColumnConfiguration> columnConfig)
        {
            var errors = new List<string>();
            var validatedRow = new Dictionary<string, string>();

            foreach (var config in columnConfig)
            {
                // Try to get the cell value from the row dictionary
                row.TryGetValue(config.ColumnHeader, out var cellValue);

                string columnErrorMessage = string.Empty;

                // Add cell value to validatedRow
                validatedRow[config.ColumnHeader] = cellValue ?? string.Empty;

                // Validate mandatory field requirement
                if (config.Mandatory && string.IsNullOrWhiteSpace(cellValue))
                {
                    _logger.LogError("Validation failed for mandatory column: {Column}", config.ColumnHeader);
                    columnErrorMessage = config.MandatoryErrorMessage;
                }

                // Validate against regex expression if provided
                if (!string.IsNullOrWhiteSpace(config.RegexExp))
                {
                    var regex = new Regex(config.RegexExp);
                    if (!regex.IsMatch(cellValue ?? string.Empty))
                    {
                        _logger.LogError("Regex validation failed for column: {Column}", config.ColumnHeader);
                        columnErrorMessage += !string.IsNullOrWhiteSpace(columnErrorMessage)
                            ? $", {config.RegexErrorMessage}"
                            : config.RegexErrorMessage;
                    }
                }

                // Add any column errors
                if (!string.IsNullOrWhiteSpace(columnErrorMessage))
                {
                    errors.Add($"{config.ColumnHeader}: {columnErrorMessage}");
                }
            }

            // Add an "Error" field with comma-separated error messages if any errors exist
            if (errors.Count > 0)
            {
                validatedRow["Error"] = string.Join(", ", errors);
            }

            return validatedRow;
        }


        //private Dictionary<string, string> ValidateRow(ExcelRangeBase[] row, List<ColumnConfiguration> columnConfig)
        //{
        //    var errors = new List<string>();

        //    // Create a dictionary to hold the validated data
        //    var validatedRow = new Dictionary<string, string>();

        //    for (int i = 0; i < columnConfig.Count; i++)
        //    {
        //        var config = columnConfig[i];
        //        var cell = row[i]; // Get the cell corresponding to the column configuration
        //        string columnErrorMessage = string.Empty;

        //        // Add the cell value to the validated row
        //        validatedRow[config.ColumnHeader] = cell.Text;

        //        // Check if the cell is mandatory and empty
        //        if (config.Mandatory && string.IsNullOrWhiteSpace(cell.Text))
        //        {
        //            _logger.LogError("Validation failed for mandatory column: {Column}", config.ColumnHeader);
        //            columnErrorMessage = config.MandatoryErrorMessage;
        //        }

        //        // Check if there's a regex expression to validate the cell's value
        //        if (!string.IsNullOrWhiteSpace(config.RegexExp))
        //        {
        //            var regex = new Regex(config.RegexExp);
        //            if (!regex.IsMatch(cell.Text))
        //            {
        //                _logger.LogError("Regex validation failed for column: {Column}", config.ColumnHeader);
        //                if (!string.IsNullOrWhiteSpace(columnErrorMessage))
        //                {
        //                    // If already a mandatory error, append the regex error
        //                    columnErrorMessage += $", {config.RegexErrorMessage}";
        //                }
        //                else
        //                {
        //                    columnErrorMessage = config.RegexErrorMessage;
        //                }
        //            }
        //        }

        //        // If there were errors for this column, add the error to the list
        //        if (!string.IsNullOrWhiteSpace(columnErrorMessage))
        //        {
        //            errors.Add($"{config.ColumnHeader}: {columnErrorMessage}");
        //        }
        //    }

        //    // If there are errors, add an "Error" field with the comma-separated error messages
        //    if (errors.Count > 0)
        //    {
        //        validatedRow["Error"] = string.Join(", ", errors); // Add error column
        //    }

        //    return validatedRow; // Return the validated row (with or without errors)
        //}
    }

}
