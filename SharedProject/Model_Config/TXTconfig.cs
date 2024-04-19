using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SharedProject.Model_Config
{
    class TXTconfig
    {
        public string Date { get; set; }
        public string Count { get; set; }
        public string Dia { get; set; }

        public TXTconfig get_txtconfig(TXTconfig xxx)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"config\txt_config.xml");
            XmlNode node_date = doc.SelectSingleNode("Root/Date");
            XmlNode node_counts = doc.SelectSingleNode("Root/Counts");
            XmlNode node_dia = doc.SelectSingleNode("Root/Dia");
            xxx.Date = node_date.InnerText;
            xxx.Count = node_counts.InnerText;
            xxx.Dia = node_dia.InnerText;
            return xxx;
        }
        public void initial_txtconf()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"config\txt_config.xml");
            XmlNode node_date = doc.SelectSingleNode("Root/Date");
            XmlNode node_counts = doc.SelectSingleNode("Root/Counts");
            XmlNode node_dia = doc.SelectSingleNode("Root/Dia");
            string dt = DateTime.Now.ToString();
            string[] dt_arr = dt.Split(' ');
            if (!node_date.InnerText.Equals(dt_arr[0]))
            {
                node_counts.InnerText = "0";
                node_date.InnerText = dt_arr[0];
            }
            doc.Save(@"config\txt_config.xml");
        }
        public string get_markerTXT(TXTconfig xxx)
        {
            string[] dt = xxx.Date.Split('\\');
            string month = dt[1].PadLeft(2, '0');
            string day = dt[2].PadLeft(2, '0');
            string date = month + day;
            string count = xxx.Count.PadLeft(3, '0');
            string markertxt = date + count + ' ' + xxx.Dia;
            return markertxt;
        }
    }
}
