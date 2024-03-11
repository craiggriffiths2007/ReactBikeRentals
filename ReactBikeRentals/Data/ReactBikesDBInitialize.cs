using Microsoft.AspNetCore.Identity;
using System;
using System.Data;
using ReactBikes.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ReactBikes.Data
{
    public class ReactBikesDBInitialize
    {
        public static async Task AddRolesAsync(UserManager<ReactBikesUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole(Roles.Manager.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.User.ToString()));
        }

        public static async Task AddManagerAsync(UserManager<ReactBikesUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var defaultUser = new ReactBikesUser
            {
                UserName = "manager",
                Email = "ste@gmail.com",
                FirstName = "Big",
                LastName = "Ste",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                ProfilePicture = System.IO.File.ReadAllBytes("wwwroot/img/PP.jpg")
            };
        
            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Shimano1+");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Manager.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.User.ToString());
                }
            }
        }

        public static void AddTestData(ReactBikesContext context)
        {
            context.Database.EnsureCreated();

            if (context.Bike.Any())
            {
                return;  
            }

            var bikes = new Bike[]
            {
                new Bike{ModelName="Ibis",Colour="Red",LocationPostcode="CH45 2NT",Rating=0,Available=true},
                new Bike{ModelName="Stumpjumper",Colour="Green",LocationPostcode="CH65 9BQ",Rating=0,Available=true},
                new Bike{ModelName="Yeti",Colour="Blue",LocationPostcode="WV11 1PF",Rating=0,Available=false},
                new Bike{ModelName="Cannondale",Colour="Red",LocationPostcode="KY11 8HD",Rating=0,Available=true},
                new Bike{ModelName="Kross SA",Colour="Green",LocationPostcode="ML2 8XT",Rating=0,Available=true},
                new Bike{ModelName="Seven Cycles",Colour="Blue",LocationPostcode="MK45 3BA",Rating=0,Available=false},
                new Bike{ModelName="Torpado",Colour="Red",LocationPostcode="CO7 6AA",Rating=0,Available=true},
                new Bike{ModelName="Rocky Mountain Bicycles",Colour="Green",LocationPostcode="BB1 2FB",Rating=0,Available=true},
                new Bike{ModelName="Polygon Bikes",Colour="Blue",LocationPostcode="ST16 1NR",Rating=0,Available=false}
            };
            foreach (Bike bike in bikes)
            {
                context.Add(bike);
            }
            context.SaveChanges();
        }
    }
}
