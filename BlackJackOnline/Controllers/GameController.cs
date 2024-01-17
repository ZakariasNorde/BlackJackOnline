using BlackJackOnline.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace BlackJackOnline.Controllers
{
    public class GameController : Controller

    {
        public static List<Game> games = new List<Game>();
       
        public IActionResult BlackJack()
        {
            try
            {
                Guid gameId = Guid.NewGuid(); // a unique id that is assigned to each game and to the user's session variable
                games.Add(new Game(gameId));
                HttpContext.Session.SetString("gameId", gameId.ToString());
                Game game = GetGameFromSession();
                string test = game.player.Test;
                return View(game);
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex.ToString();
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> BlackJack(string changed)
        {
            Game game = GetGameFromSession();
            string test = game.player.Test;
            switch (changed)
            {
                case "Start":
                    {
                        await game.InitializeHand();
                        break;
                    }
                case "bet10":
                    {
                        await game.Bet(10);
                        break;
                    }
                case "bet20":
                    {
                        await game.Bet(20);
                        break;
                    }
                case "bet50":
                    {
                        await game.Bet(50);
                        break;
                    }
                case "stand":
                    {
                        await game.Stand();
                        break;
                    }
                case "hit":
                    {
                        await game.Hit();
                        int score = game.player.visibleScore;
                        break;
                    }
            }
            return View(game);
        }

        private Game GetGameFromSession()
        {
            Guid gameId;
            try
            {
                gameId = new Guid(HttpContext.Session.GetString("gameId"));
                Game game = games.Where(g => g.Id == gameId).First();
                return game;
            }
            catch
            {
                return null;
            }
        }
    }

}
