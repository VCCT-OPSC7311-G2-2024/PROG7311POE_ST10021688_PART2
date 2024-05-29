using Microsoft.EntityFrameworkCore;
using PROG7311POE.Models;
using PROG7311POE.Data;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace PROG7311POE.Data
{
    /// <summary>
    /// This class is for developement only!! Literally dummy data can be used to make breaking the application easier. 
    /// Comment out the seeding of this dummy-data in the `Startup.cs` file, when you're done!
    /// </summary>
    public static class DummyData
    {
        /// <summary>
        /// Creates dummy data to go into the SQL-Lite db, please enjoy some digital biltong while testing... 
        /// </summary>
        /// <param name="serviceProvider"></param>
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var myContext = new MyDbContext(serviceProvider.GetRequiredService<DbContextOptions<MyDbContext>>()))
            {
                
                if (myContext.Users.Any())
                {
                    return;   // Dummy data exists! Let's save on the processing shall we...
                }

                var dummyUser1 = new User
                {
                    Username = "Harry123",
                    Password = BCrypt.Net.BCrypt.HashPassword("password"),
                    Email = "youre.a.wizard.harry@hogwarts.com",
                    Role = "Farmer"
                };

                var dummyUser2 = new User
                {
                    Username = "Jeffrey123",
                    Password = BCrypt.Net.BCrypt.HashPassword("password"),
                    Email = "jeff@amazon.com",
                    Role = "Employee"
                };

                var dummyUser3 = new User
                {
                    Username = "John",
                    Password = BCrypt.Net.BCrypt.HashPassword("password"),
                    Email = "JDOE@gmail.com",
                    Role = "Farmer"
                };

                myContext.Users.AddRange(dummyUser1, dummyUser2);
                myContext.SaveChanges();

                var dummyFarmer1 = new Farmer
                {
                    UserID = dummyUser1.UserID,
                    FarmName = "BoKap Biltong",
                    Location = "Bokap"
                };

                var dummyFarmer2 = new Farmer
                {
                    UserID = dummyUser3.UserID,
                    FarmName = "Little Italy",
                    Location = "Gardens"
                };

                myContext.Farmers.Add(dummyFarmer1);
                myContext.SaveChanges();

                myContext.Products.AddRange(
                    new Product
                    {
                        FarmerID = dummyFarmer1.FarmerID,
                        ProductName = "Droewors",
                        Category = "Dried Meat",
                        ProductionDate = DateTime.Now.AddMonths(-1)
                    },
                    new Product
                    {
                        FarmerID = dummyFarmer1.FarmerID,
                        ProductName = "Biltong",
                        Category = "Dried Meat",
                        ProductionDate = DateTime.Now.AddMonths(-2)
                    },
                    new Product
                    {
                        FarmerID = dummyFarmer1.FarmerID,
                        ProductName = "Chilli bacon goodness",
                        Category = "Dried Meat",
                        ProductionDate = DateTime.Now.AddMonths(-3)
                    }, new Product
                    {
                        FarmerID = dummyFarmer2.FarmerID,
                        ProductName = "Sour dough",
                        Category = "Bread",
                        ProductionDate = DateTime.Now.AddMonths(-1)
                    },
                    new Product
                    {
                        FarmerID = dummyFarmer2.FarmerID,
                        ProductName = "Qwa-ssant",
                        Category = "Bread",
                        ProductionDate = DateTime.Now.AddMonths(-2)
                    }
                );

                myContext.SaveChanges();
            }
        }
    }
}
// 💻🌸✨💖 --<< End of File >>-- 💖✨🌸💻