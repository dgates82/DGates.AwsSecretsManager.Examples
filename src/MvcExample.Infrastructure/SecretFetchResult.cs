namespace MvcExample.Infrastructure
{
    /// <summary>
    /// A fetched secret plus metadata about how it was retrieved, so callers can show that
    /// behavior in a UI without needing to reference DGates.AwsSecretsManager themselves.
    /// </summary>
    public class SecretFetchResult<T> where T : class
    {
        public T Secret { get; }
        public string SecretName { get; }
        public string Source { get; }

        public SecretFetchResult(T secret, string secretName, string source)
        {
            Secret = secret;
            SecretName = secretName;
            Source = source;
        }
    }
}
