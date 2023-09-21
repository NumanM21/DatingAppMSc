
// contain properties we want to display inside card component in member-list.component

namespace API.DTOs
{
    public class LikeUserDto
    {
        // same casing as memberDto
        public int Id { get; set; }

        public string Username { get; set; }
        public int Age { get; set; }
        public string KnownAs { get; set; }
        public string City { get; set; }
        public string photoUrl { get; set; }
    }
}