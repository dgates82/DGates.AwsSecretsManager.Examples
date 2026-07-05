using System;
using System.Configuration;
using System.Threading.Tasks;
using ConsoleExample.Models;
using DGates.AwsSecretsManager;

namespace ConsoleExample
{
    internal static class Program
    {
        private static async Task Main()
        {
            var settings = new SecretsManagerSettings
            {
                ServiceUrl = ConfigurationManager.AppSettings["AWSServiceURL"],
                Region     = ConfigurationManager.AppSettings["AWSRegion"],
            };

            var secretName = ConfigurationManager.AppSettings["Secrets:DbConfigName"];
            var service = SecretsManagerServiceFactory.Create(settings);

            // 1. Initial fetch — result is cached after this call
            Console.WriteLine("Fetching secret (initial)...");
            var config = await service.GetSecretAsync<DbConfig>(secretName);
            PrintConfig(config);

            // 2. Same secret as raw JSON string
            Console.WriteLine("\nFetching secret as raw string...");
            var raw = await service.GetSecretStringAsync(secretName);
            Console.WriteLine($"  Raw: {raw}");

            // 3. Evict from cache
            Console.WriteLine("\nInvalidating cache...");
            service.InvalidateCache(secretName);
            Console.WriteLine("  Cache invalidated.");

            // 4. Force fresh fetch from Secrets Manager, repopulates cache
            Console.WriteLine("\nRefreshing secret...");
            config = await service.RefreshSecretAsync<DbConfig>(secretName);
            PrintConfig(config);

            // 5. Fetch again — served from cache repopulated by RefreshSecretAsync
            Console.WriteLine("\nFetching secret (cache hit)...");
            config = await service.GetSecretAsync<DbConfig>(secretName);
            Console.WriteLine("[cache hit] Secret served from in-memory cache.");
            PrintConfig(config);

            Console.WriteLine("\nDone. Press any key to exit.");
            Console.ReadKey();
        }

        private static void PrintConfig(DbConfig config)
        {
            Console.WriteLine($"  Server:   {config.Server}");
            Console.WriteLine($"  Database: {config.Database}");
            Console.WriteLine($"  Username: {config.Username}");
            Console.WriteLine($"  Password: {new string('*', config.Password?.Length ?? 0)}");
        }
    }
}
