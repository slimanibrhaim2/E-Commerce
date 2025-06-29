using System;
using System.Collections.Generic;

namespace Catalogs.Application.DTOs;

public class ProductDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; }
    public string SKU { get; set; }
    public double StockQuantity { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsFavorite { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<MediaDTO> Media { get; set; }
    public List<ProductFeatureDTO> Features { get; set; }
} 