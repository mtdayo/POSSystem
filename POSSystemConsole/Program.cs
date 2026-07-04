Console.WriteLine("Hello, World!");

string? input;
//受け取りました。

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