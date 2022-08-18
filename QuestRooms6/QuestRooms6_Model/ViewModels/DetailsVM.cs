namespace QuestRooms6_Model.ViewModels
{
    public class DetailsVM
    {
        public DetailsVM()
        {
            Room = new Room();
        }
        public Room Room { get; set; }
        public bool ExistInCart { get; set; }
    }
}
