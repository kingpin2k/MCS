// Type: Advent.MediaCenter.MediaCenterUtil



using Advent.Common.Interop;
using Advent.Common.IO;
using Advent.MediaCenter.Mcml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Linq;

namespace Advent.MediaCenter
{
    public static class MediaCenterUtil
    {
        private const string ResScheme = "res://";
        private static int[] stringIdHashes;
        private static ushort[] stringIds;

        public static string MediaCenterPath
        {
            get
            {
                return Path.Combine(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.System)).FullName, "ehome");
            }
        }

        public static Process LaunchMediaCenter(bool forceFullScreen, bool forceWideScreen, bool gdi)
        {
            return MediaCenterUtil.LaunchMediaCenter(new ProcessStartInfo(), forceFullScreen, forceWideScreen, gdi);
        }

        public static Process LaunchMediaCenter(ProcessStartInfo processStartInfo, bool forceFullScreen, bool forceWideScreen, bool gdi)
        {
            if (processStartInfo == null)
                throw new ArgumentNullException("processStartInfo");
            processStartInfo.FileName = Path.Combine(MediaCenterUtil.MediaCenterPath, "ehshell.exe");
            processStartInfo.WindowStyle = forceFullScreen ? ProcessWindowStyle.Maximized : ProcessWindowStyle.Normal;
            string str = string.Empty;
            if (forceWideScreen)
                str = str + " /widescreen";
            if (gdi)
                str = str + " /gdi";
            processStartInfo.Arguments = str.Trim();
            return Process.Start(processStartInfo);
        }

        public static IntPtr GetMediaCenterWindow()
        {
            return NativeMethods.FindWindow("eHome Render Window", (string)null);
        }

        public static XmlReader GetXml(this IResource res)
        {
            byte[] bytes = ResourceExtensions.GetBytes(res);
            if (bytes != null)
                return MediaCenterUtil.GetXmlResource(bytes);
            else
                return (XmlReader)null;
        }

        public static McmlDocument GetMcml(this IResource res)
        {
            XmlReader xml = MediaCenterUtil.GetXml(res);
            if (xml != null)
                return new McmlDocument(XDocument.Load(xml));
            else
                return (McmlDocument)null;
        }

        public static XmlReader GetXmlResource(IResourceLibrary lib, string name, int type)
        {
            IResource resource = lib.GetResource(name, (object)type);
            if (resource != null)
                return MediaCenterUtil.GetXml(resource);
            else
                return (XmlReader)null;
        }

        public static string AttributeValue(this XElement element, XName name)
        {
            XAttribute xattribute = element.Attribute(name);
            if (xattribute != null)
                return xattribute.Value;
            else
                return (string)null;
        }

        public static XNamespace GetNamespace(this XDocument doc, string ns)
        {
            XAttribute xattribute = Enumerable.FirstOrDefault<XAttribute>(doc.Root.Attributes(), (Func<XAttribute, bool>)(o =>
            {
                if (o.IsNamespaceDeclaration)
                    return o.Name.LocalName == ns;
                else
                    return false;
            }));
            if (xattribute != null)
                return (XNamespace)xattribute.Value;
            else
                return (XNamespace)null;
        }

        public static void UpdateMcml(this IResource res, McmlDocument doc)
        {
            MediaCenterUtil.UpdateXml(res, doc.Xml);
        }

        public static void UpdateXml(this IResource res, XDocument doc)
        {
            if (doc.Declaration != null)
                doc.Declaration.Encoding = "UTF-16";
            else
                doc.Declaration = new XDeclaration("1.0", "UTF-16", (string)null);
            StringWriter stringWriter = new StringWriter();
            XmlTextWriter xmlTextWriter = new XmlTextWriter((TextWriter)stringWriter)
            {
                Formatting = Formatting.Indented
            };
            doc.WriteTo((XmlWriter)xmlTextWriter);
            ResourceExtensions.Update(res, stringWriter.ToString(), Encoding.Unicode);
        }

        public static void UpdateXml(this IResource res, XmlDocument doc)
        {
            XmlDeclaration xmlDeclaration = doc.FirstChild as XmlDeclaration;
            if (xmlDeclaration != null)
                xmlDeclaration.Encoding = "UTF-16";
            else
                doc.InsertBefore((XmlNode)doc.CreateXmlDeclaration("1.0", "UTF-16", (string)null), (XmlNode)doc.DocumentElement);
            StringWriter stringWriter = new StringWriter();
            XmlTextWriter xmlTextWriter = new XmlTextWriter((TextWriter)stringWriter)
            {
                Formatting = Formatting.Indented
            };
            doc.WriteTo((XmlWriter)xmlTextWriter);
            ResourceExtensions.Update(res, stringWriter.ToString(), Encoding.Unicode);
        }

        public static void SaveXmlResource(IResourceLibrary lib, string name, int type, XmlDocument doc)
        {
            MediaCenterUtil.UpdateXml(lib.GetResource(name, (object)type), doc);
        }

        public static XmlReader GetXmlResource(IResourceLibraryCache cache, string uri)
        {
            string resourceName;
            return MediaCenterUtil.GetXmlResource(cache, uri, out resourceName);
        }

        public static XmlReader GetXmlResource(IResourceLibraryCache cache, string uri, out string resourceName)
        {
            byte[] resource = MediaCenterUtil.GetResource(cache, uri, 23, out resourceName);
            if (resource != null)
                return MediaCenterUtil.GetXmlResource(resource);
            else
                return (XmlReader)null;
        }

        public static string GetStringResource(IResourceLibraryCache cache, string uri, bool throwIfNotFound)
        {
            string dll;
            string resource;
            if (MediaCenterUtil.TryParseResourceUri(uri, out dll, out resource))
                return ResourceExtensions.GetStringResource(cache[dll], MediaCenterUtil.GetMagicStringResourceID("#" + resource));
            if (throwIfNotFound)
                throw new Exception("Could not find string resource: " + uri);
            else
                return (string)null;
        }

        public static string GetStringResource(IResourceLibraryCache cache, string uri)
        {
            return MediaCenterUtil.GetStringResource(cache, uri, true);
        }

        public static ImageSource GetImageResource(IResourceLibraryCache cache, string uri)
        {
            string resourceName;
            return MediaCenterUtil.GetImageResource(cache, uri, out resourceName);
        }

        public static ImageSource GetImageResource(IResourceLibraryCache cache, string uri, out string resourceName)
        {
            byte[] resource = MediaCenterUtil.GetResource(cache, uri, 10, out resourceName);
            if (resource == null)
                return (ImageSource)null;
            else
                return (ImageSource)BitmapDecoder.Create((Stream)new MemoryStream(resource), BitmapCreateOptions.None, BitmapCacheOption.Default).Frames[0];
        }

        public static byte[] GetResource(IResourceLibraryCache cache, string uri, int resourceType, out string resourceName)
        {
            string dll;
            if (MediaCenterUtil.TryParseResourceUri(uri, out dll, out resourceName))
                return ResourceExtensions.GetBytes(cache[dll].GetResource(resourceName, (object)resourceType));
            else
                throw new Exception("Could not find resource: " + uri);
        }

        public static int GetMagicStringResourceID(string proc)
        {
            string str1 = proc;
            if (str1 != null && str1.StartsWith("#"))
            {
                string str2 = str1.Substring(1);
                if (str2.StartsWith("#"))
                    return -1;
                if (MediaCenterUtil.stringIdHashes == null)
                {
                    Type type = Assembly.LoadFile(Path.Combine(MediaCenterUtil.MediaCenterPath, "ehshell.dll")).GetType("Microsoft.MediaCenter.Utilities.StringResourceTables", true);
                    MediaCenterUtil.stringIdHashes = (int[])type.GetField("StringIdentifierHashes").GetValue((object)null);
                    MediaCenterUtil.stringIds = (ushort[])type.GetField("StringIdentifiers").GetValue((object)null);
                }
                int index = Array.BinarySearch<int>(MediaCenterUtil.stringIdHashes, MediaCenterUtil.GetStringHashCode(str2));
                if (index >= 0)
                    return (int)MediaCenterUtil.stringIds[index];
            }
            return -1;
        }

        public static string GetMagicString(IResourceLibraryCache cache, string proc)
        {
            int resourceID;
            return MediaCenterUtil.GetMagicString(cache, proc, out resourceID);
        }

        public static string GetMagicString(IResourceLibraryCache cache, string proc, out int resourceID)
        {
            resourceID = MediaCenterUtil.GetMagicStringResourceID(proc);
            if (resourceID >= 0)
            {
                string stringResource = ResourceExtensions.GetStringResource(cache["ehres.dll"], resourceID);
                if (stringResource != null)
                    return stringResource;
            }
            return proc;
        }

        internal static XmlReader GetXmlResource(byte[] buffer)
        {
            return (XmlReader)new XmlTextReader((Stream)new MemoryStream(buffer));
        }

        internal static bool IsGuid(string guid)
        {
            if (guid.Length < 32)
                return false;
            try
            {
                Guid guid1 = new Guid(guid);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        internal static XmlElement UncommentElement(XmlComment comment)
        {
            XmlNode parentNode = comment.ParentNode;
            XmlElement element = comment.OwnerDocument.CreateElement("temp");
            XmlElement xmlElement = (XmlElement)null;
            parentNode.AppendChild((XmlNode)element);
            try
            {
                element.InnerXml = comment.InnerText;
                xmlElement = element.ChildNodes[0] as XmlElement;
            }
            catch (XmlException)
            {
            }
            finally
            {
                parentNode.RemoveChild((XmlNode)element);
            }
            if (xmlElement != null)
            {
                parentNode.InsertAfter((XmlNode)xmlElement, (XmlNode)comment);
                parentNode.RemoveChild((XmlNode)comment);
            }
            return xmlElement;
        }

        internal static void StripChildComments(XmlNode fromNode)
        {
            List<XmlComment> list = new List<XmlComment>();
            foreach (XmlNode xmlNode in fromNode.ChildNodes)
            {
                XmlComment xmlComment = xmlNode as XmlComment;
                if (xmlComment != null)
                    list.Add(xmlComment);
            }
            foreach (XmlComment xmlComment in list)
                fromNode.RemoveChild((XmlNode)xmlComment);
        }

        internal static string ColorToString(Color color)
        {
            return string.Format("{0}, {1}, {2}, {3}", (object)color.A, (object)color.R, (object)color.G, (object)color.B);
        }

        internal static bool TryParseColor(string text, out Color color)
        {
            color = Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue);
            string[] strArray = text.Split(new char[1]
      {
        ','
      });
            byte result1;
            byte result2;
            byte result3;
            byte result4;
            if (strArray.Length != 4 || !byte.TryParse(strArray[0].Trim(), out result1) || (!byte.TryParse(strArray[1].Trim(), out result2) || !byte.TryParse(strArray[2].Trim(), out result3)) || !byte.TryParse(strArray[3].Trim(), out result4))
                return false;
            color = Color.FromArgb(result1, result2, result3, result4);
            return true;
        }

        private static unsafe int GetStringHashCode(string str)
        {
            int num1 = 5381;
            int num2 = num1;
            fixed (char* chPtr = str.ToCharArray())
            {
                //created a local pointer that isn't immutable
                char* true_pointer = chPtr;
                while ((int)true_pointer[0] != 0)
                {
                    int num3 = (int)true_pointer[0];
                    if (num3 == 46)
                        num3 = 95;
                    else if (num3 >= 65 && num3 <= 90)
                        num3 += 32;
                    num1 = (num1 << 5) + num1 ^ num3;
                    if ((int)true_pointer[1] != 0)
                    {
                        int num4 = (int)true_pointer[1];
                        if (num4 == 46)
                            num4 = 95;
                        else if (num4 >= 65 && num4 <= 90)
                            num4 += 32;
                        num2 = (num2 << 5) + num2 ^ num4;
                        
                        true_pointer += 2;
                        //TODO
                        // I think we could just do true_pointer++

                    }
                    else
                        break;
                }
            }
            return num1 + num2 * 69069;
        }

        private static bool TryParseResourceUri(string uri, out string dll, out string resource)
        {
            if (uri.StartsWith("res://"))
            {
                string[] strArray = uri.Substring("res://".Length).Split(new char[1]
        {
          '!'
        });
                dll = strArray[0] + ".dll";
                resource = strArray[1];
                return true;
            }
            else
            {
                dll = (string)null;
                resource = (string)null;
                return false;
            }
        }
    }
}
