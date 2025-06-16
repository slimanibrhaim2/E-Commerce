using System.ComponentModel.DataAnnotations;

namespace WebApi.Authentication.DTOs
{
    public class VerifyOtpRequest
    {
        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "رمز التحقق مطلوب")]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "يجب أن يتكون رمز التحقق من 4 أرقام بالضبط")]
        public string Otp { get; set; } = string.Empty;
    }
} 