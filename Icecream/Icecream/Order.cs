namespace Icecream;

public class Order
{
    public int Id { get; set; }
    public DateTime TimeReceived { get; set; }
    public DateTime? TimeFulfilled { get; set; } = new DateTime();
    public List<IceCream> IceCreamList { get; set; } = new List<IceCream>();
    
    public Order(){}

    public Order(int id, DateTime timeReceived)
    {
        Id = id;
        TimeReceived = timeReceived;
    }

    void ModifyIceCream(int)
    {
        
    }

    public void AddIceCream(IceCream ic)
    {
        IceCreamList.Add(ic);
    }

    public void DeleteIceCream(int index)
    {
        IceCreamList.RemoveAt(index);
    }
    
}