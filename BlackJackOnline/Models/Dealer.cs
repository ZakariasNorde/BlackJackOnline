using System.ComponentModel.DataAnnotations.Schema;

namespace BlackJackOnline.Models
{
	public class Dealer : Person
	{
        public int deckId { get; set; }
        [ForeignKey(nameof(deckId))]
        public virtual CardDeck Deck { get; set; } = new CardDeck();

        public bool HasAceShowing => Hand.ToList().Count == 2
								  && visibleScore == 11
								  && Hand.Where(x => x.IsVisible == false).Count() == 1;
		public Card Deal()
		{
			return Deck.Draw();
		}

		public async Task DealToSelf()
		{
			await AddCard(Deal());
		}

		public async Task DealToPlayer(Player player)
		{
			await player.AddCard(Deal());
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
