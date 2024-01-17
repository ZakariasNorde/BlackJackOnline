namespace BlackJackOnline.Models
{
	public class Game
	{

		public Guid Id { get; set; }
		public Player player {  get; set; }

		public Dealer dealer { get; set; }

		public GameEnums.GameState state { get; set; }

		public Game(Guid id)
		{
			Id = id;
			player = new Player();
			dealer = new Dealer();
			state = GameEnums.GameState.NotStarted;
		}
		public async Task Delay(int millis)
		{
			await Task.Delay(millis);
		}

		public async Task InitializeHand()
		{
			if (dealer.Deck.Count < 13)
			{
				state = GameEnums.GameState.Shuffling;
				dealer.Deck = new CardDeck();
				await Delay(1000);
			}

			state = GameEnums.GameState.Betting;
		}

		public async Task Bet(decimal amount)
		{
			if (player.Funds >= amount)
			{
				player.Bet += amount;
				await Deal();
			}
		}

		public async Task Deal()
		{
			state = GameEnums.GameState.Dealing;
			await dealer.DealOpenToPlayer(player);

			var dealerCard = dealer.Deal();
			dealerCard.IsVisible = false;
			await dealer.AddCard(dealerCard);
		
			await dealer.DealToPlayer(player);
			
			await dealer.DealToSelf();
			
			state = GameEnums.GameState.InProgress;

			if (player.hasBlackJack)
			{
				EndHand();
			}
		}

		public async Task DealerTurn()
		{
			if (dealer.visibleScore < 17)
			{
				await dealer.DealToSelf();
				await DealerTurn();
			}
		}

		public async Task Hit()
		{
			await dealer.DealOpenToPlayer(player);
			if (player.isBusted)
			{
				EndHand();
			}
		}

		public async Task Stand()
		{
			player.Standing = true;
			dealer.Reveal();

			await DealerTurn();

			EndHand();
		}

		public async Task DoubleDown()
		{
			player.Standing = true;

			player.Bet *= 2;

			await Delay(300);

			await player.AddCard(dealer.Deal());

			await Stand();
		}

		public void Insurance()
		{
			state = GameEnums.GameState.Insurance;

			if (dealer.HasAceShowing)
			{
				player.InsuranceBet = player.Bet / 2;

				if (dealer.totalScore == 21)
					dealer.Reveal();

				player.Change += player.InsuranceBet * 2;

				state = GameEnums.GameState.Payout;
				
				EndHand();
			}
			else
			{
				player.Change -= player.InsuranceBet;
			}

			state = GameEnums.GameState.InProgress;
		}

		public void EndHand()
		{
			state = GameEnums.GameState.Payout;
			if (player.hasBlackJack && dealer.visibleScore != 21)
			{
				//Player gets their bet back, plus 1.5 * the bet
				player.Change += player.Bet * 1.5M;
			}
			else if (!player.isBusted && dealer.isBusted)
			{
				player.Change += player.Bet;
			}
			else if (!dealer.isBusted
					 && !player.isBusted
					 && player.visibleScore > dealer.visibleScore)
			{

				player.Change += player.Bet;
			}
			else if (!dealer.isBusted
					 && !player.isBusted
					 && player.visibleScore == dealer.visibleScore)
			{
				//push nothing happens
			}
			//in all other cases the player loses
			else
			{
				player.Change += player.Bet * -1;
			}

			player.Bet = 0;
			player.Standing = false;
		}

		public async Task NewHand()
		{
			//Player gets paid
			player.Collect();

			player.ClearHand();
			dealer.ClearHand();

			state = GameEnums.GameState.NotStarted;

			await InitializeHand();
		}
	}
}
