//TESTING LIBRARY FOR HASH FUCTION -- MADE BY ARI TAMSKY

using HashDependencies;
using System.Diagnostics;
using System.Numerics;

namespace TestLibrary
{
    public class Tester
    {
        private static readonly char[] baseChars = Enumerable.Range('A', 26).Select(x => (char)x).ToArray();

        //Number to hexavigesimal (base 26, A-Z)
        private static string NumToString(int n)
        {
            // 32 is the worst cast buffer size for base 2 and int.MaxValue
            int i = 32;
            char[] buffer = new char[i];
            int targetBase = baseChars.Length;

            do
            {
                buffer[--i] = baseChars[n % targetBase];
                n /= targetBase;
            }
            while (n > 0);

            char[] result = new char[32 - i];
            Array.Copy(buffer, i, result, 0, 32 - i);

            return new string(result);
        }


        /// <summary>
        /// Random string generator for testing purposes
        /// </summary>
        private static string StringGenerator(int length)
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] stringChars = new char[length];
            int random = System.Security.Cryptography.RandomNumberGenerator.GetInt32(chars.Length - 1);

            for (int i = 0; i < length; i++)
                stringChars[i] = chars[random];

            var finalString = new String(stringChars);

            return finalString;
        }

        /// <summary>
        /// Runs an execution time test for the hash function using System.Diagnostics.Stopwatch
        /// </summary>
        public static void TimeTest(int rounds)
        {
            //HASH TIME TEST
            decimal[] times = new decimal[rounds - 1];
            var watch = new Stopwatch();

            Console.WriteLine($"Now testing: Execution time: (Hash) (r = {rounds})");

            for (int i = 0; i < rounds; i++)
            {
                watch.Reset();
                watch.Start();
                //byte[] shaw = System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(NumToString(i)));
                Utilities.Hash(NumToString(i));
                watch.Stop();

                if (i > 1)
                    times[i - 1] = watch.ElapsedMilliseconds;

                Console.Write($"{(decimal)(100 * i) / rounds}%\r");
            }

            Console.WriteLine($"Test Complete. Average execution time is {times.Average()}ms");
        }

        /// <summary>
        /// Runs a distribution test to see if the distribution of bits in the hash is even
        /// </summary>
        public static void DistributionTest(int rounds)
        {
            int[] bitCounter = new int[256];

            Console.WriteLine($"Now testing: Const distribution: (Hash) (r = {rounds})");
            Console.WriteLine();

            for (int i = 0; i < rounds; i++)
            {
                bitCounter = bitCounter.Zip(Utilities.GetBitList(Utilities.Hash(StringGenerator(100))), (x, y) => x + y).ToArray();

                Console.Write($"{(decimal)(100 * i) / rounds}%\r");
            }

            Console.WriteLine("Test Complete. Results: ");
            Console.WriteLine(string.Join(", ", bitCounter));
        }

        //DEPRICATED. USE BIRTHDAY TEST
        public static void CollisionTest(int rounds)
        {
            BigInteger[] duplicateDetector = new BigInteger[rounds];

            Console.WriteLine($"Now testing: Collisions: (Hash) (r = {rounds})");
            Console.WriteLine();

            for (int i = 0; i < rounds; i++)
            {
                duplicateDetector[i] = Utilities.Hash(NumToString(i));

                Console.Write($"{(decimal)(100 * i) / rounds}%\r");
            }

            int error = duplicateDetector.Length - duplicateDetector.Distinct().Count();

            Console.WriteLine($"Test Complete. Results: {error} collisions found ({Math.Round(((decimal)error * 2) / rounds, 2)}% of outputs are duplicates)");
        }

        //Mods to 16 by default
        public static void BirthdayTest(int rounds, int H = 65_536)
        {
            ulong[] duplicateDetector = new ulong[rounds];
            int[] results = new int[rounds];

            int resultCounter = 0;
            for (int i = 0, c = 0; i < rounds && c < rounds; i++, c++)
            {
                duplicateDetector[i] = (ulong)(Utilities.Hash(NumToString(c)) % (H + 1));

                //Found collision
                if ((rounds - i - 2) + duplicateDetector.Distinct().Count() < rounds)
                {
                    //Log collision
                    results[resultCounter] = i;
                    resultCounter++;

                    //Resetting for next collision detection
                    Array.Clear(duplicateDetector);

                    i = -1;

                    continue;
                }

                Console.Write($"{(decimal)(100 * c) / rounds}%\r");
            }

            results = results[0..resultCounter];

            Console.WriteLine(string.Join(",", results));
            Console.WriteLine($"Test Complete. Results: Result AVG: {(decimal)results.Sum() / results.Length}" );
        }

        public static void AllTests(int rounds)
        {
            BigInteger[] duplicateDetector = new BigInteger[rounds];
            int[] bitCounter = new int[256];
            decimal[] times = new decimal[rounds - 1];
            var watch = new Stopwatch();

            for (int i = 0; i < rounds; i++)
            {
                BigInteger hash = Utilities.Hash(NumToString(i));

                duplicateDetector[i] = hash;
                bitCounter = bitCounter.Zip(Utilities.GetBitList(hash), (x, y) => x + y).ToArray();

                watch.Reset();
                watch.Start();
                Utilities.Hash(NumToString(i));
                watch.Stop();

                if (i > 1)
                    times[i - 1] = watch.ElapsedMilliseconds;

                Console.Write($"{(decimal)(100 * i) / rounds}%\r");
            }

            int numDuplicatedHashes = duplicateDetector.Length - duplicateDetector.Distinct().Count();


            Console.WriteLine($"Test Complete. Results: {numDuplicatedHashes} collisions found. ({Math.Round(((decimal)numDuplicatedHashes * 2) / rounds, 2)}% of outputs are duplicates)");
            Console.WriteLine();
            Console.WriteLine($"Bit Distribution: \n{string.Join(" ", bitCounter)}");
            Console.WriteLine();
            Console.WriteLine($"Average execution time is {Math.Round(times.Average(), 5)} ms");
        }
    }
}

