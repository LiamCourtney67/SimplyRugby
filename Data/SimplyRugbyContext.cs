using Microsoft.EntityFrameworkCore;
using SimplyRugby.Models.Account;
using SimplyRugby.Models.Member;
using SimplyRugby.Models.Member.Skills;
using SimplyRugby.Models.Team;
using SimplyRugby.Models.Team.Match;
using SimplyRugby.Models.Team.TrainingSession;

namespace SimplyRugby.Data
{
    /// <summary>
    /// Context for the Simply Rugby database.
    /// </summary>
    public class SimplyRugbyContext : DbContext
    {
        // Account entities
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AdminAccount> AdminAccounts { get; set; }
        public DbSet<CoachAccount> CoachAccounts { get; set; }

        // Member entities
        public DbSet<Member> Members { get; set; }
        public DbSet<Coach> Coaches { get; set; }

        // Player entities
        public DbSet<Player> Players { get; set; }
        public DbSet<JuniorPlayer> JuniorPlayers { get; set; }

        public DbSet<Position> Positions { get; set; }
        public DbSet<Skills> Skills { get; set; }

        public DbSet<NextOfKin> NextOfKins { get; set; }
        public DbSet<Guardian> Guardians { get; set; }
        public DbSet<Doctor> Doctors { get; set; }

        // Team entities
        public DbSet<Team> Teams { get; set; }

        public DbSet<Match> Matches { get; set; }
        public DbSet<TrainingSession> TrainingSessions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Options for the database connection
            optionsBuilder.UseSqlServer("Server=tcp:simply-rugby.database.windows.net,1433;Initial Catalog=simply-rugby-db;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication=Active Directory Default;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Set up the discriminator column
            modelBuilder.Entity<Account>()
                .HasDiscriminator<string>("AccountType")
                .HasValue<Account>("User")
                .HasValue<AdminAccount>("Admin")
                .HasValue<CoachAccount>("Coach");

            modelBuilder.Entity<Member>()
                .HasDiscriminator<string>("MemberType")
                .HasValue<Member>("Member")
                .HasValue<Coach>("Coach")
                .HasValue<Player>("Player")
                .HasValue<JuniorPlayer>("JuniorPlayer");

            modelBuilder.Entity<NextOfKin>()
                .HasDiscriminator<string>("KinType")
                .HasValue<NextOfKin>("NextOfKin")
                .HasValue<Guardian>("Guardian");

            // Configuring the Skills entity to own one Kicking entity.
            modelBuilder.Entity<Skills>()
                .OwnsOne(s => s.Kicking);

            // Configuring the Skills entity to own one Passing entity.
            modelBuilder.Entity<Skills>()
                .OwnsOne(s => s.Passing);

            // Configuring the Skills entity to own one Tackling entity.
            modelBuilder.Entity<Skills>()
                .OwnsOne(s => s.Tackling);

            // Configuring the Matches entity to own one FirstHalf entity.
            modelBuilder.Entity<Match>()
                .OwnsOne(m => m.FirstHalf);

            // Configuring the Matches entity to own one SecondHalf entity.
            modelBuilder.Entity<Match>()
                .OwnsOne(m => m.SecondHalf);

            // Populate the Positions with some initial data.
            modelBuilder.Entity<Position>().HasData(
                new Position { PositionID = 1, Name = "Full Back" },
                new Position { PositionID = 2, Name = "Wing" },
                new Position { PositionID = 3, Name = "Centre" },
                new Position { PositionID = 4, Name = "Fly Half" },
                new Position { PositionID = 5, Name = "Scrum Half" },
                new Position { PositionID = 6, Name = "Hooker" },
                new Position { PositionID = 7, Name = "Prop" },
                new Position { PositionID = 8, Name = "2nd Row" },
                new Position { PositionID = 9, Name = "Back Row" }
                );
        }
    }
}
