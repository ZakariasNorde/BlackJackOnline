using BlackJackOnline.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace BlackJackOnline.Controllers
{
    public class GameController : Controller

    {
        public static List<Game> games = new List<Game>();
        public UserManager<User> _userManager {  get; set; }

        public SiteContext _siteContext { get; set; }
        public GameController(UserManager<User> userManager, SiteContext siteContext)
        {
            _userManager = userManager;
            _siteContext = siteContext;
        }
        public async Task<IActionResult> BlackJack()
        {
            try
            {
                Guid gameId = Guid.NewGuid(); // a unique id that is assigned to each game and to the user's session variable
                string userName = User.Identity.Name;
                if (userName != null)
                {
                    Game newGame = new Game(gameId, _userManager);
                    await newGame.CreatePlayer(userName);
                    games.Add(newGame);
              
                }
                else
                {
                    games.Add(new Game(gameId, _userManager));
                }
                
                    
                
                HttpContext.Session.SetString("gameId", gameId.ToString());
                Game game = GetGameFromSession();
                //varför är denna variabeln 0 även fast players funds blir 500 i båda konstruktorerna
                decimal funds = game.player.Funds;
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
            switch (changed)
            {
                case "start":
                    {
                        //Ifall user inte trycker på keep going kommer de fortfarande få vinsten till sitt konto
                        //fixa på nåt sätt så att usercollect körs automatiskt
                        if (User.Identity.IsAuthenticated)
                        {
                            await UserCollect();
                        }
                        await game.NewHand();
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
                        game.NewStand();
                        break;
                        
                    }
                case "dealerTurn":
                    {
                        await game.DealerTurn();
                        break;
                    }
                case "hit":
                    {
                        await game.Hit();
                        break;
                    }
                case "insurance":
                    {
                        game.Insurance();
                        break;
                    }
                case "double":
                    {
                        await game.DoubleDown();
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

        private async Task UserCollect()
        {
            User user = await _userManager.FindByNameAsync(User.Identity.Name);
            Game game = GetGameFromSession();
            user.Funds += game.player.Change;
            decimal fundTest = user.Funds;
            _userManager.UpdateAsync(user);
            _siteContext.Update(user);

        }
    }

}
