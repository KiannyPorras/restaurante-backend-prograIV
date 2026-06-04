using Microsoft.EntityFrameworkCore;
using RestauranteApi.Entities;

namespace RestauranteApi.DataBase
{
    public class RestauranteApiDbContext : DbContext
    {
        public RestauranteApiDbContext(DbContextOptions<RestauranteApiDbContext> options) : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<LockTable> LockTables { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ReservationHistory> ReservationsHistory { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<Turn> Turns { get; set; }
        public DbSet<WaitingList> WaitingLists { get; set; }
        public DbSet<Zone> Zones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── Configuración de relaciones ───────────────────────────

            // Zone -> Section (1..N)
            modelBuilder.Entity<Zone>()
                .HasMany(z => z.Sections)
                .WithOne(s => s.Zone)
                .HasForeignKey(s => s.ZoneId)
                .OnDelete(DeleteBehavior.Restrict);

            // Section -> Table (1..N)
            modelBuilder.Entity<Section>()
                .HasMany(s => s.Tables)
                .WithOne(t => t.Section)
                .HasForeignKey(t => t.SectionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Table -> Reservation (1..N)
            modelBuilder.Entity<Table>()
                .HasMany(t => t.Reservations)
                .WithOne(r => r.Table)
                .HasForeignKey(r => r.TableId)
                .OnDelete(DeleteBehavior.Restrict);

            // Client -> Reservation (1..N)
            modelBuilder.Entity<Client>()
                .HasMany(c => c.Reservations)
                .WithOne(r => r.Client)
                .HasForeignKey(r => r.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Turn -> Reservation (1..N)
            modelBuilder.Entity<Turn>()
                .HasMany(t => t.Reservations)
                .WithOne(r => r.Turn)
                .HasForeignKey(r => r.TurnId)
                .OnDelete(DeleteBehavior.Restrict);

            // Reservation -> ReservationHistory (1..N)
            modelBuilder.Entity<Reservation>()
                .HasMany(r => r.ReservationHistories)
                .WithOne(h => h.Reservation)
                .HasForeignKey(h => h.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Table -> LockTable (1..N)
            modelBuilder.Entity<Table>()
                .HasMany(t => t.LockTables)
                .WithOne(b => b.Table)
                .HasForeignKey(b => b.TableId)
                .OnDelete(DeleteBehavior.Cascade);

            // Client -> WaitingList (1..N)
            modelBuilder.Entity<Client>()
                .HasMany(c => c.WaitingLists)
                .WithOne(w => w.Client)
                .HasForeignKey(w => w.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Table>()
                .HasIndex(t => t.Number)
                .IsUnique();

            modelBuilder.Entity<Client>()
                .HasIndex(c => c.Phone)
                .IsUnique();

            modelBuilder.Entity<Reservation>()
                .HasIndex(r => new { r.TableId, r.Date, r.TurnId })
                .HasFilter("\"Status\" = 0")
                .IsUnique();
            

            // Zones
            modelBuilder.Entity<Zone>().HasData(
                new Zone { Id = 1, Name = "Terraza" },
                new Zone { Id = 2, Name = "Interior" },
                new Zone { Id = 3, Name = "VIP" }
            );

            // Sections
            modelBuilder.Entity<Section>().HasData(
                new Section { Id = 1, Name = "Terraza Norte", ZoneId = 1 },
                new Section { Id = 2, Name = "Terraza Sur", ZoneId = 1 },
                new Section { Id = 3, Name = "Sala Principal", ZoneId = 2 },
                new Section { Id = 4, Name = "Sala Privada", ZoneId = 2 },
                new Section { Id = 5, Name = "VIP", ZoneId = 3 }
            );

            // Tables
            modelBuilder.Entity<Table>().HasData(
                // Terraza Norte
                new Table { Id = 1, Number = "TN-01", Capacity = 4, IsActive = true, SectionId = 1 },
                new Table { Id = 2, Number = "TN-02", Capacity = 6, IsActive = true, SectionId = 1 },
                new Table { Id = 3, Number = "TN-03", Capacity = 2, IsActive = true, SectionId = 1 },
                new Table { Id = 4, Number = "TN-04", Capacity = 4, IsActive = true, SectionId = 1 },
                // Terraza Sur
                new Table { Id = 5, Number = "TS-05", Capacity = 4, IsActive = true, SectionId = 2 },
                new Table { Id = 6, Number = "TS-06", Capacity = 6, IsActive = true, SectionId = 2 },
                new Table { Id = 7, Number = "TS-07", Capacity = 2, IsActive = true, SectionId = 2 },
                new Table { Id = 8, Number = "TS-08", Capacity = 4, IsActive = true, SectionId = 2 },
                // Sala Principal
                new Table { Id = 9, Number = "SP-01", Capacity = 8, IsActive = true, SectionId = 3 },
                new Table { Id = 10, Number = "SP-02", Capacity = 6, IsActive = true, SectionId = 3 },
                new Table { Id = 11, Number = "SP-03", Capacity = 4, IsActive = true, SectionId = 3 },
                new Table { Id = 12, Number = "SP-04", Capacity = 10, IsActive = true, SectionId = 3 },
                new Table { Id = 13, Number = "SP-05", Capacity = 4, IsActive = true, SectionId = 3 },
                // Sala Privada
                new Table { Id = 14, Number = "SPr-01", Capacity = 10, IsActive = true, SectionId = 4 },
                // VIP
                new Table { Id = 15, Number = "VIP-01", Capacity = 12, IsActive = true, SectionId = 5 },
                new Table { Id = 16, Number = "VIP-02", Capacity = 8, IsActive = true, SectionId = 5 },
                new Table { Id = 17, Number = "VIP-03", Capacity = 6, IsActive = true, SectionId = 5 }
            );

            // Clients
            modelBuilder.Entity<Client>().HasData(
                new Client { Id = 1, Name = "Moises", LastName = "Caicedo", Phone = "12345678" },
                new Client { Id = 2, Name = "Lionel Andres", LastName = "Messi", Phone = "09876543" },
                new Client { Id = 3, Name = "Cristiano Ronaldo", LastName = "Dos Santos Aveiro", Phone = "11223344" },
                new Client { Id = 4, Name = "Angel", LastName = "Morales", Phone = "55667788" },
                new Client { Id = 5, Name = "Mariano", LastName = "Torres", Phone = "66778899" },
                new Client { Id = 6, Name = "Jorkaef", LastName = "Azofeifa", Phone = "77889900" },
                new Client { Id = 7, Name = "Orlando", LastName = "Sinclair", Phone = "88990011" },
                new Client { Id = 8, Name = "Josue", LastName = "Moreira", Phone = "99001122" },
                new Client { Id = 9, Name = "Javon", LastName = "East", Phone = "10101010" },
                new Client { Id = 10, Name = "Angie", LastName = "Bustos", Phone = "20202020" }
            );

            // Turns
            modelBuilder.Entity<Turn>().HasData(
                new Turn { Id = 1, Name = "Almuerzo", StartTime = TimeSpan.FromHours(12), EndTime = TimeSpan.FromHours(16), IsActive = true },
                new Turn { Id = 2, Name = "Cena", StartTime = TimeSpan.FromHours(18), EndTime = TimeSpan.FromHours(21), IsActive = true }
            );

            // Reservations (fechas fijas para los Seeds)
            modelBuilder.Entity<Reservation>().HasData(
                new Reservation
                {
                    Id = 1,
                    Date = new DateOnly(2026, 5, 6),
                    ReservationTime = new TimeSpan(13, 0, 0),
                    GuestCount = 2,
                    Status = ReservationStatus.Active,
                    CreatedAt = new DateTime(2026, 5, 6, 10, 0, 0),
                    ClientId = 2,
                    TableId = 3,
                    TurnId = 1
                },
                new Reservation
                {
                    Id = 2,
                    Date = new DateOnly(2026, 5, 6),
                    ReservationTime = new TimeSpan(19, 0, 0),
                    GuestCount = 8,
                    Status = ReservationStatus.Active,
                    CreatedAt = new DateTime(2026, 5, 6, 10, 0, 0),
                    ClientId = 3,
                    TableId = 14,
                    TurnId = 2
                },
                new Reservation
                {
                    Id = 3,
                    Date = new DateOnly(2026, 5, 7),
                    ReservationTime = new TimeSpan(12, 30, 0),
                    GuestCount = 6,
                    Status = ReservationStatus.Active,
                    CreatedAt = new DateTime(2026, 5, 6, 10, 0, 0),
                    ClientId = 9,
                    TableId = 17,
                    TurnId = 1
                },
                new Reservation
                {
                    Id = 4,
                    Date = new DateOnly(2026, 5, 5),
                    ReservationTime = new TimeSpan(20, 0, 0),
                    GuestCount = 7,
                    Status = ReservationStatus.Attended,
                    CreatedAt = new DateTime(2026, 5, 5, 10, 0, 0),
                    ClientId = 8,
                    TableId = 16,
                    TurnId = 2
                },
                new Reservation
                {
                    Id = 5,
                    Date = new DateOnly(2026, 5, 4),
                    ReservationTime = new TimeSpan(14, 30, 0),
                    GuestCount = 5,
                    Status = ReservationStatus.Cancelled,
                    CreatedAt = new DateTime(2026, 5, 4, 10, 0, 0),
                    ClientId = 6,
                    TableId = 10,
                    TurnId = 1
                }
            );

            // LockTables
            modelBuilder.Entity<LockTable>().HasData(
                new LockTable
                {
                    Id = 1,
                    Reason = "Mantenimiento",
                    From = new DateTime(2026, 5, 8, 0, 0, 0),
                    To = new DateTime(2026, 5, 8, 4, 0, 0),
                    IsActive = true,
                    TableId = 5
                },
                new LockTable
                {
                    Id = 2,
                    Reason = "Mantenimiento",
                    From = new DateTime(2026, 5, 9, 0, 0, 0),
                    To = new DateTime(2026, 5, 9, 4, 0, 0),
                    IsActive = true,
                    TableId = 6
                }
            );

            // WaitingLists
            modelBuilder.Entity<WaitingList>().HasData(
                new WaitingList
                {
                    Id = 1,
                    ReqDate = new DateTime(2026, 5, 6, 10, 0, 0),
                    DesiredDay = new DateOnly(2026, 5, 7),
                    DesiredTime = new TimeSpan(19, 30, 0),
                    GuestCount = 6,
                    PreferZone = "Interior",
                    Status = "Pending",
                    ClientId = 1
                },
                new WaitingList
                {
                    Id = 2,
                    ReqDate = new DateTime(2026, 5, 6, 10, 0, 0),
                    DesiredDay = new DateOnly(2026, 5, 8),
                    DesiredTime = new TimeSpan(12, 0, 0),
                    GuestCount = 2,
                    PreferZone = "VIP",
                    Status = "Pending",
                    ClientId = 5
                }
            );

            // ReservationHistories
            modelBuilder.Entity<ReservationHistory>().HasData(
                new ReservationHistory
                {
                    Id = 1,
                    PrevState = ReservationStatus.Active,
                    NewState = ReservationStatus.Attended,
                    ChangedAt = new DateTime(2026, 5, 5, 22, 0, 0),
                    ReservationId = 4
                },
                new ReservationHistory
                {
                    Id = 2,
                    PrevState = ReservationStatus.Active,
                    NewState = ReservationStatus.Cancelled,
                    ChangedAt = new DateTime(2026, 5, 4, 16, 0, 0),
                    ReservationId = 5
                }
            );
        }
    }
}
