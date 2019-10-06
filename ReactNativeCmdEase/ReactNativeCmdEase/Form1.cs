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
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Net;

namespace ReactNativeCmdEase
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string jhome, ahome, emulatorp;
        public static string projectdir;

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!File.Exists(Path.Combine(Application.StartupPath, "Newtonsoft.Json.dll")))
                File.WriteAllBytes(Path.Combine(Application.StartupPath, "Newtonsoft.Json.dll"), Properties.Resources.Newtonsoft_Json);

            textBox1.Text = Application.StartupPath;
            projectdir = Application.StartupPath;

            jhome = System.Environment.GetEnvironmentVariable("JAVA_HOME", EnvironmentVariableTarget.User);
            if(jhome == "") jhome = System.Environment.GetEnvironmentVariable("JAVA_HOME", EnvironmentVariableTarget.Machine);
            textBox4.Text = jhome;
            ahome = System.Environment.GetEnvironmentVariable("ANDROID_HOME", EnvironmentVariableTarget.User);
            if (ahome == "") ahome = System.Environment.GetEnvironmentVariable("ANDROID_HOME", EnvironmentVariableTarget.Machine);
            textBox5.Text = ahome;
            emulatorp = Path.Combine(ahome, @"tools\emulator.exe");
            string originalPathEnv = Environment.GetEnvironmentVariable("PATH");
            string[] paths = originalPathEnv.Split(new char[1] { Path.PathSeparator }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in paths)
            {
                string pathEnv = Environment.ExpandEnvironmentVariables(s);
                if (pathEnv.Contains("Java"))
                    textBox7.Text = pathEnv;
            }

            checkDirectory(textBox1.Text);
            timer1.Enabled = true;
        }

        private void parseData(string output)
        {
            if (output.Contains("nodex") && output.Contains("npmx") && output.Contains("yarnx") &&
                output.Contains("reactx"))
            {
                int p1 = output.IndexOf("nodex");
                int p2 = output.IndexOf("npmx");
                int p3 = output.IndexOf("yarnx");
                int p4 = output.IndexOf("reactx");

                string s1 = output.Substring(p1, p2 - p1);
                Regex pattern = new Regex(@"\d+(\.\d+)+");
                var match = pattern.Match(s1);
                if (match.Success)
                    label11.Text = "Installed [v" + match.Value + "]";
                else
                    label11.Text = "Not Installed";

                string s2 = output.Substring(p2, p3 - p2);
                var match2 = pattern.Match(s2);
                if (match2.Success)
                    label12.Text = "Installed [v" + match2.Value + "]";
                else
                    label12.Text = "Not Installed";

                string s3 = output.Substring(p3, p4 - p3);
                var match3 = pattern.Match(s3);
                if (match3.Success)
                    label13.Text = "Installed [v" + match3.Value + "]";
                else
                    label13.Text = "Not Installed";

                string s4 = output.Substring(p4);
                var match4 = pattern.Match(s4);
                if (match4.Success)
                    label14.Text = "Installed [v" + match4.Value + "]";
                else
                    label14.Text = "Not Installed";
            }
            else
            {
                label11.Text = "Error occured when checking";
                label12.Text = "Error occured when checking";
                label13.Text = "Error occured when checking";
                label14.Text = "Error occured when checking";
            }
        }

        private void checkDirectory(string dpath)
        {
            textBox1.Text = dpath;
            projectdir = dpath;
            string jsonpath = Path.Combine(dpath, "package.json");
            bool errors = false;
            if (File.Exists(jsonpath))
            {
                try
                {
                    var json = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(jsonpath));
                    pictureBox1.BackColor = Color.Green;
                    label3.Text = "Found";
                    button2.Enabled = false;
                    textBox2.Enabled = false;
                    textBox3.Enabled = false;
                    textBox2.BackColor = SystemColors.Control;
                    textBox3.BackColor = SystemColors.Control;
                    textBox2.Text = json["name"];
                    textBox3.Text = json["dependencies"]["react-native"];
                    
                    button5.Enabled = true;
                    button7.Enabled = true;
                    button3.Enabled = true;
                    button4.Enabled = true;
                    button6.Enabled = true;
                    button8.Enabled = true;
                    //button9.Enabled = true;
                    //button10.Enabled = true;
                    //button13.Enabled = true;
                    //button24.Enabled = true;
                    button15.Enabled = true;
                    button16.Enabled = true;
                    button26.Enabled = true;

                    comboBox3.Enabled = true;

                    comboBox3.Items.Clear();
                    comboBox3.Text = "react-native";
                    foreach (JProperty x in json["dependencies"])
                    {
                        //if((string)x.Name != "react" && (string)x.Name != "react-native")
                        comboBox3.Items.Add((string)x.Name);
                    }

                    textBox6.Text = "";
                    if (File.Exists(Path.Combine(dpath, "yarn.lock")))
                    {
                        textBox6.Text = "Yarn";
                        
                    }

                    if(File.Exists(Path.Combine(dpath, "package-lock.json")))
                    {
                        if(textBox6.Text == "")
                            textBox6.Text = "Npm";
                        else
                            textBox6.Text = textBox6.Text + " + Npm";

                    }
                    
                    if(textBox6.Text == "")
                    {
                        textBox6.Text = "[unknown]";
                    }
                }
                catch
                {
                    errors = true;
                }
            }

            if(!File.Exists(jsonpath) || errors)
            {
                pictureBox1.BackColor = Color.Red;
                label3.Text = "Not Found";
                button2.Enabled = true;
                textBox2.Enabled = true;
                textBox3.Enabled = true;
                textBox2.BackColor = SystemColors.Window;
                //textBox3.BackColor = SystemColors.Window;
                //Forcing users to start with latest version
                textBox3.BackColor = SystemColors.Control;
                textBox2.Text = "";
                button3.Enabled = false;
                button4.Enabled = false;
                button5.Enabled = false;
                button6.Enabled = false;
                button7.Enabled = false;
                button8.Enabled = false;
                //button13.Enabled = false;
                button24.Enabled = false;
                button15.Enabled = false;
                button16.Enabled = false;
                comboBox3.Enabled = false;
                button26.Enabled = false;
                //button9.Enabled = false;
                //button10.Enabled = false;
                textBox3.Text = "[latest]";
                textBox2.Focus();
            }
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            textBox1.Text = folderBrowserDialog1.SelectedPath;
            checkDirectory(textBox1.Text);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if(textBox2.Text.Length != 0)
            {
                Process cmd = new Process();
                cmd.StartInfo.FileName = "cmd.exe";
                string ar;
                if (textBox3.Text == "[latest]" || textBox3.Text.Length == 0)
                    ar = "/c react-native init " + textBox2.Text;
                else
                    ar = "/c react-native init " + textBox2.Text + " --version=\"" + textBox3.Text + "\"";
                cmd.StartInfo.Arguments = ar;
                cmd.StartInfo.WorkingDirectory = textBox1.Text;
                cmd.StartInfo.UseShellExecute = false;
                cmd.Start();
                cmd.WaitForExit();
                string newdir = Path.Combine(textBox1.Text, textBox2.Text);
                if (Directory.Exists(newdir))
                {
                    string newexe = Path.Combine(newdir, Path.GetFileName(Application.ExecutablePath));
                    File.Copy(Application.ExecutablePath, newexe, true);
                    if (File.Exists(newexe))
                    {
                        Process p = new Process();
                        p.StartInfo.FileName = newexe;
                        p.StartInfo.UseShellExecute = false;
                        p.Start();
                        Application.Exit();
                    }
                    else
                    {
                        checkDirectory(newdir);
                    }
                    
                }
                else
                {
                    MessageBox.Show("An error occured while starting the project.");
                }
                
            }
            else
            {
                MessageBox.Show("A new project name required to init!");
            }
        }

        private void Label8_Click(object sender, EventArgs e)
        {

        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = "/c \"echo nodex & node -v & echo npmx & npm -v & echo yarnx & yarn -v & echo reactx & react-native -v\"";
            cmd.StartInfo.WorkingDirectory = textBox1.Text;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.RedirectStandardError = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();
            string output = cmd.StandardOutput.ReadToEnd();
            cmd.WaitForExit();
            parseData(output);
            button10.PerformClick();

            Process cmd2 = new Process();
            cmd2.StartInfo.FileName = "cmd.exe";
            cmd2.StartInfo.Arguments = "/c \"java -version\"";
            //cmd2.StartInfo.WorkingDirectory = textBox1.Text;
            cmd2.StartInfo.RedirectStandardOutput = true;
            cmd2.StartInfo.RedirectStandardError = true;
            cmd2.StartInfo.CreateNoWindow = true;
            cmd2.StartInfo.UseShellExecute = false;
            cmd2.Start();
            string output2 = cmd2.StandardError.ReadToEnd();
            cmd2.WaitForExit();

            Regex pattern = new Regex(@"\d+(\.\d+)+");
            var match = pattern.Match(output2);
            if (match.Success)
                label22.Text = "Installed [v" + match.Value + "]";
            else
                label22.Text = "Not Installed";

            string sdkman = Path.Combine(ahome, @"tools\bin\sdkmanager.bat");
            if (File.Exists(sdkman))
            {
                Process cmd3 = new Process();
                cmd3.StartInfo.FileName = "cmd.exe";
                cmd3.StartInfo.Arguments = "/c \"" + sdkman + "\" --version";
                cmd3.StartInfo.RedirectStandardOutput = true;
                cmd3.StartInfo.RedirectStandardError = true;
                cmd3.StartInfo.CreateNoWindow = true;
                cmd3.StartInfo.UseShellExecute = false;
                cmd3.Start();
                string output3 = cmd3.StandardOutput.ReadToEnd();
                cmd3.WaitForExit();
                var match2 = pattern.Match(output3);
                if (match2.Success)
                    label24.Text = "Installed [v" + match2.Value + "]";
                else
                    label24.Text = "Not Installed";
            }
            else
            {
                label24.Text = "Not Installed";
            }
        }

        private void Button6_Click(object sender, EventArgs e)
        {

        }

        private void Button5_Click(object sender, EventArgs e)
        {
            /*
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = "/c rm -rf node_modules";
            cmd.StartInfo.WorkingDirectory = textBox1.Text;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();
            //cmd.WaitForExit(); */
            Process.Start("cmd.exe", "/c rmdir /Q /S \"" + Path.Combine(textBox1.Text, "node_modules") + "\"");
        }

        private void Button6_Click_1(object sender, EventArgs e)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = "/c npm cache clean";
            cmd.StartInfo.WorkingDirectory = textBox1.Text;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();
            //cmd.WaitForExit();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = "/c npm install";
            cmd.StartInfo.WorkingDirectory = textBox1.Text;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();
            //cmd.WaitForExit();
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = "/c npm update";
            cmd.StartInfo.WorkingDirectory = textBox1.Text;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();
            //cmd.WaitForExit();
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = "/c yarn install --check-files";
            cmd.StartInfo.WorkingDirectory = textBox1.Text;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();
            //cmd.WaitForExit();
        }

        private void Button8_Click(object sender, EventArgs e)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = "/c gradlew clean";
            cmd.StartInfo.WorkingDirectory = Path.Combine(textBox1.Text, "android");
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();
            //cmd.WaitForExit();
        }

        private void Button9_Click(object sender, EventArgs e)
        {
            foreach (Process p in Process.GetProcessesByName("adb"))
            {
                try
                {
                    p.Kill();
                }
                catch(Exception ec)
                {
                    MessageBox.Show(ec.Message);
                }
                
            }
        }

        private void Button11_Click(object sender, EventArgs e)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = emulatorp;
            cmd.StartInfo.Arguments = "-avd " + comboBox1.Text + " -no-boot-anim -netspeed full -netdelay none";
            cmd.StartInfo.WorkingDirectory = textBox1.Text;
            cmd.StartInfo.UseShellExecute = true;
            cmd.Start();
        }

        private void Button12_Click(object sender, EventArgs e)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = emulatorp;
            cmd.StartInfo.Arguments = "-avd " + comboBox1.Text + " -wipe-data -no-boot-anim -netspeed full -netdelay none";
            cmd.StartInfo.WorkingDirectory = textBox1.Text;
            cmd.StartInfo.UseShellExecute = true;
            cmd.Start();
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Timer2_Tick(object sender, EventArgs e)
        {
            Process[] p = Process.GetProcessesByName("adb");
            Process[] em = Process.GetProcessesByName("emulator");
            Process[] n = Process.GetProcessesByName("node");
            Process[] j = Process.GetProcessesByName("java");
            if (p.Length > 0)
            {
                label20.Text = "Android emulators: ["+ p.Length+" adb running]";
                button9.Enabled = true;
            }
            else
            {
                label20.Text = "Android emulators: ";
                button9.Enabled = false;
            }
            if (em.Length > 0)
            {
                label20.Text = label20.Text + " [" + em.Length + " emulator running]";
                if (button8.Enabled)
                {
                    button13.Enabled = true;
                    button14.Enabled = true;
                }
                else
                {
                    button13.Enabled = false;
                    button14.Enabled = false;
                }
            }
            else
            {
                button13.Enabled = false;
                button14.Enabled = false;
            }

            if (n.Length > 0)
                label17.Text = "Project commands: [" + n.Length + " node running]";
            else
                label17.Text = "Project commands:";

            if (j.Length > 0)
                label17.Text = label17.Text + " [" + j.Length + " java running]";

            if (Directory.Exists(Path.Combine(textBox1.Text, "node_modules")) && button6.Enabled)
                button5.Enabled = true;
            else
                button5.Enabled = false;

        }

        private void Button13_Click(object sender, EventArgs e)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = "/c react-native run-android";
            cmd.StartInfo.WorkingDirectory = textBox1.Text;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();
            //cmd.WaitForExit();
        }

        private void Button14_Click(object sender, EventArgs e)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = "/c react-native run-android --variant=release";
            cmd.StartInfo.WorkingDirectory = textBox1.Text;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();
            //cmd.WaitForExit();
        }

        private void Button15_Click(object sender, EventArgs e)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = "/c gradlew bundleRelease";
            cmd.StartInfo.WorkingDirectory = Path.Combine(textBox1.Text, "android");
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();
            //cmd.WaitForExit();
        }

        private void Button16_Click(object sender, EventArgs e)
        {
            Process.Start(Path.Combine(textBox1.Text, @"android\app\build\outputs"));
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            if (Directory.Exists(textBox1.Text))
                button17.Enabled = true;
            else
                button17.Enabled = false;
        }

        private void Button17_Click(object sender, EventArgs e)
        {
            Process.Start(textBox1.Text);
        }

        private void ComboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            //validate();
        }

        private void ComboBox3_TextChanged(object sender, EventArgs e)
        {
            label29.ForeColor = Color.Black;
            label28.Text = "N/A";
            label30.Text = "N/A";
            label32.Text = "N/A";
            label34.Text = "N/A";
            label28.ForeColor = Color.Black;
            label28.Cursor = Cursors.Default;
            button22.Enabled = false;
            button23.Enabled = false;
            button18.Enabled = false;
            button19.Enabled = false;
            button20.Enabled = false;
            button21.Enabled = false;
            timer3.Enabled = false;
            timer3.Enabled = true;
        }

        private void validate()
        {
            string jsonpath = Path.Combine(textBox1.Text, "package.json");
            bool errors = false;
            if (File.Exists(jsonpath))
            {
                try
                {
                    var json = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(jsonpath));
                    bool packageExists = false;
                    foreach (JProperty x in json["dependencies"])
                    {
                        if ((string)x.Name == comboBox3.Text)
                            packageExists = true;
                    }

                    string version = "0.0.0";
                    if (packageExists)
                    {
                        button20.Enabled = true;
                        button21.Enabled = true;
                        version = json["dependencies"][comboBox3.Text];
                        version = version.Replace("^", "");
                        label32.Text = version;
                        label34.Text = "Installed";
                    }
                    else
                    {
                        button20.Enabled = false;
                        button21.Enabled = false;
                        label32.Text = "N/A";
                        label34.Text = "Not Installed";
                    }

                    if(comboBox3.Text != "")
                    {
                        label29.Text = "[checking]";
                        label29.ForeColor = Color.DarkBlue;
                        string searchingpackage = comboBox3.Text;
                        
                        Task task = new Task(delegate { DoAsyncOperation(searchingpackage, packageExists, version); });
                        task.Start();
                    }
                    else
                    {
                        label29.Text = "[not selected]";
                        label29.ForeColor = Color.Black;
                        label28.Text = "N/A";
                        label30.Text = "N/A";
                        label34.Text = "N/A";
                        label28.ForeColor = Color.Black;
                        label28.Cursor = Cursors.Default;
                        button22.Enabled = false;
                        button23.Enabled = false;
                        button18.Enabled = false;
                        button19.Enabled = false;
                    }
                    
                }
                catch
                {
                    errors = true;
                }
            }

            if (!File.Exists(jsonpath) || errors)
            {
                label29.Text = "[not selected]";
                label29.ForeColor = Color.Black;
                label28.Text = "N/A";
                label30.Text = "N/A";
                label32.Text = "N/A";
                label34.Text = "N/A";
                label28.ForeColor = Color.Black;
                label28.Cursor = Cursors.Default;
                button20.Enabled = false;
                button21.Enabled = false;
                button18.Enabled = false;
                button19.Enabled = false;
                button22.Enabled = false;
                button23.Enabled = false;
            }
        }

        struct CheckingResult
        {
            public string packageName;
            public bool isExists;
            public string version;
            public string link;
        }


        private void DoAsyncOperation(string package, bool packageExists, string version)
        {
            CheckingResult cr = new CheckingResult();
            cr.packageName = package;
            cr.isExists = false;
            string html = string.Empty;
            string url = @"http://registry.npmjs.com/-/v1/search?text=" + package + "&size=1";

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                //request.AutomaticDecompression = DecompressionMethods.GZip;
                request.Timeout = 10000;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    html = reader.ReadToEnd();
                }

                var json = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(html);
                JArray items = (JArray)json["objects"];
                if(items.Count == 1 && (string)json["objects"][0]["package"]["name"] == package)
                {
                    this.BeginInvoke(new MethodInvoker(() => {
                        label29.Text = "[present]";
                        label29.ForeColor = Color.Green;
                        try
                        {
                            label28.Text = json["objects"][0]["package"]["links"]["repository"];
                            cr.link = json["objects"][0]["package"]["links"]["repository"];
                        }
                        catch
                        {
                            label28.Text = json["objects"][0]["package"]["links"]["npm"];
                            cr.link = json["objects"][0]["package"]["links"]["npm"];
                        }
                        
                        label30.Text = json["objects"][0]["package"]["version"];
                        cr.version = json["objects"][0]["package"]["version"];
                        label28.ForeColor = Color.Blue;
                        label28.Cursor = Cursors.Hand;
                        if (!packageExists)
                        {
                            button22.Enabled = true;
                            button23.Enabled = true;
                            button18.Enabled = false;
                            button19.Enabled = false;
                        }
                        else
                        {
                            button22.Enabled = false;
                            button23.Enabled = false;
                            if(version != (string)json["objects"][0]["package"]["version"])
                            {
                                button18.Enabled = true;
                                button19.Enabled = true;
                            }
                            else
                            {
                                button18.Enabled = false;
                                button19.Enabled = false;
                            }
                        }
                    }));
                }
                else
                {
                    this.BeginInvoke(new MethodInvoker(() => {
                        label29.Text = "[not found]";
                        label29.ForeColor = Color.Red;
                        label28.Text = "N/A";
                        label30.Text = "N/A";
                        label28.ForeColor = Color.Black;
                        label28.Cursor = Cursors.Default;
                        button22.Enabled = false;
                        button23.Enabled = false;
                        button18.Enabled = false;
                        button19.Enabled = false;
                    }));
                }
            }
            catch
            {
                this.BeginInvoke(new MethodInvoker(() => {
                    label29.Text = "[error]";
                    label29.ForeColor = Color.Red;
                    label28.Text = "N/A";
                    label30.Text = "N/A";
                    label28.ForeColor = Color.Black;
                    label28.Cursor = Cursors.Default;
                    button22.Enabled = false;
                    button23.Enabled = false;
                    button18.Enabled = false;
                    button19.Enabled = false;
                }));
            }
            

            //return cr;
        }

        private void Label28_Click(object sender, EventArgs e)
        {
            if(label28.Cursor == Cursors.Hand)
            {
                Process.Start(label28.Text);
            }
        }

        private void Button23_Click(object sender, EventArgs e)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = "/c yarn add " + comboBox3.Text;
            cmd.StartInfo.WorkingDirectory = textBox1.Text;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();
            cmd.WaitForExit();
            string package = comboBox3.Text;
            checkDirectory(textBox1.Text);
            comboBox3.Text = package;
        }

        private void Button22_Click(object sender, EventArgs e)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = "/c npm install " + comboBox3.Text;
            cmd.StartInfo.WorkingDirectory = textBox1.Text;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();
            cmd.WaitForExit();
            string package = comboBox3.Text;
            checkDirectory(textBox1.Text);
            comboBox3.Text = package;
        }

        private void Button21_Click(object sender, EventArgs e)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = "/c yarn remove " + comboBox3.Text;
            cmd.StartInfo.WorkingDirectory = textBox1.Text;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();
            cmd.WaitForExit();
            string package = comboBox3.Text;
            checkDirectory(textBox1.Text);
            comboBox3.Text = package;
        }

        private void Button20_Click(object sender, EventArgs e)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = "/c npm uninstall " + comboBox3.Text;
            cmd.StartInfo.WorkingDirectory = textBox1.Text;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();
            cmd.WaitForExit();
            string package = comboBox3.Text;
            checkDirectory(textBox1.Text);
            comboBox3.Text = package;
        }

        private void Button18_Click(object sender, EventArgs e)
        {
            //yarn upgrade left - pad--latest
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = "/c yarn upgrade " + comboBox3.Text + " --latest";
            cmd.StartInfo.WorkingDirectory = textBox1.Text;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();
            cmd.WaitForExit();
            string package = comboBox3.Text;
            checkDirectory(textBox1.Text);
            comboBox3.Text = package;
        }

        private void Button19_Click(object sender, EventArgs e)
        {
            //npm update --save package_name
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = "/c npm install --save " + comboBox3.Text;
            cmd.StartInfo.WorkingDirectory = textBox1.Text;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();
            cmd.WaitForExit();
            string package = comboBox3.Text;
            checkDirectory(textBox1.Text);
            comboBox3.Text = package;
        }

        private void Timer3_Tick(object sender, EventArgs e)
        {
            timer3.Enabled = false;
            validate();
        }

        private void Button26_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            f.ShowDialog();
        }

        private void Button24_Click(object sender, EventArgs e)
        {

        }

        private void Button27_Click(object sender, EventArgs e)
        {

        }

        private void Button25_Click(object sender, EventArgs e)
        {

        }

        private void Button24_Click_1(object sender, EventArgs e)
        {
            Form3 f = new Form3();
            f.ShowDialog();
        }

        private void Button10_Click(object sender, EventArgs e)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = "/c \"" + emulatorp + "\" -list-avds";
            cmd.StartInfo.WorkingDirectory = textBox1.Text;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.RedirectStandardError = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();
            string output = cmd.StandardOutput.ReadToEnd();
            cmd.WaitForExit();
            comboBox1.Text = "";
            comboBox1.Items.Clear();
            button11.Enabled = false;
            button12.Enabled = false;
            foreach (string line in output.Split(new[] { "\r\n", "\r", "\n" },StringSplitOptions.RemoveEmptyEntries))
            {
                comboBox1.Items.Add(line);
                comboBox1.Text = line;
                if(!button11.Enabled) button11.Enabled = true;
                if(!button12.Enabled) button12.Enabled = true;
            }
        }

        private void Label14_Click(object sender, EventArgs e)
        {

        }
    }
}
