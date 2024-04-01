using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
namespace BlackJackOnline.Models
{
	public class Game
	{

		public Guid Id { get; set; }
		public Player player { get; set; }


		//nytt för split funktion test
		public List<Person> hands { get; set; }
		public Dealer dealer { get; set; }

		public UserManager<User> _userManager { get; set; }
		public GameEnums.GameState state { get; set; }
		public decimal TotalBet 
		{
			get
			{
				decimal total = 0;
				foreach(var person in hands)
				{
					total += person.Bet;
				}
				return total;
			}
			set
			{
				TotalBet = value;
			}
		}

		

        public Game(Guid id, UserManager<User> userManager)
        {
            Id = id;
            _userManager = userManager;
            dealer = new Dealer();
			player = new Player(500M, true);
            
			//nytt för split funktion test
			hands = new List<Person>();
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
				NewEndHand();
			}
		}

		public async Task DealFirst()
		{
			foreach (Person hand in hands)
			{
				await dealer.DealOpenToPlayer(hand);
			}
			state = GameEnums.GameState.DealingSecond;
		}

		public async Task DealSecond()
		{
			await dealer.DealToSelf();
			state = GameEnums.GameState.DealingThird;
		}

		public async Task DealThird()
		{
			foreach (Person hand in hands)
			{
				await dealer.DealOpenToPlayer(hand);
				
					if (hand.visibleScore == 21)
					{
						hand.Standing = true;
					}
			}
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

    //        if (player.hasBlackJack)
    //        {
				//dealer.OpenFirst();
    //            NewEndHand();
    //        }
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
				NewEndHand();
			}

		}

		public async Task DealerRevealFirst()
		{
			dealer.OpenFirst();
		}

		public async Task Hit(Person hand)
		{
			await dealer.DealOpenToPlayer(player);
			if (player.isBusted)
			{
				NewEndHand();
			}
			if(player.visibleScore == 21)
			{
				NewStand(hand);
			}
		}

        //nytt för split funktion test
        public async Task Hit(Person hand, int index)
		{
            await dealer.DealOpenToPlayer(hand);

			if (index < (hands.Count - 1))
			{

				if (hand.isBusted)
				{
					
				}
				if (hand.visibleScore == 21)
				{
					NewStand(hand);
				}
			}

			else
			{
                if (hand.isBusted)
                {
                    NewEndHand();
                }
                if (hand.visibleScore == 21)
                {
                    NewStand(hand);
                }
            }
			
        }

		//public async Task Stand()
		//{
		//	player.Standing = true;
		//	dealer.OpenFirst();
		//	dealer.Reveal();
		//	await DealerTurn();
		//	EndHand();
		//}

		public void Stand(Person hand)
		{
            hand.Standing = true;
            dealer.OpenFirst();
            if (dealer.visibleScore < 17)
            {
                state = GameEnums.GameState.DealerTurn;
            }
            else
            {
                NewEndHand();
            }
        }

		public void NewStand(Person hand)
		{
			hand.Standing = true;
			if(FindActive() == null)
			{
                dealer.OpenFirst();
                if (dealer.visibleScore < 17)
                {
                    state = GameEnums.GameState.DealerTurn;
                }
                else
                {
                    NewEndHand();
                }
            }
			else
			{
				Person nextHand = FindActive();
				nextHand.ActiveHand = true;
			}

		}
		
		public Person FindActive()
		{
			foreach(Person hand in hands)
			{
				if(!hand.Standing && !hand.isBusted)
				{
					return hand;
				}
			}
			return null;
		}

		public bool AnyActive()
		{
			bool anyFound = false;
				
				for(int i = 0; i < hands.Count; i++)
				{
					Person hand = hands[i];
						
						if (hand.ActiveHand)
						{
							return true;			
						}
				}

			return anyFound;
		}
		public async Task DoubleDown(Person hand)
		{
			hand.Standing = true;

			hand.Bet *= 2;

			await dealer.DealOpenToPlayer(hand);

			NewStand(hand);
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

					NewEndHand();
				}
				else
				{
					player.Change -= player.InsuranceBet;
				}

				state = GameEnums.GameState.InProgress;
			}
		}

		public void NewEndHand()
		{
			state = GameEnums.GameState.Payout;
			foreach(Person hand in hands)
			{
                if (hand.hasBlackJack && !dealer.hasBlackJack)
                {
                    //Player gets their bet back, plus 1.5 * the bet
                    player.Change += hand.Bet * 1.5M;
                }
                else if (!hand.isBusted && dealer.isBusted)
                {
                    player.Change += hand.Bet;
                }
                else if (!dealer.isBusted
                         && !hand.isBusted
                         && hand.visibleScore > dealer.visibleScore)
                {

                    player.Change += hand.Bet;
                }
                else if (!dealer.isBusted
                         && !hand.isBusted
                         && hand.visibleScore == dealer.visibleScore)
                {
                    //push nothing happens
                }
                //in all other cases the player loses
                else
                {
                    player.Change += hand.Bet * -1;
					hand.HasLost = true;
                }
            }
            player.Bet = 0;
            player.Standing = false;
        }
			public void EndHand()
			{
				state = GameEnums.GameState.Payout;
				if (player.hasBlackJack && !dealer.hasBlackJack)
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
				hands.Clear();
				player.Collect();
				dealer.ClearHand();
				state = GameEnums.GameState.NotStarted;

				await InitializeHand();
			}

		public async Task CreatePlayer(string userName)
		{
            User user = await _userManager.FindByNameAsync(userName);
			decimal funds = user.Funds;
			player = new Player(funds, true);
        }

        //nytt för split funktion test
        public async Task Split(Person hand)
		{
			Card card2 = hand.Hand[1];
			hand.Hand.RemoveAt(1);
			decimal bet = hand.Bet;
			Player hand2 = new Player(bet, true);
			hand2.Bet = hand2.Funds;
			hand2.Hand.Add(card2);
			hands.Add(hand2);
            await Task.WhenAll(DealToHand(hand),
			 DealToHand(hand2)
			);
        }

		public void AddHand(decimal bet)
		{
			Player hand = new Player(bet, true);
			hand.Bet = bet;
			hands.Add(hand);
		}

		public async Task DealToHand(Person hand)
		{
            await dealer.DealOpenToPlayer(hand);

            if (hand.visibleScore == 21)
            {
                hand.Standing = true;
            }
        }
		
		}
	}

