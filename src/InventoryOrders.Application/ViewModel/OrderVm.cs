using InventoryOrders.Core.Entities.BaseEntities;
using System.ComponentModel.DataAnnotations;

namespace InventoryOrders.Application.ViewModel;

public class OrderVm:BaseEntity
{
    [Required]
    [MaxLength(150)]
    public string CustomerName { get; set; } = string.Empty;

    public DateTime OrderDate { get; set; } = DateTime.Now;

    public decimal TotalAmount { get; set; }

    public List<OrderItemVm> Items { get; set; } = new();
}
