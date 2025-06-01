using Microsoft.AspNetCore.Identity;

namespace tcc_in305b.Models
{
    public class Admin
    {
        public int Id { get; set; }
        public required string Nome { get; set; }

        
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
    }
}
