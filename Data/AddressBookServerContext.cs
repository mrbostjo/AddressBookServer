using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AddressBookServer.Models;

namespace AddressBookServer.Data
{
    public class AddressBookServerContext : DbContext
    {
        public AddressBookServerContext (DbContextOptions<AddressBookServerContext> options)
            : base(options)
        {
        }

        public DbSet<AddressBookServer.Models.Contact> Contact { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contact>()
                .HasIndex(contact => new { contact.id, contact.phone })
                .IsUnique(true);
        }
    }
}
