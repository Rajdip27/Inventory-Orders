using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;

namespace InventoryOrders.Web.Pdf;

public class CustomAssemblyLoadContext:AssemblyLoadContext
{
    public IntPtr LoadUnmanagedLibrary(string absolutePath)
    {
        if (!File.Exists(absolutePath))
            throw new FileNotFoundException(
                $"Unmanaged library not found: {absolutePath}");

        return LoadUnmanagedDll(absolutePath);
    }

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        return NativeLibrary.Load(unmanagedDllName);
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        return null;
    }
}
