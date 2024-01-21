namespace BlackJackOnline.Models
{
	public class Player : Person
	{
		public decimal Funds { get; set; }

		public decimal Bet { get; set; }
		public decimal InsuranceBet { get; set; }
		public bool HasInsurance => InsuranceBet > 0;

		//Hur mycket funds ska ändras med
		public decimal Change {  get; set; }
		
		//om spelar valt att stå istället för att fortsätta...
		public bool Standing { get; set; }

		public Player(decimal funds)
		{
			Funds = funds;
		}

		public void Collect()
		{
			Funds += Change;
			Change = 0M;
			InsuranceBet = 0M;
		}

	}
}
