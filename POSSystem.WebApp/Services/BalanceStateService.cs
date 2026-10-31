namespace POSSystem.WebApp.Services
{
    public class BalanceStateService
    {
        public event Action? OnChange;

        public void NotifyBalanceChanged()
        {
            OnChange?.Invoke();
        }
    }
}
