using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BibLib.Models.ViewModels
{
    public class RegistrationViewModel
    {
        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Электронная почта")]
        [EmailAddress(ErrorMessage = "Неверный адрес")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Подтвердите электронную почту")]
        [Compare("Email", ErrorMessage = "Адреса почты не совпадают")]
        public string ConfirmEmail { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Подтвердите пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Секретный вопрос")]
        public string SecretQuestion { get; set; }
        
        public List<string> ListOfSecretQuestions { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Ответ на секреный вопрос")]
        public string Answer { get; set; }
    }
}