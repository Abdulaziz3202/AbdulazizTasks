using System.ComponentModel.DataAnnotations;

namespace KPMGTask.Models
{
    public class Permission
    {
        [Key]
        public long Id { get; set; }

        public string Name { get; set; }

        public ICollection<RolePermissions> RolePermissions { get; set; }




    }
}
