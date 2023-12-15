namespace VirtualPaymentService.Business.Interface
{
    public interface ISecretConfigurationService
    {
        /// <summary>
        /// Gets the secret value for the secret key passed in.
        /// </summary>
        /// <param name="secretKeyName">Key name of the secret on the secret store.</param>
        /// <returns>Secret value.</returns>
        public string GetSecretValueForKey(string secretKeyName);
    }
}
