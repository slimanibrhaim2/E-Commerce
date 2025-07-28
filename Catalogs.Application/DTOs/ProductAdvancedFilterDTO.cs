using System.ComponentModel.DataAnnotations;

namespace Catalogs.Application.DTOs
{
    public class ProductAdvancedFilterDTO
    {
        public Guid? CategoryId { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Minimum price must be greater than or equal to 0")]
        public decimal? MinPrice { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Maximum price must be greater than or equal to 0")]
        public decimal? MaxPrice { get; set; }
        
        /// <summary>
        /// List of feature filters. Each item contains FeatureName and optionally FeatureValue
        /// If FeatureValue is null/empty, it will filter by feature existence only
        /// </summary>
        public List<FeatureFilterDTO>? Features { get; set; }
        
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class FeatureFilterDTO
    {
        [Required]
        public string FeatureName { get; set; } = string.Empty;
        
        /// <summary>
        /// Optional feature value. If null/empty, will filter by feature name existence only
        /// </summary>
        public string? FeatureValue { get; set; }
    }
} 