using RecommendationEngine.Domain.Entities;
using RecommendationEngine.Shared.Enums;
using RecommendationEngine.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationEngine.Application.Services
{
    public interface IColumnConfigurationService
    {
        Task<List<ColumnConfiguration>> GetColumnConfigurations(TemplateType templateType, string region, string language);
    }
}
