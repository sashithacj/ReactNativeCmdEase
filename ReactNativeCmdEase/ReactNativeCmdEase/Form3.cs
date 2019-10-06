using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ReactNativeCmdEase
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        string bgpath = Path.Combine(Form1.projectdir, @"android\app\build.gradle");
        string packagejson = Path.Combine(Form1.projectdir, @"package.json");
        string appjson = Path.Combine(Form1.projectdir, @"app.json");
        dynamic json = null;
        dynamic json2 = null;
        GradleConfig gc = null;

        private void Form3_Load(object sender, EventArgs e)
        {
            if (File.Exists(packagejson) && File.Exists(appjson))
            {
                try
                {
                    json = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(packagejson));
                    json2 = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(appjson));
                    textBox7.Text = json2["displayName"];

                    gc = new GradleConfig(bgpath);

                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
