using Fr.TPerez.Swidca.Readers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Fr.TPerez.Swidca.Tests
{
    [TestClass]
    public class UnitTestDictionaryReader
    {
        private static Assembly assembly;

        private static Stream dictionaryStream;
        private static Stream toSplitDictionaryStream;

        private static string testament_dictionary = "Fr.TPerez.Swidca.Tests.ressources.testament_dictionary.txt";
        private static string testament_dictionary_to_split = "Fr.TPerez.Swidca.Tests.ressources.testament_dictionary_to_split.txt";

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            UnitTestDictionaryReader.assembly = Assembly.GetExecutingAssembly();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            UnitTestDictionaryReader.dictionaryStream = UnitTestDictionaryReader.assembly.GetManifestResourceStream(UnitTestDictionaryReader.testament_dictionary);
            UnitTestDictionaryReader.toSplitDictionaryStream = UnitTestDictionaryReader.assembly.GetManifestResourceStream(UnitTestDictionaryReader.testament_dictionary_to_split);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            UnitTestDictionaryReader.dictionaryStream.Dispose();
            UnitTestDictionaryReader.toSplitDictionaryStream.Dispose();
        }

        [TestMethod]
        public void Count_On_Read_testament_dictionary_Equals_9()
        {
            IEnumerable<string> text = DictionaryReader.ReadLineByLine(dictionaryStream);

            int expected = 9;
            int actual = text.Count<string>();

            Assert.AreEqual<int>(expected, actual);
        }

        [TestMethod]
        public void Read_testament_dictionary_Contains_All_Words()
        {
            IEnumerable<string> text = DictionaryReader.ReadLineByLine(dictionaryStream);

            bool property = true;

            property = property && text.Contains<string>("te");
            property = property && text.Contains<string>("test");
            property = property && text.Contains<string>("testa");
            property = property && text.Contains<string>("testament");
            property = property && text.Contains<string>("m");
            property = property && text.Contains<string>("ment");
            property = property && text.Contains<string>("am");
            property = property && text.Contains<string>("ament");
            property = property && text.Contains<string>("ent");

            Assert.IsTrue(property);
        }

        [TestMethod]
        public void Count_On_Read_testament_dictionary_to_split_Equals_9()
        {
            IEnumerable<string> text = DictionaryReader.ReadBySpliting(toSplitDictionaryStream, new string[] { "dico" });

            int expected = 9;
            int actual = text.Count<string>();

            Assert.AreEqual<int>(expected, actual);
        }

        [TestMethod]
        public void Read_testament_dictionary_to_split_Contains_All_Words()
        {
            IEnumerable<string> text = DictionaryReader.ReadBySpliting(toSplitDictionaryStream, new string[] { "dico" });

            bool property = true;

            property = property && text.Contains<string>("te");
            property = property && text.Contains<string>("test");
            property = property && text.Contains<string>("testa");
            property = property && text.Contains<string>("testament");
            property = property && text.Contains<string>("m");
            property = property && text.Contains<string>("ment");
            property = property && text.Contains<string>("am");
            property = property && text.Contains<string>("ament");
            property = property && text.Contains<string>("ent");

            Assert.IsTrue(property);
        }
    }
}
