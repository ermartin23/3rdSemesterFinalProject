using System.ComponentModel.DataAnnotations;

namespace api
{
    public class AppOptions
    {
        [Required]
        public string DbConnectionString { get; set; } = string.Empty;

        [Required]
        public string JwtIssuer { get; set; } = string.Empty;

        [Required]
        public string JwtAudience { get; set; } = string.Empty;

        [Required, MinLength(32)]
        public string JwtSecret { get; set; } = string.Empty; }
}