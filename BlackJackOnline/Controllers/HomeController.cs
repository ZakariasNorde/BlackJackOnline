using System.Diagnostics;
using BlackJackOnline.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlackJackOnline.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private UserManager<User> _userManager;
		private SiteContext _context;

		public HomeController(ILogger<HomeController> logger, SiteContext context, 
				UserManager<User> userManager)
		{
			_logger = logger;
			_context = context;
			_userManager = userManager;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult LeaderBoard()
		{
			var allUsers = _context.Users.OrderByDescending(u => u.Funds).ToList();
			return View(allUsers);
		}
		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
