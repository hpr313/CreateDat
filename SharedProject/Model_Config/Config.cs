using System;
using System.Collections.Generic;
using System.Xml;

namespace SharedProject.Model_Config
{
    class Config
    {
        public string Date { get; set; }
        public int current_count { get; set; }
        //public string streamNum { get; set; }
        //public string fileName { get; set; }
        //public string  fileTitle { get; set; }
        public Config(string confName)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(confName);
            XmlNodeList nodes = xdoc.SelectNodes("//Root");
            foreach (XmlNode node in nodes)
            {
                XmlNode date = node.SelectSingleNode("Date");
                Date = parse_date(date.InnerText);
                XmlNode count = node.SelectSingleNode("Counts");
                current_count = int.Parse(count.InnerText);
            }
        }
        public string parse_date(string dt)
        {
            string[] dt_arr = dt.Split('/');
            string year = dt_arr[0].Substring(2, 2);
            string month = dt_arr[1].PadLeft(2, '0');
            string day = dt_arr[2].PadLeft(2, '0');
            string date = year + month + day;
            return date;
        }
        public void initial_conf()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("config.xml");
            XmlNode node_date = doc.SelectSingleNode("Root/Date");
            XmlNode node_counts = doc.SelectSingleNode("Root/Counts");
            string dt = DateTime.Now.ToString();
            string[] dt_arr = dt.Split(' ');
            if (node_date.InnerText != dt_arr[0])
            {
                node_date.InnerText = dt_arr[0];
                node_counts.InnerText = "0";
            }
            doc.Save("config.xml");
        }
        public void update_conf(List<string> strs)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("config.xml");
            XmlNode node_date = doc.SelectSingleNode("Root/Date");
            string dt = DateTime.Now.ToString();
            string[] dt_arr = dt.Split(' ');
            node_date.InnerText = dt_arr[0];
            XmlNode node_counts = doc.SelectSingleNode("Root/Counts");
            node_counts.InnerText = get_counts(strs);
            doc.Save("config.xml");
        }
        public string get_counts(List<string> xxx)
        {
            string f = xxx[xxx.Count - 1].Substring(11, 4);
            string c = int.Parse(f).ToString();
            return c;
        }

    }
}
