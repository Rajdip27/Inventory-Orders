using InventoryOrders.Core.Entities.BaseEntities;
using System.ComponentModel.DataAnnotations;

namespace InventoryOrders.Application.ViewModel;

public class OrderItemVm:BaseEntity
{
    [Required]
    public long ProductId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Range(typeof(decimal), "0.01", "9999999999")]
    public decimal UnitPrice { get; set; }
    public string ProductName { get; set; }
    public decimal LineTotal => Quantity * UnitPrice;
}
