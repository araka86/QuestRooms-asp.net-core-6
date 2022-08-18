using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestRooms6_DataAccess.Data;
using QuestRooms6_Model;

namespace QuestRooms6.Controllers
{
    [Authorize(Roles = WebConstanta.AdminRole)]
    public class RoomsController : Controller
    {
        private readonly QuestRoomsContextDb _questRoomsContextDb;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public RoomsController(QuestRoomsContextDb questRoomsContextDb, IWebHostEnvironment webHostEnvironment)
        {
            _questRoomsContextDb = questRoomsContextDb;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index(string? searchName = null,
            int? searchCountPlayer = null,
            double? searchPrice = null,
            string? srchDifLvl = null,
            string? ScrFerLvl = null,
            string? inputreset = null)
        {

            IEnumerable<Room>? rooms = _questRoomsContextDb.Rooms;
            if (inputreset == null)
            {
                if (!string.IsNullOrEmpty(searchName))
                {
                    rooms = rooms.Where(r => r.Name.ToLower().Contains(searchName.ToLower()));
                }
                if (searchCountPlayer != null)
                {
                    rooms = rooms.Where(r => r.Price == searchPrice);
                }
                if (searchPrice != null)
                {
                    rooms = rooms.Where(r => r.Price == searchPrice);
                }
                if (!string.IsNullOrEmpty(srchDifLvl) && srchDifLvl != "--Difficult Level--")
                {
                    rooms = rooms.Where(r => r.DifltLevel.ToLower().Contains(srchDifLvl.ToLower()));

                }
                if (!string.IsNullOrEmpty(ScrFerLvl) && ScrFerLvl != "--Fear Level--")
                {
                    rooms = rooms.Where(r => r.FLevel.ToLower().Contains(ScrFerLvl.ToLower()));

                }
            }
            return View(rooms);
        }

        //Get - Upsert(Views-->Index (create/update))

        public async Task<IActionResult> Upsert(int? id)
        {

            var room = new Room();

            if (id == null) //Check object
            {
                //this is for create
                return View(room);
            }
            else
            {
                //update
                room = await _questRoomsContextDb.Rooms.FindAsync(id);

                // string to enum
                var resultDificultLevel = Enum.GetValues(typeof(DifficultLevel)).OfType<DifficultLevel>()
                       .Where(x => x.ToString().StartsWith(room.DifltLevel))
                       .Select(x => (int)x).ToArray();
                var resultFeraltLevel = Enum.GetValues(typeof(FearLevel)).OfType<FearLevel>()
                     .Where(x => x.ToString().StartsWith(room.FLevel))
                     .Select(x => (int)x).ToArray();

                room.DifficultLevel = (DifficultLevel)resultDificultLevel[0];
                room.FearLevel = (FearLevel)resultFeraltLevel[0];

                if (room == null)
                    return NotFound();


                //Convert byte arry to base64string   
                string imreBase64Data = Convert.ToBase64String(room.BytesImage);
                string imgDataURL = string.Format("data:image/png;base64,{0}", imreBase64Data);
                //Passing image data in viewbag to view  
                ViewBag.ImageData = imgDataURL;
                return View(room);
            }
        }


        //Post - Upsert (Views-->Upsert(only UPDATE) )
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Room room)
        {
            if (ModelState.IsValid)
            {
                byte[] convertByte;
                var files = HttpContext.Request.Form.Files; //get image
                string webRootPath = _webHostEnvironment.WebRootPath; //get path to wwwroot

                if (room.Id == 0) //-------------> create <-----------------------
                {
                    var createRoom = new Room();

                    var filePath = Path.GetTempFileName();


                    if (room.Image != "" || files.Count() != 0)
                    {

                        //string upload = webRootPath + WebConstanta.ImagePath; //get path from image
                        //string fileName = Guid.NewGuid().ToString();       //greate random guid
                        //string extension = Path.GetExtension(files[0].FileName); //get extension file which uploaded
                        //using (var filestream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        //{
                        //    files[0].CopyTo(filestream);
                        //}
                        //createRoom.Image = fileName + extension;

                        if (!Directory.Exists(WebConstanta.ImagePathUser))
                            Directory.CreateDirectory(WebConstanta.ImagePathUser);
                        

                        string pathstringforFile = string.Empty;
                        string upload = WebConstanta.ImagePathUser; //get path from image
                        string fileName = Guid.NewGuid().ToString();       //greate random guid
                        string extension = Path.GetExtension(files[0].FileName); //get extension file which uploaded
                        using (var filestream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {

                            files[0].CopyTo(filestream); //sendFile to temporary place
                            pathstringforFile = filestream.Name.ToString();
                        }
                        convertByte = ReadImageFile(pathstringforFile);
                        createRoom.BytesImage = convertByte;
                    }

                    createRoom.Name = room.Name;
                    createRoom.Description = room.Description;
                    createRoom.CountPlayers = room.CountPlayers;
                    createRoom.Price = room.Price;
                    createRoom.DifltLevel = room.DifficultLevel.ToString();
                    createRoom.FLevel = room.FearLevel.ToString();
                    await _questRoomsContextDb.Rooms.AddAsync(createRoom);
                }
                else //update
                {
                    //updating
                    var objFromDB = _questRoomsContextDb.Rooms.AsNoTracking().FirstOrDefault(u => u.Id == room.Id);  //  off Tracking id
                    if (files.Count > 0)
                    {
                        //string upload = webRootPath + WebConstanta.ImagePath; //get path from image
                        //string fileName = Guid.NewGuid().ToString();       //greate random guid
                        //var oldFile = Path.Combine(upload, objFromDB.Image); // link on the old fille
                        //                                                     //check and dell old fille
                        //if (System.IO.File.Exists(oldFile))
                        //    System.IO.File.Delete(oldFile);

                        //string extension = Path.GetExtension(files[0].FileName); //get extension file which uploaded
                        //using (var filestream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        //{
                        //    files[0].CopyTo(filestream);
                        //}
                        //objFromDB.Image = fileName + extension;

                        if (!Directory.Exists(WebConstanta.ImagePathUser))
                            Directory.CreateDirectory(WebConstanta.ImagePathUser);

                        string pathstringforFile = string.Empty;
                        string upload = WebConstanta.ImagePathUser; //get path from image
                        string fileName = Guid.NewGuid().ToString();       //greate random guid
                        string extension = Path.GetExtension(files[0].FileName); //get extension file which uploaded
                        using (var filestream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {

                            files[0].CopyTo(filestream); //sendFile to temporary place
                            pathstringforFile = filestream.Name.ToString();
                        }
                        convertByte = new byte[ReadImageFile(pathstringforFile).Length];
                        convertByte = ReadImageFile(pathstringforFile);
                        objFromDB.BytesImage = convertByte;
                        objFromDB.Name = room.Name;
                        objFromDB.Description = room.Description;
                        objFromDB.CountPlayers = room.CountPlayers;
                        objFromDB.Price = room.Price;
                        objFromDB.DifltLevel = room.DifficultLevel.ToString();
                        objFromDB.FLevel = room.FearLevel.ToString();
                    }
                    else
                    {
                        objFromDB.Name = room.Name;
                        objFromDB.Description = room.Description;
                        objFromDB.CountPlayers = room.CountPlayers;
                        objFromDB.Price = room.Price;
                        objFromDB.DifltLevel = room.DifficultLevel.ToString();
                        objFromDB.FLevel = room.FearLevel.ToString();

                    }
                    _questRoomsContextDb.Attach(objFromDB).State = EntityState.Modified;
                }
            }
            await _questRoomsContextDb.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public static byte[] ReadImageFile(string imageLocation)
        {
            byte[] imageData = null;
            FileInfo fileInfo = new FileInfo(imageLocation);
            long imageFileLength = fileInfo.Length;
            FileStream fs = new FileStream(imageLocation, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            imageData = br.ReadBytes((int)imageFileLength);
            return imageData;
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            var room = await _questRoomsContextDb.Rooms.FindAsync(id);
            _questRoomsContextDb.Rooms.Remove(room);
            await _questRoomsContextDb.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
