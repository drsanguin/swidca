using Fr.TPerez.Swidca.Readers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Fr.TPerez.Swidca.Tries
{
    public class ComponentsTrie
    {
        private enum state { STATE_NONFINAL, STATE_FINAL };
        private static int nodeCount = 0;

        private class Node
        {
            public state state;
            public int Id;

            public Node(state state)
            {
                this.state = state;
                this.Id = nodeCount++;
            }

            public bool IsFinal()
            {
                return this.state == state.STATE_FINAL;
            }
        }

        private Node root;
        private IDictionary<Node, IDictionary<char, Node>> trie;
        private IEnumerable<string> words;

        public ComponentsTrie(string path)
        {
            Node root = new Node(state.STATE_NONFINAL);

            this.root = root;
            this.trie = new Dictionary<Node, IDictionary<char, Node>>();
            this.trie.Add(root, new Dictionary<char, Node>());

            Stopwatch sw = new Stopwatch();

            this.words = FileReader.Read(path);

            Console.WriteLine("Building trie...");
            sw.Start();

            foreach (string word in this.words)
            {
                if(word != "")
                {
                    this.add(word, this.root);
                }
            }

            sw.Stop();
            Console.WriteLine("Done ! Took {0} ms.", sw.ElapsedMilliseconds);
        }

        private void add(string word, Node node)
        {
            if(word.Equals(""))
            {
                node.state = state.STATE_FINAL;
            }
            else
            {
                char firstLetter = word[0];

                IDictionary<char, Node> sons = this.trie[node];

                if(sons.ContainsKey(firstLetter))
                {
                    this.add(word.Substring(1), sons[firstLetter]);
                }
                else
                {
                    Node newNode = new Node(state.STATE_NONFINAL);

                    this.trie.Add(newNode, new Dictionary<char, Node>());
                    sons.Add(firstLetter, newNode);

                    this.add(word.Substring(1), newNode);
                }
            }
        }

        public IEnumerable<KeyValuePair<string, IEnumerable<IEnumerable<string>>>> GetComponents()
        {
            IDictionary<string, IEnumerable<IEnumerable<string>>> result = new ConcurrentDictionary<string, IEnumerable<IEnumerable<string>>>();
            
            Stopwatch sw = new Stopwatch();

            Console.WriteLine("Computing components for all dictionary's words...");
            sw.Start();

            Parallel.ForEach<string>(this.words,
                (word) =>
                {
                    IList<IList<string>> components = new List<IList<string>>();

                    getComponents(word, this.root, components, true);

                    result.Add(word, components);
                }
            );

            sw.Stop();
            Console.WriteLine("Done ! Took {0} ms.", sw.ElapsedMilliseconds);

            return result;
        }

        public IEnumerable<IEnumerable<string>> GetComponents(string word)
        {
            Stopwatch sw = new Stopwatch();

            Console.WriteLine("Computing components for \"{0}\"...", word);
            sw.Start();

            IList<IList<string>> result = new List<IList<string>>();

            getComponents(word, this.root, result, true);

            sw.Stop();
            Console.WriteLine("Done ! Took {0} ms.", sw.ElapsedMilliseconds);

            return result;
        }

        private void getComponents(string word, Node currentNode, IList<IList<string>> components, bool initialWord)
        {
            StringBuilder workingCopy = new StringBuilder(word);
            StringBuilder component = new StringBuilder("");

            while (!component.ToString().Equals(word) && this.trie[currentNode].Count != 0)
            {
                char character = workingCopy[0];

                workingCopy.Remove(0, 1);
                component.Append(character);

                if (this.trie[currentNode].ContainsKey(character))
                {
                    currentNode = this.trie[currentNode][character];

                    if (currentNode.IsFinal() && this.trie[currentNode].Count == 0)
                    {
                        string debut = component.ToString();

                        if (!initialWord && workingCopy.Length == 0)
                        {
                            components[0].Add(debut);
                        }
                    }
                    else if (currentNode.IsFinal())
                    {
                        IList<IList<string>> subComponents = new List<IList<string>>();
                        subComponents.Add(new List<string>());

                        string debut = component.ToString();
                        string rest = workingCopy.ToString();

                        getComponents(rest, this.root, subComponents, false);

                        if (subComponents[0].Count > 0)
                        {
                            foreach (List<string> subComponent in subComponents)
                            {
                                subComponent.Insert(0, debut);
                                components.Add(subComponent);
                            }
                        }
                    }
                }
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("digraph trieDico {\n");

            Queue<Node> fifo = new Queue<Node>();
            fifo.Enqueue(this.root);

            while (!(fifo.Count == 0))
            {
                Node node = fifo.Dequeue();

                sb.Append("\"s_" + node.Id + "\"" + ((node.IsFinal()) ? "[color = Blue, shape = doublecircle]" : "") + "\n");
                
                foreach(Node son in this.trie[node].Values)
                {
                    fifo.Enqueue(son);
                }
            }

            foreach(Node father in this.trie.Keys)
            {
                foreach(char key in this.trie[father].Keys)
                {
                    sb.Append("\"s_" + father.Id + "\" -> \"s_" + this.trie[father][key].Id + "\" [label=\"" + key + "\"];\n");
                }
            }

            sb.Append("}\n");

            return sb.ToString();
        }
    }
}
