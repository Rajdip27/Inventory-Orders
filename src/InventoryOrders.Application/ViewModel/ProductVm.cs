using InventoryOrders.Core.Entities.BaseEntities;
using System.ComponentModel.DataAnnotations;

namespace InventoryOrders.Application.ViewModel;

public class ProductVm:BaseEntity
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string SKU { get; set; } = string.Empty;

    [Range(0.01, 9999999999)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int QuantityInStock { get; set; }

    public DateTime CreatedAt { get; set; }
}
