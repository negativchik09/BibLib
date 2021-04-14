using System.ComponentModel.DataAnnotations;

namespace BibLib.Models.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Электронная почта")]
        [EmailAddress(ErrorMessage = "Неверный адрес")]
        public string Email { get; set; }
    }
}