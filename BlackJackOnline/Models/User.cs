using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlackJackOnline.Models
{
	public class User : IdentityUser
	{
		public decimal Funds {  get; set; }

		public User(decimal funds ) : base()
        {
            Funds = funds;
        }
    }
}
