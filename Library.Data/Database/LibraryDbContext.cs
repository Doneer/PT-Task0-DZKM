using Microsoft.EntityFrameworkCore;

namespace Library.Data.Database
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
        {
        }

        public DbSet<UserEntity> Users { get; set; }
        public DbSet<BookEntity> Books { get; set; }
        public DbSet<BookCopyEntity> BookCopies { get; set; }
        public DbSet<LibraryEventEntity> LibraryEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEntity>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.Type).IsRequired();
                entity.Property(e => e.RegistrationDate).IsRequired();
            });

            modelBuilder.Entity<BookEntity>(entity =>
            {
                entity.ToTable("Books");
                entity.HasKey(e => e.ISBN);
                entity.Property(e => e.ISBN).HasMaxLength(20);
                entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Author).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Publisher).HasMaxLength(100);
                entity.Property(e => e.Genre).HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(500);
            });

            modelBuilder.Entity<BookCopyEntity>(entity =>
            {
                entity.ToTable("BookCopies");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.ISBN).HasMaxLength(20).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.AcquisitionDate).IsRequired();
                entity.Property(e => e.Location).HasMaxLength(50).IsRequired();
            });

            modelBuilder.Entity<LibraryEventEntity>(entity =>
            {
                entity.ToTable("LibraryEvents");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Type).IsRequired();
                entity.Property(e => e.ISBN).HasMaxLength(20);
                entity.Property(e => e.Timestamp).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500).IsRequired();
            });
        }
    }

    public class UserEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public int Type { get; set; }
        public DateTime RegistrationDate { get; set; }
    }

    public class BookEntity
    {
        public string ISBN { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string? Publisher { get; set; }
        public int PublicationYear { get; set; }
        public string? Genre { get; set; }
        public string? Description { get; set; }
    }

    public class BookCopyEntity
    {
        public int Id { get; set; }
        public string ISBN { get; set; } = string.Empty;
        public int Status { get; set; }
        public DateTime AcquisitionDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public int? CurrentBorrowerId { get; set; }
        public DateTime? DueDate { get; set; }
    }

    public class LibraryEventEntity
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public int? UserId { get; set; }
        public string? ISBN { get; set; }
        public int? BookCopyId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}