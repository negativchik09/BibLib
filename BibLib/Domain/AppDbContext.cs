using BibLib.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BibLib.Domain
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        public DbSet<BookDTO> Books { get; set; }
        public DbSet<AuthorDTO> Authors { get; set; }
        public DbSet<GenreDTO> Genres { get; set; }
        public DbSet<SecretQuestion> SecretQuestions { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Bookmark> Bookmarks { get; set; }
        public DbSet<AuthorBook> AuthorBook { get; set; }
        public DbSet<GenreBook> GenreBook { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new AuthorBookConfiguration());
            builder.ApplyConfiguration(new GenreBookConfiguration());
        }

        public class AuthorBookConfiguration : IEntityTypeConfiguration<AuthorBook>
        {
            public void Configure(EntityTypeBuilder<AuthorBook> builder)
            {
                builder.HasKey(t => new {t.AuthorId, t.BookId});
                builder.HasOne(t => t.Author)
                    .WithMany(a => a.Books)
                    .HasForeignKey(t => t.AuthorId);
                builder.HasOne(t => t.Book)
                    .WithMany(b => b.Authors)
                    .HasForeignKey(t => new {t.BookId});
            }
        }
        public class GenreBookConfiguration : IEntityTypeConfiguration<GenreBook>
        {
            public void Configure(EntityTypeBuilder<GenreBook> builder)
            {
                builder.HasKey(t => new {t.GenreId, t.BookId});
                builder.HasOne(t => t.Genre)
                    .WithMany(g => g.Books)
                    .HasForeignKey(t => new {t.GenreId});
                builder.HasOne(t => t.Book)
                    .WithMany(b => b.Genres)
                    .HasForeignKey(t => new {t.BookId});
            }
        }
    }
}