using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Web.Hosting;
using DGates.AwsSecretsManager;
using NLog.Extensions.Logging;

namespace MvcExample.Infrastructure
{
    /// <summary>
    /// Wraps the singleton <see cref="ISecretsManagerService"/> so consumers never need to
    /// reference DGates.AwsSecretsManager directly.
    /// </summary>
    public static class SecretsManagerAccessor
    {
        private static ISecretsManagerService _instance;
        private static string _source;
        
        private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        public static void Initialize()
        {
            var serviceUrl = ConfigurationManager.AppSettings["AWSServiceURL"];
            var localJsonFallbackPath =
                ResolveLocalJsonFallbackPath(ConfigurationManager.AppSettings["LocalJsonFallbackPath"]);

            var settings = new SecretsManagerSettings
            {
                ServiceUrl = serviceUrl,
                Region = ConfigurationManager.AppSettings["AWSRegion"],
                AccessKey = ConfigurationManager.AppSettings["AWSAccessKey"],
                SecretKey = ConfigurationManager.AppSettings["AWSSecretKey"],
                LocalJsonFallbackPath = localJsonFallbackPath
            };

            _source = !string.IsNullOrWhiteSpace(localJsonFallbackPath)
                ? "local JSON fallback file"
                : !string.IsNullOrWhiteSpace(serviceUrl)
                    ? "LocalStack (via ServiceUrl override)"
                    : "AWS Secrets Manager";
            
            Log.Info("SecretsManagerAccessor initializing, backend source: {Source}", _source);
            if (!string.IsNullOrWhiteSpace(localJsonFallbackPath))
            {
                Log.Info("Resolved LocalJsonFallbackPath: {ResolvedPath}", localJsonFallbackPath);
            }

            var loggerProvider = new NLogLoggerProvider();
            var logger = loggerProvider.CreateLogger(nameof(SecretsManagerService));
            
            _instance = SecretsManagerServiceFactory.Create(settings, logger);
        }

        public static async Task<SecretFetchResult<T>> GetSecretAsync<T>(string secretName) where T : class
        {
            var secret = await EnsureInitialized().GetSecretAsync<T>(secretName);
            return new SecretFetchResult<T>(secret, secretName, _source);
        }

        private static ISecretsManagerService EnsureInitialized()
        {
            return _instance ?? throw new InvalidOperationException(
                "SecretsManagerAccessor not initialized. Call Initialize() from Application_Start.");
        }

        private static string ResolveLocalJsonFallbackPath(string configuredPath)
        {
            if (string.IsNullOrWhiteSpace(configuredPath) || Path.IsPathRooted(configuredPath))
            {
                return configuredPath;
            }

            // IIS/IIS Express's worker process working directory has nothing to do with the
            // site's physical root, so a plain relative path (unlike a console app, whose
            // working directory is its own bin\ output folder) won't resolve against the
            // fixture file's actual location without this.
            return Path.Combine(HostingEnvironment.ApplicationPhysicalPath, configuredPath);
        }
    }
}
