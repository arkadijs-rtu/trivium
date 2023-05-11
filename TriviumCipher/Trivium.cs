using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriviumCipher
{
    public class Trivium
    {
        //internal state 288-bit (s1,...,s288)
        private byte[] s = new byte[36];
        //80-bit key
        private byte[] key = new byte[10];
        //80-bit iv
        private byte[] iv = new byte[10];

        public Trivium(byte[] key, byte[] iv)
        {
            this.key = key;
            this.iv  = iv;
        }


        public void Init()
        {
            if (key == null || iv == null)
                throw new Exception("Trivium error, key and iv cannot be empty.");
            if (key.Length != 10 && iv.Length != 10)
                throw new Exception("Trivium error, key and iv should be 80-bit length.");

            //set internal state to 0s
            emptyState();
            //setup KEY
            for(int i =0;i < 10;i++)
            {
                s[i] = reverseInt8(key[9 - i]);
            }
            //setup IV
            for(int i=0;i < 10;i++)
            {
                s[12 + i] = reverseInt8(iv[9 - i]);
            }
            //Move IV starting from bit 94
            for(int i = 11;i < 22;i++)
            {
                s[i] = (byte)(s[i + 1] << 5 | s[i] >> 3);
            }

            SetBit(286, 1);
            SetBit(287, 1);
            SetBit(288, 1);

            for (int i = 0; i < (4 * 288); i++)
            {
                int t1 = GetBit(66);
                t1 ^= (GetBit(91) & GetBit(92));
                t1 ^= GetBit(93) ^ GetBit(171);

                int t2 = GetBit(162);
                t2 ^= (GetBit(175) & GetBit(176));
                t2 ^= GetBit(177) ^ GetBit(264);

                int t3 = GetBit(243);
                t3 ^= (GetBit(286) & GetBit(287));
                t3 ^= GetBit(288) ^ GetBit(69);

                shiftState();

                SetBit(1, t3);
                SetBit(94, t1);
                SetBit(178, t2);
            }
        }

        /*
         * Shifting state to the right by 1 bit
         * */
        protected void shiftState()
        {
            byte res = 0;
            for (int i = 0; i < 36; i++)
            {
                //get lowest bit
                byte tmp = (byte)(s[i] >> 7);
                //shift current byte bits to the right
                s[i] = (byte)(s[i] << 1);
                //add bit to the current byte if the previous bit result is 1
                s[i] |= res;
                //hold previous result
                res = tmp;
            }
        }

        /*
         * Generate bit stream
         * */
        protected int generateBit()
        {
            int t1 = GetBit(66);
            t1 ^= GetBit(93);

            int t2 = GetBit(162);
            t2 ^= GetBit(177);

            int t3 = GetBit(243);
            t3 ^= GetBit(288);

            int z = t1 ^ t2 ^ t3;

            t1 ^= GetBit(91) & GetBit(92);
            t1 ^= GetBit(171);

            t2 ^= GetBit(175) & GetBit(176);
            t2 ^= GetBit(264);

            t3 ^= GetBit(286) & GetBit(287);
            t3 ^= GetBit(69);


            shiftState();

            SetBit(1, t3);
            SetBit(94, t1);
            SetBit(178, t2);

            return z;
        }

        protected byte reverseInt8(byte value)
        {
            value = (byte)( ((value & 0xF0) >> 4) | ((value & 0x0F) << 4));
            value = (byte)( ((value & 0xCC) >> 2) | ((value & 0x33) << 2));
            value = (byte)( ((value & 0xAA) >> 1) | ((value & 0x55) << 1));

            return value;
        }

        protected byte generateByte()
        {
            byte b = 0;

            for(int i =0;i < 8;i++)
            {
                int getBit = generateBit();
                b |= (byte)(getBit << i);
            }

            return b;
        }

        public byte[] Crypt(string input)
        {
            StringBuilder output    = new StringBuilder();
            StringBuilder keystream = new StringBuilder();

            for(int i =0;i < input.Length;i++)
            {
                byte b = generateByte();
                output.Append((input[i] ^ b).ToString("X2"));
                keystream.Append(b.ToString("X2"));
            }

            Console.WriteLine("\nKey stream:  {0,10}", keystream.ToString());
            Console.WriteLine("Cipher text: {0,10}\n", output.ToString());

            return StringToByteArray(output.ToString());
        }

        public string Crypt(byte[] input)
        {
            StringBuilder output    = new StringBuilder();
            StringBuilder keystream = new StringBuilder();
            StringBuilder outputhex = new StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                byte b = generateByte();
                output.Append((char)(input[i] ^ b));
                outputhex.Append((input[i] ^ b).ToString("X2"));
                keystream.Append(b.ToString("X2"));
            }

            Console.WriteLine("\nKey stream:  {0,10}", keystream.ToString());
            Console.WriteLine("Cipher text: {0,10}\n", outputhex.ToString());

            return output.ToString();
        }

        protected byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        protected int GetBit(int n)
        {
            return (s[(n - 1) / 8] >> ((n - 1) % 8) & 1);
        }

        protected void SetBit(int n, int val)
        {
            s[(n - 1) / 8] = (byte)((s[(n - 1)/8] & (byte)~(1<<((n - 1) % 8))) | (val << ((n -1) % 8)));
        }

        protected string PrintByte(byte b)
        {
            StringBuilder sb = new StringBuilder();

            for(int i =0;i < 8;i++)
            {
                sb.Append(b >> i & 1);
            }

            return sb.ToString();
        }

        public void PrintState()
        {
            for(int i = 0; i < 36; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    int curBit = (s[i] >> j) & 1;
                    Console.Write(curBit);
                }

                Console.Write(" = {0}-{1} bits\n", (i * 8) + 1, (i * 8) + 8);
            }
        }

        protected void emptyState()
        {
            for (int i = 0; i < 36; i++) s[i] = 0;
        }
    }
}
