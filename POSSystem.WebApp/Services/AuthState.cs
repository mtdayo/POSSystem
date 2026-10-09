//JWTトークンの状態を管理するクラス

namespace POSSystem.WebApp.Services
{
    public class AuthState
    {
        public string? Token { get; private set; }

        public bool IsLoggedIn => !string.IsNullOrEmpty(Token);

        //ログイン状態のイベント
        public event Action? OnChange;

        public void SetToken(string token)
        {
            Token = token;
            OnChange?.Invoke();
        }


        //ログアウト時にトークンをクリアするメソッドを追加
        public void Logout()
        {
            Token = null;
            OnChange?.Invoke();
        }
    }
}
