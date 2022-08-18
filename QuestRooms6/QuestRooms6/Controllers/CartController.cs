using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestRooms6_DataAccess.Data;
using QuestRooms6_Model;
using QuestRooms6_Model.ViewModels;
using System.Linq.Expressions;
using System.Security.Claims;
namespace QuestRooms6.Controllers
{
    public class CartController : Controller
    {
        public DbSet<Room> dbSet;
        public DbSet<OrderHeader> dbSetOH;
        private readonly IQueryable<AplicationUser> dbSetAplicationUser;
        private readonly QuestRoomsContextDb _questRoomsContextDb;
        private readonly UserManager<AplicationUser> _userManager;

        [BindProperty]
        public RoomUserVM _RoomUserVM { get; set; }

        public CartController(QuestRoomsContextDb questRoomsContextDb, UserManager<AplicationUser> userManager)
        {
            _questRoomsContextDb = questRoomsContextDb;
            dbSet = questRoomsContextDb.Set<Room>();
            dbSetAplicationUser = questRoomsContextDb.Set<AplicationUser>();
            dbSetOH = questRoomsContextDb.Set<OrderHeader>();
            _userManager = userManager;
        }

        //CART INDEX
        public IActionResult Index()
        {

            List<Room> shoppingCarts = new List<Room>();
            if (HttpContext.Session.Get<IEnumerable<Room>>(WebConstanta.SessionCart) != null &&
              HttpContext.Session.Get<IEnumerable<Room>>(WebConstanta.SessionCart).Count() > 0)
            {
                //session exist
                shoppingCarts = HttpContext.Session.Get<IEnumerable<Room>>(WebConstanta.SessionCart).ToList();
            }
            List<int> roomCart = shoppingCarts.Select(x => x.Id).ToList();
            IEnumerable<Room> roomListTemp = GetAll(u => roomCart.Contains(u.Id)); //temporary Roomlist

            var claimsIndentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIndentity.FindFirst(ClaimTypes.NameIdentifier);

            var FindUserFullName = dbSetAplicationUser.FirstOrDefault(x => x.FullName == claimsIndentity.Name);

            _RoomUserVM = new RoomUserVM()
            {
                AplicationUser = FindUserFullName
            };

            foreach (var cartObj in shoppingCarts)
            {
                Room prodtemp = roomListTemp.FirstOrDefault(u => u.Id == cartObj.Id);
                _RoomUserVM.RoomList.Add(prodtemp);
            }

            for (int i = 0; i < shoppingCarts.Count(); i++)
            {
                _RoomUserVM.RoomList[i].DateOrderRoom = shoppingCarts[i].DateOrderRoom;
                _RoomUserVM.RoomList[i].TimeOrderRoom = shoppingCarts[i].TimeOrderRoom;
            }

           

            return View(_RoomUserVM);
        }

        #region SomeMethods
        public IEnumerable<Room> GetAll(Expression<Func<Room, bool>> filter)
        {
            IQueryable<Room> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return query.ToList();
        }
        public IActionResult Remove(int id)
        {
            List<Room> shoppingCartsList = new List<Room>();
            if (HttpContext.Session.Get<IEnumerable<Room>>(WebConstanta.SessionCart) != null &&
               HttpContext.Session.Get<IEnumerable<Room>>(WebConstanta.SessionCart).Count() > 0)
            {
                //session exist
                shoppingCartsList = HttpContext.Session.Get<IEnumerable<Room>>(WebConstanta.SessionCart).ToList();
            }
            shoppingCartsList.Remove(shoppingCartsList.FirstOrDefault(u => u.Id == id));
            HttpContext.Session.Set(WebConstanta.SessionCart, shoppingCartsList); //Set sessions after removal
            return RedirectToAction(nameof(Index));
        }
        public IActionResult RemoveAll(int id)
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        #endregion
        public async Task<IActionResult> Send(RoomUserVM roomUserVM)
        {
            var claimsIndentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIndentity.FindFirst(ClaimTypes.NameIdentifier);
            var FindUserFullName = dbSetAplicationUser.FirstOrDefaultAsync(x => x.FullName == claimsIndentity.Name);
            roomUserVM.AplicationUser = await FindUserFullName;

            //////////////////////COLLECTION Room - OK
            ///USER - OK
            var user = _userManager.FindByNameAsync(User.Identity.Name);
            OrderHeader orderHeader = new OrderHeader()
            {
                CreatedByUserId = claim.Value,
                PhoneNumber = roomUserVM.AplicationUser.PhoneNumber,
                FullName = roomUserVM.AplicationUser.FullName,
                OrderDate = DateTime.Now,
                CountRoom = roomUserVM.RoomList.Count(),
                FinalTotalPrice = roomUserVM.RoomList.Sum(x => x.Price),
                Email = roomUserVM.AplicationUser.Email,


            };
             _questRoomsContextDb.OrderHeader.Add(orderHeader);
             _questRoomsContextDb.SaveChanges();

            foreach (var room in roomUserVM.RoomList)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    OrderHeaderId = orderHeader.Id,
                    RoomId = room.Id,
                    Price = room.Price,
                    DateOrderRoom = room.DateOrderRoom,
                    TimeOrderRoom = room.TimeOrderRoom

                };
                await _questRoomsContextDb.OrderDetail.AddAsync(orderDetail);
            }
            await _questRoomsContextDb.SaveChangesAsync();

            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");



        }

    }
}
