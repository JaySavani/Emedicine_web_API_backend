using Microsoft.EntityFrameworkCore;

namespace EMedicine.Models
{
    [Keyless]
    public class Loginuser
    {
        public string Password { get; set; }
        public string Email { get; set; }
    }
}
