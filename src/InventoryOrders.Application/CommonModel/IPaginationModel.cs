

namespace InventoryOrders.Application.CommonModel;

public interface IPaginationModel
{
    int PageNumber { get; }
    int PageSize { get; }
    int TotalItems { get; }
    int TotalPages { get; }
    bool HasPreviousPage { get; }
    bool HasNextPage { get; }
}
