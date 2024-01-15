using static BlackJackOnline.Models.GameEnums;
namespace BlackJackOnline.Models
{
    public class CardDeck
    {
        public Stack<Card> Cards { get; set; } = new Stack<Card>();

        public CardDeck() 
        { 
            List<Card> cards = new List<Card>();
            
            foreach(CardSuit suit in (CardSuit[])Enum.GetValues(typeof(CardSuit)))
            {
                foreach(CardValue value in (CardValue[])Enum.GetValues(typeof(CardValue)))
                {
                    Card card = new Card { Suit = suit, Value = value, IsVisible = false};
                    cards.Add(card);
                }
            }
			var array = cards.ToArray();

			Random rnd = new Random();

			// For each unshuffled item in the collection
			for (int n = array.Count() - 1; n > 0; --n)
			{
				//Randomly pick an element 
				//  which has not been shuffled
				int k = rnd.Next(n + 1);

				//Swap the selected element with the 
				//  last "unstruck" element in the collection
				Card temp = array[n];
				array[n] = array[k];
				array[k] = temp;
			}

			// insert the now-shuffled cards into the Cards property.
			for (int n = 0; n < array.Count(); n++)
			{
				Cards.Push(array[n]);
			}
		}
        public int Count
        {
            get
            {
                return Cards.Count;
            }
        }

        public void Add(Card card)
        {
            Cards.Push(card);
        }
        public Card Draw()
        {
            return Cards.Pop();
        }
    }
}
