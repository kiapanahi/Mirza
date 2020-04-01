namespace Mirza.Web.Models
{
    public class UserTeam
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public MirzaUser User { get; set; }
        public int TeamId { get; set; }
        public Team Team { get; set; }
    }
}