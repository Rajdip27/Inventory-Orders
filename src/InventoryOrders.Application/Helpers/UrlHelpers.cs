namespace InventoryOrders.Application.Helpers;

public static class UrlHelpers
{
    /// <summary>
    /// Generates a full image URL for an event based on the provided image file name and location,
    /// or returns a placeholder image path if no image is specified.
    /// </summary>
    /// <param name="imageUrl">The file name of the image. If null or empty, returns the placeholder.</param>
    /// <param name="location">Optional base path to prepend to the image file name.</param>
    /// <returns>Full URL string for the image or placeholder</returns>
    public static string GetEventImageUrl(this string imageUrl, string location = null)
    {
        string placeholder = "~/images/default.png";
        try
        {
            // Determine final path
            string finalPath = !string.IsNullOrEmpty(location) ? location.TrimEnd('/') : "";

            // Return image URL or placeholder
            return !string.IsNullOrEmpty(imageUrl) && !string.IsNullOrEmpty(finalPath)
                ? $"{finalPath}/{imageUrl}"
                : placeholder;
        }
        catch (Exception ex)
        {
            // Log exception and fallback to placeholder
            Console.WriteLine($"[GetEventImageUrl] Error: {ex.Message}");
            return placeholder;
        }
    }
}



