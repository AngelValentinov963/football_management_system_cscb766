namespace football_management_system_cscb.Models
{
    using Microsoft.AspNetCore.Identity;

    public class ApplicationUser : IdentityUser
    {
        public int? TeamId { get; set; }
    }
}
