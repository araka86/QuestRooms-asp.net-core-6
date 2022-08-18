using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuestRooms6_DataAccess.Data;
using QuestRooms6_Model;
using QuestRooms6_Model.ViewModels;
using System.Diagnostics;
namespace QuestRooms6.Controllers
{
    public class HomeController : Controller
    {
        private readonly QuestRoomsContextDb _questRoomsContextDb;
        public static List<string> timeList = new List<string> { "12:00", "13:00", "14:00", "15:00", "16:00", "17:00", "18:00", "19:00", "20:00", "21:00", "22:00" };

        public HomeController( QuestRoomsContextDb questRoomsContextDb) =>  _questRoomsContextDb = questRoomsContextDb; 

        public async Task<IActionResult> Index(
            int page = 1,
            string? searchName = null,
            int? searchCountPlayer = null,
            double? searchPrice = null,
            string? srchDifLvl = null,
            string? ScrFerLvl = null,
            string? inputreset = null)
        {

            IQueryable<Room> rooms = _questRoomsContextDb.Rooms;
            var viewModel = new IndexViewModel();

            if (inputreset == null)
            {
                if (!string.IsNullOrEmpty(searchName))
                {
                    rooms = rooms.Where(r => r.Name.ToLower().Contains(searchName.ToLower()));
                }
                if (searchCountPlayer != null)
                {
                    rooms = rooms.Where(r => r.CountPlayers == searchCountPlayer);
                }
                if (searchPrice != null)
                {
                    rooms = rooms.Where(r => r.Price == searchPrice);
                }
                if (!string.IsNullOrEmpty(srchDifLvl) && srchDifLvl != "--Difficult Level--")
                {
                    rooms = rooms.Where(r => r.DifltLevel == srchDifLvl);
                }
                if (!string.IsNullOrEmpty(ScrFerLvl) && ScrFerLvl != "--Fear Level--")
                {
                    rooms = rooms.Where(r => r.FLevel == ScrFerLvl);
                }
            }
            else
            {
                viewModel = await PaginationMethod(rooms, page);
                return RedirectToAction(nameof(Index));
            }




            viewModel = await PaginationMethod(rooms, page);

            return View(viewModel);
        }

        public async Task<IndexViewModel> PaginationMethod(IQueryable<Room> rooms, int page)
        {
            int pageSize = 5;
            var count = await rooms.CountAsync();
            var items = await rooms.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);
            IndexViewModel viewModel = new IndexViewModel
            {
                PageViewModel = pageViewModel,
                Rooms = items
            };
            foreach (Room room in rooms)
            {
                viewModel.CountPrice.Add(room.Price);
                viewModel.CountPerson.Add(room.CountPlayers);
            }
            viewModel.CountPrice = viewModel.CountPrice.Distinct().ToList();
            viewModel.CountPerson = viewModel.CountPerson.Distinct().ToList();
            viewModel.CountPrice.Sort();
            viewModel.CountPerson.Sort();
            return viewModel;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize(Roles = $"{WebConstanta.AdminRole},{WebConstanta.CustomerRole}")]
       
        //Details Room
        public async Task<IActionResult> Details(int id)
        {

            List<Room> shoppingCartList = new List<Room>();
            if (HttpContext.Session.Get<IEnumerable<Room>>(WebConstanta.SessionCart) != null &&
               HttpContext.Session.Get<IEnumerable<Room>>(WebConstanta.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<Room>>(WebConstanta.SessionCart);
            }

            IndexViewModel indexViewModel = new IndexViewModel()
            {
                Room = await _questRoomsContextDb.Rooms.Where(u => u.Id == id).FirstOrDefaultAsync(),
                ExistInCart = false
            };
        
            foreach (var item in shoppingCartList)
            {
                if(item.Id == id)
                {
                    indexViewModel.ExistInCart = true;
                }
            }

            ViewBag.timeList = new SelectList(timeList);
            string imreBase64Data = Convert.ToBase64String(indexViewModel.Room.BytesImage);
            string imgDataURL = string.Format("data:image/png;base64,{0}", imreBase64Data);
            ViewBag.ImageData = imgDataURL;


            return View(indexViewModel);
        }


        //Add Datail to cart
     
        [HttpPost, ActionName("Details")]
        public IActionResult DetailsPost(int id, IndexViewModel detailsAdd)
        {
           
            List<Room> shoppingCartList = new List<Room>();
            if (HttpContext.Session.Get<IEnumerable<Room>>(WebConstanta.SessionCart) != null &&
               HttpContext.Session.Get<IEnumerable<Room>>(WebConstanta.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<Room>>(WebConstanta.SessionCart);
            }
            shoppingCartList.Add(new Room {
                Id = id,
                DateOrderRoom = detailsAdd.Room.DateOrderRoom,
                TimeOrderRoom = detailsAdd.Room.TimeOrderRoom
            });
            HttpContext.Session.Set(WebConstanta.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Index));
        }

        //Delete Detail
        public IActionResult RemoveFromCart(int id)
        {

            List<Room> shoppingCartList = new List<Room>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstanta.SessionCart) != null &&
               HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstanta.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<Room>>(WebConstanta.SessionCart);
            }

            var itemToRemove = shoppingCartList.SingleOrDefault(r => r.Id == id);

            if (itemToRemove != null)
                shoppingCartList.Remove(itemToRemove);


            HttpContext.Session.Set(WebConstanta.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Index));
        }

    }
}