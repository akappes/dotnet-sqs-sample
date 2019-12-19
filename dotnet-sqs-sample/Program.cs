using System.IO;
using Elastic.Apm.NetCoreAll;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Amazon.SQS;

using Microsoft.Extensions.Configuration;

namespace dotnet_sqs_sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
        
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: false, reloadOnChange: true)
                        .AddEnvironmentVariables()
                        .Build();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    // AWS Configuration
                    var options = hostContext.Configuration.GetAWSOptions();
                    services.AddDefaultAWSOptions(options);
                    services.AddAWSService<IAmazonSQS>();
                    services.AddHostedService<Worker>();
                });
    }
}