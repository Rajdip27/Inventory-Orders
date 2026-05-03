using System.Globalization;

namespace InventoryOrders.Application.Extensions;

public static class NullableExtensions
{
    public static T GetValueOrDefault<T>(this T value)
    {
        return value == null ? default : value;
    }

    public static decimal ParseAmount(string amount)
    {
        if (!decimal.TryParse(
            amount,
            NumberStyles.Any,
            CultureInfo.InvariantCulture,
            out var result))
        {
            throw new Exception("Invalid amount from payment gateway");
        }

        return result;
    }

}
