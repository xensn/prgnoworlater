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
    public void ModifyIceCream()
    {
        while(true)
        {
            try 
            {
                Console.Write("Please select an ice cream to modify: ");
                int icecreamopt = Convert.ToInt32(Console.ReadLine());
                
                // making sure the customer will only input what is within their ice cream list
                if (1 <= icecreamopt && icecreamopt <= IceCreamList.Count() + 1) 
                {
                    // asking if they want to change their option 
                    Console.Write("Do you want to change the option of the ice cream selected[Y/N]?");
                    string? input1 = Console.ReadLine()?.ToLower();
                    
                    if (!(input1 == "y" || input1 == "n"))
                    {
                        Console.Write("Invalid input. Please enter 'Y' or 'N'.");
                    }
                    
                    if (input1 == "y")
                    {
                        ChangeIceCreamOption(icecreamopt);
                    }
                    else
                    {
                        Console.Write($"The option of your ice cream will remain as a {IceCreamList[icecreamopt-1].Option}");
                    }
                    // changing the scoops and flavour and toppings 
                    ChangingScoopsFlavourToppings(icecreamopt);
                }
                else
                {
                    throw new Exception("Please enter a correct ice cream within your current order to edit!");
                }
                
                break;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }
    }

    public void AddIceCream(IceCream ic)
    {
        IceCreamList.Add(ic);
    }

    public void DeleteIceCream()
    {
        while (true)
        {
            try
            {
                Console.Write("Please select an ice cream that you want to delete");
                int icecreamtodelete = Convert.ToInt32(Console.ReadLine());

                if (1 <= icecreamtodelete && icecreamtodelete <= IceCreamList.Count() + 1)
                {
                    if(!(IceCreamList.Count == 1))
                    {
                        IceCreamList.RemoveAt(icecreamtodelete - 1);
                    }
                    else
                    {
                        Console.WriteLine("There is only one ice cream object within the order. You cannot have zero ice creams in your order");
                    }
                }
                else
                {
                    throw new Exception("Please enter a correct ice cream within your current order to delete!");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
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
    
    // Editing the option of the ice cream
    public void ChangeIceCreamOption(int icecreamopt)
    {
        IceCream chosenIceCream = IceCreamList[icecreamopt - 1];
         bool whileloop = true;
                    while (whileloop)
                    {
                        Console.WriteLine("Please select a new option (Cup, Cone, Waffle): ");
                        string modifyoption = Console.ReadLine().ToLower();
                        
                        // this list is to check if the user inputs the correct option 
                        List<string> icecreamoption = new List<string> { "cup", "cone", "waffle" };

                        if (modifyoption == chosenIceCream.Option.ToLower())
                        {
                            Console.WriteLine($"Your ice cream is already a {modifyoption}. Please input another option.");
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
                                Console.WriteLine("Do you want your cone to be dipped?[Y/N]That will be an extra $2.");
                                string? input2 = Console.ReadLine()?.ToLower();
                                if (!(input2 == "y" || input2 == "n"))
                                {
                                    Console.WriteLine("Invalid input. Please enter 'Y' or 'N'.");
                                }
                                
                                if (input2 == "y")
                                {
                                    newicecream = new Cone(modifyoption, chosenIceCream.Scoops, chosenIceCream.Flavours,
                                        chosenIceCream.Toppings, true);
                                }
                                else
                                {
                                    newicecream = new Cone(modifyoption, chosenIceCream.Scoops, chosenIceCream.Flavours, chosenIceCream.Toppings, false);
                                }
                            }
                            else if (modifyoption == "waffle")
                            {
                                string waffleflavour =
                                    Program.CheckUserInput( new List<string>() { "original", "red velvet", "charcoal", "pandan"},
                                        "Choose a waffle flavour: ");
                                newicecream = new Waffle(modifyoption, chosenIceCream.Scoops, chosenIceCream.Flavours,
                                    chosenIceCream.Toppings, waffleflavour);

                            }
                            if (newicecream != null)
                            {
                                IceCreamList[icecreamopt - 1] = newicecream; // replacing the old ice cream with a new ice cream
                                Console.WriteLine($"The option of your ice cream has changed to a {newicecream.Option}");
                            } 
                            whileloop = false; // break the while loop
                        }
                    }
    }
    
    // Modifying the Scoops, Flavours and the Toppings of the ice cream
    public void ChangingScoopsFlavourToppings(int icecreamopt)
    {
        bool whileloop = true;
        while (whileloop)
        {
            int whattomodify = Program.CheckIntInput("[1] The number of scoops & the flavours of the ice cream" +
                                                        "\n[2] The Toppings added to the ice cream" +
                                                        "\n[3] Exit" +
                                                        "\nPlease pick what you want to modify in your ice cream", 1, 3);
            switch (whattomodify)
            {
                case 1:
                    int scoops = Program.CheckIntInput("Enter number of scoops(1-3): ", 1, 3);
                    Program.GetFlavours(scoops);
                    List<Flavour> flavours = Program.GetFlavours(scoops);
                    IceCreamList[icecreamopt - 1].Scoops = scoops;
                    IceCreamList[icecreamopt - 1].Flavours = flavours;
                    break;
                case 2:
                    List<Topping> toppings = Program.GetToppings();
                    IceCreamList[icecreamopt - 1].Toppings = toppings;
                    break;
                case 3:
                    whileloop = false;
                    break;
            }
        }
    }
    
}
