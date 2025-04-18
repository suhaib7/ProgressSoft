using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models.BusinessCard;
using Models.User;
using Utility;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DataAccess.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<BusinessCardModel> BusinessCards { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserModel>().HasData(
                    new UserModel { Id = -1, Name = "sa", Email = "superadmin@gmail.com",PasswordHash = PasswordUtils.HashPassword("1")}
                );
        }
    }
}