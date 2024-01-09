using System.Drawing;

// Need to check and redo

namespace Icecream;

public class Customer
{
    public string Name { get; set; }
    public int Memberid { get; set; }
    public DateTime Dob { get; set; }
    public Order CurrentOrder { get; set; }
    public List<Order> OrderHistory { get; set; } = new List<Order>();
    public PointCard Rewards { get; set; }
    
    public Customer(){}

    public Customer(string name, int memberid, DateTime dob)
    {
        Name = name;
        Memberid = memberid;
        Dob = dob;
        
        CurrentOrder = new Order();
        OrderHistory = new List<Order>();
        Rewards = new PointCard();
    }
    public Order MakeOrder()
    {
        return null;
    }

    public bool isBirthday()
    {
        if (Dob == DateTime.Now)
        {
            return true;
        }

        return false;
    }

    public override string ToString()
    {
        return $"Name: {Name} | ID: {Memberid} | Dob: {Dob}";
    }
}