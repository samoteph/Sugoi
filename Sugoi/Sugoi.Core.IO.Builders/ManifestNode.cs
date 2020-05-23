using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Sugoi.Core.IO.Builders
{
    public abstract class ManifestNode
    {
        public abstract void Read(XmlElement node);

        public abstract void Write(BinaryWriter writer);

        /// <summary>
        /// Ecriture d'une chaine de caractère d'une taille max
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="stringToWrite"></param>
        /// <param name="maxLength"></param>

        protected void WriteString(BinaryWriter writer, string stringToWrite, int maxLength)
        {
            char[] charArray = new char[maxLength];

            if(stringToWrite.Length > maxLength)
            {
                throw new Exception("the string '" + stringToWrite + "' is to long (max " + maxLength + " characters)");
            }

            var charactersToWrite = stringToWrite.ToCharArray();

            charactersToWrite.CopyTo(charArray, 0);
            writer.Write(charArray);
        }

        protected byte GetByteAttribute(XmlElement element, string attributeName, bool isManadatory, byte defaultValue)
        {
            try
            {
                string value = GetAttribute(element, attributeName, isManadatory);

                if (value != null)
                {
                    return byte.Parse(value);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error has occured during the reading of the integer attribute '" + attributeName + "' of the element '" + element.Name + "'. Exception message=" + ex.Message);
            }

            return defaultValue;
        }

        protected int GetIntAttribute(XmlElement element, string attributeName, bool isManadatory, int defaultValue = -1)
        {
            try
            {
                string value = GetAttribute(element, attributeName, isManadatory);

                if (value != null)
                {
                    return int.Parse(value);
                }
            }
            catch(Exception ex)
            {
                throw new Exception("An error has occured during the reading of the integer attribute '" + attributeName + "' of the element '" + element.Name + "'. Exception message=" + ex.Message);
            }

            return defaultValue;
        }

        protected string GetAttribute(XmlElement element, string attributeName, bool isMandatory)
        {
            if (element.HasAttribute(attributeName))
            {
                return element.GetAttribute(attributeName);
            }

            if (isMandatory == true)
            {
                throw new Exception("the attribute " + attributeName + " is mandatory!");
            }

            return null;
        }

        protected TEnum GetEnumAttribute<TEnum>(XmlElement element, string attributeName, bool isMandatory, TEnum defaultValue) where TEnum : struct
        {
            try
            {
                string value = GetAttribute(element, attributeName, isMandatory);

                if (value != null)
                {
                    if (Enum.TryParse<TEnum>(value, true, out var result))
                    {
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error has occured during the reading of the enum attribute '" + attributeName + "' of the element '" + element.Name + "'. Exception message=" + ex.Message);
            }

            return defaultValue;
        }
    }
}
