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
		public static string NumToString(int n)
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

        public static string StringGenerator(int length)
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
                Utilities.Hash(NumToString(i));
				watch.Stop();

				if (i > 1)
					times[i - 1] = watch.ElapsedMilliseconds;

                Console.Write($"{(decimal)(100 * i) / rounds}%\r");
            }

            Console.WriteLine($"Test Complete. Average execution time is {Math.Round(times.Average(), 6)}ms");
		}

		/// <summary>
		/// Tries to crack the hash function, essencially testing either preimage or collision resistance
		/// </summary>
		public static void CrackTest(int rounds, int length)
		{
			
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

		public static void CollisionTest(int rounds)
		{
			BigInteger[] duplicateDetector = new BigInteger[rounds];

            Console.WriteLine($"Now testing: Collisions: (Hash) (r = {rounds})");
            Console.WriteLine();

            for (int i = 0; i < rounds; i++)
            {
				duplicateDetector[i] = Utilities.Hash(NumToString(i));

                Console.Write($"{(decimal)(100 * i)/rounds}%\r");
            }

			int error = duplicateDetector.Length - duplicateDetector.Distinct().Count();

            Console.WriteLine($"Test Complete. Results: {error} collisions found ({Math.Round(((decimal)error * 2) / rounds, 2)}% of outputs are duplicates)");
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
            Console.WriteLine($"Bit Distribution: {string.Join(" ", bitCounter)}");
            Console.WriteLine($"Average execution time is {Math.Round(times.Average(), 5)} ms");
        }
    }
}

