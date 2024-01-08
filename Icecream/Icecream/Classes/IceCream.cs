namespace Icecream;

public abstract class IceCream
{
    public string Option { get; set; }
    public int Scoops { get; set; }
    public List<Flavour> Flavours { get; set; }
    public List<Topping> Toppings { get; set; }
    
    public IceCream(){}

    public IceCream(string option, int scoops, List<Flavour> flavours, List<Topping> toppings)
    {
        Option = option;
        Scoops = scoops;
        Flavours = flavours;
        Toppings = toppings;
    }
    
    public abstract double CalculatePrice();

    public override string ToString()
    {
        return $"Option: {Option} | Scoops: {Scoops} | Flavours: {string.Join(",", Flavours)} | Toppings: {string.Join(",", Toppings)}";
    }
}