using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Grpc.Server;
using Xunit;

namespace Grpc.Tests.FunctionalTests.Helpers
{
    public class FunctionalTestBase : IClassFixture<GrpcServerFactory<Startup>>, IDisposable
    {
        private const string Address = "https://localhost:5001";
        protected readonly GrpcChannel _channel;
        protected readonly HttpClient _client;
        public FunctionalTestBase(GrpcServerFactory<Startup> factory)
        {
            _client = factory.CreateDefaultClient(new ResponseVersionHandler());
            _channel = GrpcChannel.ForAddress(Address, new GrpcChannelOptions { HttpClient = _client });
        }
        private class ResponseVersionHandler : DelegatingHandler
        {
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var response = await base.SendAsync(request, cancellationToken);
                response.Version = request.Version;

                return response;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _channel.Dispose();
                    _client.Dispose();
                }
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
