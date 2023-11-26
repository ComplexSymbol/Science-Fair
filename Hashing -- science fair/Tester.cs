using System;
using HashDependencies;
using System.Diagnostics;
using System.Linq;

namespace TestLibrary
{
	public class Tester
	{
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
		
		public static void TimeTest(int rounds, int length)
		{
			//HASH TIME TEST
			decimal[] times = new decimal[rounds - 1];
			var watch = new Stopwatch();

			Console.WriteLine("Now testing: Execution time: (Hash)");
			Console.WriteLine("...");

			for (int i = 0; i < rounds; i++)
			{
				watch.Reset();
				watch.Start();
                Utilities.Hash(StringGenerator(length));
				watch.Stop();

				if (i > 1)
					times[i - 1] = watch.ElapsedMilliseconds;
			}

			Console.WriteLine($"Test Complete. Average execution time is {times.Average()}ms for string size {length}");
		}

		public static void CrackTest(int rounds, int length)
		{
			for (int i = 0; i < rounds; i++)
			{
				ulong[] initialHashVals = new ulong[8];
				ulong[] consts = new ulong[64];

				(ulong, ulong)[] currentHashReturn = Utilities.Hash(StringGenerator(length));

				for (int j = 0; j < currentHashReturn.Length; j++)
				{
					if (j < 8)
						initialHashVals[j] = currentHashReturn[j].Item2;

					consts[j] = currentHashReturn[j].Item1;
				}

				
			}
		}

		public static void DistributionTest(int rounds, int length)
		{
			int[] bitCounter = new int[32];

            for (int i = 0; i < rounds; i++)
            {
                ulong[] initialHashVals = new ulong[8];
                ulong[] consts = new ulong[64];

                (ulong, ulong)[] currentHashReturn = Utilities.Hash(StringGenerator(length));

                for (int j = 0; j < currentHashReturn.Length; j++)
                {
                    if (j < 8)
                        initialHashVals[j] = currentHashReturn[j].Item2;

                    consts[j] = currentHashReturn[j].Item1;
                }

				for (int j = 0; j < consts.Length; j++)
					bitCounter.Zip(Utilities.GetBitList(consts[j]));
            }

			Console.WriteLine(string.Join(", ", bitCounter));
        }
	}
}

