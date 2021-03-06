using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace ReactNativeCmdEase
{
    public partial class Form2 : Form
    {
        public Form2(bool IsDarkTheme)
        {
            IsDark = IsDarkTheme;
            InitializeComponent();
        }

        bool IsDark = true;
        string jhome = "";
        string keytoolpath = "";
        readonly byte[] pb = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(Application.ProductName+Application.UserAppDataPath));
        string bgpath = Path.Combine(Form1.projectdir, @"android\app\build.gradle");

        private void Form2_Load(object sender, EventArgs e)
        {
            jhome = System.Environment.GetEnvironmentVariable("JAVA_HOME", EnvironmentVariableTarget.User);
            if (jhome == "") jhome = System.Environment.GetEnvironmentVariable("JAVA_HOME", EnvironmentVariableTarget.Machine);

            textBox7.Text = @"%JAVA_HOME%\bin\keytool.exe";
            keytoolpath = Path.Combine(jhome, @"bin\keytool.exe");
            timer1.Enabled = true;

            if (IsDark)
                activateDarkMood();
            else
                activateLightMood();

        }

        private void activateLightMood()
        {
            foreach (Control c in this.Controls)
            {
                if (c.Name != "pictureBox1")
                {
                    c.ForeColor = Color.Black;
                    c.BackColor = SystemColors.Control;
                }
            }
            this.BackColor = SystemColors.Control;
            this.ForeColor = Color.Black;
            //this.listView1.BackColor = Color.White;
        }

        private void activateDarkMood()
        {
            foreach (Control c in this.Controls)
            {
                if (c.Name != "pictureBox1")
                {
                    c.ForeColor = Color.White;
                    c.BackColor = Color.Black;
                }
            }
            this.BackColor = Color.Black;
            this.ForeColor = Color.White;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if(File.Exists(keytoolpath))
            {
                label2.Text = "OK";
                label2.ForeColor = Color.Green;
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                textBox3.Enabled = true;
                textBox4.Enabled = true;
                textBox5.Enabled = true;
                textBox6.Enabled = true;
                textBox8.Enabled = true;
                textBox9.Enabled = true;
                textBox10.Enabled = true;
                button1.Enabled = true;
            }
            else
            {
                label2.Text = "Not Exist";
                label2.ForeColor = Color.Red;
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                textBox3.Enabled = false;
                textBox4.Enabled = false;
                textBox5.Enabled = false;
                textBox6.Enabled = false;
                textBox8.Enabled = false;
                textBox9.Enabled = false;
                textBox10.Enabled = false;
                button1.Enabled = false;
            }

            string keystorepath = Path.Combine(Form1.projectdir, @"android\app", textBox1.Text);
            int i = 1;
            if (File.Exists(keystorepath))
            {
                do
                {
                    i++;
                    string fn = Path.GetFileNameWithoutExtension(textBox1.Text);
                    if (fn.Contains("-"))
                    {
                        string[] arr = fn.Split(new[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                        int l = arr.Length;
                        fn = "";
                        for (int j=0;j<l-1;j++)
                        {
                            fn += arr[j] + "-";
                        }
                        if (int.TryParse(arr[l - 1], out int n))
                            fn = fn.Substring(0, fn.Length - 1);
                        else
                            fn += arr[l - 1];

                    }
                    string ex = Path.GetExtension(textBox1.Text);
                    keystorepath = Path.Combine(Form1.projectdir, @"android\app", fn + "-" + i + ex);
                    if(!File.Exists(keystorepath))
                        textBox1.Text = fn + "-" + i + ex;
                } while (File.Exists(keystorepath));
            }

            //string[] files = Directory.GetFiles(Path.Combine(Form1.projectdir, @"android\app"),"*.keystore",SearchOption.TopDirectoryOnly);
            FileInfo[] files = new DirectoryInfo(Path.Combine(Form1.projectdir, @"android\app"))
                        .GetFiles("*.keystore")
                        .OrderBy(f => f.CreationTime)
                        .ToArray();
            if (listBox1.Items.Count != files.Length)
            {
                listBox1.Items.Clear();
                foreach (FileInfo file in files)
                {
                    listBox1.Items.Add(file.Name);
                }
                if (listBox1.Items.Count > 0)
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;


            }

            if (File.Exists(bgpath))
            {
                bool contains = false;
                try
                {
                    contains = File.ReadAllText(bgpath).Contains(listBox1.SelectedItem.ToString());
                }
                catch
                {

                }
                if (!contains && textBox11.Enabled)
                {
                    button2.Enabled = true;
                }
                else if(contains && textBox11.Enabled)
                {
                    button2.Enabled = false;
                }
                else
                {
                    button2.Enabled = false;
                }
            }
        }

        private void TextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void Label5_Click(object sender, EventArgs e)
        {

        }

        private void TextBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            string dname = "\"" + "cn=" + textBox6.Text.Replace("\"","") + ", ou=" + textBox8.Text.Replace("\"", "") + ", o=" + textBox9.Text.Replace("\"", "")
                + ", c=" + textBox10.Text.Replace("\"", "") + "\"";
            string keystorepath = Path.Combine(Form1.projectdir, @"android\app", textBox1.Text);

            string args = "/c \"echo y | \"" + keytoolpath + "\" -genkeypair -dname " + dname + " -alias \""
                + textBox3.Text + "\" -keypass \"" + textBox4.Text + "\" -keystore \"" + keystorepath + 
                "\" -storepass \"" + textBox2.Text + "\" -keyalg RSA -keysize 2048 -validity " + 
                textBox5.Text + " && echo Successfully generated new keystore file. Now select the created release keystore from the below list and do the post actions. That is.\"";

            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = args;
            cmd.StartInfo.WorkingDirectory = Path.Combine(Form1.projectdir, @"android\app");
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.RedirectStandardError = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.Start();
            string output = cmd.StandardOutput.ReadToEnd();
            cmd.WaitForExit();

            string pws = textBox1.Text + Environment.NewLine + textBox2.Text + Environment.NewLine +
                textBox3.Text + Environment.NewLine + textBox4.Text;
            byte[] outputb = AES_Encrypt(Encoding.UTF8.GetBytes(pws), pb);
            if (File.Exists(keystorepath + ".rnce")) File.Delete(keystorepath + ".rnce");
            File.WriteAllBytes(keystorepath + ".rnce", outputb);
            timer1.Enabled = true;

            MessageBox.Show(output);
        }

        public byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;

            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        public byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;

            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }

        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string filename = Path.Combine(Form1.projectdir, @"android\app", listBox1.SelectedItem.ToString());
                if (File.Exists(filename) && File.Exists(filename + ".rnce"))
                {
                    string dcrp = Encoding.UTF8.GetString(AES_Decrypt(File.ReadAllBytes(filename + ".rnce"), pb));
                    string[] arr = dcrp.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    if (arr.Length != 4) throw new System.InvalidOperationException("File decrypting failed.");
                    textBox11.Text = "release {" + Environment.NewLine +
                    "\tstoreFile file('" + arr[0] + "')" + Environment.NewLine +
                    "\tstorePassword '" + arr[1] + "'" + Environment.NewLine +
                    "\tkeyAlias '" + arr[2] + "'" + Environment.NewLine +
                    "\tkeyPassword '" + arr[3] + "'" + Environment.NewLine + "}";
                    //button2.Enabled = true;
                    button3.Enabled = true;
                    textBox11.Enabled = true;
                    label16.Visible = true;
                }
                else
                {
                    textBox11.Text = "";
                    //button2.Enabled = false;
                    button3.Enabled = false;
                    textBox11.Enabled = false;
                    label16.Visible = false;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                textBox11.Text = "";
                //button2.Enabled = false;
                button3.Enabled = false;
                textBox11.Enabled = false;
                label16.Visible = false;
                return;
            }
        }

        private void TextBox11_TextChanged(object sender, EventArgs e)
        {

        }

        private void TextBox11_Clicked(object sender, EventArgs e)
        {
            textBox11.SelectAll();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            try
            {
                string txt = File.ReadAllText(bgpath);
                if (!File.Exists(bgpath + ".backup")) File.WriteAllText(bgpath + ".backup", txt);
                //rechecking because 100ms timer interval
                bool contains = txt.Contains(textBox11.Text);
                if (!contains)
                {
                    string filename = Path.Combine(Form1.projectdir, @"android\app", listBox1.SelectedItem.ToString() + ".rnce");
                    string dcrp = Encoding.UTF8.GetString(AES_Decrypt(File.ReadAllBytes(filename), pb));
                    string[] arr = dcrp.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    if (arr.Length != 4) throw new System.InvalidOperationException("File decrypting failed.");

                    GradleConfig gc = new GradleConfig(bgpath);
                    GradleNode g = gc.ROOT.TryGetNode("android/signingConfigs/release");
                    
                    if(g == null)
                    {
                        GradleNode sc = gc.ROOT.FindChildNodeByName("android").FindChildNodeByName("signingConfigs");
                        GradleNode r = new GradleNode("release");
                        sc.AppendChildNode(r);
                        r.ReplaceContenOrAddStartsWith("storeFile", "storeFile file('" + arr[0] +"')");
                        r.ReplaceContenOrAddStartsWith("storePassword", "storePassword '" + arr[1] + "'");
                        r.ReplaceContenOrAddStartsWith("keyAlias", "keyAlias '" + arr[2] + "'");
                        r.ReplaceContenOrAddStartsWith("keyPassword", "keyPassword '" + arr[3] + "'");
                    }
                    else
                    {
                        g.ReplaceContenOrAddStartsWith("storeFile", "storeFile file('" + arr[0] + "')");
                        g.ReplaceContenOrAddStartsWith("storePassword", "storePassword '" + arr[1] + "'");
                        g.ReplaceContenOrAddStartsWith("keyAlias", "keyAlias '" + arr[2] + "'");
                        g.ReplaceContenOrAddStartsWith("keyPassword", "keyPassword '" + arr[3] + "'");
                    }

                    GradleNode g2 = gc.ROOT.TryGetNode("android/buildTypes/release");
                    if(g2 != null)
                    {
                        g2.ReplaceContenOrAddStartsWith("signingConfig", "signingConfig signingConfigs.release");

                    }

                    gc.Save();
                    MessageBox.Show("Successfully wrote into build.gradle file.");
                }
                else
                {
                    MessageBox.Show("build.gradle file contains the above text.");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            Process.Start(bgpath);
        }
    }

    
}
