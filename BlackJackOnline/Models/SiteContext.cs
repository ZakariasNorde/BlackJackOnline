using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BlackJackOnline.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace BlackJackOnline.Models
{
	public class SiteContext : IdentityDbContext<User>
	{
		public SiteContext(DbContextOptions<SiteContext> options) : base(options) { }
		public DbSet<User> users { get; set; }

		public DbSet<Game> games { get; set; }

        public DbSet<Player> players { get; set; }

        public DbSet<Dealer> dealers { get; set; }

        public DbSet<Card> cards { get; set; }

        public DbSet<CardDeck> decks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Card>().HasData(
                new Card()
                {
                    Id = 1,
                    Suit = GameEnums.CardSuit.Diamonds,
                    Value = GameEnums.CardValue.Two
                },
            new Card()
            {
                Id = 2,
                Suit = GameEnums.CardSuit.Diamonds,
                Value = GameEnums.CardValue.Three
            },
            new Card()
            {
                Id = 3,
                Suit = GameEnums.CardSuit.Diamonds,
                Value = GameEnums.CardValue.Four
            });
            modelBuilder.Entity<CardDeck>().HasData(
                new CardDeck()
                {
                    Id = 1,
                    Cards = new List<Card> ()
                });
            modelBuilder.Entity<Player>().HasData( 
                new Player()
                {
                    Id = 1
                });
            modelBuilder.Entity<Dealer>().HasData(
                new Dealer()
                {
                    Id = 1
                });
            modelBuilder.Entity<Game>().HasData(
                new Game()
                {
                    Id = 1,
                    playerId = 1,
                    dealerId = 1,
                    state = GameEnums.GameState.NotStarted
                });

        }

    }
}
