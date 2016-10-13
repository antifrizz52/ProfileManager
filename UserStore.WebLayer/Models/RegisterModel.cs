using System.ComponentModel.DataAnnotations;

namespace UserStore.WebLayer.Models
{
    public class RegisterModel
    {
        [Required]
        [Display(Name = "Имя")]
        public string Name { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Некорректный формат email адреса!")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [Display(Name = "Повторите пароль")]
        public string ConfirmPassword { get; set; }
    }
}