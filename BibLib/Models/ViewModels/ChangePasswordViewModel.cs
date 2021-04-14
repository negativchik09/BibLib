using System.ComponentModel.DataAnnotations;

namespace BibLib.Models.ViewModels
{
    public class ChangePasswordViewModel
    {
        public string Email { get; set; }
        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Подтвердите пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
    }
}
