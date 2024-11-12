partial class Program
{
    private static void Main(string[] args)
    {
        Welcome_8064();
        Welcome_3817();
        Console.ReadKey();
    }
    static partial void Welcome_3817();
    private static void Welcome_8064()
    {
        Console.WriteLine("Enter your name: ");
        string name = Console.ReadLine();
        Console.WriteLine("{0} Welcome to my first console application", name);
    }
}