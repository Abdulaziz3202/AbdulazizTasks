using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KPMGTask.Models
{
    public class Role : IdentityRole<Guid>
    {

      
        public DateTime CreationDate { get; set; }


        public DateTime? LastEditDate { get; set; }



        public ICollection<RolePermissions> RolePermissions { get; set; }
    }
}
