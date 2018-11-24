using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LIBRARY_WA.Models;

public class LibraryContext : DbContext
{
    public LibraryContext(DbContextOptions<LibraryContext> options)
             : base(options)
    {
    }
 //   public UserContext UserContext { get; set; }
    public DbSet<Book> Resource { get; set; }
    public DbSet<User> User { get; set; }
    public DbSet<Volume> Volume { get; set; }
}
