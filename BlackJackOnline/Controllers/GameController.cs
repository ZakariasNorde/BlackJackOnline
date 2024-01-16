using BlackJackOnline.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlackJackOnline.Controllers
{
    public class GameController : Controller

    {

        public SiteContext _context { get; set; }

        Game game { get; set; }
        public GameController(SiteContext context) 
        { 
            _context = context;

        }

        public Game getGame()
        {
            var gameList =  from game in _context.games
                   select game;
            return gameList.FirstOrDefault();
        }
        public IActionResult BlackJack()
        {
            game = getGame();
            return View(game);
        }

        [HttpPost]
        public async Task<IActionResult> BlackJack(string act)
        {
            var game = getGame();
            switch (act)
            {
                case "start":
                    {
                        await game.InitializeHand();
                        _context.Update(game);
                        _context.SaveChanges();
                        break;
                    }
                case "bet10":
                    {
                        await game.Bet(10);
                        _context.Update(game);
                        _context.SaveChanges();
                        break;
                    }
                case "bet20":
                    {
                        await game.Bet(20);
                        _context.Update(game);
                        _context.SaveChanges();
                        break;
                    }
                case "bet50":
                    {
                        await game.Bet(50);
                        _context.Update(game);
                        _context.SaveChanges();
                        break;
                    }
            }
            return View(game);
        }
    }

}
