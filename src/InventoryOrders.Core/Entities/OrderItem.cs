using InventoryOrders.Core.Entities.BaseEntities;
using System.ComponentModel.DataAnnotations;

namespace InventoryOrders.Core.Entities;

public class OrderItem:AuditableEntity
{
    [Required]
    public long OrderId { get; set; }

    [Required]
    public long ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Range(typeof(decimal), "0.01", "9999999999")]
    public decimal UnitPrice { get; set; }

    public Order Order { get; set; }
    public Product Product { get; set; }
}
