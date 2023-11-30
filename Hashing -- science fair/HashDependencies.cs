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
        private static uint[] consts12 = new uint[0];

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

        public static uint AdditionMod32(uint a, uint b, uint c = 0, uint d = 0, uint e = 0)
        {
            ulong A = a;
            ulong B = b;
            ulong C = c;
            ulong D = d;
            ulong E = e;

            return (uint)((((((((A + B) % uint.MaxValue) + C) % uint.MaxValue) + D) % uint.MaxValue) + E) % uint.MaxValue);
        }

        /// <summary>
        /// Writes the contents of a 2d array to the console
        /// </summary>
        public static void Print2DArray<T>(T[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write(matrix[i, j] + "  ");
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Necessary hash generation functions (setting is 1-6)
        /// </summary>
        public static uint Functions(int setting, uint x, uint y = 0, uint z = 0)
        {
            //12 arr length
            if (consts12.Length < 12)
                throw new ArgumentException($"Length of array arguement 'consts' must be greater than or equal to 12 ({consts12.Length} < 12)");

            switch (setting)
            {
                case 1: //Ch
                    return (x & y) ^ (~x & z); 

                case 2: //Maj
                    return (x & y) ^ (x & z) ^ (y & z);

                case 3: //S0
                    return BitOperations.RotateRight(x, (int)(consts12[1] % 32)) ^ BitOperations.RotateRight(x, (int)(consts12[2] % 32)) ^ BitOperations.RotateRight(x, (int)(consts12[3] % 32));

                case 4: //S1
                    return BitOperations.RotateRight(x, (int)(consts12[4] % 32)) ^ BitOperations.RotateRight(x, (int)(consts12[5] % 32)) ^ BitOperations.RotateRight(x, (int)(consts12[6] % 32));

                case 5: //s0
                    return BitOperations.RotateRight(x, (int)(consts12[7] % 32)) ^ BitOperations.RotateRight(x, (int)(consts12[8] % 32)) ^ (x >> (int)(consts12[9] % 32));

                case 6: //s1
                    return BitOperations.RotateRight(x, (int)(consts12[10] % 32)) ^ BitOperations.RotateRight(x, (int)(consts12[11] % 32)) ^ (x >> (int)(consts12[12] % 32));

                default: //Not a valid setting
                    throw new ArgumentException($"Illegal input setting of '{setting}' (setting range: 1-6)");
            }
        }

        ///<summary>
        ///Setup for hash function, returns a tuple array of Item1: consts, Item2: initialHashVals
        ///</summary>
        public static ulong[] Hash(string input, bool constIHVMode = true)
        {
            //HASH SETUP -- consts, IHV, formatting

            int popCount = 0;
            char[] chars = input.ToCharArray();
            byte[] asciiByteArr = new byte[chars.Length];
            ulong[] initialHashVals = new ulong[8];
            ulong[] consts = new ulong[64];

            //Convert characters to numbers (in bytes)
            for (int i = 0; i < asciiByteArr.Length; i++)
                asciiByteArr[i] = Encoding.Default.GetBytes(chars[i].ToString())[0];

            //Count the number of bits that are on in the input
            foreach (byte b in asciiByteArr)
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
                }

                consts[i - 1] = BitOperations.RotateLeft(SetBit(GetFraction((decimal)(2 * Math.Log(popCount * popCount * Math.Pow(i, 6) + 2)), 32), 1, 1), i);
                consts[i - 1] %= 0xEFFFFFFF;
                consts[i - 1] += 0x10000000;
            }

            //PREPROCESSING -- Blocking

            //Making the blocks, a block is formatted as a 2d list of words (byte, or 8 bits),
            //where the index of the block comes first (0-based)
            //and the index of the byte in the block comes second (0 based)
            //blocks are formatted into a byte for convienience, but they will be turned into
            //an array of ints later on.
            byte[,] blocksAsBytes = new byte[(int)Math.Ceiling((float)asciiByteArr.Length / 62), 64];

            //Add length words to each block
            for (int i = 0; i < blocksAsBytes.GetLength(0); i++)
            {
                blocksAsBytes[i, 62] = (byte)((asciiByteArr.Length & 0xFF00) >>> 8);
                blocksAsBytes[i, 63] = (byte) (asciiByteArr.Length & 0x00FF);
            }

            //Fill blocks
            int count = 0;
            for (int i = 0; i < blocksAsBytes.GetLength(0); i++)
            {
                for (int f = 0; f < blocksAsBytes.GetLength(1) - 2; f++)
                {
                    if (count < asciiByteArr.Length)
                    {
                        blocksAsBytes[i, f] = asciiByteArr[count];
                        count++;
                    }
                }
            }

            //Append 1
            for (int i = 0; i < blocksAsBytes.GetLength(0); i++)
            {
                for (int f = 0; f < blocksAsBytes.GetLength(1) - 2; f++)
                {
                    //Block is full
                    if (f >= 62)
                    {
                        blocksAsBytes[i, 61] <<= 1;
                        blocksAsBytes[i, 61] += 1;
                        break;
                    }

                    //Find first word that is empty
                    else if (blocksAsBytes[i, f] == 0)
                    {
                        blocksAsBytes[i, f] = 1;
                        break;
                    }
                }
            }

            Print2DArray(blocksAsBytes);

            //HASH GENERATION -- Merkle Damgard Construction

            //REMEMBER:
            //Ch: 1
            //Maj: 2
            //S0: 3
            //S1: 4
            //s0: 5
            //s1: 6

            uint[,] blocks = new uint[blocksAsBytes.GetLength(0), 16];

            for (int i = 0; i < blocksAsBytes.GetLength(1); i++)
            {
                for (int f = 0; f < blocksAsBytes.GetLength(1); i += 4)
                {
                    uint temp = 0;

                    temp += blocksAsBytes[i, f];
                    temp <<= 4;

                    temp += blocksAsBytes[i, f + 1];
                    temp <<= 4;

                    temp += blocksAsBytes[i, f + 2];
                    temp <<= 4;

                    temp += blocksAsBytes[i, f + 3];
                    temp <<= 4;

                    blocks[i, f / 4] = temp;
                }
            }

            uint[,] intermediateHVs = new uint[8, blocks.GetLength(0)];

            for (int i = 0; i < initialHashVals.Length; i++)
                intermediateHVs[i, 0] = (uint)initialHashVals[i];


            for (int i = 0; i < blocks.GetLength(0); i++)
            {
                consts12 = consts[0..12].Select(item => (uint)item).ToArray();

                //Prepare message schedule
                uint W(int t)
                {
                    if (0 <= t && t <= 15)
                        return blocks[i, t];

                    else if (16 <= t && t <= 64)
                        return AdditionMod32(
                                        Functions(6, W(t - 2)),
                                        W(t - 7),
                                        Functions(5, W(t - 15)),
                                        W(t - 16));
                    else
                        throw new ArgumentException($"Arguement 't' must be between 0 and 64. (t = {t})");
                }


                //Initialize working variables

                uint a = intermediateHVs[0, i];
                uint b = intermediateHVs[1, i];
                uint c = intermediateHVs[2, i];
                uint d = intermediateHVs[3, i];
                uint e = intermediateHVs[4, i];
                uint f = intermediateHVs[5, i];
                uint g = intermediateHVs[6, i];
                uint h = intermediateHVs[7, i];

                //Calculate
                for (int t = 0; t < 64; t++)
                {
                    uint T_1 = AdditionMod32(h, Functions(4, e), Functions(1, e, f, g), (uint)consts[t], W(t));
                    uint T_2 = AdditionMod32(Functions(5, a), Functions(2, a, b, c));

                    h = g;
                    g = f;
                    f = e;
                    e = d + T_1;
                    d = c;
                    c = b;
                    b = a;
                    a = T_1 + T_2;
                }

                //Compute intermediate hash values
                
            }

            return null;
        }
    }
}

