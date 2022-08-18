namespace QuestRooms6_Model.ViewModels
{
    public class RoomUserVM
    {
        public AplicationUser? AplicationUser { get; set; }
        public IList<Room> RoomList { get; set; }
        public RoomUserVM()
        {
            RoomList = new List<Room>();
        }
    }
}
