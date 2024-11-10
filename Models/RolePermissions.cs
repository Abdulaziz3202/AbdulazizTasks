using System.ComponentModel.DataAnnotations.Schema;

namespace KPMGTask.Models
{
    public class RolePermissions 
    {

        public Guid Id { get; set; }
        public long ApplicationPermissionId { get; set; }
        public Permission ApplicationPermission { get; set; }

        public Guid ApplicationRoleId { get; set; }
        public Role ApplicationRole { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreationDate { get; set; }

        public DateTime? LastEditDate { get; set; }


        public string? AllowedOptions { get; set; }//this field contains the options that user is allowd to view (e.x Analyte Group options).

    }
}
