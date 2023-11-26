#define DEBUGGING

//HASHING FUNCTION FOR 2023 SCIENCE FAIR -- MADE BY ARI TAMSKY
//VERSION 1: 11-20-23 (WroteInput conversion)
//VERSION 2: 11-23-23 (Wrote Library, wrote IHV and const generation)
//VERSION 3: 11-24-23 (Improved Library, improved IHV and const generation)
//VERSION 4: 11-25-23 (Improved Library, finished IHV and const generation)
//VERSION 5: 11-25-23 (Improved Library, fixed edgecase scenarios, wrote a hash timer, started a hash cracker)

//TODO: make the individual hash functions
//TODO: make a hash cracker

using System.Numerics;
using HashDependencies;
using TestLibrary;


#if DEBUGGING

Console.WriteLine();
Tester.DistributionTest(100, 100);
//Tester.TimeTest(1000000, 1000);

Console.WriteLine(string.Join("", Utilities.GetBitList(0x513)));

Console.WriteLine();

#endif


#if !DEBUGGING
while (true)
{
    Console.Write("String to hash: ");

    string str = Console.ReadLine();

    Utilities.Hash(str);
}
#endif