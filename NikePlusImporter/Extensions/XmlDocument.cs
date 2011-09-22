using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Maximum.Helper;

namespace Maximum.Extensions
{
    public static class XmlDocumentExtension
    {
        public static T GetValue<T>(this XmlDocument xmlDoc, string xPath, T defaultValue = default(T))
        {
            T value = defaultValue;

            var node = xmlDoc.SelectSingleNode(xPath);

            if (node != null)
            {
                if (string.IsNullOrWhiteSpace(node.Value) && (node.FirstChild != null))
                {
                    value = StringParser.Convert<T>(node.FirstChild.Value); 
                }
                else
                { 
                    value = StringParser.Convert<T>(node.Value); 
                }
            }

            return value;
        }

        public static T GetAttributeValue<T>(this XmlDocument xmlDoc, string xPath, string attributeName)
        {
            T value = default(T);

            var node = xmlDoc.SelectSingleNode(xPath);

            if (node != null)
            {
                value = StringParser.Convert<T>(node.Attributes[attributeName].Value);
            }

            return value;
        }
    }
}
