using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace BIRTHDAY
{
    class DataLayer
    {
        static string pathXml = ConfigurationSettings
            .AppSettings["pathXmlData"].ToString();

        static string pathTxt = ConfigurationSettings
            .AppSettings["pathTxtData"].ToString();


        public static string[] GetPeopleName()
        { 
            //Open FileStream
            FileStream fs = new FileStream(pathXml, FileMode.Open, 
                FileAccess.Read, FileShare.ReadWrite);

            //Create the xmlDocument
            XmlDocument xmlDoc = new XmlDocument();

            //Load the xml data into XmlDocument
            xmlDoc.Load(fs);

            ArrayList name_SurName = new ArrayList();

            for (int i = 0; i < xmlDoc.DocumentElement.ChildNodes.Count; i++)
                name_SurName.Add(xmlDoc.DocumentElement.ChildNodes[i]
                    .SelectSingleNode("name").InnerText +
                    " " + 
                    xmlDoc.DocumentElement.ChildNodes[i]
                    .SelectSingleNode("surName").InnerText);

            //Get the count of nodes in the xml document
            /*for (int i = 0; i < xmlDoc.DocumentElement.ChildNodes.Count; i++)
            {
                string temp = xmlDoc.DocumentElement.ChildNodes[i].FirstChild.InnerText;
                temp += " " + xmlDoc.DocumentElement.ChildNodes[i].FirstChild.NextSibling.InnerText;

                name_SurName.Add(temp);
            }

            for (int i = 0; i < xmlDoc.DocumentElement.ChildNodes.Count; i++)
            {
                string temp = string.Empty;

                for (int j = 0; j < xmlDoc.DocumentElement.ChildNodes[i].ChildNodes.Count; j++)
                {
                    if (xmlDoc.DocumentElement.ChildNodes[i].ChildNodes[j].Name == "name" || 
                        xmlDoc.DocumentElement.ChildNodes[i].ChildNodes[j].Name == "surName")
                        temp += " " + xmlDoc.DocumentElement.ChildNodes[i].ChildNodes[j].InnerText;
                }

                name_SurName.Add(temp);
            }

            fs.Close();*/



            return (string[])name_SurName.ToArray(typeof(string));
        }

        public static Man GetManDetails(string name, string surName)
        {
            string fileDescrName = pathTxt + "\\" + name+surName + "\\description.txt";

            Man man = new Man();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(pathXml);

            XmlNode root = xmlDoc.DocumentElement;
            XmlNode node = root.SelectSingleNode(
                String.Format("Man[name='{0}' and surName='{1}']",
                name, surName));

            XmlNodeList list = node.ChildNodes;

            for (int i = 0; i < list.Count; i++)
                man.ManInfo[list.Item(i).Name] = list.Item(i).InnerText;

            StreamReader str = null;
            try
            {
                str = new StreamReader(fileDescrName);

                string lineText = "";
                string fullText = "";

                while ((lineText = str.ReadLine()) != null)
                    fullText += lineText;

                str.Dispose();

                man.Description = fullText;
            }
            catch (OutOfMemoryException eee)
            {
                man.Description = "Не хватает оперативной памяти\n\n" + eee.Message;
            }
            catch (FileNotFoundException e)
            {
                man.Description = "Файла с описанием данной личности не обнаружено\n\n" + e.Message;
            }
            catch (DirectoryNotFoundException ee)
            {
                man.Description = "Папки с файлом описания не обнаружено\n\n" + ee.Message;
            }
            finally
            {
                if (str != null)
                    str.Dispose();
            }

            return man;
        }

        public static bool AddPerson(Man newMan, ref string message)
        { 
            if (CheckForExistPerson(newMan))
            {
                message = "Такая персона уже имеется в базе данных";
                return false;
            }

            FileStream fs;
            XmlDocument xmlDoc;
            try
            {
                using (fs = new FileStream(pathXml, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    xmlDoc = new XmlDocument();
                    xmlDoc.Load(fs);
                    fs.Close();
                }
            }
            catch
            {
                message = "Не удалось создать персону. Файл БД поврежден, или отсутствует";
                return false;
            }
            try
            {
                XmlElement newItem;
                XmlElement newOuterItem = xmlDoc.CreateElement("Man");

                foreach (KeyValuePair<string, string> kvp in newMan.ManInfo)
                {
                    newItem = xmlDoc.CreateElement(kvp.Key);
                    newItem.InnerText = kvp.Value;
                    newOuterItem.AppendChild(newItem);
                }
                xmlDoc.DocumentElement.InsertAfter(newOuterItem,
                    xmlDoc.DocumentElement.LastChild);

                FileStream writer = new FileStream(pathXml, 
                    FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite);
                xmlDoc.Save(writer);
                
                writer.Dispose();
            }
            catch
            {
                message = "Персону не удалось создать";
                return false;
            }

            if (AddDescription(newMan))
                message = "Персона успешно создана и файл описания успешно создан";
            else
                message = "Персона успешно создана а её файл описания не был создан";
            
            return true;
        }

        public static bool DeletePerson(Man deletedMan)
        {
            string directoryDescName = pathTxt + "\\" + deletedMan.Name+deletedMan.SurName;
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(pathXml);

                XmlNode root = xmlDoc.DocumentElement;
                XmlNode node = root.SelectSingleNode(string.Format(
                    "Man[name='{0}' and surName='{1}' and mobilePhone='{2}']",
                    deletedMan.Name, deletedMan.SurName, deletedMan.MobilePhone));

                XmlNode outer = node.ParentNode;
                outer.RemoveChild(node);
                xmlDoc.Save(pathXml);

                try
                {
                    if(Directory.Exists(directoryDescName))
                        Directory.Delete(directoryDescName, true);
                }
                catch
                { 
                
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string UpdatePerson(string currentName, 
            string currenSurName, Man proposedMan)
        {
            string currentDirectoryDescName = pathTxt + "\\" + currentName+currenSurName;
            string proposedDirectoryDescName = pathTxt + "\\" + proposedMan.Name+proposedMan.SurName;

            if(currentName != proposedMan.Name || currenSurName != proposedMan.SurName)
                if (CheckForExistPerson(proposedMan))
                    return "Такая персона уже имеется в БД. Измените параметры персоны";

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(pathXml);

                XmlNode root = xmlDoc.DocumentElement;
                XmlNode node = root.SelectSingleNode(String.Format(
                    "Man[name='{0}' and surName='{1}']", currentName, currenSurName));

                XmlNodeList list = node.ChildNodes;
                for(int i = 0; i < list.Count; i ++)
                    list.Item(i).InnerText = 
                        proposedMan.ManInfo[list.Item(i).Name];

                FileStream fs = new FileStream(pathXml,
                    FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite);

                xmlDoc.Save(fs);
                fs.Dispose();

                if (currentName != proposedMan.Name || currenSurName != proposedMan.SurName)
                {
                    try
                    {
                        Directory.Delete(currentDirectoryDescName, true);
                    }
                    catch 
                    {
                        
                    }
                }
                if (AddDescription(proposedMan))
                    return "Персона успешно обновлена";

                return "Персона успешно обновлена, но файл с описанием не создан!";
            }
            catch
            {
                return "Персона не обновлена";
            }
        }
        
        private static bool CheckForExistPerson(Man newMan)
        {
            XmlDocument xmlDoc;
            try
            {
                xmlDoc = new XmlDocument();
                xmlDoc.Load(pathXml);
            }
            catch
            {
                return true;
            }

            XmlNodeList xmlListName = xmlDoc.DocumentElement.GetElementsByTagName("name");
            XmlNodeList xmlListSurName = xmlDoc.DocumentElement.GetElementsByTagName("surName");
            for (int j = 0; j < xmlListName.Count; j++ )
                if (xmlListName[j].InnerText == newMan.ManInfo["name"] &&
                    xmlListSurName[j].InnerText == newMan.ManInfo["surName"])
                    return true;

            return false;
        }

        static bool AddDescription(Man man)
        {
            StreamWriter fileInfo;
            try
            {
                string directoryDescrName = pathTxt + "\\" + man.Name + man.SurName;
                string fileDescrName = directoryDescrName + "\\description.txt";
                DirectoryInfo dirInfo = Directory.CreateDirectory(directoryDescrName);

                using (fileInfo = new StreamWriter(fileDescrName))
                {
                    fileInfo.Write(man.Description);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static string GetEarlyBirthDay()
        {
            string monthNow = DateTime.Today.ToString("MM");
            string dayNow = DateTime.Today.ToString("dd");
            string dayTomorrow = DateTime.Today.AddDays(3).ToString("dd");


            return GetBirthdayList(false, dayNow, dayTomorrow, monthNow);
        }

        internal static string GetTodayBirthDay()
        {
            string monthNow = DateTime.Today.ToString("MM");
            string dayNow = DateTime.Today.ToString("dd");


            return GetBirthdayList(true, dayNow, null, monthNow);
        }

        static string GetBirthdayList(bool isToday, string dayNow_, string dayTomorrow_, string monthNow_)
        {
            String message = string.Empty;

            #region LoadXml
            XmlDocument xmlDoc;
            try
            {
                xmlDoc = new XmlDocument();
                xmlDoc.Load(pathXml);
            }
            catch
            {
                return "Не удалось загрузить БД";
            }
        #endregion

            string monthNow = monthNow_;
            string dayNow = dayNow_;
            string dayTomorrow = dayTomorrow_;

            XmlNode root = xmlDoc.DocumentElement;

            if (!isToday)
            {
                //Если мы хотим получить всю ветку Man
                XmlNodeList nodeList = root.SelectNodes(
                    String.Format(
                    "Man[substring(dateOfBirth, 1, 2) > '{0}' and " +
                    "substring(dateOfBirth, 1, 2) < '{1}' and " +
                    "substring(dateOfBirth, 4, 2) = '{2}']",
                    dayNow, dayTomorrow, monthNow));

                //если мы хотим получить конкретно узел dateOfBirth
                /*XmlNodeList nodeList = root.SelectNodes(
                    String.Format(
                    "//dateOfBirth[substring(text(), 1, 2) > '{0}' and " +
                    "substring(text(), 1, 2) < '{1}' and " +
                    "substring(text(), 4, 2) < '{2}']",
                    dayNow, dayTomorrow, monthNow));*/

                for (int i = 0; i < nodeList.Count; i++)
                    message += nodeList[i]["name"].InnerText + " " +
                        nodeList[i]["surName"].InnerText + " " +
                        nodeList[i]["dateOfBirth"].InnerText + "\n";
            }
            else
            {
                //Если мы хотим получить всю ветку Man
                XmlNodeList nodeList = root.SelectNodes(
                    String.Format(
                    "Man[substring(dateOfBirth, 1, 2) = '{0}' and " +
                    "substring(dateOfBirth, 4, 2) = '{2}']",
                    dayNow, dayTomorrow, monthNow));

                for (int i = 0; i < nodeList.Count; i++)
                    message += nodeList[i]["name"].InnerText + " " +
                        nodeList[i]["surName"].InnerText + " " +
                        nodeList[i]["dateOfBirth"].InnerText + "\n";
            }

            return message;
        }
    }
}
