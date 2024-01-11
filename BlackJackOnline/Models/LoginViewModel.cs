using System.ComponentModel.DataAnnotations;

namespace BlackJack.Models
{
	public class LoginViewModel
	{
		[Required(ErrorMessage = "Please choose a Username")]
		public string UserName { get; set; }

		[Required(ErrorMessage = "Please choose a Password")]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		public bool RememberMe { get; set; }
	}
}
