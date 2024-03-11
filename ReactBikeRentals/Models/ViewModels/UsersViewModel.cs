using ReactBikes.Data;

namespace ReactBikes.Models
{
    public class UserRolesViewModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public bool Selected { get; set; }
    }
    public class UsersViewModel
    {
        public ReactBikesUser? User { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public List<UserRolesViewModel>? UserRolesViewModels { get; set; }
    }
}
