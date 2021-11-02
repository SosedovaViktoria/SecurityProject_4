using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Task_4_Sosedova
{
    class Program
    {

        static Dictionary<byte, byte> GetEncoder(Encoding code)
        {
            Dictionary<byte, byte> res = new Dictionary<byte, byte>();

            res.Add(code.GetBytes(new char[] { 'а' })[0], code.GetBytes(new char[] { 'a' })[0]);
            res.Add(code.GetBytes(new char[] { 'А' })[0], code.GetBytes(new char[] { 'A' })[0]);
            res.Add(code.GetBytes(new char[] { 'е' })[0], code.GetBytes(new char[] { 'e' })[0]);
            res.Add(code.GetBytes(new char[] { 'Е' })[0], code.GetBytes(new char[] { 'E' })[0]);
            res.Add(code.GetBytes(new char[] { 'о' })[0], code.GetBytes(new char[] { 'o' })[0]);
            res.Add(code.GetBytes(new char[] { 'О' })[0], code.GetBytes(new char[] { 'O' })[0]);
            res.Add(code.GetBytes(new char[] { 'с' })[0], code.GetBytes(new char[] { 'c' })[0]);
            res.Add(code.GetBytes(new char[] { 'С' })[0], code.GetBytes(new char[] { 'C' })[0]);
            res.Add(code.GetBytes(new char[] { 'р' })[0], code.GetBytes(new char[] { 'p' })[0]);
            res.Add(code.GetBytes(new char[] { 'Р' })[0], code.GetBytes(new char[] { 'P' })[0]);
            res.Add(code.GetBytes(new char[] { 'х' })[0], code.GetBytes(new char[] { 'x' })[0]);
            res.Add(code.GetBytes(new char[] { 'Х' })[0], code.GetBytes(new char[] { 'X' })[0]);
            res.Add(code.GetBytes(new char[] { 'К' })[0], code.GetBytes(new char[] { 'K' })[0]);
            res.Add(code.GetBytes(new char[] { 'М' })[0], code.GetBytes(new char[] { 'M' })[0]);
            res.Add(code.GetBytes(new char[] { 'Н' })[0], code.GetBytes(new char[] { 'H' })[0]);
            res.Add(code.GetBytes(new char[] { 'у' })[0], code.GetBytes(new char[] { 'y' })[0]);
            return res;
        }
        static void Main(string[] args)
        {
            var code = Encoding.GetEncoding(1251);
            
            var encoder = GetEncoder(code);

            Console.WriteLine("Выберите опцию:");
            Console.WriteLine("1. Шифрование");
            Console.WriteLine("2. Дешифрование");

            int opt = int.Parse(Console.ReadLine());
            
            if (opt == 1)
            {
                string inputText = "";
                string dataText = "";

                using (StreamReader sr = new StreamReader("input.txt", code))
                {
                    inputText = sr.ReadToEnd();
                }

                using (StreamReader sr = new StreamReader("data.txt", code))
                {
                    dataText = sr.ReadToEnd();
                }

                using (StreamWriter sw = new StreamWriter("data.txt", false, code))
                {
                    int iter = 0;
                    List<byte> Lst = new List<byte>();
                    foreach (char ch in inputText)
                    {
                        byte[] bytes = code.GetBytes(new char[] { ch }).Take(1).ToArray();

                        BitArray bits = new BitArray(bytes);
                        bool[] Msd = new bool[8];
                        bits.CopyTo(Msd, 0);
                        foreach (bool item in Msd.Reverse())
                        {
                            bool charFound = false;

                            while (!charFound)
                            {
                                byte checker = code.GetBytes(new char[] { dataText[iter] })[0];

                                if (encoder.ContainsKey(checker))
                                {
                                    charFound = true;
                                    Lst.Add(checker);
                                    if (item)
                                    {
                                        
                                        dataText = dataText.Remove(iter, 1).Insert(iter, code.GetString(new byte[] { encoder[checker] }));
                                    }
                                }

                                iter++;
                            }
                        }
                    }

                    sw.Write(dataText);
                }
            }
            else if (opt == 2)
            {
                using (StreamReader sr = new StreamReader("data.txt", code))
                {
                    using (StreamWriter sw = new StreamWriter("output.txt", false, code))
                    {
                        bool shouldContinue = true;
                        string dataText = sr.ReadToEnd();
                        int iter = 0;
                        List<byte> Lst = new List<byte>();
                        do
                        {
                            BitArray bits = new BitArray(8, false);

                            for (int i = 0; i < bits.Length; i++)
                            {
                                bool charFound = false;

                                while (!charFound)
                                {
                                    byte checker = code.GetBytes(new char[] { dataText[iter] })[0];

                                    if (encoder.ContainsKey(checker) || encoder.ContainsValue(checker))
                                    {
                                        charFound = true;
                                        Lst.Add(checker);
                                        if (encoder.ContainsValue(checker))
                                        {
                                            bits[i] = true;
                                        }
                                    }

                                    iter++;
                                }
                            }
                            bool[] bts = new bool[8];
                            bits.CopyTo(bts, 0);
                            BitArray bitArray = new BitArray(bts.Reverse().ToArray());
                            
                            byte[] bytes = new byte[1];
                            bitArray.CopyTo(bytes, 0);
                            
                            if (bytes[0] == byte.MinValue)
                            {
                                shouldContinue = false;
                            }
                            else
                            {
                                var her = code.GetString(bytes);
                                sw.Write(her);
                                
                            }
                        }
                        while (shouldContinue);
                    }
                }
            }
            Console.ReadLine();
        }
    }
}
