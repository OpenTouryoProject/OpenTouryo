using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace WinStore_sample.Converter
{
    public class WSErrorInfo
    {
        public string ErrorType;
        public string MessageID;
        public string Message;
        public string Information;

        public WSErrorInfo(string err)
        {
            using (XmlReader reader = XmlReader.Create(new StringReader(err)))
            {
                // Parse the file and display each of the nodes.
                string ename = "";
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            //System.Diagnostics.Debug.WriteLine(reader.Name);
                            ename = reader.Name;
                            break;

                        case XmlNodeType.Text:
                            //System.Diagnostics.Debug.WriteLine(reader.Value);
                            if (ename.ToUpper() == "ERRORTYPE")
                            {
                                this.ErrorType = reader.Value;
                            }
                            else if (ename.ToUpper() == "MESSAGEID")
                            {
                                this.MessageID = reader.Value;
                            }
                            else if (ename.ToUpper() == "MESSAGE")
                            {
                                this.Message = reader.Value;
                            }
                            else if (ename.ToUpper() == "INFORMATION")
                            {
                                this.Information = reader.Value;
                            }
                            break;

                        //case XmlNodeType.XmlDeclaration:
                        //    System.Diagnostics.Debug.WriteLine(reader.Name + "," + reader.Value);
                        //    break;
                        //case XmlNodeType.ProcessingInstruction:
                        //    System.Diagnostics.Debug.WriteLine(reader.Name + "," + reader.Value);
                        //    break;
                        //case XmlNodeType.Comment:
                        //    System.Diagnostics.Debug.WriteLine(reader.Value);
                        //    break;
                        case XmlNodeType.EndElement:
                            //System.Diagnostics.Debug.WriteLine(reader.Name);
                            ename = "";
                            break;
                    }
                }

                if (this.ErrorType.ToUpper() != "BUSINESSAPPLICATIONEXCEPTION")
                {
                    throw new Exception(this.ErrorType + " , "+ this.MessageID + " , " + Message + " , " + Information);
                }
            }
        }
    }
}
