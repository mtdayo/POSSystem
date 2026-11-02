namespace POSSystem.Infrastructure.Repositories
{
    public static class DatabaseConnection
    {
        public static readonly string ConnectionString = 
            $"Data Source={Path.GetFullPath(Path.Combine(AppContext.BaseDirectory,"..","..","..","..","POSSystem.Infrastructure","Database","pos.db"))}";
    }
}
