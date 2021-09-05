using System.ComponentModel.DataAnnotations;

namespace MyApiTemplate.Domain.Auth
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
