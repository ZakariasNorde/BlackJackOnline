using static BlackJackOnline.Models.CardEnums;

namespace BlackJackOnline.Models
{
	public class Person
	{
		public List<Card> Hand {  get; set; } = new List<Card>();

		public int visibleScore 
		{
			get
			{
				return score();
			}
		
		}

		public bool hasBlackJack => Hand.Count == 2 && visibleScore == 21;

		public bool isBusted => visibleScore > 21; 
		public int score()
		{	
			int score = 0;
			List<Card> visibleCards = Hand.Where(c => c.IsVisible).ToList();
			foreach(Card card in visibleCards)
			{
				score += card.score();
			}
			if(score <= 21)
			{
				return score;
			}
			else
			{
				bool hasAce = visibleCards.Any(c => c.Value == CardValue.Ace);
				int aceCount = visibleCards.Where(c => c.Value == CardValue.Ace).Count();
				
				if (hasAce)
				{
					for (int i = 0; i < aceCount; i++)
					{
						if(score - (i * 10) <= 21)
						{
							return score - (i * 10);
						}
					}
				}
				return score;
			}
		}
		public async Task AddCard(Card card)
		{
			Hand.Add(card);
			await Task.Delay(300);
		}

		public void ClearHand()
		{
			Hand.Clear();
		}
	}
}
