using Microsoft.EntityFrameworkCore;

namespace FFProject1.Models
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions options) : base(options) {}
        public DbSet<User> Users {get;set;}
        public DbSet<Player> Players {get;set;}
        public DbSet<Team> UserTeams {get;set;}
        public DbSet<WatchPlayer> WatchedPlayers {get;set;}
    }
}