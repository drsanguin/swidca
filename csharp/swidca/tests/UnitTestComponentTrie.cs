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

        private static string testament_dictionary = "Fr.TPerez.Swidca.Tests.ressources.testament_dictionary.txt";
        private static string testament_dictionary_to_split = "Fr.TPerez.Swidca.Tests.ressources.testament_dictionary_to_split.txt";

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            UnitTestComponentTrie.assembly = Assembly.GetExecutingAssembly();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            UnitTestComponentTrie.dictionaryStream = UnitTestComponentTrie.assembly.GetManifestResourceStream(UnitTestComponentTrie.testament_dictionary);
            UnitTestComponentTrie.toSplitDictionaryStream = UnitTestComponentTrie.assembly.GetManifestResourceStream(UnitTestComponentTrie.testament_dictionary_to_split);
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
