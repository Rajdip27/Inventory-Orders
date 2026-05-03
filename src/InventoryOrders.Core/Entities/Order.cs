using InventoryOrders.Core.Entities.BaseEntities;
using System.ComponentModel.DataAnnotations;

namespace InventoryOrders.Core.Entities;

public class Order:AuditableEntity
{
    [MaxLength(150)]
    public string CustomerName { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.Now;
    [Range(typeof(decimal), "0", "9999999999")]
    public decimal TotalAmount { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
