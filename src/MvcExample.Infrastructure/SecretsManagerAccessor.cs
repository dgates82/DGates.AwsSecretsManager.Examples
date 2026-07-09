using System;
using System.Configuration;
using System.Threading.Tasks;
using DGates.AwsSecretsManager;

namespace MvcExample.Infrastructure
{
    /// <summary>
    /// Wraps the singleton <see cref="ISecretsManagerService"/> so consumers never need to
    /// reference DGates.AwsSecretsManager directly.
    /// </summary>
    public static class SecretsManagerAccessor
    {
        private static ISecretsManagerService _instance;

        public static void Initialize()
        {
            var settings = new SecretsManagerSettings
            {
                ServiceUrl = ConfigurationManager.AppSettings["AWSServiceURL"],
                Region = ConfigurationManager.AppSettings["AWSRegion"],
                AccessKey = ConfigurationManager.AppSettings["AWSAccessKey"],
                SecretKey = ConfigurationManager.AppSettings["AWSSecretKey"],
                LocalJsonFallbackPath = ConfigurationManager.AppSettings["LocalJsonFallbackPath"]
            };

            _instance = SecretsManagerServiceFactory.Create(settings);
        }

        public static Task<T> GetSecretAsync<T>(string secretName) where T : class
        {
            return EnsureInitialized().GetSecretAsync<T>(secretName);
        }

        private static ISecretsManagerService EnsureInitialized()
        {
            return _instance ?? throw new InvalidOperationException(
                "SecretsManagerAccessor not initialized. Call Initialize() from Application_Start.");
        }
    }
}
