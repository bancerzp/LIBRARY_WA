using Microsoft.EntityFrameworkCore;
using LIBRARY_WA.Models;
using LIBRARY_WA.Models.database;

public class LibraryContext : DbContext
{
    public LibraryContext(DbContextOptions<LibraryContext> options)
             : base(options)
    {
    }
    public DbSet<Book> Book { get; set; }
    public DbSet<User> User { get; set; }
    public DbSet<Volume> Volume { get; set; }
    public DbSet<Reservation> Reservation { get; set; }
    public DbSet<Rent> Rent { get; set; }
    public DbSet<Renth> Renth { get; set; }
    public DbSet<Suggestion> Suggestion { get; set; }
    public DbSet<Author> Author { get; set; }
}