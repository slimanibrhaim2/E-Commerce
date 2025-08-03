using System.ComponentModel.DataAnnotations;

namespace WebApi.Authentication.DTOs
{
    public class ExternalSystemRegisterRequest
    {
        [Required(ErrorMessage = "اسم النظام مطلوب")]
        public string SystemName { get; set; } = string.Empty;

        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "نوع النظام مطلوب")]
        public string SystemType { get; set; } = string.Empty;

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [RegularExpression(@"^09\d{8}$", ErrorMessage = "يجب أن يبدأ رقم الهاتف بـ 09 ويتكون من 10 أرقام")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صحيح")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [MinLength(8, ErrorMessage = "كلمة المرور يجب أن تكون 8 أحرف على الأقل")]
        public string Password { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}