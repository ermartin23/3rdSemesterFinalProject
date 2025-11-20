using DataAccess.Pricing;

namespace DataAccess.Pricing;

public static class ChosenNumbersPackages
{
    public static decimal GetPrice(int count)
    {
        return count switch
        {
            1 => 10,
            2 => 15,
            3 => 18,
            4 => 19,
            5 => 20,
            6 => 25,
            7 => 30,
            _ => 0
        };
    }
}