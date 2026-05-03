using InventoryOrders.Application.CommonModel;

namespace InventoryOrders.Application.Helpers;

public static class PaginationHelper
{
    public static List<object> GeneratePageList(IPaginationModel model, int pageNeighbours = 1)
    {
        var pages = new List<object>();
        int totalPages = model.TotalPages;
        int currentPage = model.PageNumber;

        if (totalPages == 0) return pages;

        // Always show first page
        pages.Add(1);

        int start = Math.Max(2, currentPage - pageNeighbours);
        int end = Math.Min(totalPages - 1, currentPage + pageNeighbours);

        if (start > 2)
            pages.Add("...");

        for (int i = start; i <= end; i++)
            pages.Add(i);

        if (end < totalPages - 1)
            pages.Add("...");

        // Always show last page
        if (totalPages > 1)
            pages.Add(totalPages);

        return pages;
    }
}
