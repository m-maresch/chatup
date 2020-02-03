using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Repositories
{
    public class UsersContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(Startup.Configuration["dbConnStr"]);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(u => u.UserName)
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.EMail)
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.EMail)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.PhoneNumber)
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.PhoneNumber)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.Password)
                .IsRequired();
            
            modelBuilder.Entity<User>()
                .HasKey(u => u.UserID);

            modelBuilder.Entity<User>()
                .Property(u => u.UserID)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserID)
                .IsUnique();

            modelBuilder.Entity<UserContact>()
                .HasKey(uc => new {uc.UserID, uc.ContactID});

            modelBuilder.Entity<UserContact>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.Contacts);
        }
    }
}
