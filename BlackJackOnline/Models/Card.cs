using static BlackJackOnline.Models.GameEnums;

namespace BlackJackOnline.Models
{
	public class Card
	{
		public CardSuit Suit { get; set; }
		public CardValue Value { get; set; }

		public bool IsVisible { get; set; }

		public int Id { get; set; }

		public string ImageName { 
			get{
				return $"{Value.ToString()}_of_{Suit.ToString()}.png";
			}
			set
			{
				ImageName = value;
			}
		}
		public int score()
		{
			if(Value == CardValue.Jack && Value == CardValue.Queen && Value == CardValue.King)
			{
				return 10;
			}
			if(Value == CardValue.Ace)
			{
				return 11;
			}
			return (int)Value;
		}

	}
}
