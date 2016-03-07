using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Fr.TPerez.Swidca.Readers
{
    public class DictionaryReader
    {
        private DictionaryReader() {}

        public static IEnumerable<string> ReadLineByLine(Stream stream)
        {
            return DictionaryReader.ReadBySpliting(stream, new string[] { Environment.NewLine });
        }

        public static IEnumerable<string> ReadBySpliting(Stream stream, string[] separators)
        {
            Stopwatch sw = new Stopwatch();

            Console.WriteLine("Reading dictionary file...");
            sw.Start();

            IEnumerable<string> result = null;
            StreamReader read = null;

            try
            {
                read = new StreamReader(stream);

                string text = read.ReadToEnd();

                result = text.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            }
            catch(Exception e)
            {
                throw e;
            }
            finally
            {
                if(read != null)
                {
                    read.Dispose();
                }
                
                sw.Stop();
                Console.WriteLine("Done ! Took {0} ms.", sw.ElapsedMilliseconds);
            }

            return result;
        }
    }
}
