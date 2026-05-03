namespace InventoryOrders.Application.Helpers;

public static class EnumHelper
{
    /// <summary>
    /// Converts a list of string values to a combined Flags enum value.
    /// </summary>
    public static T ParseFlagsEnum<T>(IEnumerable<string> values) where T : struct, Enum
    {
        T result = (T)Enum.ToObject(typeof(T), 0); // default None / 0

        if (values == null) return result;

        foreach (var value in values)
        {
            if (Enum.TryParse<T>(value, out var parsed))
            {
                result = (T)(object)(((int)(object)result) | ((int)(object)parsed));
            }
        }

        return result;
    }
}
