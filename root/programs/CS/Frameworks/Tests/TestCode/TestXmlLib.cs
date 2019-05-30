using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Xml;
using Touryo.Infrastructure.Public.Diagnostics;

namespace TestCode
{
    /// <summary>Program</summary>
    public class TestXmlLib
    {
        #region public
        /// <summary>Root</summary>
        public static void Root()
        {
            // Xmlロード
            string xml = EmbeddedResourceLoader.LoadXMLAsString(
                "OpenTouryo.Public", "Touryo.Infrastructure.Public.Xml.TestXml.xml");

            // Xsdによる検証
            if (XmlLib.ValidateByEmbeddedXsd(
                xml, "OpenTouryo.Public", "Touryo.Infrastructure.Public.Xml.TestXsd.xsd", "urn:bookstore-schema"))
            {
                MyDebug.OutputDebugAndConsole("XmlLib", "is working properly.");
            }
            else
            {
                MyDebug.OutputDebugAndConsole("XmlLib", "is not working properly.");
            }
        }
        #endregion
    }
}