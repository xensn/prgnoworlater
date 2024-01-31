namespace Icecream;
//========================================================== 
// Student Number : S10258427D
// Student Name : Senthilkumar Dhavasre
// Partner Name : Chia Eason
//========================================================== 

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