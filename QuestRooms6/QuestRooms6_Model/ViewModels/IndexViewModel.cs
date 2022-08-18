namespace QuestRooms6_Model.ViewModels
{
    public class IndexViewModel
    {
        public IndexViewModel()
        {
            Room = new Room();
        }
        public IndexViewModel(Room room)
        {
            Room = room;
        }
        public Room Room { get; set; }
        public IEnumerable<Room>? Rooms { get; set; }
        public List<double> CountPrice = new();
        public List<int> CountPerson = new();
        public PageViewModel? PageViewModel { get; set; }
        public bool ExistInCart { get; set; }
    }
}