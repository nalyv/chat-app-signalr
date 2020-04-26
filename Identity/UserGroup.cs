namespace chat_application.Identity
{
    public class UserGroup
    {
        public int UserGroupId { get; set; }
        public AppIdentityUser User { get; set; }
        public Group Group { get; set; }
    }
}