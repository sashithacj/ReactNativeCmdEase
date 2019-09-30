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

namespace ReactNativeCmdEase
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string jhome, ahome, emulatorp;

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!File.Exists(Path.Combine(Application.StartupPath, "Newtonsoft.Json.dll")))
                File.WriteAllBytes(Path.Combine(Application.StartupPath, "Newtonsoft.Json.dll"), Properties.Resources.Newtonsoft_Json);

            textBox1.Text = Application.StartupPath;
            checkDirectory(textBox1.Text);
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
                    //button14.Enabled = true;
                    button15.Enabled = true;
                    button16.Enabled = true;

                    if (File.Exists(Path.Combine(dpath, "yarn.lock")))
                    {
                        textBox6.Text = "Yarn";
                        
                    }
                    else if(File.Exists(Path.Combine(dpath, "package-lock.json")))
                    {
                        textBox6.Text = "Npm";
                        
                    }
                    else
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
                //button14.Enabled = false;
                button15.Enabled = false;
                button16.Enabled = false;
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
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = "/c rm -rf node_modules";
            cmd.StartInfo.WorkingDirectory = textBox1.Text;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();
            //cmd.WaitForExit();
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
