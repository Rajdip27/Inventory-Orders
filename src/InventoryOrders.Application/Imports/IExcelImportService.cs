using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System.Reflection;
using InventoryOrders.Application.Utilities;

namespace InventoryOrders.Application.Imports;

public interface IExcelImportService
{
    Task<List<T>> ReadExcelAsync<T>(IFormFile file) where T : class, new();
}
public class ExcelImportService : IExcelImportService
{
    public async Task<List<T>> ReadExcelAsync<T>(IFormFile file) where T : class, new()
    {
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);

        using var package = new ExcelPackage(stream);
        var ws = package.Workbook.Worksheets.First();

        // Read headers and log
        var headers = Enumerable.Range(1, ws.Dimension.Columns)
                                .Select(c => ws.Cells[1, c].Text.Trim())
                                .LINQLogger("ExcelHeaders", h => h)
                                .ToArray();

        // Map rows and log each object including null values
        var rows = Enumerable.Range(2, ws.Dimension.Rows - 1)
                             .Select(r =>
                             {
                                 var obj = new T();
                                 headers.Select((h, i) =>
                                 {
                                     var prop = typeof(T).GetProperty(h, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                                     var cellValue = ws.Cells[r, i + 1].Text;
                                     if (prop != null)
                                     {
                                         if (!string.IsNullOrWhiteSpace(cellValue))
                                         {
                                             var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                                             var value = Convert.ChangeType(cellValue, targetType);
                                             prop.SetValue(obj, value);
                                         }
                                         else
                                         {
                                             // Set null for reference or nullable types
                                             if (!prop.PropertyType.IsValueType || Nullable.GetUnderlyingType(prop.PropertyType) != null)
                                                 prop.SetValue(obj, null);
                                         }
                                     }
                                     return 0;
                                 }).ToArray();
                                 return obj;
                             })
                             .LINQLogger("ExcelRows", item =>
                             {
                                 return string.Join(", ", headers.Select(h =>
                                 {
                                     var prop = typeof(T).GetProperty(h, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                                     var value = prop?.GetValue(item);
                                     return $"{h}={(value != null ? value.ToString() : "<null>")}";
                                 }));
                             })
                             .ToList();

        return rows;
    }
}