namespace BlackJackOnline.Models
{
	public class Player : Person
	{
		public decimal Funds { get; set; }

		public string Test {  get; set; }
		public decimal Bet { get; set; }
		public decimal InsuranceBet { get; set; }
		public bool HasInsurance => InsuranceBet > 0;

		//Hur mycket funds ska ändras med
		public decimal Change {  get; set; }
		
		//om spelar valt att stå istället för att fortsätta...
		public bool Standing { get; set; }

		public Player()
		{
			Funds = 500M;
			Test = "hejj";
		}

		public void Collect()
		{
			Funds += Change;
			Change = 0M;
			InsuranceBet = 0M;
		}

	}
}
