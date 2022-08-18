using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuestRooms6_DataAccess.Data;
using QuestRooms6_Model;
namespace QuestRooms6.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly QuestRoomsContextDb _db;
        private readonly UserManager<AplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(QuestRoomsContextDb db, UserManager<AplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public void Initialize()
        {
            try
            {
             //   _db.Database.Migrate(); //complete migrations
                //Checking incomplete migrations
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate(); //complete migrations
                }
                else
                {
                   
                }
            }
            catch
            {

            }
            // (create role and user (initializing)) .GetAwaiter().GetResult() - Instead of await
            if (!_roleManager.RoleExistsAsync(WebConstanta.AdminRole).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(WebConstanta.AdminRole)).GetAwaiter().GetResult(); //роль админа
                _roleManager.CreateAsync(new IdentityRole(WebConstanta.CustomerRole)).GetAwaiter().GetResult(); //роль юзера
            }
            _userManager.CreateAsync(new AplicationUser
            {
                UserName = "Admin",
                Email = "test@admin.com",
                EmailConfirmed = true,
                FullName = "Admin",
                PhoneNumber = "0931111111"
            }, "123!@#QWEqwe").GetAwaiter().GetResult();

            AplicationUser user = _db.AplicationUser.First(u => u.Email == "test@admin.com");
            _userManager.AddToRoleAsync(user, WebConstanta.AdminRole).GetAwaiter().GetResult();
        }
    }
}
