using SharedProject.Model_Config;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace create_dat
{
    public partial class Form1 : Form
    {
        public string space = new String(' ', 6); //全域變數: 6個字元的空白
        public Form1()
        {
            InitializeComponent();  // 預設的初始化
            initial_conf(); // config.xml的初始化
            initial_txtconf(); // txtcong.xml的初始化
            initial_txtTextBox_line4();
            initial_models();
            initial_materials();
            initial_csv();
            initial_measures();
            initial_fileTitle();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<string> fileNames = create_fileName();
            string savePath = txtBox_savePath.Text;
            create_dat(fileNames, savePath);
            create_txt(fileNames, savePath);
            update_conf(fileNames);
            update_txtconf(fileNames);
            initial_txtTextBox_line4();
        }
        private void create_folder(string savepath)
        {
            string dir = @savepath;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
        public void initial_txtconf()
        {
            XmlDocument doc = new XmlDocument(); // 建立xml檔的物件
            doc.Load(@"config\txt_config.xml"); // 讀取txt_config.xml，並且把內容放在doc
            //讀取node內的內容
            XmlNode node_date = doc.SelectSingleNode("Root/Date"); // 最近一次程式執行時的日期
            XmlNode node_counts = doc.SelectSingleNode("Root/Counts"); // 最近一次程式執行時的日期中，產生的工單筆數
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
        public void initial_txtTextBox_line4()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"config\txt_config.xml");
            XmlNode node_date = doc.SelectSingleNode("Root/Date");
            XmlNode node_counts = doc.SelectSingleNode("Root/Counts");
            XmlNode node_dia = doc.SelectSingleNode("Root/Dia");
            string[] dt = DateTime.Now.ToString().Split(' ', '/');
            string date = dt[1].PadLeft(2, '0') + dt[2].PadLeft(2, '0');
            string count = node_counts.InnerText.PadLeft(3, '0');
            string dia = node_dia.InnerText;
            txtBox_line4.Text = date + count + ' ' + dia;
        }
        private void create_txt(List<string> fnames, string path)
        {
            create_folder(path);
            string lasermark = txtBox_line4.Text;
            string[] lm = lasermark.Split(' ');
            string date = lm[0].Substring(0, 4);
            int count = Int16.Parse(lm[0].Substring(4, 3));
            string dia = lm[1];
            for (int k = 0; k < fnames.Count; k++)
            {
                string counts = (count + k + 1).ToString().PadLeft(3, '0');
                string fileName = String.Format("{0}.txt", fnames[k]);
                using (StreamWriter sw = File.CreateText(path + @"\" + fileName))
                {
                    sw.WriteLine(txtBox_line1.Text);
                    sw.WriteLine("");
                    sw.WriteLine("");
                    sw.WriteLine(date + counts + ' ' + dia);
                    sw.WriteLine(txtBox_line5.Text);
                }
            }
        }
        public void update_txtconf(List<string> strs)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"config\txt_config.xml");
            XmlNode node_date = doc.SelectSingleNode("Root/Date");
            string dt = DateTime.Now.ToString();
            string[] dt_arr = dt.Split(' ');
            node_date.InnerText = dt_arr[0];
            XmlNode node_counts = doc.SelectSingleNode("Root/Counts");
            node_counts.InnerText = get_counts(strs);
            doc.Save(@"config\txt_config.xml");
        }
        private void create_dat(List<string> fnames, string path)
        {
            create_folder(path);
            for (int k = 0; k < fnames.Count; k++)
            {
                string fileName = String.Format("{0}.dat", fnames[k]);
                using (StreamWriter sw = File.CreateText(path + @"\" + fileName))
                {
                    write_Order_Material_Measure(fnames[k], sw);
                    for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                    {
                        for (int j = 0; j < 1; j++)
                        {
                            if (dataText(i, j).Contains("["))
                            {
                                sw.WriteLine("");
                                sw.WriteLine(dataText(i, j));
                                sw.WriteLine(space + dataText(i, j + 1) + ": " + dataText(i, j + 2));
                            }
                            else if (!dataText(i, j).Contains("[") & dataText(i, j + 1) != "")
                            {
                                sw.WriteLine(dataText(i, j) + dataText(i, j + 1) + ": " + dataText(i, j + 2));
                            }
                            else
                            {
                                sw.WriteLine("");
                            }
                        }
                    }
                }
            }
        }
        private void write_Order_Material_Measure(string filename, StreamWriter sw)
        {
            //sw.WriteLine("");
            sw.WriteLine("[Order]");
            sw.WriteLine(space + "Model Code: " + String.Format(cbBox_Model.Text.Substring(0, 3)));
            sw.WriteLine(space + "Lot Number: " + filename);
            //sw.WriteLine(space + "SO Number: ");
            sw.WriteLine("");
            sw.WriteLine("[Material]");
            sw.WriteLine(space + "Material Code: " + String.Format(cbBox_Material.Text.Substring(0, 2)));
            sw.WriteLine("");
            sw.WriteLine("[Measure]");
            sw.WriteLine(space + "Measure Code: " + cbBox_Measure.Text);
        }
        private string dataText(int i, int j)
        {
            string str = dataGridView1.Rows[i].Cells[j].Value.ToString();
            if (j == 0 & !str.Contains("["))
            {
                str = new String(' ', 6);
            }
            if (j == 2 & str == "")
            {
                str = "0.00";
            }
            return str;
        }
        private List<string> create_fileName()
        {
            Config conf = new Config(@"config\config.xml");
            int create_times = Convert.ToInt16(numericUpDown_times.Value);
            string type = cbBox_fileType.Text;
            List<string> fName = new List<string>();
            for (int i = conf.current_count + 1; i < conf.current_count + create_times + 1; i++)
            {
                string no = i.ToString();
                string streamNums = no.PadLeft(4, '0');
                string f_name = type + "-" + conf.Date + streamNums;
                fName.Add(f_name);
            }
            return fName;
        }
        public void update_conf(List<string> strs)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"config\config.xml");
            XmlNode node_date = doc.SelectSingleNode("Root/Date");
            string dt = DateTime.Now.ToString();
            string[] dt_arr = dt.Split(' ');
            node_date.InnerText = dt_arr[0];
            XmlNode node_counts = doc.SelectSingleNode("Root/Counts");
            node_counts.InnerText = get_counts(strs);
            doc.Save(@"config\config.xml");
        }
        public string get_counts(List<string> xxx)
        {
            string f = xxx[xxx.Count - 1].Substring(11, 4);
            string c = int.Parse(f).ToString();
            return c;
        }
        public List<string> get_Modelconf()
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(@"config\model_config.xml");
            XmlNodeList nodes = xdoc.SelectNodes("//Root/Models/Model");
            List<string> models = new List<string>();
            foreach (XmlNode node in nodes)
            {
                string code = node["Code"].InnerText;
                string name = node["Name"].InnerText;
                string model_code = code + ": " + name;
                models.Add(model_code);
            }
            return models;
        }
        public List<string> get_CSVconf()
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(@"config\csv_config.xml");
            XmlNodeList nodes = xdoc.SelectNodes("//Root/Designs/Design");
            List<string> label_value = new List<string>();
            foreach (XmlNode node in nodes)
            {
                string name = node["Name"].InnerText;
                label_value.Add(name);
            }
            return label_value;
        }
        public void initial_csv()
        {
            List<string> csv = get_CSVconf();
            for (int i = 0; i < csv.Count; i++)
            {
                cbBox_import_csv.Items.Add(csv[i]);
            }
            cbBox_import_csv.Text = csv[0];
            dataGridView1.DataSource = ConvertCSVtoDataTable(String.Format(@"csv\{0}.csv", csv[0]));
            DataGridViewColumn column = dataGridView1.Columns[1];
            column.Width = 150;
        }
        public List<string> get_Materialconf()
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(@"config\material_config.xml");
            XmlNodeList nodes = xdoc.SelectNodes("//Root/Materials/Material");
            List<string> models = new List<string>();
            foreach (XmlNode node in nodes)
            {
                string code = node["Code"].InnerText;
                string name = node["Name"].InnerText;
                string model_code = code + ": " + name;
                models.Add(model_code);
            }
            return models;
        }
        public void initial_materials()
        {
            List<string> materials = get_Materialconf();
            for (int i = 0; i < materials.Count; i++)
            {
                cbBox_Material.Items.Add(materials[i]);
            }
            cbBox_Material.Text = materials[1];
        }
        public List<string> get_Measureconf()
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(@"config\measure_config.xml");
            XmlNodeList nodes = xdoc.SelectNodes("//Root/Measures/Measure");
            List<string> measures = new List<string>();
            foreach (XmlNode node in nodes)
            {
                string code = node["Code"].InnerText;
                measures.Add(code);
            }
            return measures;
        }
        public void initial_measures()
        {
            List<string> measures = get_Measureconf();
            for (int i = 0; i < measures.Count; i++)
            {
                cbBox_Measure.Items.Add(measures[i]);
            }
            cbBox_Measure.Text = measures[0];
        }
        public List<string> get_fileTitleconf()
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(@"config\fileTitle_config.xml");
            XmlNodeList nodes = xdoc.SelectNodes("//Root/Titles/Title");
            List<string> titles = new List<string>();
            foreach (XmlNode node in nodes)
            {
                string code = node["Code"].InnerText;
                titles.Add(code);
            }
            return titles;
        }
        public void initial_fileTitle()
        {
            List<string> titles = get_fileTitleconf();
            for (int i = 0; i < titles.Count; i++)
            {
                cbBox_fileType.Items.Add(titles[i]);
            }
            cbBox_fileType.Text = titles[0];
        }
        private void btn_import_csv_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = ConvertCSVtoDataTable(@String.Format(@"csv\{0}.csv", cbBox_import_csv.Text));
            DataGridViewColumn column = dataGridView1.Columns[1];
            column.Width = 190;
        }
        public void initial_models()
        {
            List<string> models = get_Modelconf();
            for (int i = 0; i < models.Count; i++)
            {
                cbBox_Model.Items.Add(models[i]);
            }
            cbBox_Model.Text = models[1];
        }
        public void initial_conf()
        {
            XmlDocument doc = new XmlDocument(); // 建立xml檔的物件
            doc.Load(@"config\config.xml"); // 讀取config.xml，並且把內容放在doc
            //讀取node內的內容
            XmlNode node_date = doc.SelectSingleNode("Root/Date"); // 最近一次程式執行時的日期
            XmlNode node_counts = doc.SelectSingleNode("Root/Counts"); // 最近一次程式執行時的日期中，產生的工單筆數
            XmlNode node_savePath = doc.SelectSingleNode("Root/SavePath"); // 工單產生後的儲存路徑
            string dt = DateTime.Now.ToString(); //將程式執行時的日期、時間存成字串
            string[] dt_arr = dt.Split(' '); //將日期與時間分別存成字串的陣列，dt_arr[0]:日期、dt_arr[1]:時間
            if (!node_date.InnerText.Equals(dt_arr[0]))
            {
                // 如果 "Root/Date"的內容不是dt_arr[0]
                node_counts.InnerText = "0"; // 把"Root/Counts"的內容歸零
                node_date.InnerText = dt_arr[0]; // 程式執行時的日期填入"Root/Date"
            }
            txtBox_savePath.Text = node_savePath.InnerText; //儲存路徑的textbox
            doc.Save(@"config\config.xml"); // 儲存至config.xml
        }
        private void btn_savePath_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtBox_savePath.Text = folderBrowserDialog1.SelectedPath;
            }
        }
        public DataTable ConvertCSVtoDataTable(string strFilePath)
        {
            DataTable dt = new DataTable();
            using (StreamReader sr = new StreamReader(strFilePath))
            {
                string[] headers = sr.ReadLine().Split(',');
                foreach (string header in headers)
                {
                    dt.Columns.Add(header);
                }
                while (!sr.EndOfStream)
                {
                    string[] rows = sr.ReadLine().Split(',');
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        dr[i] = rows[i];
                    }
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }
    }
}
