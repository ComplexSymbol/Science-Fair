//LIBRARY OF NECESSARY TOOLS FOR HASH FUNCTION -- MADE BY ARI TAMSKY

using System.Numerics;
using System.Text;

namespace HashDependencies
{
    ///<summary>
    ///Library of necessary bitwise functions for the hash function
    ///that are rarely included in official libraries.
    ///</summary>
    public class Utilities
	{
		///<summary>
		///Returns the first n bits in a ulong
		///</summary>
		public static ulong GetBits(ulong value, int n, bool ignoreLeading0Count = false) =>
            value >>> (64 - (ignoreLeading0Count ? 0 : BitOperations.LeadingZeroCount(value)) - n);

        ///<summary>
        ///Returns the number of decimal places in a number
        ///</summary>        
        public static int GetDecimalPlaces(decimal n)
        {
            n = Math.Abs(n); //make sure it is positive.
            n -= (int)n;     //remove the integer part of the number.

            var decimalPlaces = 0;

            while (n > 0)
            {
                decimalPlaces++;
                n *= 10;
                n -= (int)n;
            }
            return decimalPlaces;
        }

        ///<summary>
        ///Returns the fraction part of a decimal (in ulong format)
        ///</summary>
        public static ulong GetFraction(decimal value, int n = 64)
		{
			decimal v = value;

            int numPlaces = GetDecimalPlaces(v);

            return (ulong)((double)(value % 1) * Math.Pow(10, numPlaces));
		}

        /// <summary>
        /// Returns a the value inputted formatted into an array of 0s or 1s in binary
        /// </summary>
        public static int[] GetBitList(ulong value)
        {
            int[] bitList = new int[64];

            for (int i = 0; i < 64; i++)
                bitList[i] = (value & (1ul << (63 - i))) > 0 ? 1 : 0;

            /* Readable version:
            for (int i = 0; i < 64; i++)
            {
                ulong mask = 1ul << (63 - i);

                if ((value & mask) > 0)
                    bitList[i] = 1;

                else
                    bitList[i] = 0;
            }
            */

            return bitList;
        }

		///<summary>
		///Sets the nth bit of a ulong to off (0), on (1), or toggle (2)
		///</summary>
		public static ulong SetBit(ulong value, int n, int setting)
		{
			if (n > 64)
				throw new ArgumentException("Arguement 'n' is larger than the amount of bits supported by a ulong");

            if (setting is not (0 or 1 or 2)) //Setting is not a valid option
                throw new ArgumentException("Arguement 'setting' is not a valid option (0, 1, or 2 are the valid options for 'setting')");

            //bit n in a ulong
            ulong mask = BitOperations.RotateRight(1ul, n);

			//If value contains bit n and setting equals off, turn bit off
			if (value >= mask && setting == 0)
				return value - mask;

			//If value does not contain bit n and setting equals on, turn bit on
			else if (value < mask && setting == 1)
				return value + mask;

            //If the setting is toggle
            else if (setting == 2)
                return value ^ mask;

			return value;
        }

        ///<summary>
        ///Setup for hash function, returns a tuple array of Item1: consts, Item2: initialHashVals
        ///</summary>
        public static (ulong, ulong)[] Hash(string input, bool constIHVMode = true)
        {
            char[] chars = input.ToCharArray();

            int popCount = 0;
            byte[] asciiArr = new byte[chars.Length];

            ulong[] initialHashVals = new ulong[8];
            ulong[] consts = new ulong[64];
            (ulong, ulong)[] allVals = new (ulong, ulong)[64];


            //Convert characters to numbers (in bytes)
            for (int i = 0; i < asciiArr.Length; i++)
                asciiArr[i] = Encoding.Default.GetBytes(chars[i].ToString())[0];

            //Count the number of bits that are on in the input
            foreach (byte b in asciiArr)
                popCount += BitOperations.PopCount((ulong)b);

            //Set the 8 initial hash values (see background research plan)
            for (int i = 1; i < 65; i++)
            {
                if (i - 1 < 8)
                {
                    initialHashVals[i - 1] = BitOperations.RotateLeft(SetBit(GetFraction((decimal)(2 * Math.Log(popCount * popCount * Math.Pow(i, 5) + 2)), 32), 1, 1), i);
                    //Just making the values conform to 8 hex digits (simply %= 0xFFFFFFFF wouldn't work because values slightly above 0xFFFFFFF would be too small.)
                    initialHashVals[i - 1] %= 0xEFFFFFFF;
                    initialHashVals[i - 1] += 0x10000000;

                    allVals[i - 1].Item2 = initialHashVals[i - 1];
                }

                consts[i - 1] = BitOperations.RotateLeft(SetBit(GetFraction((decimal)(2 * Math.Log(popCount * popCount * Math.Pow(i, 6) + 2)), 32), 1, 1), i);
                consts[i - 1] %= 0xEFFFFFFF;
                consts[i - 1] += 0x10000000;

                allVals[i - 1].Item1 = consts[i - 1];
            }

            foreach (ulong i in initialHashVals)
                Console.WriteLine("0x" + i.ToString("X"));

            Console.WriteLine();
            Console.WriteLine("consts: ");

            foreach (ulong i in consts)
                Console.WriteLine("0x" + i.ToString("X"));

            return allVals;
        }
    }
}

