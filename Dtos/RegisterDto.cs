using System.ComponentModel.DataAnnotations;

namespace KPMGTask.Dtos
{
    public class RegisterDto
    {

        [Required, StringLength(100)]
        public string FirstName { get; set; }

        [Required, StringLength(100)]
        public string LastName { get; set; }

        [Required, StringLength(50)]
        public string UserName { get; set; }
        [Required, StringLength(60)]
        public string RoleName { get; set; }

        [Required, StringLength(128)]
        public string Email { get; set; }

        [Required, StringLength(256)]
        public string Password { get; set; }
        public string PhoneNumber { get; set; }


    }
}
