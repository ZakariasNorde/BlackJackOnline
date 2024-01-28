using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
namespace BlackJackOnline.Models
{
	public class Game
	{

		public Guid Id { get; set; }
		public Player player { get; set; }

		public Dealer dealer { get; set; }

		public UserManager<User> _userManager { get; set; }
		public GameEnums.GameState state { get; set; }

        public Game(Guid id, UserManager<User> userManager)
        {
            Id = id;
            _userManager = userManager;
            dealer = new Dealer();
			player = new Player(500M);
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
				await DealFirst();
			}
		}

		public async Task Deal()
		{
			state = GameEnums.GameState.Dealing;
			await dealer.DealOpenToPlayer(player);

			await dealer.DealOpenToSelf();

			await dealer.DealOpenToPlayer(player);

			await dealer.DealToSelf();

			state = GameEnums.GameState.InProgress;

			if (player.hasBlackJack)
			{
				EndHand();
			}
		}

		public async Task DealFirst()
		{
			await dealer.DealOpenToPlayer(player);
			state = GameEnums.GameState.DealingSecond;
		}

		public async Task DealSecond()
		{
			await dealer.DealToSelf();
			state = GameEnums.GameState.DealingThird;
		}

		public async Task DealThird()
		{
			await dealer.DealOpenToPlayer(player);
			state = GameEnums.GameState.DealingFourth;
		}

		public async Task DealFourth()
		{
			await dealer.DealOpenToSelf();
			state = GameEnums.GameState.DealingLast;
		}

		public void EndDealing()
		{
            state = GameEnums.GameState.InProgress;

            if (player.hasBlackJack)
            {
                EndHand();
            }
        }
		public async Task DealerTurn()
		{
			state = GameEnums.GameState.DealerTurn;
			if (dealer.visibleScore < 17)
			{
				await dealer.DealOpenToSelf();
				//await DealerTurn();
			}
			else
			{
				EndHand();
			}

		}

		public async Task DealerRevealFirst()
		{
			dealer.OpenFirst();
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
			dealer.OpenFirst();
			dealer.Reveal();
			await DealerTurn();
			EndHand();
		}

		public void NewStand()
		{
            player.Standing = true;
            dealer.OpenFirst();
            if (dealer.visibleScore < 17)
            {
                state = GameEnums.GameState.DealerTurn;
            }
            else
            {
                EndHand();
            }
        }

		public async Task DoubleDown()
		{
			player.Standing = true;

			player.Bet *= 2;

			await dealer.DealOpenToPlayer(player);

			NewStand();
		}

		public void Insurance()
		{
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

		public async Task CreatePlayer(string userName)
		{
            User user = await _userManager.FindByNameAsync(userName);
			decimal funds = user.Funds;
			player = new Player(funds);
        }

		
		}
	}

