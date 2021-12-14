using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using CloudLearningAPI.Interfaces;
using CloudLearningAPI.Models;

namespace CloudLearningAPI.Services
{
    public class DataParser : IDataParser
    {
        #region Private Methods
        /// <summary>
        /// Convert the XML data to DataSet
        /// </summary>
        /// <param name="dataStream"></param>
        /// <returns></returns>
        private DataSet XMLToDataSet(Stream dataStream)
        {
            DataSet ds = new();

            XmlDocument xml = new();
            xml.Load(dataStream);
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xml.NameTable);
            nsmgr.AddNamespace("ss", "urn:schemas-microsoft-com:office:spreadsheet");
            XmlElement root = xml.DocumentElement;

            foreach (XmlNode node in root.SelectNodes("//ss:Worksheet", nsmgr))
            {
                DataTable dt = new(node.Attributes["ss:Name"].Value);
                ds.Tables.Add(dt);
                XmlNodeList rows = node.SelectNodes("ss:Table/ss:Row", nsmgr);
                if (rows.Count > 0)
                {
                    foreach (XmlNode data in rows[0].SelectNodes("ss:Cell/ss:Data", nsmgr))
                    {
                        dt.Columns.Add(data.InnerText, typeof(string));
                    }

                    for (int i = 1; i < rows.Count; i++)
                    {
                        var cells = rows[i].SelectNodes("ss:Cell/ss:Data", nsmgr);
                        DataRow row = dt.NewRow();
                        int columnIndex = 0;
                        foreach (XmlNode cell in cells)
                        {
                            row[columnIndex] = cell.InnerText;
                            columnIndex++;
                        }
                        dt.Rows.Add(row);
                    }
                }
            }

            return ds;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Parse the downloaded data.
        /// </summary>
        /// <param name="dataStream"></param>
        /// <returns></returns>
        public List<Course> ParseData(Stream dataStream)
        {
            var dataSet = XMLToDataSet(dataStream).Tables[0]?.AsEnumerable();
            if (dataSet is null) 
            {
                return null;
            }

            List<Participant> participants = dataSet.Select(datarow => new Participant
            {
                ID = datarow.Field<string>("person_ID"),
                Firstname = datarow.Field<string>("Vorname"),
                Lastname = datarow.Field<string>("Nachname"),
                Email = datarow.Field<string>("EMail").ToLower(),
                Coursenumber = datarow.Field<string>("Kursnummer")
            }).ToList();

            HashSet<string> courseNumbers = new();
            List<Course> courses = new();

            foreach (var datarow in dataSet)
            {
                string cn = datarow.Field<string>("Kursnummer");
                if (!courseNumbers.Contains(cn))
                {
                    courses.Add(new Course()
                    {
                        Coursenumber = cn,
                        Language = datarow.Field<string>("Sprache"),
                        Level = datarow.Field<string>("Niveau"),
                        Participants = participants.Where(participant => participant.Coursenumber == cn).ToList()
                    });
                    courseNumbers.Add(cn);

                }
            }

            return courses;
        }
        #endregion
    }
}
