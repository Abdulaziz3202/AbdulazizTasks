using System.ComponentModel.DataAnnotations;

namespace KPMGTask.Dtos
{
    public class AddRoleDto
    {

        [Required]
        public string UserId { get; set; }
        [Required]
        public string RoleName { get; set; }

        [Required]
        public Guid ApplicationOrganizationId { get; set; }

    }
}
