using BlackJackOnline.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlackJackOnline.Controllers
{
    public class GameController : Controller

    {
        public Game game { get; set; }
        public GameController() 
        {
            Dealer newDealer = new Dealer();
            Player newPlayer = new Player();
            game = new Game { dealer = newDealer, player = newPlayer, state = GameEnums.GameState.NotStarted };
        }
        public IActionResult BlackJack()
        {
            
            return View(game);
        }

        [HttpPost]
        public async Task<IActionResult> BlackJack(string changed)
        {
            switch (changed)
            {
                case "Start":
                    {
                        await game.InitializeHand();
                        break;
                    }
            }
            return View(game);
        }
    }

}
