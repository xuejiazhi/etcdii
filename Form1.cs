using System;
using System.Drawing;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;


namespace etcdii
{
    public enum Operate
    {
        Version,
        AllKeys,
    }


    public partial class Form1 : Form
    {
        public static string GLOBAL_ENDPOINT;
        public static bool GLOBAL_CONNECT_STATUS;

        //ETCD Struct Version
        public struct EtcdVersion
        {
            public string etcdserver { get; set; }
            public string etcdcluster { get; set; }
        }

        public struct EtcdKvRange
        {
            public string count { get; set; }
            public EtcdHeader header { get; set; }
            public EtcdKvs[] kvs { get; set; }
        }

        public struct EtcdHeader
        {
            public string cluster_id { get; set; }
            public string member_id { get; set; }
            public string revision { get; set; }
            public string raft_term { get; set; }
        }

        public struct EtcdKvs
        {
            public string key { get; set; }
            public string create_revision { get; set; }
            public string mod_revision { get; set; }
            public string version { get; set; }
            public string value { get; set; }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                GLOBAL_ENDPOINT = textBox1.Text;
            }
            else
            {
                GLOBAL_ENDPOINT = "127.0.0.1:2379";
            }

            var ver = getEtcdValue(Operate.Version);
            if (ver != "")
            {
                try
                {
                    GLOBAL_CONNECT_STATUS = true;
                    listBox1.Items.Clear();
                   //set label2
                   label2.Text = "Connect Success";
                    label2.ForeColor = Color.Green;
                    //set label3
                    EtcdVersion jss = JsonSerializer.Deserialize<EtcdVersion>(ver);
                    label3.Text = "etcd server version:" + jss.etcdserver;
                    label3.ForeColor = Color.Green;
                    //set list
                    var listStr = postEtcdValue(Operate.AllKeys, "{\"key\": \"AA==\",\"range_end\": \"AA==\"}");
                    EtcdKvRange etcdKvRange = JsonSerializer.Deserialize<EtcdKvRange>(listStr);
                    if (etcdKvRange.kvs.Length > 0)
                    {
                        foreach (var item in etcdKvRange.kvs)
                        {
                            listBox1.Items.Add(base64Decode(item.key));
                        }
                    }
                }
                catch (Exception ex)
                {
                    label2.Text = "Connect Failed";
                    label2.ForeColor = Color.Red;
                }
            }
            else
            {
                label2.Text = "Connect Failed";
                label2.ForeColor = Color.Red;
            }

            //textBox2.Text = getEtcdValue();
        }

        static string getEtcdValue(Operate o)
        {
            HttpClient httpClient = new HttpClient();
            string urlStr = "";
            switch (o)
            {
                case Operate.Version:
                    urlStr = String.Format("http://{0}/version", GLOBAL_ENDPOINT);
                    break;
                default:
                    break;
            }


            try
            {
                var response = httpClient.GetStringAsync(urlStr).Result;

                return response.ToString();
            }
            catch (Exception ex)
            {
                return "";
            }


        }

        static string postEtcdValue(Operate o, string data)
        {
            HttpClient httpClient = new HttpClient();
            HttpContent content = new StringContent(data);
            string urlStr = "";
            switch (o)
            {
                case Operate.AllKeys:
                    urlStr = String.Format("http://{0}/v3/kv/range", GLOBAL_ENDPOINT);
                    break;
                default:
                    break;
            }

            try
            {
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = httpClient.PostAsync(urlStr, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    return result;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "";
            }
            return "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GLOBAL_CONNECT_STATUS = false;
            textBox1.Text = "";
            label2.Text = "DisConnect";
            label3.Text = "";
            label2.ForeColor = Color.Red;
            listBox1.Items.Clear();
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
        }

        private string base64Encode(string code)
        {
            byte[] bytes = Encoding.Default.GetBytes(code);
            string str = Convert.ToBase64String(bytes);
            return str;
        }

        private string base64Decode(string code)
        {
            byte[] outputb = Convert.FromBase64String(code);
            string str = Encoding.Default.GetString(outputb);
            return str;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var selectMsg = base64Encode(listBox1.Text.ToString());
                //set list
                var listStr = postEtcdValue(Operate.AllKeys, "{\"key\":\"" + selectMsg + "\"}");
                EtcdKvRange etcdKvRange = JsonSerializer.Deserialize<EtcdKvRange>(listStr);
                if (etcdKvRange.kvs.Length > 0)
                {
                    foreach (var item in etcdKvRange.kvs)
                    {
                        textBox2.Text = base64Decode(item.value);
                        textBox3.Text = item.create_revision;
                        textBox4.Text = item.mod_revision;
                        textBox5.Text = item.version;
                    }
                }
            }catch(Exception ex)
            {

            }
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (GLOBAL_CONNECT_STATUS == true)
            {
                Form2 childForm = new Form2(GLOBAL_ENDPOINT);
                childForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("No Connect");
            }
         
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }
    }
}
