using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UserStore.WebLayer.Models
{
    public class DepartmentModel
    {
        [Display(Name = "№")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите наименование отдела!")]
        [Display(Name = "Наименование")]
        public string Name { get; set; }

        public virtual ICollection<UserProfileModel> Users { get; set; } 
    }
}