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
        private static string parsingRegex;
        private static string outputFile;
        private static bool verbose = false;
        private static bool allWords = false;

        private static bool parsingRegexHasBeenSetUp = false;
        private static bool outputFileHasBeenSetUp = false;

        static void Main(string[] args)
        {
            Program.parseArgs(args);

            ComponentsTrie trie;

            if(Program.parsingRegexHasBeenSetUp)
            {
                trie = new ComponentsTrie(new FileStream(args[args.Length - 1], FileMode.Open), new string[] { Program.parsingRegex });
            }
            else
            {
                trie = new ComponentsTrie(new FileStream(args[args.Length - 1], FileMode.Open));
            }

            string printableResult;

            if(Program.allWords)
            {
                IEnumerable<KeyValuePair<string, IEnumerable<IEnumerable<string>>>> components = trie.GetComponents();

                printableResult = Program.getPrintableResults(components);
            }
            else
            {
                IEnumerable<IEnumerable<string>> components = trie.GetComponents(args[args.Length - 2]);

                printableResult = Program.getPrintableResult(components, args[args.Length - 2]);
            }

            if(Program.outputFileHasBeenSetUp)
            {
                File.WriteAllText(Program.outputFile, printableResult);
            }
            else
            {
                System.Console.WriteLine(printableResult);
            }

            System.Console.WriteLine("Press any key to exit:");
            System.Console.ReadKey();
        }

        private static void parseArgs(string[] args)
        {
            if (args.Length == 0)
            {
                Program.exit();
            }

            for (int i = 0; i < args.Length; i++)
            {
                switch(args[i])
                {
                    case "-re":
                        if (Program.parsingRegexHasBeenSetUp)
                        {
                            Program.exit("\"-re\" has already been set up.");
                        }

                        Program.parsingRegex = args[i + 1];
                        Program.parsingRegexHasBeenSetUp = true;

                        i++;

                        break;

                    case "-v":
                        if (Program.verbose)
                        {
                            Program.exit("\"-v\" has already been set up.");
                        }

                        bool argEquals0 = args[i + 1].Equals("0");
                        bool argEquals1 = args[i + 1].Equals("1");

                        if (! argEquals0 || argEquals1)
                        {
                            Program.exit("Bad argument for \"-v\". Must be \"0\" or \"1\".");
                        }
                        else
                        {
                            Program.verbose = (argEquals0) ? false : true;
                        }

                        i++;

                        break;

                    case "-o":
                        if (Program.outputFileHasBeenSetUp)
                        {
                            Program.exit("\"-o\" has already been set up.");
                        }

                        try
                        {
                            File.Create(args[i + 1]);

                        }
                        catch (Exception)
                        {
                            Program.exit("Failed to create output file \"" + args[i + 1] + "\".");
                        }

                        Program.outputFileHasBeenSetUp = true;
                        Program.outputFile = args[i + 1];

                        i++;

                        break;

                    case "-a":
                        if (Program.allWords)
                        {
                            Program.exit("\"-a\" has already been set up.");
                        }

                        Program.allWords = true;

                        break;
                }
            }
        }

        private static void exit()
        {
            Console.WriteLine("Usage: swidca [OPTIONS...] WORD FILE");
            Environment.Exit(-1);
        }

        private static void exit(string message)
        {
            Console.WriteLine("{0}\nUsage: swidca [OPTIONS...] WORD FILE", message);
            Environment.Exit(-1);
        }

        private static string getPrintableResults(IEnumerable<KeyValuePair<string, IEnumerable<IEnumerable<string>>>> components)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var kv in components)
            {
                sb.AppendLine(Program.getPrintableResult(kv.Value, kv.Key));
            }

            return sb.ToString();
        }

        private static string getPrintableResult(IEnumerable<IEnumerable<string>> components, string word)
        {
            StringBuilder sb = new StringBuilder();

            if (components.Count() == 0)
            {
                sb.AppendFormat("No components found for word \"{0}\".", word);
                sb.AppendLine();
            }
            else
            {
                sb.AppendFormat("Found {0} components for word \"{1}\":", components.Count(), word);
                sb.AppendLine();

                foreach (IEnumerable<string> component in components)
                {
                    sb.AppendFormat("\"{0}\" = ", word);

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
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }
    }
}
