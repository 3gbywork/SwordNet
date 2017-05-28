using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UnitTest.ContentPanelTest
{
    [TestClass]
    public class CollectionTest
    {
        [TestMethod]
        [TestProperty("author", "apli")]
        public void SortedListTest()
        {
            SortedList<string, string> sortedList = new SortedList<string, string>();
            sortedList.Add("123", "34");
            sortedList.Add("abc", "34");
            sortedList.Add("ac", "34");
            sortedList.Add("997", "34");
            sortedList.Add("453", "34");
        }

        [TestMethod]
        [TestProperty("author", "apli")]
        public void SortedSetTest()
        {
            SortedSet<string> sortedSet = new SortedSet<string>();
            sortedSet.Add("213");
            sortedSet.Add("123");
            sortedSet.Add("wer");
            sortedSet.Add("asd");
            sortedSet.Add("adfs");
        }

        [TestMethod]
        [TestProperty("author", "apli")]
        public void SortedDictionaryTest()
        {
            SortedDictionary<string, string> sortedDic = new SortedDictionary<string, string>();
            sortedDic.Add("123", "sdf");
            sortedDic.Add("11", "sdf");
            sortedDic.Add("dfdf", "sdf");
            sortedDic.Add("adf", "sdf");
            sortedDic.Add("asre", "sdf");
        }
    }
}
