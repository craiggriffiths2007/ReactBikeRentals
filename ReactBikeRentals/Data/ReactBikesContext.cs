using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReactBikes.Models;

namespace ReactBikes.Data
{
    public class ReactBikesContext : IdentityDbContext<ReactBikesUser>
    {
        public ReactBikesContext (DbContextOptions<ReactBikesContext> options)
            : base(options)
        {
        }

        public DbSet<ReactBikes.Models.Bike> Bike { get; set; } = default!;

        public DbSet<ReactBikes.Models.Rental> Rental { get; set; }
    }
}
