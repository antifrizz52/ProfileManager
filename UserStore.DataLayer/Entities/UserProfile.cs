using System.ComponentModel.DataAnnotations.Schema;
using UserStore.DataLayer.Models;

namespace UserStore.DataLayer.Entities
{
    
    public class UserProfile
    {
        [ForeignKey("AppUser")]
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public string Email { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }
        
        [ForeignKey("Department")]
        public int? DepartmentId { get; set; }
        
        public virtual Department Department { get; set; }
        public virtual AppUser AppUser { get; set; }
    }
}
