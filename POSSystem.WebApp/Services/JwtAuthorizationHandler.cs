using System.Net.Http.Headers;

namespace POSSystem.WebApp.Services
{
    public class JwtAuthorizationHandler : DelegatingHandler
    {
        private readonly AuthState _authState;


        public JwtAuthorizationHandler(AuthState authState)
        {
            _authState = authState;
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Console.WriteLine(
    $"Handler側 JWT存在: {!string.IsNullOrEmpty(_authState.Token)}");

            Console.WriteLine(
                $"Handler側 AuthState: {_authState.GetHashCode()}");


            if (!string.IsNullOrEmpty(_authState.Token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authState.Token);

                Console.WriteLine("Authorizationヘッダーを追加しました");
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
