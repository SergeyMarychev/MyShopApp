using Serilog.Sinks.File.Archive;
using System.IO.Compression;

namespace MyShopApp.WebApi.Logging
{
    public class SerilogHooks
    {
        // 336 архивов
        public static ArchiveHooks ArchiveHooks => new ArchiveHooks(14 * 24, CompressionLevel.Fastest);
    }
}
