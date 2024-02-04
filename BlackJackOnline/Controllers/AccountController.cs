using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using BlackJackOnline.Models;
using BlackJack.Models;

namespace BlackJackOnline.Controllers
{
	public class AccountController : Controller
	{
		private UserManager<User> userManager;
		private SignInManager<User> signInManager;
		private SiteContext _context;

		public AccountController(SiteContext context, UserManager<User> userManager, SignInManager<User> signInManager)
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
			_context = context;
		}

		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
		{
			try
			{
				if (ModelState.IsValid)
				{   //creates an empty user object where and assigns the from the registerviewmodel
					User user = new User(500M);
					user.UserName = registerViewModel.UserName;
					//creates the user with the usermanager and if succeded the new user gets signed in 
					//and a message is displayed
					var result = await userManager.CreateAsync(user, registerViewModel.Password);
					if (result.Succeeded)
					{
						await signInManager.SignInAsync(user, isPersistent: true);
						TempData["AlertMessage"] = "User was registered succesfully";
						return RedirectToAction("BlackJack", "Game");
					}
					else
					{
						foreach (var error in result.Errors)
						{
							ModelState.AddModelError(string.Empty, error.Description);
						}
					}
				}

				return View();
			}
			catch (DbUpdateException Dbex)
			{
				return View("Views/Error/ErrorView.cshtml", "An error has occured in the database");
			}
			catch (Exception ex)
			{
				return View("Views/Error/ErrorView.cshtml", "The new user could not be registered");
			}
		}

		[HttpGet]

		public IActionResult Login()
		{
			LoginViewModel loginViewModel = new LoginViewModel();
			return View(loginViewModel);
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel loginViewModel)
		{
			try
			{
				if (ModelState.IsValid)
				{   //uses the signinmanager class to attempt a sign in by sending the values from the loginviewmodel
					//to the signinmanagers method signin if succeded the user is signed in if not a message is displayed
					var result = await signInManager.PasswordSignInAsync(
					loginViewModel.UserName, loginViewModel.Password, isPersistent: loginViewModel.RememberMe,
					lockoutOnFailure: false);
					if (result.Succeeded)
					{
						return RedirectToAction("BlackJack", "Game");
					}
					else
					{
						ModelState.AddModelError("", "Wrong username/password");
					}
				}
				return View(loginViewModel);
			}
			catch (DbUpdateException Dbex)
			{
				return View("Views/Error/ErrorView.cshtml", "An error has occured in the database");
			}
			catch (Exception ex)
			{
				return View("Views/Error/ErrorView.cshtml", "The user could not sign in");
			}
		}

		[HttpPost]
		public async Task<IActionResult> Logout()
		{
			await signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}

		//[Authorize]
		//[HttpGet]
		//public IActionResult ChangePassword()
		//{
		//	ChangePasswordViewModel viewModel = new ChangePasswordViewModel();
		//	return View(viewModel);
		//}


		//[HttpPost]
		//public async Task<IActionResult> ChangePassword(ChangePasswordViewModel changePasswordViewModel)
		//{
		//	try
		//	{
		//		//takes the values from the changepassword viewmodel and attempts password change
		//		//if the user input is correct the password is changed
		//		if (ModelState.IsValid)
		//		{
		//			var users = _context.Users;
		//			User user = users.Where(user => user.UserName == User.Identity.Name).First();
		//			var result = await userManager.ChangePasswordAsync(user, changePasswordViewModel.CurrentPassword, changePasswordViewModel.NewPassword);
		//			if (result.Succeeded)
		//			{
		//				TempData["AlertMessage"] = "Password was updated succesfully";

		//				return RedirectToAction("Search", "Resume");
		//			}
		//			else
		//			{
		//				//adds errors so they can be displayed
		//				foreach (var error in result.Errors)
		//				{
		//					ModelState.AddModelError(string.Empty, error.Description);
		//				}

		//			}
		//		}
		//		return View(changePasswordViewModel);
		//	}
		//	catch (DbUpdateException Dbex)
		//	{
		//		return View("Views/Error/ErrorView.cshtml", "An error has occured in the database");
		//	}
		//	catch (Exception ex)
		//	{
		//		return View("Views/Error/ErrorView.cshtml", "The user could not change password");
		//	}
		//}

		//[Authorize]
		//[HttpGet]
		//public IActionResult ChangeUserName()
		//{
		//	return View();
		//}

		//[HttpPost]
		//public async Task<IActionResult> ChangeUserName(ChangeUserNameViewModel changeUserNameViewModel)
		//{
		//	try
		//	{
		//		//takes the values from the changeUserNameviewmodel and attempts Username change
		//		//if the user input is correct the Username is changed
		//		if (ModelState.IsValid)
		//		{
		//			var users = _context.Users;
		//			User user = users.Where(user => user.UserName == User.Identity.Name).First();
		//			user.UserName = changeUserNameViewModel.NewUserName;
		//			var result = await userManager.UpdateAsync(user);
		//			if (result.Succeeded)
		//			{
		//				TempData["AlertMessage"] = "Username was updated succesfully";

		//				return RedirectToAction("Search", "Resume");
		//			}
		//			else
		//			{
		//				foreach (var error in result.Errors)
		//				{
		//					ModelState.AddModelError(string.Empty, error.Description);
		//				}
		//				return View(changeUserNameViewModel);
		//			}
		//		}
		//		else
		//		{
		//			return View(changeUserNameViewModel);
		//		}

		//	}
		//	catch (DbUpdateException Dbex)
		//	{
		//		return View("Views/Error/ErrorView.cshtml", "An error has occured in the database");
		//	}
		//	catch (Exception ex)
		//	{
		//		return View("Views/Error/ErrorView.cshtml", "The username could not be changed");
		//	}
		//}

	}

}
