using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KPMGTask.Models
{
    public class User : IdentityUser<Guid>


    {
        public DateTime CreationDate { get; set; }
        public DateTime? LastEditDate { get; set; }


        [Required, MaxLength(50)]
        public string FirstName { get; set; }
        [Required, MaxLength(50)]
        public string LastName { get; set; }

        //we don't need to add DbSet becuase we have added owned annotation above the class name
        public List<RefreshToken>? RefreshTokens { get; set; }

     
        public string Status { get; set; }

        public bool? ForgotPasswordRequested { get; set; }
        public DateTime? LatestForgotPasswordRequest { get; set; }
        public long? NumberOfLogins { get; set; }

        public DateTime? LastLoginTime { get; set; }

        public bool? ForgotPaswordCompleted { get; set; }

        public FinancialAccount FinancialAccount { get; set; }


        public ICollection<UserNotification> UserNotifications { get; set; }


    }
}
