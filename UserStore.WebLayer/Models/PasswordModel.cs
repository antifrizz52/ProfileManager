using System.ComponentModel.DataAnnotations;

namespace UserStore.WebLayer.Models
{
    public class PasswordModel
    {
        [Required(ErrorMessage = "Поле не может быть пустым!")]
        [DataType(DataType.Password)]
        [Display(Name = "Текущий пароль")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Поле не может быть пустым!")]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Поле не может быть пустым!")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают!")]
        [Display(Name = "Повторите пароль")]
        public string ConfirmPassword { get; set; }
    }
}