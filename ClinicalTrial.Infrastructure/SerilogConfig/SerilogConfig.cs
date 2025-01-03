using Serilog;
using Serilog.Core;

namespace ClinicalTrial.Infrastructure.SerilogConfig
{
    public static class SerilogConfig
    {
        internal static Logger SetupSerilog()
        {
            return new LoggerConfiguration()
                .MinimumLevel.Information()
                .CreateLogger();
        }
    }
}
