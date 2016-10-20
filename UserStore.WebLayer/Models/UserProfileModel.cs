using System.ComponentModel.DataAnnotations;

namespace UserStore.WebLayer.Models
{
    public class UserProfileModel
    {
        [Display(Name = "№")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите email адрес!")]
        [EmailAddress(ErrorMessage = "Некорректный формат email адреса!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Введите имя сотрудника!")]
        [Display(Name = "Имя")]
        public string Name { get; set; }

        [Display(Name = "Адрес")]
        public string Address { get; set; }

        [Display(Name = "Телефон")]
        public string Phone { get; set; }


        [Display(Name = "Системная роль")]
        public Role Role { get; set; }

        [Display(Name = "Наименование отдела")]
        public int? DepartmentId { get; set; }
    }

    public enum Role
    {
        [Display(Name = "Пользователь")]
        User,

        [Display(Name = "Администратор")]
        Admin
    }
}