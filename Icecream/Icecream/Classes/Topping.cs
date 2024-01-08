namespace Icecream;

public class Topping
{
    public string Type { get; set; }
    
    public Topping(){}

    public Topping(string type)
    {
        Type = type;
    }

    public override string ToString()
    {
        return $"Type: {Type}";
    }
}