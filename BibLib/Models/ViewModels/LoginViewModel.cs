using System.ComponentModel.DataAnnotations;

namespace BibLib.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Электронная почта")]
        [EmailAddress(ErrorMessage = "Неверный адрес")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
        [Display(Name = "Запомнить меня")]
        public bool RememberMe { get; set; }
    }
}