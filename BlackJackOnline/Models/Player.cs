namespace BlackJackOnline.Models
{
	public class Player : Person
	{
		public decimal Funds { get; set; }

		public decimal InsuranceBet { get; set; }
		public bool HasInsurance => InsuranceBet > 0;

		//Hur mycket funds ska ändras med
		public decimal Change {  get; set; }
		
		

		public Player(decimal funds, bool Active)
		{
			Funds = funds;
			ActiveHand = Active;
			HasLost = false;
		}

		public void Collect()
		{
			Funds += Change;
			Change = 0M;
			InsuranceBet = 0M;
		}

	}
}
