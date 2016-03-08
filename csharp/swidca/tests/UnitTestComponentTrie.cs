using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.IO;
using Fr.TPerez.Swidca.Tries;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fr.TPerez.Swidca.Tests
{
    [TestClass]
    public class UnitTestComponentTrie
    {
        private static Assembly assembly;

        private static Stream dictionaryStream;
        private static Stream toSplitDictionaryStream;

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            UnitTestComponentTrie.assembly = Assembly.GetExecutingAssembly();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            UnitTestComponentTrie.dictionaryStream = UnitTestComponentTrie.assembly.GetManifestResourceStream("Fr.TPerez.Swidca.Tests.ressources.testament_dictionary.txt");
            UnitTestComponentTrie.toSplitDictionaryStream = UnitTestComponentTrie.assembly.GetManifestResourceStream("Fr.TPerez.Swidca.Tests.ressources.testament_dictionary_to_split.txt");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            UnitTestComponentTrie.dictionaryStream.Dispose();
            UnitTestComponentTrie.toSplitDictionaryStream.Dispose();
        }

        [TestMethod]
        public void GetComponents_Input_Testament_Result_Count_Equals_4()
        {
            ComponentsTrie trie = new ComponentsTrie(UnitTestComponentTrie.dictionaryStream);

            IEnumerable<IEnumerable<string>> components = trie.GetComponents("testament");

            int expected = 4;
            int actual = components.Count<IEnumerable<string>>();

            Assert.AreEqual<int>(expected, actual);
        }

        [TestMethod]
        public void GetComponents_Input_Testament_Result_Contains_All_Values()
        {
            ComponentsTrie trie = new ComponentsTrie(UnitTestComponentTrie.dictionaryStream);

            IEnumerable<IEnumerable<string>> components = trie.GetComponents("testament");

            bool property = true;

            property = property && components.Count<IEnumerable<string>>() == 4;

            bool foundComposition1 = false;
            bool foundComposition2 = false;
            bool foundComposition3 = false;
            bool foundComposition4 = false;

            foreach (IEnumerable<string> component in components)
            {
                if (component.Count<string>() == 2)
                {
                    if (component.Contains<string>("testa") && !foundComposition1)
                    {
                        property = property && component.Contains<string>("ment");

                        foundComposition1 = true;
                    }
                    else if (component.Contains<string>("test") && !foundComposition2)
                    {
                        property = property && component.Contains<string>("ament");

                        foundComposition2 = true;
                    }
                }
                else
                {
                    property = property && component.Count<string>() == 3;

                    if (component.Contains<string>("testa") && !foundComposition3)
                    {
                        property = property && component.Contains<string>("m") && component.Contains<string>("ent");

                        foundComposition3 = true;
                    }
                    else if (component.Contains<string>("test") && !foundComposition4)
                    {
                        property = property && component.Contains<string>("am") && component.Contains<string>("ent");

                        foundComposition4 = true;
                    }
                }
            }

            property = property && foundComposition1 && foundComposition2 && foundComposition3 && foundComposition4; 

            Assert.IsTrue(property);
        }

        [TestMethod]
        public void GetComponents_With_Regex_Input_Testament_Result_Count_Equals_4()
        {
            ComponentsTrie trie = new ComponentsTrie(UnitTestComponentTrie.toSplitDictionaryStream, new string[] { "dico" });

            IEnumerable<IEnumerable<string>> components = trie.GetComponents("testament");

            int expected = 4;
            int actual = components.Count<IEnumerable<string>>();

            Assert.AreEqual<int>(expected, actual);
        }

        [TestMethod]
        public void GetComponents_With_Regex_Input_Testament_Result_Contains_All_Values()
        {
            ComponentsTrie trie = new ComponentsTrie(UnitTestComponentTrie.toSplitDictionaryStream, new string[] { "dico" });

            IEnumerable<IEnumerable<string>> components = trie.GetComponents("testament");

            bool property = true;

            IEnumerable<string> element = components.ElementAt<IEnumerable<string>>(0);

            property = property && element.Contains<string>("test");
            property = property && element.Contains<string>("ament");

            element = components.ElementAt<IEnumerable<string>>(1);

            property = property && element.Contains<string>("test");
            property = property && element.Contains<string>("am");
            property = property && element.Contains<string>("ent");

            element = components.ElementAt<IEnumerable<string>>(2);

            property = property && element.Contains<string>("testa");
            property = property && element.Contains<string>("ment");

            element = components.ElementAt<IEnumerable<string>>(3);

            property = property && element.Contains<string>("testa");
            property = property && element.Contains<string>("m");
            property = property && element.Contains<string>("ent");

            Assert.IsTrue(property);
        }
    }
}
