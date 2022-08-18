namespace QuestRooms6_Model.ViewModels
{
    public class OrderVm
    {
        public OrderHeader OrderHeader { get; set; }
        public IEnumerable<OrderDetail> OrderDetails { get; set; }
    }
}