﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UserStore.WebLayer.Models
{
    public class UserProfileModel
    {
        [Display(Name = "№")]
        public int Id { get; set; }

        public string Email { get; set; }

        [Display(Name = "Имя")]
        public string Name { get; set; }

        [Display(Name = "Адрес")]
        public string Address { get; set; }

        [Display(Name = "Телефон")]
        public string Phone { get; set; }

        public string Role { get; set; }

        public int? DepartmentId { get; set; }

        [Display(Name = "Отдел")]
        public string Department { get; set; }
        //public virtual DepartmentModel Department { get; set; }
    }
}