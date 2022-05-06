using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace NorthwindApiApp
{
    /// <summary>
    /// A main program class.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// An application entry point.
        /// </summary>
        /// <param name="args">Application arguments.</param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Creates a host builder.
        /// </summary>
        /// <param name="args">Application arguments.</param>
        /// <returns>A created host builder.</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
