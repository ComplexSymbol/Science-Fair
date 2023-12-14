
//HASHING FUNCTION FOR 2023 SCIENCE FAIR -- MADE BY ARI TAMSKY
//VERSION 1: 11-20-23 (Wrote Input conversion)
//VERSION 2: 11-23-23 (Wrote Library, wrote IHV and const generation)
//VERSION 3: 11-24-23 (Improved Library, improved IHV and const generation)
//VERSION 4: 11-25-23 (Improved Library, finished IHV and const generation)
//VERSION 5: 11-25-23 (Improved Library, fixed edgecase scenarios, wrote a hash timer, started a hash cracker, wrote a random string generator, started GitHub for project)
//VERSION 6: 11-26-23 (Improved Library, wrote a bit distribution test, reformatted Program and HashDependencies)

//TODO: make the individual hash functions
//TODO: make a hash cracker

using System.Numerics;
using HashDependencies;
using TestLibrary;

static void Test()
{
    Console.WriteLine();
    Console.Write("Enter number of rounds (10000 recommended): ");

    int rounds = int.Parse(Console.ReadLine());

    Console.WriteLine();
    Tester.AllTests(rounds);
    Console.WriteLine();

    Console.WriteLine("Test Complete!");

    return;
}

Console.WriteLine("You are running Ari's Hash Function! If you wish to exit, simply type \"I COMMAND EXIT\". If you wish to enter test mode type \"I COMMAND TEST\".");

while (true)
{
    Console.Write("String to hash [type and then press enter]: ");

    string str = Console.ReadLine();

    switch (str.ToUpper())
    {
        case "I COMMAND EXIT":
            return;

        case "I COMMAND TEST":
            Test();
            break;
    }

    BigInteger hash = Utilities.Hash(str);

    Console.WriteLine(hash.ToString("X").TrimStart('0'));
    Console.WriteLine();
}