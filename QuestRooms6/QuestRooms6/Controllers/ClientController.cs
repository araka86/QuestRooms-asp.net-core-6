using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestRooms6.Model.ViewModels;
using QuestRooms6_DataAccess.Data;
using QuestRooms6_Model;
using System.Security.Claims;

namespace QuestRooms6.Controllers
{
    public class ClientController : Controller
    {
        private readonly UserManager<AplicationUser> _userManager;
        private readonly QuestRoomsContextDb _questRoomsContextDb;
        private readonly IQueryable<AplicationUser> _dbSetAplicationUser;
        private readonly DbSet<OrderHeader> _orderHeader;

        public ClientController(QuestRoomsContextDb questRoomsContextDb, UserManager<AplicationUser> userManager)
        {
            _questRoomsContextDb = questRoomsContextDb;
            _userManager = userManager;
            _orderHeader = questRoomsContextDb.Set<OrderHeader>();
            _dbSetAplicationUser = questRoomsContextDb.Set<AplicationUser>();
        }
        public async Task<IActionResult> Index() => View(await _questRoomsContextDb.AplicationUser.ToListAsync());
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
                return NotFound();

            var user = await _questRoomsContextDb.AplicationUser.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
                return NotFound();


            return View(user);
        }
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
                return NotFound();


            var user = await _questRoomsContextDb.AplicationUser.FindAsync(id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        // POST: Clients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string? id, LoginRegistrViewModel user)
        {
            AplicationUser _user = await _questRoomsContextDb.AplicationUser.FirstAsync(x => x.Id == id);
            if (_user == null)
                return NotFound();

            try
            {
                _user.UserName = user.FullName;
                _user.Email = user.Email;
                _user.PhoneNumber = user.PhoneNumber;
                _questRoomsContextDb.Update(_user);
                await _questRoomsContextDb.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();

            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {


            if (id == null)
                return NotFound();

            var user = await _questRoomsContextDb.AplicationUser.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string? id)
        {

            var user = await _questRoomsContextDb.AplicationUser.FirstOrDefaultAsync(m => m.Id == id);





            var idUserEmail = User.FindFirstValue(ClaimTypes.Email);

            if (user.Email != idUserEmail)
            {
                var orderOroom = _questRoomsContextDb
               .OrderHeader
               .Include(x => x.CreatedBy)
               .Where(y => y.CreatedByUserId == user.Id);
                IEnumerable<OrderHeader> orderOroom2 = _questRoomsContextDb.OrderHeader.Where(x => x.CreatedByUserId == user.Id);
                if (orderOroom2 != null)
                    _questRoomsContextDb.OrderHeader.RemoveRange(orderOroom2);

                _questRoomsContextDb.AplicationUser.Remove(user);
                await _questRoomsContextDb.SaveChangesAsync();

            }
            else
            {

                ViewData["Message"] = "You can't delete Admin";
                
                //  return RedirectToAction(nameof(Delete));
         
                return View(user);


            }











            return RedirectToAction(nameof(Index));
        }
    }
}
