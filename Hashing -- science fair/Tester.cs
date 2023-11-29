//TESTING LIBRARY FOR HASH FUCTION -- MADE BY ARI TAMSKY

using HashDependencies;
using System.Diagnostics;

namespace TestLibrary
{
	public class Tester
	{
		/// <summary>
		/// Returns a random string from the set of characters that is variable length
		/// </summary>
		public static string StringGenerator(int length)
		{
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] stringChars = new char[length];
            Random random = new Random();

            for (int i = 0; i < length; i++)
                stringChars[i] = chars[random.Next(chars.Length)];

            var finalString = new String(stringChars);

			return finalString;
        }

		/// <summary>
		/// Runs an execution time test for the hash function using System.Diagnostics.Stopwatch
		/// </summary>
		public static void TimeTest(int rounds, int length)
		{
			//HASH TIME TEST
			decimal[] times = new decimal[rounds - 1];
			var watch = new Stopwatch();

			Console.WriteLine($"Now testing: Execution time: (Hash) (l = {length}, r = {rounds})");

			for (int i = 0; i < rounds; i++)
			{
				watch.Reset();
				watch.Start();
                Utilities.Hash(StringGenerator(length));
				watch.Stop();

				if (i > 1)
					times[i - 1] = watch.ElapsedMilliseconds;

                Console.Write($"{Math.Round(((float)i / rounds) * 100, 2)}%\r");
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
		public static void DistributionTest(int rounds, int length)
		{
			int[] bitCounter = new int[64];

			Console.WriteLine($"Now testing: Const distribution: (Hash) (l = {length}, r = {rounds})");
			Console.WriteLine();

            for (int i = 0; i < rounds; i++)
            {
                ulong[] currentHashReturn = Utilities.Hash(StringGenerator(length));

				for (int j = 0; j < currentHashReturn.Length; j++)
					bitCounter.Zip(Utilities.GetBitList(currentHashReturn[j]));

                Console.Write($"{Math.Round(((float)i / rounds) * 100, 2)}%\r");
            }

            Console.WriteLine("Test Complete. Results: ");
			Console.WriteLine(string.Join(", ", bitCounter));
        }

		public static void CollisionTest(int rounds, int length)
		{
			ulong[][] duplicateDetector = new ulong[rounds][];

            Console.WriteLine($"Now testing: Collisions: (Hash) (l = {length}, r = {rounds})");
            Console.WriteLine();

            for (int i = 0; i < rounds; i++)
            {
                ulong[] currentHashReturn = Utilities.Hash(StringGenerator(length));

                duplicateDetector[i] = currentHashReturn;

                Console.Write($"{Math.Round(((float)i / rounds) * 100, 2)}%\r");
            }

			int error = duplicateDetector.Length - duplicateDetector.Distinct().Count();

            Console.WriteLine($"Test Complete. Results: {error} collisions found ({Math.Round((decimal)error / (rounds * 64), 2)}% of outputs are duplicates)");
        }
	}
}

