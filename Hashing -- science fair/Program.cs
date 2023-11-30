//#define TESTING

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


#if TESTING

Console.WriteLine();
Tester.CollisionTest(1000000, 1000);
Console.WriteLine();
Tester.DistributionTest(1000000, 1000);
Console.WriteLine();
Tester.TimeTest(1000000, 1000);
Console.WriteLine();

#endif


#if !TESTING
while (true)
{
    Console.Write("String to hash: ");

    string str = Console.ReadLine();

    Utilities.Hash(str, true);
}
#endif