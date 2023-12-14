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
        private static uint[] consts12;

        ///<summary>
        ///Returns the first n bits in a ulong
        ///</summary>
        public static ulong GetBits(ulong value, int n, bool ignoreLeading0Count = false) =>
            value >>> (64 - (ignoreLeading0Count ? 0 : BitOperations.LeadingZeroCount(value)) - n);

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
        public static ulong GetFraction(decimal n)
        {
            n = Math.Abs(n); //make sure it is positive.
            n -= (int)n;     //remove the integer part of the number.

            ulong result = 0;

            while (n > 0)
            {
                if (GetDecimalPlaces(n) > 19)
                {
                    n *= 10;
                    n %= 1m;

                    continue;
                }

                n *= 10;
                result *= 10;

                result += (ulong)n;
                n %= 1m;
            }
            
            return result;
        }

        /// <summary>
        /// Returns a the value inputted formatted into an array of 0s or 1s in binary
        /// </summary>
        public static int[] GetBitList(BigInteger value)
        {
            int[] bitList = new int[value.GetBitLength()];

            for (int i = 0; i < bitList.Length; i++)
                bitList[i] = (value & ((BigInteger)1ul << (bitList.Length - 1 - i))) > 0 ? 1 : 0;

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
        ///Returns the sum of the inputs modded to the uint max value 
        ///</summary>
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
        /// Necessary hash generation functions (setting is 1-6)
        /// </summary>
        public static uint Functions(int setting, uint x, uint y = 0, uint z = 0)
        {
            //12 arr length
            if (consts12.Length < 12)
                throw new ArgumentException($"Length of array arguement 'consts' must be greater than or equal to 12 ({consts12.Length} < 12)");

            return setting switch
            {
                //Ch
                1 => (x & y) ^ (~x & z),
                //Maj
                2 => (x & y) ^ (x & z) ^ (y & z),
                //S0
                3 => BitOperations.RotateRight(x, (int)(consts12[0] % 32)) ^ BitOperations.RotateRight(x, (int)(consts12[1] % 32)) ^ BitOperations.RotateRight(x, (int)(consts12[2] % 32)),
                //S1
                4 => BitOperations.RotateRight(x, (int)(consts12[3] % 32)) ^ BitOperations.RotateRight(x, (int)(consts12[4] % 32)) ^ BitOperations.RotateRight(x, (int)(consts12[5] % 32)),
                //s0
                5 => BitOperations.RotateRight(x, (int)(consts12[6] % 32)) ^ BitOperations.RotateRight(x, (int)(consts12[7] % 32)) ^ (x >> (int)(consts12[8] % 32)),
                //s1
                6 => BitOperations.RotateRight(x, (int)(consts12[9] % 32)) ^ BitOperations.RotateRight(x, (int)(consts12[10] % 32)) ^ (x >> (int)(consts12[11] % 32)),
                //Not a valid setting
                _ => throw new ArgumentException($"Illegal input setting of '{setting}' (setting range: 1-6)"),
            };
        }

        public static uint BytesToUint(byte b1, byte b2, byte b3, byte b4)
        {
            uint B1 = b1;
            uint B2 = b2;
            uint B3 = b3;
            uint B4 = b4;

            return (B1 << 24) | (B2 << 16) | (B3 << 8) | B4;
        }

        public static void Print2DArray(uint[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write(Convert.ToString(matrix[i, j], 2) + " ");
                }
                Console.WriteLine();
            }
        }

        ///<summary>
        ///Setup for hash function, returns a tuple array of Item1: consts, Item2: initialHashVals
        ///</summary>
        public static BigInteger Hash(string input)
        {
            //HASH SETUP -- consts, IHV, formatting

            int popCount = 0;
            char[] chars = input.ToCharArray();
            byte[] asciiByteArr = new byte[chars.Length];
            uint[] initialHashVals = new uint[8];
            uint[] consts = new uint[64];
            uint identifier = 0xFFFFFFFF;

            //Convert characters to numbers (in bytes)
            for (int i = 0; i < asciiByteArr.Length; i++)
                asciiByteArr[i] = Encoding.UTF8.GetBytes(chars[i].ToString())[0];

            //Count the number of bits that are on in the input
            foreach (byte b in asciiByteArr)
            {
                popCount += BitOperations.PopCount((ulong)b);

                identifier ^= b;
                identifier = (BitOperations.RotateLeft(identifier, 8)) ^ b;
                identifier = (BitOperations.RotateLeft(identifier, 8)) ^ b;
                identifier = (BitOperations.RotateLeft(identifier, 8)) ^ b;
                identifier = BitOperations.RotateRight(identifier, b);
            }

            //Set the 8 initial hash values and consts (see background research plan) (setting consts down here because
            //there are values generated right before this that are necessary to const generation)
            for (int i = 1; i < 65; i++)
            {
                ulong temp;

                if (i - 1 < 8)
                {
                    temp = GetFraction((decimal)Math.Abs(Math.Log(((double)popCount * popCount * identifier * (i * i * i) / (popCount << (i % 8))) + 2)));
                    temp %= 0x7FFFFFFF;
                    temp += 0x80000000;

                    initialHashVals[i - 1] = (uint)temp ^ (i > 1 ? initialHashVals[i - 2] : 0) ^ identifier;
                }

                temp = GetFraction((decimal)Math.Abs(Math.Log(((double)popCount * popCount * identifier * i * i / (popCount << (i % 8))) + 2)));
                temp %= 0x7FFFFFFF;
                temp += 0x80000000;

                consts[i - 1] = (uint)temp;

                if (i > 1)
                    consts[i - 1] ^= consts[(popCount & identifier) % (i - 1)] ^ (i > 1 ? consts[i - 2] : 0) ^ identifier;

                identifier ^= consts[i - 1] ^ initialHashVals[Math.Min(i - 1, 7)];
            }

            //PREPROCESSING -- Blocking

            //Making the blocks, a block is formatted as a 2d list of words (byte, or 8 bits),
            //where the index of the block comes first (0-based)
            //and the index of the byte in the block comes second (0 based)
            //blocks are formatted into a byte for convienience, but they will be turned into
            //an array of ints later on.
            byte[,] blocksAsBytes = new byte[(int)Math.Ceiling((float)asciiByteArr.Length / 62), 64];

            //Blocks as a uint array (same indexing rules as before)
            uint[,] blocks = new uint[blocksAsBytes.GetLength(0), 16];

            //HASH GENERATION -- Merkle Damgard Construction

            //REMEMBER:
            //Ch: 1
            //Maj: 2
            //S0: 3
            //S1: 4
            //s0: 5
            //s1: 6

            uint W(int t, int i)
            {
                if (0 <= t && t <= 15)
                    return blocks[i, t];

                else if (16 <= t && t <= 64)
                    return AdditionMod32(Functions(6, W(t - 2, i)),
                                         W(t - 7, i),
                                         Functions(5, W(t - 15, i)),
                                         W(t - 16, i));
                else
                    throw new ArgumentException($"Arguement 't' must be between 0 and 64. (t = {t})");
            }

            consts12 = consts[0..12];

            uint[,] intermediateHVs = new uint[8, blocks.GetLength(0) + 1];

            for (int i = 0; i < initialHashVals.Length; i++)
                intermediateHVs[i, 0] = initialHashVals[i];

            for (int i = 0, count = 0; i < blocksAsBytes.GetLength(0); i++)
            {
                bool found = false;

                //Add length words to each block
                blocksAsBytes[i, 62] = (byte)((asciiByteArr.Length & 0xFF00) >>> 8);
                blocksAsBytes[i, 63] = (byte)(asciiByteArr.Length & 0x00FF);

                for (int t = 0; t < 64; t++)
                {
                    //Fill:
                    if (count < asciiByteArr.Length)
                    {
                        blocksAsBytes[i, t] = asciiByteArr[count];
                        count++;
                    }

                    //Append 1: 
                    //Block is full
                    if (t >= 62 && !found)
                    {
                        blocksAsBytes[i, 61] <<= 1;
                        blocksAsBytes[i, 61] += 1;
                        found = true;
                    }

                    //Find first word that is emptye
                    else if (blocksAsBytes[i, t] == 0 && !found)
                    {
                        blocksAsBytes[i, t] = 1;
                        found = true;
                    }
                }

                for (int t = 0; t < 16; t++)
                {
                    //Fill blocks (as uint)
                    blocks[i, t] = BytesToUint(blocksAsBytes[i, (t * 4) + 0],
                                               blocksAsBytes[i, (t * 4) + 1],
                                               blocksAsBytes[i, (t * 4) + 2],
                                               blocksAsBytes[i, (t * 4) + 3]);
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
                    uint T_1 = AdditionMod32(h, Functions(4, e), Functions(1, e, f, g), consts[t], W(t, i));
                    uint T_2 = AdditionMod32(Functions(5, a), Functions(2, a, b, c));

                    h = g;
                    g = f;
                    f = e;
                    e = AdditionMod32(d, T_1);
                    d = c;
                    c = b;
                    b = a;
                    a = AdditionMod32(T_1, T_2);
                }

                //Compute intermediate hash values
                for (int j = 0; j < 8; j++)
                    intermediateHVs[j, i + 1] = AdditionMod32(a, intermediateHVs[j, i]);
            }


            Console.WriteLine();
            Console.WriteLine();

            foreach (uint c in consts)
            {
                Console.WriteLine(Convert.ToString(c, 2));
            }

            Print2DArray(intermediateHVs);
            Console.WriteLine();

            BigInteger output = 0;

            for (int i = 0; i < 8; i++)
            {
                Console.WriteLine(Convert.ToString(intermediateHVs[i, intermediateHVs.GetLength(1) - 1], 2));
                output <<= 32;
                output += intermediateHVs[i, intermediateHVs.GetLength(1) - 1];
            }

            //0 followed by 255 1's in binary
            //output %= BigInteger.Parse("57896044618658097711785492504343953926634992332820282019728792003956564819967");

            //1 followed by 255 0's in binary
            //output += BigInteger.Parse("57896044618658097711785492504343953926634992332820282019728792003956564819968");

            Console.WriteLine(string.Join("", GetBitList(output)));

            return output;
        }
    }
}
