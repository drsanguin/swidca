using Fr.TPerez.Swidca.Readers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Fr.TPerez.Swidca.Tries
{
    /// <summary>
    /// A Trie where each node map to another with a letter.
    /// If the path from the root to a node is a word, the node is flaged a Final.
    /// So each first level final node will map a 1 letter world, each 2nd level map a 2 words letter and so on.
    /// </summary>
    public class ComponentsTrie
    {
        private enum state { STATE_NONFINAL, STATE_FINAL };
        private static int nodeCount = 0;

        /// <summary>
        /// The Node class for ComponentTrie's node modeling.
        /// </summary>
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

        public ComponentsTrie(Stream stream) : this(stream, new string[] { Environment.NewLine }) { }

        public ComponentsTrie(Stream stream, string[] separators)
        {
            Node root = new Node(state.STATE_NONFINAL);

            this.root = root;
            this.trie = new Dictionary<Node, IDictionary<char, Node>>();
            this.trie.Add(root, new Dictionary<char, Node>());

            Stopwatch sw = new Stopwatch();

            this.words = DictionaryReader.ReadBySpliting(stream, separators);

            Console.WriteLine("Building trie...");
            sw.Start();

            foreach (string word in this.words)
            {
                if(word != "")
                {
                    this.add(new StringBuilder(word), this.root);
                }
            }

            sw.Stop();
            Console.WriteLine("Done ! Took {0} ms.", sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// Tail-recursive function who add a word to the Trie. Does nothing if the Trie already contains the word.
        /// </summary>
        /// <param name="word">The word to add to the Trie.</param>
        /// <param name="node">The current node during the traversal.</param>
        private void add(StringBuilder word, Node node)
        {
            if(word.Length == 0)
            {
                node.state = state.STATE_FINAL;
            }
            else
            {
                char firstLetter = word[0];

                IDictionary<char, Node> sons = this.trie[node];

                if(sons.ContainsKey(firstLetter))
                {
                    this.add(word.Remove(0, 1), sons[firstLetter]);
                }
                else
                {
                    Node newNode = new Node(state.STATE_NONFINAL);

                    this.trie.Add(newNode, new Dictionary<char, Node>());
                    sons.Add(firstLetter, newNode);

                    this.add(word.Remove(0, 1), newNode);
                }
            }
        }

        /// <summary>
        /// Search all the possible decompositions of all the words in the dictionary.
        /// </summary>
        /// <returns>A IEnumerable where each word is key who is associated to his decompositions.</returns>
        public IEnumerable<KeyValuePair<string, IEnumerable<IEnumerable<string>>>> GetComponents()
        {
            IDictionary<string, IEnumerable<IEnumerable<string>>> result = new ConcurrentDictionary<string, IEnumerable<IEnumerable<string>>>();
            
            Stopwatch sw = new Stopwatch();

            Console.WriteLine("Searching components for all dictionary's words...");
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

        /// <summary>
        /// Search every decompositions for the specified word that are possible using only the words of the dictionary.
        /// </summary>
        /// <param name="word">The word that you wan to to decompose.</param>
        /// <returns>The word's decompositions.</returns>
        public IEnumerable<IEnumerable<string>> GetComponents(string word)
        {
            Stopwatch sw = new Stopwatch();

            Console.WriteLine("Searching components for \"{0}\"...", word);
            sw.Start();

            IList<IList<string>> result = new List<IList<string>>();

            getComponents(word, this.root, result, true);

            sw.Stop();
            Console.WriteLine("Done ! Took {0} ms.", sw.ElapsedMilliseconds);

            return result;
        }

        /// <summary>
        /// Auxiliary function of the GetComponents function.
        /// </summary>
        /// <param name="word">The word for wich we want to find components.</param>
        /// <param name="currentNode">The current node during the traversal.</param>
        /// <param name="components">The accumulator where the result is stored.</param>
        /// <param name="currentWordIsInitialWord">A flag used to detect if the parameter word is equal to word. Used to avoid adding the word to the result.</param>
        private void getComponents(string word, Node currentNode, IList<IList<string>> components, bool currentWordIsInitialWord)
        {
            /// The copy of the word that will be modified during the loop
            StringBuilder workingCopy = new StringBuilder(word);
            /// The current component of the word that will be construct during the loop
            StringBuilder component = new StringBuilder();

            /// Loop until the word is totaly consume or we've reach a root.
            while (!component.ToString().Equals(word) && this.trie[currentNode].Count != 0)
            {
                /// Getting the current char we search in the current node's sons
                char character = workingCopy[0];

                /// Updating copy and components with the current char
                workingCopy.Remove(0, 1);
                component.Append(character);

                /// We can continue our traversal
                if (this.trie[currentNode].ContainsKey(character))
                {
                    currentNode = this.trie[currentNode][character];

                    /// We've reach a leaf
                    if (currentNode.IsFinal() && this.trie[currentNode].Count == 0)
                    {
                        string debut = component.ToString();

                        if (!currentWordIsInitialWord && workingCopy.Length == 0)
                        {
                            components[0].Add(debut);
                        }
                    }
                    /// We've reach a node who's traversal from the root from him is a word of the dictionary.
                    else if (currentNode.IsFinal())
                    {
                        IList<IList<string>> subComponents = new List<IList<string>>();
                        subComponents.Add(new List<string>());

                        /// A first component of the word
                        string debut = component.ToString();
                        /// The rest of the word for wich we're gonna try to find components
                        string rest = workingCopy.ToString();

                        getComponents(rest, this.root, subComponents, false);

                        /// If we've found components for the rest of the word, we add them to the result.
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

        /// <summary>
        /// Convert the ComponentTrie to it's String representation. The String is formated in order to be interpreted by graphviz.
        /// </summary>
        /// <returns>A String that represents the ComponentTrie</returns>
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
