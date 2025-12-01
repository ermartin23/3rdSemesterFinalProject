namespace api
{
    public class AppOptions
    {
        public string DbConnectionString { get; set; } = string.Empty;
        public string JwtSecret { get; set; } = string.Empty;
    }
}