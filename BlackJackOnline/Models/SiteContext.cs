using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace BlackJackOnline.Models
{
	public class SiteContext : IdentityDbContext<User>
	{
		public SiteContext(DbContextOptions<SiteContext> options) : base(options) { }
	}
}
