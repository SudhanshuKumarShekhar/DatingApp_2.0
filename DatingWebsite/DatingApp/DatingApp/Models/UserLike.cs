namespace DatingApp.Models
{
    public class UserLike
    {
        public AppUser? SourceUser { get; set; }
        public int SourceUserId { get; set; }
        public AppUser? TragetUser { get; set; }
        public int TragetUserId { get; set; }
    }
}
