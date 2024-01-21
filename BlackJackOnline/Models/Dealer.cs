namespace BlackJackOnline.Models
{
	public class Dealer : Person
	{
		public CardDeck Deck { get; set; } = new CardDeck();

		public bool HasAceShowing => Hand.Count == 2
								  && visibleScore == 11
								  && Hand.Where(x => x.IsVisible == false).Count() == 1;
		public Card Deal()
		{	
			return Deck.Draw();
        }

		public Card DealOpen()
		{
			Card card = Deck.Draw();
			card.IsVisible = true;
			return card;
		}

		public async Task DealToSelf()
		{
			await AddCard(Deal());
		}

		public async Task DealOpenToSelf()
		{
			await AddCard(DealOpen());
		}

		public void OpenFirst()
		{
			var notVisible = Hand.Where(c => c.IsVisible == false).ToList();
			Card first = notVisible.FirstOrDefault();
			first.IsVisible = true;
		}
		public async Task DealToPlayer(Player player)
		{
			await player.AddCard(Deal());
		}

		public async Task DealOpenToPlayer(Player player)
		{
			await player.AddCard(DealOpen());
		}

		public void Reveal()
		{
			foreach(Card card in Hand)
			{
				card.IsVisible = true;
			}
		}
	}
}
