using System.Diagnostics;

namespace InventoryOrders.Application.Utilities;

public static class DebugExtensions
{
    /// <summary>
    /// Logs each item of the enumerable during debugging.
    /// </summary>
    public static IEnumerable<T> LINQLogger<T>(
        this IEnumerable<T> enumerable,
        string logName = "LINQLogger",
        Func<T, string> printMethod = null)
    {

        #if DEBUG
            int count = 0;
            foreach (var item in enumerable)
            {
                string message = printMethod != null
                    ? printMethod(item)
                    : item?.ToString() ?? "<null>";

                Debug.WriteLine($"{logName} | item {count} = {message}");
                count++;
                yield return item;
            }
            Debug.WriteLine($"{logName} | Total count = {count}");
       #else
            foreach (var item in enumerable)
            {
                yield return item;
            }
      #endif
    }
}
