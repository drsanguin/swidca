using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Fr.TPerez.Swidca.Readers
{
    public class FileReader
    {
        private FileReader() { }

        public static IEnumerable<string> Read(string path)
        {
            Stopwatch sw = new Stopwatch();

            Console.WriteLine("Parsing dictionary file...");
            sw.Start();

            IEnumerable<string> result;

            try
            {
                result = File.ReadAllLines(path).ToList<string>();
            }
            catch (Exception)
            {
                result = Enumerable.Empty<string>();
            }
            finally
            {
                sw.Stop();
                Console.WriteLine("Done ! Took {0} ms.", sw.ElapsedMilliseconds);
            }

            return result;
        }
    }
}
