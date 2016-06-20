namespace WebServer.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using IdentityServer3.Core.Models;
    using IdentityServer3.Core.Services;
    public class UriValidator : IRedirectUriValidator
    {
        // TODO: Get the subdomains from the provider/tenant
        private List<string> uris = new List<string>
                                        {
                                            "https://cheese.servertest.local:44310/",
                                            "https://fish.servertest.local:44310/"
                                        };

        public Task<bool> IsRedirectUriValidAsync(string requestedUri, Client client)
        {            
            var result = this.uris.Contains(requestedUri);
            return Task.FromResult(result);
        }

        public Task<bool> IsPostLogoutRedirectUriValidAsync(string requestedUri, Client client)
        {
            var result = this.uris.Contains(requestedUri);
            return Task.FromResult(result);
        }
    }
}