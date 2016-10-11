using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UserStore.WebLayer.Models
{
    public class LoginModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Некорректный формат email адреса!")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
    }
}