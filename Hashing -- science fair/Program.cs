using System.Numerics;
using HashDependencies;
using TestLibrary;

static void Test()
{
    Console.WriteLine();
    Console.Write("Enter number of rounds (10000 recommended): ");

    int rounds = int.Parse(Console.ReadLine());

    Console.WriteLine();
    //Tester.AllTests(rounds);
    Tester.TimeTest(rounds);
    Console.WriteLine();

    Console.WriteLine("Test(s) Complete!");
}

Console.WriteLine("You are running Ari's Hash Function! If you wish to exit, simply type \"EXIT\" (or type CNTRL + C). If you wish to enter test mode type \"TEST\".");
Console.WriteLine();


int safetyLimit = 1000; // Max amount of hashes before automatic quit to prevent crashes.
for (int i = 0; i < safetyLimit; i++)
{
    Console.Write("String to hash [type and then press enter]: ");

    string str = Console.ReadLine();

    if (String.IsNullOrEmpty(str))
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine("There was an issue reading the input");
        Console.ForegroundColor = ConsoleColor.Black;

        continue;
    }

    switch (str)
    {
        case "EXIT":
            return;

        case "TEST":
            Test();
            return;
    }

    BigInteger hash = Utilities.Hash(str);
    Console.WriteLine(hash.ToString("X").TrimStart('0'));
    Console.WriteLine();
}