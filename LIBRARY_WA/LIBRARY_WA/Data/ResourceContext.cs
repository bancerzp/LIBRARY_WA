using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MySql.Data.EntityFrameworkCore.Extensions;
using LIBRARY_WA.Models;

    public class ResourceContext : DbContext
    {
        public ResourceContext (DbContextOptions<ResourceContext> options)
            : base(options)
        {
        }

        public DbSet<LIBRARY_WA.Models.Resource> Resource { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySQL("server=localhost;database=library;user=root;password=admin");
    }
}
