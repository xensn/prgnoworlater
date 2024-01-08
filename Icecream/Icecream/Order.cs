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
    
    // Need to edit the property
    void ModifyIceCream(int i)
    {
        IceCream chosenIceCream = IceCreamList[i - 1];
        
    }

    public void AddIceCream(IceCream ic)
    {
        IceCreamList.Add(ic);
    }

    public void DeleteIceCream(int index)
    {
        IceCreamList.RemoveAt(index);
    }
    
    // Not sure if working, need to test
    public double CalculateTotal()
    {
        double total = 0;
        foreach (IceCream ic in IceCreamList)
        {
            total += ic.CalculatePrice();
        }

        return total;
    }

    public override string ToString()
    {
        return $"ID: {Id} | Time Received: {TimeReceived:MM/dd/yyyy} | ";
    }
    
}