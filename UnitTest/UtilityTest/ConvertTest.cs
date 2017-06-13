using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Cryptography;
//using Xunit;

namespace UtilityTest.CoreTest
{
    [TestClass]
    public class ConvertTest
    {
        //[Fact]
        [TestMethod]
        [TestProperty("author", "apli")]
        public void TryParseTest()
        {
            var actualResult = Converter.TryParse("false", true);
            bool result = false;

            //Xunit.Assert.Equal(result, actualResult);

            Assert.AreEqual(result, actualResult);
        }

        [TestMethod]
        [TestProperty("author", "apli")]
        public void CryptoTest()
        {
            Cryptograph.ComputeHash(MD5.Create(), "test");
        }
    }
}
