using DinkToPdf;
using DinkToPdf.Contracts;
using System.Runtime.InteropServices;

namespace InventoryOrders.Web.Pdf;

public static class DinkToPdfRegistration
{
    public static IServiceCollection AddDinkToPdf(
   this IServiceCollection services,
   string contentRootPath)
    {
        string platform = GetPlatform();
        string arch = GetArchitecture();

        string fileName = platform switch
        {
            "win" => "libwkhtmltox.dll",
            "linux" => "libwkhtmltox.so",
            "osx" => "libwkhtmltox.dylib",
            _ => throw new Exception("Unsupported OS")
        };

        string nativePath = Path.Combine(
            contentRootPath,
            "runtimes",
            $"{platform}-{arch}",
            "native",
            fileName
        );

        if (!File.Exists(nativePath))
            throw new FileNotFoundException(
                $"Native PDF library not found: {nativePath}");

        var context = new CustomAssemblyLoadContext();
        context.LoadUnmanagedLibrary(nativePath);

        services.AddSingleton<IConverter>(
            new SynchronizedConverter(new PdfTools())
        );

        return services;
    }

    private static string GetPlatform()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return "win";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return "linux";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return "osx";
        throw new Exception("Unsupported OS");
    }

    private static string GetArchitecture()
    {
        return RuntimeInformation.OSArchitecture switch
        {
            Architecture.X64 => "x64",
            Architecture.X86 => "x86",
            Architecture.Arm64 => "arm64",
            _ => throw new Exception("Unsupported architecture")
        };
    }
}
