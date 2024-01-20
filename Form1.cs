using System;
using System.Drawing;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;


namespace etcdii
{

    public partial class Form1 : Form
    {
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
                httpclient.GLOBAL_ENDPOINT = textBox1.Text;
            }
            else
            {
                httpclient.GLOBAL_ENDPOINT = "127.0.0.1:2379";
            }

            var ver = httpclient.GetEtcdValue(Operate.Version);
            if (ver != "")
            {
                try
                {
                    httpclient.GLOBAL_CONNECT_STATUS = true;
                    listBox1.Items.Clear();
                   //set label2
                   label2.Text = "Connect Success";
                    label2.ForeColor = Color.Green;
                    //set label3
                    EtcdVersion jss = JsonSerializer.Deserialize<EtcdVersion>(ver);
                    label3.Text = "etcd server version:" + jss.etcdserver;
                    label3.ForeColor = Color.Green;
                    //set list
                    var listStr = httpclient.PostEtcdValue(Operate.AllKeys, "{\"key\": \"AA==\",\"range_end\": \"AA==\"}");
                    EtcdKvRange etcdKvRange = JsonSerializer.Deserialize<EtcdKvRange>(listStr);
                    if (etcdKvRange.kvs.Length > 0)
                    {
                        foreach (var item in etcdKvRange.kvs)
                        {
                            listBox1.Items.Add(util.base64Decode(item.key));
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

       

        

        private void button2_Click(object sender, EventArgs e)
        {
            httpclient.GLOBAL_CONNECT_STATUS = false;
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

    
        

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var selectMsg = util.base64Encode(listBox1.Text.ToString());
                //set list
                var listStr = httpclient.PostEtcdValue(Operate.AllKeys, "{\"key\":\"" + selectMsg + "\"}");
                EtcdKvRange etcdKvRange = JsonSerializer.Deserialize<EtcdKvRange>(listStr);
                if (etcdKvRange.kvs.Length > 0)
                {
                    foreach (var item in etcdKvRange.kvs)
                    {
                        textBox2.Text = util.base64Decode(item.value);
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
            if (httpclient.GLOBAL_CONNECT_STATUS == true)
            {
                Form2 childForm = new Form2(httpclient.GLOBAL_ENDPOINT);
                childForm.AddKeys += ChildForm_Addkey;
                childForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("No Connect");
            }
         
        }

        private void ChildForm_Addkey(object sender, bool data)
        {
            listBox1.Items.Clear();
            //set list
            var listStr = httpclient.PostEtcdValue(Operate.AllKeys, "{\"key\": \"AA==\",\"range_end\": \"AA==\"}");
            EtcdKvRange etcdKvRange = JsonSerializer.Deserialize<EtcdKvRange>(listStr);
            if (etcdKvRange.kvs.Length > 0)
            {
                foreach (var item in etcdKvRange.kvs)
                {
                    listBox1.Items.Add(util.base64Decode(item.key));
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }
    }
}
