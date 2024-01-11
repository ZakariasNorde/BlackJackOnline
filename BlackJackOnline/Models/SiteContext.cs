using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BlackJackOnline.Models;
namespace BlackJackOnline.Models
{
	public class SiteContext : IdentityDbContext<User>
	{
		public SiteContext(DbContextOptions<SiteContext> options) : base(options) { }
		DbSet<User> users { get; set; }
	}
}
