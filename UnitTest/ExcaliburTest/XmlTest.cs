using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace UnitTest.ExcaliburTest
{
    [TestClass]
    public class XmlTest
    {
        [TestMethod]
        [TestProperty("author", "apli")]
        public void GetAttribute()
        {
            try
            {
                string xml = "<test><element des=\"bbb\"/></test>";
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                var file= ((XmlElement)doc.DocumentElement.FirstChild).GetAttribute("file");

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        [TestMethod]
        [TestProperty("author", "apli")]
        public void OperatorTest()
        {
            bool a = true;
            bool b = false;

            var r1 = a | b;
            var r2 = a & b;

            r2 |= r1;
            r2 &= r1;

        }

        [TestMethod]
        [TestProperty("author", "apli")]
        public void RandTest()
        {
            double d1 = -1.25;
            double result = Math.Round(d1, 1, MidpointRounding.AwayFromZero);
        }
    }
}
