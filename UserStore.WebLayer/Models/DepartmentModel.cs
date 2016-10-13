using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UserStore.WebLayer.Models
{
    public class DepartmentModel
    {
        [Display(Name = "№")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите наименование отдела!")]
        [Display(Name = "Наименование отдела")]
        public string Name { get; set; }

        public virtual ICollection<UserProfileModel> Users { get; set; } 
    }
}