using RecommendationEngine.Domain.Entities;
using RecommendationEngine.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using RecommendationEngine.Shared.Enums;
namespace RecommendationEngine.Application.Services
{
    public class ColumnConfigurationService : IColumnConfigurationService
    {
        private readonly MySqlDbContext _context;

        public ColumnConfigurationService(MySqlDbContext context)
        {
            _context = context;
        }

        public async Task<List<ColumnConfiguration>> GetAllConfigurationsAsync()
        {
            return await _context.ColumnConfigurations.ToListAsync();
        }


        public async Task<List<ColumnConfiguration>> GetColumnConfigurations(TemplateType templateType, string region, string language)
        {
            // Retrieve the template_id based on templateName (templateType) and language

            if (region.ToLower() == RegionCodeEnum.MY.ToString().ToLower())
            {
                // Map TemplateType enum to the corresponding template_name value in the database
                string templateName = templateType switch
                {
                    TemplateType.ANCHOR_RECOMMENDATION => "Anchor_Recommendation",
                    TemplateType.DEALER_RECOMMENDATION => "Dealer_Recommendation",
                    TemplateType.INVOICE => "Invoice", // Add other cases as needed
                    _ => throw new ArgumentOutOfRangeException(nameof(templateType), $"Unsupported template type: {templateType}")
                };

                // Query to fetch the template ID based on template_name and language from TemplateConfiguration table
                var templateConfiguration = await _context.TemplateConfigurations
                    .Where(t => t.TemplateName.Equals(templateName, StringComparison.OrdinalIgnoreCase) &&
                                t.Language.Equals(language, StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefaultAsync();

                if (templateConfiguration == null)
                {
                    throw new Exception($"No template found for {templateType} and language {language}");
                }

                // Query the ColumnConfiguration table using the retrieved template ID
                var columnConfigurations = await _context.ColumnConfigurations
                    .Where(c => c.TemplateId == templateConfiguration.TemplateId)
                    .ToListAsync();

                return columnConfigurations;
            }
            else if (region.ToLower() == RegionCodeEnum.VN.ToString().ToLower())
            {
                // Map TemplateType enum to the corresponding template_name value in the database
                string templateName = templateType switch
                {
                    TemplateType.ANCHOR_RECOMMENDATION => "Dealer_Recommendation",
                    TemplateType.DEALER_RECOMMENDATION => "Customer_Recommendation",
                    TemplateType.INVOICE => "Invoice", // Add other cases as needed
                    _ => throw new ArgumentOutOfRangeException(nameof(templateType), $"Unsupported template type: {templateType}")
                };

                // Query to fetch the template ID based on template_name and language from TemplateConfiguration table
                var templateConfiguration = await _context.TemplateConfigurations
                    .Where(t => t.TemplateName.Equals(templateName, StringComparison.OrdinalIgnoreCase) &&
                                t.Language.Equals(language, StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefaultAsync();

                if (templateConfiguration == null)
                {
                    throw new Exception($"No template found for {templateType} and language {language}");
                }

                // Query the ColumnConfiguration table using the retrieved template ID
                var columnConfigurations = await _context.ColumnConfigurations
                    .Where(c => c.TemplateId == templateConfiguration.TemplateId)
                    .ToListAsync();

                return columnConfigurations;


            }
            else
            {
                return new List<ColumnConfiguration>();
            }

        }

    }
}

