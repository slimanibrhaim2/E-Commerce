using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Catalogs.Application.DTOs
{
    public class CreateProductAggregateDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public Guid CategoryId { get; set; }
        public string SKU { get; set; }
        public int StockQuantity { get; set; }
        public bool IsAvailable { get; set; }
        
        [BindNever]
        public List<CreateMediaDTO> Media { get; set; } = new();

        [FromForm(Name = "features")]
        public List<CreateFeatureDTO> Features { get; set; } = new();

        public void SetMedia(List<CreateMediaDTO> media)
        {
            Media = media ?? new List<CreateMediaDTO>();
        }
    }
} 