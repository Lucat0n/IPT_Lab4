using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPT_Lab4_233985
{
    static class Program
    {
        static void Main(string[] args)
        {
            int itercount;
            int k;

            if (args.Length != 5)
                Console.WriteLine("Invalid args count.");
            else if (!File.Exists(args[0]))
                Console.WriteLine("Pattern file path is invalid.");
            else if (!File.Exists(args[1]))
                Console.WriteLine("Text file path is invalid");
            else if (!Int32.TryParse(args[3], out itercount))
                Console.WriteLine("Invalid index.");
            else if (!Int32.TryParse(args[4], out k) || k < 0)
                Console.WriteLine("Invalid k");
            else
            {
                string[] patterns = File.ReadAllLines(args[0]);
                string text = File.ReadAllText(args[1]);
                //TestCountFilter(args, itercount);
                GenerateAllPatterns(args, k);
            }
            Console.WriteLine("Done. Press any key to continue...");
            Console.ReadKey();
        }

        static int ComputeHammingDist(string s1, string s2)
        {
            if (s1.Length != s2.Length)
                throw new ArgumentException("Size of params is not equal.");
            int distance = 0;
            for (int i = 0; i < s1.Length; i++)
            {
                if (s1[i] != s2[i])
                    distance++;
            }
            return distance;
        }

        static void GenerateAllPatterns(string[] args, int k)
        {
            foreach (string pattern in File.ReadAllLines(args[0]))
            {
                var outputs = GeneratePatterns(pattern, k);
                StreamWriter sw = File.AppendText(args[0].Split('.')[0] + "_multi.txt");
                foreach (var el in outputs)
                {
                    sw.WriteLine(el);
                    sw.Flush();
                }
                sw.Close();
            }
        }

        static HashSet<String> GeneratePatterns(String P, int k)
        {
            HashSet<String> outputs = new HashSet<string>();
            char[] letters = P.ToCharArray().Distinct().ToArray();
            outputs.Add(P);
            GetPermutation(P, outputs, letters, 0, k);

            return outputs;
        }

        private static void GetPermutation(String P, HashSet<String> result, char[] letters, int i, int k)
        {
            if(k >= 1)
            {
                for(int j=i; j<P.Length-k+1; j++)
                {
                    for (int l = 0; l < letters.Length; l++)
                    {
                        char[] temp = P.ToCharArray();
                        temp[j] = letters[l];
                        String tempstr = new string(temp);
                        result.Add(tempstr);
                        GetPermutation(tempstr, result, letters, j, k-1);
                    }
                }
            }
        }

        static int CountingFilter(string text, int n, string pattern, int m, int k, bool logging = false)
        {
            int[] A = new int[char.MaxValue];
            for (int i = 0; i < m; i++)
                A[pattern[i]]++;
            int count = -(m - k);
            int j;

            for (j = 0; j < m;)
                if (A[text[j++]]-- > 0)
                    count++;
            while (j < n)
            {
                if (count >= 0)
                {
                    if (ComputeHammingDist(text.AsSpan(j - m, m).ToString(), pattern) <= k && logging)
                        Console.WriteLine("Pattern found at " + (j - m));
                }
                if (++A[text[j - m]] > 0)
                    count--;
                if (A[text[j++]]-- > 0)
                    count++;
            }
            return count;
        }

        static void TestCountFilter(string[] args, int itercount)
        {
            Stopwatch stopwatch = new Stopwatch();
            string[] patterns = File.ReadAllLines(args[0]);
            string text = File.ReadAllText(args[1]);
            FileStream fs = File.OpenWrite(args[2]);
            StreamWriter sw = new StreamWriter(fs);
            long total = 0;
            Console.WriteLine("Testing...");
            foreach (string pattern in patterns)
            {
                Console.WriteLine("Pattern: " + pattern);
                for (int i = 0; i < itercount; i++)
                {
                    stopwatch.Start();
                    CountingFilter(text, text.Length, pattern, pattern.Length, 1);
                    stopwatch.Stop();
                    total += stopwatch.ElapsedMilliseconds;
                    stopwatch.Reset();
                }
                sw.WriteLine("CountingFilter m=" + pattern.Length + ',' + (total / itercount));
                sw.Flush();
                total = 0;
            }
        }

        #region utilities

        private static bool CompareStrings(string str, int index, string pattern)
        {
            for (int i = index; i < index + pattern.Length; i++)
                if (str[i] != pattern[i - index])
                    return false;
            return true;
        }

        #endregion
    }
}

