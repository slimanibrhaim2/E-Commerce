using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Catalogs.Application.DTOs
{
    public class CreateFeatureDTO
    {
        [Required(ErrorMessage = "Feature name is required")]
        [StringLength(100, ErrorMessage = "Feature name cannot be longer than 100 characters")]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Feature value is required")]
        [StringLength(500, ErrorMessage = "Feature value cannot be longer than 500 characters")]
        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}
