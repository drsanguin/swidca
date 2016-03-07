using Fr.TPerez.Swidca.Tries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace core
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: swidca WORD FILE");
            }
            else
            {
                FileStream fs = new FileStream(args[args.Length - 1], FileMode.Open);

                ComponentsTrie trie = new ComponentsTrie(fs);

                IEnumerable<IEnumerable<string>> components = trie.GetComponents(args[0]);

                if (components.Count() == 0)
                {
                    Console.WriteLine("No components found for word \"" + args[0] + "\".");
                }
                else
                {
                    Console.WriteLine("Found " + components.Count() + " components for word \"" + args[0] + "\":");

                    foreach (IEnumerable<string> component in components)
                    {
                        StringBuilder sb = new StringBuilder("\"" + args[0] + "\" =");

                        string lastOne = Enumerable.Last<string>(component);

                        foreach (var pieceOfComponent in component)
                        {
                            if (pieceOfComponent.Equals(lastOne))
                            {
                                sb.AppendFormat("\"{0}\"", pieceOfComponent);
                            }
                            else
                            {
                                sb.AppendFormat("\"{0}\" + ", pieceOfComponent);
                            }
                        }

                        Console.WriteLine(sb.ToString());
                    }
                }
            }

            System.Console.WriteLine("Press any key to exit:");
            System.Console.ReadKey();
        }
    }
}
