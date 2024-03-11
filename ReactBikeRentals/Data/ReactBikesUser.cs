using Microsoft.AspNetCore.Identity;

namespace ReactBikes.Data
{
    public enum Roles
    {
        Manager,
        User
    }
    public class ReactBikesUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte[]? ProfilePicture { get; set; }
    }
}
