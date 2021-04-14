using System.ComponentModel.DataAnnotations;

namespace BibLib.Models.ViewModels
{
    public class SecurityCheckViewModel
    {
        public string Email { get; set; }
        [Display(Name = "Секретный вопрос")]
        public string Question { get; set; }
        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Ответ на секретный вопрос")]
        public string Answer { get; set; }
    }
}