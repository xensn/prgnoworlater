using System.Collections.Concurrent;
using System.Threading.Channels;

namespace Icecream;

// Need to check and redo

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
    void ModifyIceCream()
    {
        try
        {
            List<string> icecreamoption = new List<string> { "cup", "cone", "waffle" };
            Console.WriteLine("Please select an ice cream to modify: ");
            int icecreamopt = Convert.ToInt32(Console.ReadLine());
            if (1 <= icecreamopt && icecreamopt <= IceCreamList.Count() + 1) 
            {
                IceCream chosenIceCream = IceCreamList[icecreamopt - 1];
                Console.WriteLine("Do you want to change the option of the ice cream selected[Y/N]?");
                string? input = Console.ReadLine()?.ToLower();
                if (!(input == "y" || input == "n"))
                {
                    Console.WriteLine("Invalid input. Please enter 'Y' or 'N'.");
                }

                if (input == "y")
                {
                    bool whileloop = true;
                    while (whileloop)
                    {
                        Console.WriteLine("Please select a new option (Cup, Cone, Waffle): ");
                        string modifyoption = Console.ReadLine().ToLower();

                        if (modifyoption == chosenIceCream.Option.ToLower())
                        {
                            Console.WriteLine(
                                $"Your ice cream is already a {modifyoption}. Please input another option.");
                        }
                        else if (!icecreamoption.Contains(modifyoption))
                        {
                            Console.WriteLine("Please choose from Cup, Cone, or Waffle.");
                        }
                        else
                        {
                            IceCream newicecream = null;
                            if (modifyoption == "cup")
                            {
                                newicecream = new Cup(modifyoption, chosenIceCream.Scoops, chosenIceCream.Flavours, chosenIceCream.Toppings);
                            }
                            else if (modifyoption == "cone")
                            {
                                
                            }
                            else if (modifyoption == "waffle")
                            {
                                
                            }

                            if (newicecream != null)
                            {
                                
                            }

                            IceCreamList[icecreamopt - 1] = newicecream;
                            whileloop = false;
                        }
                    }
                    
                }
                else
                {
                    
                }

            }
            else
            {
                throw new Exception("Please enter a correct ice cream within your current order to edit!");
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
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
        return $"ID: {Id} | Time Received: {TimeReceived:MM/dd/yyyy} | IceCream: {string.Join(", ", IceCreamList)} ";
    }
    
}