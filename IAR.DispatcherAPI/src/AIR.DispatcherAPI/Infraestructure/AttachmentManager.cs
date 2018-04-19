using System;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using NLog;

namespace IAR.DispatcherAPI.Infraestructure
{
    public static class AttachmentManager
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        private static StringBuilder sbuilderComplete = new StringBuilder();

        private static string RemoveTags(string source)
        {
            var objRegExp = new Regex("<(.|\n)+?>", RegexOptions.IgnoreCase);

            //Replace all HTML tag matches with the empty string
            string strOutput = objRegExp.Replace(source, "");
            string strOutputAsAttributes = string.Empty;

            //Replace all < and > with &lt; and &gt;
            strOutput = strOutput.Replace("<", "&lt;");
            strOutput = strOutput.Replace(">", "&gt;");

            //Try to parsing on the assumption that source has attributes instead of elements
            if (strOutput.Replace("\r\n", string.Empty).Trim() == string.Empty)
            {
                sbuilderComplete.Clear();
                strOutputAsAttributes = getInfoDocument(source).ToString();

                if (strOutputAsAttributes.Replace("\n", string.Empty).Trim() != string.Empty)
                {
                    strOutput = strOutputAsAttributes;
                }
            }

            return strOutput;
        }

        private static string GetMessageFromFile(string file)
        {
            string result = string.Empty;

            if (File.Exists(file))
            {
                try
                {
                    result = File.ReadAllText(file);
                }
                catch (Exception ex)
                {
                    Logger.Error("Error when trying to read file: {0}. Message: {1}", file, ex.Message);
                }
            }

            return result;
        }

        private static void RemoveFile(string file)
        {
            if (File.Exists(file))
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    Logger.Error("Error when trying to delete file: {0}. Message: {1}", file, ex.Message);
                }
            }
        }

        public static string GetMessage(MailMessage message, string attachmentsFolder, string validTypes)
        {
            var sb = new StringBuilder();
            var validFileTypes = new Regex(validTypes);

            if (string.IsNullOrEmpty(attachmentsFolder))
            {
                Logger.Error("Attachments folder was not defined in the configuration file.");
                return string.Empty;
            }

            if (!attachmentsFolder.EndsWith(@"\", StringComparison.Ordinal))
            {
                attachmentsFolder += @"\";
            }

            if (message.Attachments.Count > 0)
            {
                Logger.Debug("Number of attachments in message are: {0}", message.Attachments.Count);

                // Save to attachments folder
                foreach (Attachment a in message.Attachments)
                {
                    //if (validFileTypes.Match(a.Name).Success)
                    //{
                    //    if (a. .SaveToFolder(attachmentsFolder, true))
                    //    {
                    //        Logger.Debug("Attachment: {0}, saved", a.Name);

                    //        Open file
                    //        string content = GetMessageFromFile(attachmentsFolder + a.Name);

                    //        Get parsed content
                    //       content = RemoveTags(content);
                    //        sb.Append(content);

                    //        Delete file
                    //        RemoveFile(attachmentsFolder + a.Name);
                    //    }
                    //    else
                    //    {
                    //        Logger.Debug("Couldn't save attachment: {0}", a.Name);
                    //    }
                    //}
                    //else
                    //{
                    //    Logger.Debug("Invlid attachment type: {0}", a.Name);
                    //}
                }
            }
            else
            {
                //Logger.Debug("Message {0} has no attachments", message.Filename);
            }

            return sb.ToString();
        }

        private static StringBuilder ProcessContentAsXML(string source)
        {
            var sBuilder = new StringBuilder();

            sbuilderComplete.Clear();
            sBuilder = getInfoDocument(source);

            return sBuilder;
        }

        private static StringBuilder getInfoDocument(string source)
        {
            var sBuilder = new StringBuilder();
            var xmlDoc = new XmlDocument();

            xmlDoc.LoadXml(source);

            // Remove all XML declaration nodes
            foreach (XmlNode node in xmlDoc.ChildNodes)
            {
                if (node.NodeType == XmlNodeType.XmlDeclaration)
                {
                    xmlDoc.RemoveChild(node);
                }
            }

            XmlNode xmlNode = xmlDoc.FirstChild;
            sBuilder = getInfoNode(xmlNode);

            return sBuilder;
        }

        private static StringBuilder getInfoNode(XmlNode xmlNode)
        {
            var elementName = string.Empty;
            var attributeAndValueFormated = string.Empty;
            var elementAndValueFormated = string.Empty;

            if (xmlNode != null)
            {
                elementName = string.Format("[{0}]{1}\n", xmlNode.Name.ToUpper(), !string.IsNullOrEmpty(xmlNode.Value) ?
                                string.Format(":{0}", xmlNode.Value) :
                                string.Empty);

                sbuilderComplete.Append(elementName);

                // This node, has attributes?
                foreach (XmlAttribute attribute in xmlNode.Attributes)
                {
                    var attName = attribute.Name;
                    var attValue = attribute.Value;

                    attributeAndValueFormated = string.Format("{0}:{1}\n", attName, attValue);
                    sbuilderComplete.Append(attributeAndValueFormated);
                }

                // This node, has a value as Element?
                if (xmlNode.FirstChild != null &&
                    xmlNode.FirstChild.Value != null &&
                    xmlNode.FirstChild.Value != string.Empty &&
                    xmlNode.FirstChild.NodeType != XmlNodeType.Comment)
                {
                    var elemName = xmlNode.Name;
                    var elemValue = xmlNode.FirstChild.Value;

                    elementAndValueFormated = string.Format("{0}:{1}\n", elemName, elemValue);
                    sbuilderComplete.Append(elementAndValueFormated);
                }

                //This node, has Child nodes?
                foreach (XmlNode childNode in xmlNode.ChildNodes)
                {
                    if (childNode.NodeType == XmlNodeType.Element)
                    {
                        getInfoNode(childNode);
                    }
                }
            }

            return sbuilderComplete;
        }

    }
}
