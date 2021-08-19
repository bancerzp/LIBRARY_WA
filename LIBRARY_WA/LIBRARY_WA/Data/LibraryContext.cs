using Microsoft.EntityFrameworkCore;
using Library.Models;
using Library.Models.database;

public class LibraryContext : DbContext
{
    public LibraryContext(DbContextOptions<LibraryContext> options)
             : base(options)
    {
    }
    public DbSet<Book> Books { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Volume> Volumes { get; set; }
    public DbSet<Reservation> Reservation { get; set; }
    public DbSet<Rent> Rents { get; set; }
    public DbSet<Renth> RentsHistory { get; set; }
    public DbSet<Suggestion> Suggestion { get; set; }
    public DbSet<Author> Authors { get; set; }
}