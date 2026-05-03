namespace InventoryOrders.Application.Extensions;

public static class SlugGenerator
{
    public static string GenerateSlug(string title)
    {
        return title.ToLower()
                    .Replace(" ", "-")
                    .Replace(".", "")
                    .Replace(",", "")
                    .Replace("/", "");
    }
}
