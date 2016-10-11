using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserStore.DataLayer.Entities
{

    public class Department
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<UserProfile> Users { get; private set; }
    }
}
