using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LIBRARY_WA.Models;

    public class ResourceContext : DbContext
    {
        public ResourceContext (DbContextOptions<ResourceContext> options)
            : base(options)
        {
        }

        public DbSet<Resource> Resource { get; set; }
        public DbSet<User> User { get; set; }
}
