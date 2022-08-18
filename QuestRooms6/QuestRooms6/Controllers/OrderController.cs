using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestRooms6_DataAccess.Data;
using QuestRooms6_Model;
using QuestRooms6_Model.ViewModels;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
namespace QuestRooms6.Controllers
{
    public class OrderController : Controller
    {
        private readonly QuestRoomsContextDb _questRoomsContextDb;
        internal DbSet<OrderDetail> dbSet;
        private readonly UserManager<AplicationUser> _userManager;
        public OrderController(QuestRoomsContextDb questRoomsContextDb, UserManager<AplicationUser> userManager)
        {
            _questRoomsContextDb = questRoomsContextDb;
            dbSet = questRoomsContextDb.Set<OrderDetail>();
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string? searchName = null,
            int? searchCountPlayer = null,
            double? searchPrice = null,
            string? srchDifLvl = null,
            string? ScrFerLvl = null,
            string? inputreset = null)
        {

            if (User.IsInRole(WebConstanta.AdminRole)) //if admin then all orders
            {
                IEnumerable<OrderHeader>? orderHeaders = _questRoomsContextDb.OrderHeader;
                return View(orderHeaders);
            }
            else
            {
                var _user = await _userManager.FindByNameAsync(User.Identity.Name); //Searching for a logged in user
                IEnumerable<OrderHeader>? orderHeaders = _questRoomsContextDb.OrderHeader.Where(x => x.CreatedByUserId == _user.Id);
                return View(orderHeaders);
            }
        }

        public async Task<IActionResult> Detail(int id)
        {
            OrderVm orderVm = new OrderVm()
            {
                OrderHeader = await _questRoomsContextDb.OrderHeader.FirstOrDefaultAsync(x => x.Id == id),
                OrderDetails = GetAll(x => x.OrderHeaderId == id, "Room")
            };
            return View(orderVm);
        }
        public IEnumerable<OrderDetail> GetAll(Expression<Func<OrderDetail, bool>>? filter, string includeProperties)
        {
            IQueryable<OrderDetail> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (includeProperties != null)
            {
                foreach (var includePror in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includePror);
                }
            }
            return query.ToList();
        }

    }
}