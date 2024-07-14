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
                decimal funds = game.player.Funds;  // varför är denna variabeln 0 även fast players funds blir 500 i båda konstruktorerna
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
            var index = 0;
            if(changed.Contains(" "))
            {
                var bothCommands = changed.Split(" ");
                index =  int.Parse(bothCommands[1]);
                changed = bothCommands[0];
            }
            switch (changed)
            {
                case "start":
                    {
                        //Ifall user inte trycker på keep going kommer de fortfarande få vinsten till sitt konto
                        //fixa på nåt sätt så att usercollect körs automatiskt 
                        //kan även lägga till en annan knapp än keep going också som också kör Usercollect
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
                        game.AddHand(10);
                        break;
                    }
                case "bet20":
                    {
                        game.AddHand(20);
                        break;
                    }
                case "bet50":
                    {
                        game.AddHand(50);
                        break;
                    }
                case "finalizeBets":
                    {
                        await game.DealFirst();
                        if (User.Identity.IsAuthenticated)
                        {
                            var userName = User.Identity.Name;
                            var user = await _userManager.FindByNameAsync(userName);
                            user.Funds -= game.TotalBet;
                            _siteContext.SaveChanges();
                        }
                        break;
                    }
                case "dealingSecond":
                    {
                        await game.DealSecond();
                        break;
                    }
                case "dealingThird":
                    {
                        await game.DealThird();
                        break;
                    }
                case "dealingFourth":
                    {
                        await game.DealFourth();
                        break;
                    }
                case "dealingLast":
                    {
                        game.EndDealing();
                        break;
                    }
                case "stand":
                    {
                        Person player = game.hands[index];
                        game.NewStand(player);
                        break;
                        
                    }
                case "dealerTurn":
                    {
                        await game.DealerTurn();
                        break;
                    }
                case "hit":
                    {
                        Person player = game.hands[index];
                        await game.Hit(player);
                        break;
                    }
                case "insurance":
                    {
                        game.Insurance();
                        break;
                    }
                case "double":
                    {
                        Person player = game.hands[index];
                        await game.DoubleDown(player);
                        break;
                    }
                case "quit":
                    {
                        if (User.Identity.IsAuthenticated)
                        {
                            await UserCollect();
                        }
                        return RedirectToAction("Index", "Home");
                        break;
                    }
                case "split":
                    {
                        Person player = game.hands[index];
                        game.Split(player);
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

        public IActionResult PlaceBets(decimal funds)
        {

            ViewBag.Funds = funds;
            return View();
        }

        private async Task UserCollect()
        {
            User user = await _userManager.FindByNameAsync(User.Identity.Name);
            Game game = GetGameFromSession();
            user.Funds += game.player.Change;
            decimal fundTest = user.Funds;
            await _userManager.UpdateAsync(user);
            _siteContext.Update(user);
            _siteContext.SaveChanges();

        }
    }

}
