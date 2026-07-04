Console.WriteLine("Hello, World!");

string? input;

do
{
    Console.Write("a:");
    input = Console.ReadLine();     

    if (input == "exit")
    {
        Console.WriteLine("Exiting the program.");
        break;
    }
    Console.WriteLine($"You entered: {input}");

} while (input != "exit");