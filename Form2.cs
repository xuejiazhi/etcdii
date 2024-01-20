using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace etcdii
{
    public partial class Form2 : Form
    {
        public event EventHandler<bool> AddKeys;

        public Form2()
        {
            InitializeComponent();
        }

        public Form2(string text)
        {
            InitializeComponent();
            try
            {
                label1.Text = text;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //在线
                if (httpclient.GLOBAL_CONNECT_STATUS == true)
                {
                    var key = util.base64Encode(textBox1.Text.ToString());
                    var value = util.base64Encode(textBox2.Text.ToString());
                    string putValue = "{\"key\":\""+key+"\",\"value\":\""+value+"\"}";
                    httpclient.PostEtcdValue(Operate.PutKey, putValue);
                    //
                    AddKeys(this, true);
                    //set list

                    MessageBox.Show("Add Key Success");
                    textBox1.Text = "";
                    textBox2.Text = "";
                }
                else
                {
                    MessageBox.Show("No Connect");
                }
            }
            catch(Exception ex)
            {

            }
        }
    }
}
