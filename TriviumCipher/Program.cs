using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace TriviumCipher
{
    class Program
    {
        
        static Random rnd = new Random();
        static void Main(string[] args)
        {
            byte[] key = new byte[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            byte[] iv = new byte[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            //Generate random IV
            for(int i =0;i < iv.Length; i++)
            {
                iv[i] = (byte)rnd.Next(255);
            }
            
            Console.Write("Input key (max 10 characters): ");
            string skey = Console.ReadLine();

            Console.Write("Input text: ");
            string input = Console.ReadLine();

            byte[] keyb = Encoding.ASCII.GetBytes(skey);

            for(int i =0;i < key.Length;i++)
            {
                if (i >= keyb.Length) break;
                key[i] = keyb[i];
            }

            Console.WriteLine("\n[!] KEY: {0,15}", printBytes(key));
            Console.WriteLine("[!] IV:  {0,15}", printBytes(iv));


            Console.WriteLine("\n======================Encryption started==========================");

            Trivium triv = new Trivium(key, iv);

            triv.Init();

            byte[] encdata = triv.Crypt(input);

            Console.WriteLine("\n======================Decryption started==========================");

            Trivium decrypt = new Trivium(key, iv);

            decrypt.Init();

            string result = decrypt.Crypt(encdata);

            Console.WriteLine("[!] Decryption result: {0}", result);

            Console.ReadKey();
        }

        static string printBytes(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var b in bytes)
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
