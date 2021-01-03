using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

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

            if (File.Exists(Path.Combine(Application.StartupPath, "package.json")))
            {
                textBox1.Text = Application.StartupPath;
                projectdir = Application.StartupPath;
            }
            else
            {
                RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ReactNativeCmdEase");
                if (key != null)
                {
                    string last = key.GetValue("last") as string;
                    if (string.IsNullOrEmpty(last))
                    {
                        textBox1.Text = Application.StartupPath;
                        projectdir = Application.StartupPath;
                    }
                    else
                    {
                        textBox1.Text = last;
                        projectdir = last;
                    }

                }
            }

            try
            {
                jhome = System.Environment.GetEnvironmentVariable("JAVA_HOME", EnvironmentVariableTarget.User);
            }
            catch
            {
                jhome = string.Empty;
            }
            try
            {
                ahome = System.Environment.GetEnvironmentVariable("ANDROID_HOME", EnvironmentVariableTarget.User);
            }
            catch
            {
                ahome = string.Empty;
            }

            //Console.WriteLine(existingPathFolderVariable);
            //Application.Exit();
            //jhome = System.Environment.GetEnvironmentVariable("JAVA_HOME", EnvironmentVariableTarget.User);
            //ahome = System.Environment.GetEnvironmentVariable("ANDROID_HOME", EnvironmentVariableTarget.User);

            string suj = string.Empty;
            try
            {
                suj = Directory.EnumerateDirectories(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Java")).Where(d => d.Contains("jdk")).OrderByDescending(d => new DirectoryInfo(d).LastWriteTime).First();
            }
            catch
            {
                DialogResult dialogResult = MessageBox.Show("Java Developement Kit cannot be found, Do you want to install JDK 8 now? " +
                    Environment.NewLine + "(If YES, this program will be closed & JDK installation will be started. You need to reopen this program after installation is finished)",
                    "Install JDK 8", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    if (!File.Exists(Path.Combine(Application.StartupPath, "jdk-8u261-windows-x64.exe")))
                        File.WriteAllBytes(Path.Combine(Application.StartupPath, "jdk-8u261-windows-x64.exe"), Properties.Resources.jdk_8u261_windows_x64);
                    Process p = new Process();
                    p.StartInfo.FileName = Path.Combine(Application.StartupPath, "jdk-8u261-windows-x64.exe");
                    p.StartInfo.UseShellExecute = true;
                    p.StartInfo.Verb = "runas";
                    p.Start();
                    Application.Exit();
                }
                else if (dialogResult == DialogResult.No)
                {

                }
            }

            if (jhome != suj && !string.IsNullOrEmpty(suj))
            {
                DialogResult dialogResult = MessageBox.Show("Current JAVA_HOME variable: " + jhome +
                    Environment.NewLine + "Suggested JAVA_HOME variable: " + suj +
                    Environment.NewLine + "Do you agree with this variable change?",
                    "Latest JDK (JAVA_HOME) Mismatch", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    Environment.SetEnvironmentVariable("JAVA_HOME", suj, EnvironmentVariableTarget.User);
                    jhome = suj;
                }
                else if (dialogResult == DialogResult.No)
                {

                }
            }

            string sua = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Android\\Sdk");
            if (ahome != sua && !string.IsNullOrEmpty(sua) && Directory.Exists(sua))
            {
                DialogResult dialogResult = MessageBox.Show("Current ANDROID_HOME variable: " + ahome +
                    Environment.NewLine + "Suggested ANDROID_HOME variable: " + sua +
                    Environment.NewLine + "Do you agree with this variable change?",
                    "Latest ASDK (ANDROID_HOME) Mismatch", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    Environment.SetEnvironmentVariable("ANDROID_HOME", sua, EnvironmentVariableTarget.User);
                    ahome = sua;
                }
                else if (dialogResult == DialogResult.No)
                {

                }
            }

            if (ahome == null) ahome = "";
            string platform_tools_dir = Path.Combine(ahome, "platform-tools");
            string originalPathEnv = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User);
            string[] paths = originalPathEnv.Split(new char[1] { Path.PathSeparator }, StringSplitOptions.RemoveEmptyEntries);
            bool is_found = false;
            foreach (string s in paths)
            {
                string pathEnv = Environment.ExpandEnvironmentVariables(s);
                if (pathEnv.Equals(platform_tools_dir))
                {
                    textBox7.Text = pathEnv;
                    is_found = true;
                }
            }

            if (!is_found && Directory.Exists(platform_tools_dir))
            {
                DialogResult dialogResult = MessageBox.Show("Do you agree to add android platform-tools directory to PATH environment variable?" +
                    Environment.NewLine + "Suggested plaform tools directory: " + platform_tools_dir,
                    "Android platform tools variable Not found", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    Environment.SetEnvironmentVariable("PATH", originalPathEnv + platform_tools_dir + Path.PathSeparator, EnvironmentVariableTarget.User);
                    textBox7.Text = platform_tools_dir;
                }
                else if (dialogResult == DialogResult.No)
                {

                }
            }

            emulatorp = Path.Combine(ahome, @"tools\emulator.exe");
            textBox4.Text = jhome;
            textBox5.Text = ahome;

            /*
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
            }*/

            checkDirectory(textBox1.Text);
            timer1.Enabled = true;

            RegistryKey key2 = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ReactNativeCmdEase");
            if (key2 != null)
            {
                string mood = key2.GetValue("mood") as string;
                if (string.IsNullOrEmpty(mood))
                {
                    activateLightMood();
                    radioButton1.Checked = true;
                    radioButton2.Checked = false;
                }
                else if (mood == "light")
                {
                    activateLightMood();
                    radioButton1.Checked = true;
                    radioButton2.Checked = false;
                }
                else if (mood == "dark")
                {
                    activateDarkMood();
                    radioButton1.Checked = false;
                    radioButton2.Checked = true;
                }
                else
                {
                    activateLightMood();
                    radioButton1.Checked = true;
                    radioButton2.Checked = false;
                }

            }
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
                    textBox8.Enabled = false;
                    //textBox2.BackColor = SystemColors.Control;
                    //textBox3.BackColor = SystemColors.Control;
                    //textBox8.BackColor = SystemColors.Control;
                    textBox2.Text = json["name"];
                    textBox3.Text = json["dependencies"]["react-native"];
                    textBox8.Text = "[default]";

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
                    button25.Enabled = true;
                    comboBox3.Enabled = true;
                    button13.Enabled = true;
                    button14.Enabled = true;
                    button28.Enabled = true;

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

                    if (File.Exists(Path.Combine(dpath, "package-lock.json")))
                    {
                        if (textBox6.Text == "")
                            textBox6.Text = "Npm";
                        else
                            textBox6.Text = textBox6.Text + " + Npm";

                    }

                    if (textBox6.Text == "")
                    {
                        textBox6.Text = "[unknown]";
                    }

                    RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ReactNativeCmdEase");
                    if (key != null)
                    {
                        key.SetValue("last", dpath);
                    }
                }
                catch
                {
                    errors = true;
                }
            }

            if (!File.Exists(jsonpath) || errors)
            {
                pictureBox1.BackColor = Color.Red;
                label3.Text = "Not Found";
                button2.Enabled = true;
                textBox2.Enabled = true;
                textBox3.Enabled = true;
                textBox8.Enabled = true;
                //textBox2.BackColor = SystemColors.Window;
                //textBox3.BackColor = SystemColors.Window;
                //textBox8.BackColor = SystemColors.Window;
                //Forcing users to start with latest version
                //textBox3.BackColor = SystemColors.Control;
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
                button25.Enabled = false;
                button13.Enabled = false;
                button14.Enabled = false;
                button28.Enabled = false;
                //button9.Enabled = false;
                //button10.Enabled = false;
                textBox3.Text = "[latest]";
                textBox8.Text = "[default]";
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
            if (textBox2.Text.Length != 0)
            {
                if (ProList.Count > 0)
                {
                    foreach (Process p in ProList)
                    {
                        try
                        {
                            p.Kill();
                        }
                        catch
                        {

                        }
                    }
                    ProList.Clear();
                }
                listView1.Items.Clear();
                Process cmd = new Process();
                cmd.StartInfo.FileName = "cmd.exe";
                string ar;
                if (textBox3.Text == "[latest]" || textBox3.Text.Length == 0)
                {
                    ar = "/c npx react-native init " + textBox2.Text;
                }

                else
                {
                    ar = "/c npx react-native init " + textBox2.Text + " --version=\"" + textBox3.Text + "\"";

                }

                if (textBox8.Text != "[default]" && textBox8.Text.Length != 0)
                {
                    ar = ar + " --template " + textBox8.Text;
                }
                //[default]
                cmd.StartInfo.Arguments = ar;
                cmd.StartInfo.WorkingDirectory = textBox1.Text;
                cmd.StartInfo.UseShellExecute = false;
                if (checkBox1.Checked)
                {
                    cmd.StartInfo.CreateNoWindow = true;
                    cmd.StartInfo.RedirectStandardError = true;
                    cmd.StartInfo.RedirectStandardOutput = true;
                    cmd.StartInfo.RedirectStandardInput = false;
                    cmd.EnableRaisingEvents = true;
                    cmd.OutputDataReceived += (a, b) => addToConsole("P", b.Data, 0);
                    cmd.ErrorDataReceived += (a, b) => addToConsole("P", b.Data, 1);
                    //cmd.Exited += (a, b) => addToConsole("P", "Package manager process exited.", 2);
                    cmd.Exited += (a, b) => {
                        addToConsole("P", "Package manager process exited.", 2);
                        this.Invoke((MethodInvoker)delegate {
                            string newdir = Path.Combine(textBox1.Text, textBox2.Text);
                            if (Directory.Exists(newdir))
                            {
                                string newexe = Path.Combine(newdir, Path.GetFileName(Application.ExecutablePath));
                                checkDirectory(newdir);
                                

                            }
                            else
                            {
                                addToConsole("P", "An error occured while starting the project.", 2);
                            }
                        });
                    };
                    cmd.Start();
                    ProList.Add(cmd);
                    addToConsole("P", "Started the operation with new process (" + cmd.Id + ").", 2);
                    cmd.BeginErrorReadLine();
                    cmd.BeginOutputReadLine();
                }
                else
                {
                    cmd.Start();
                    cmd.WaitForExit();
                    string newdir = Path.Combine(textBox1.Text, textBox2.Text);
                    if (Directory.Exists(newdir))
                    {
                        string newexe = Path.Combine(newdir, Path.GetFileName(Application.ExecutablePath));
                        
                        checkDirectory(newdir);
                        

                    }
                    else
                    {
                        MessageBox.Show("An error occured while starting the project.");
                    }
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
            cmd.StartInfo.Arguments = "/c \"echo nodex & node -v & echo npmx & npm -v & echo yarnx & yarn -v & echo reactx & npx react-native -v\"";
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
            if (ProList.Count > 0)
            {
                foreach (Process p in ProList)
                {
                    try
                    {
                        p.Kill();
                    }
                    catch
                    {

                    }
                }
                ProList.Clear();
            }
            listView1.Items.Clear();
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = "/c npm cache clean";
            cmd.StartInfo.WorkingDirectory = textBox1.Text;
            cmd.StartInfo.UseShellExecute = false;
            if (checkBox1.Checked)
            {
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.RedirectStandardError = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.RedirectStandardInput = false;
                cmd.EnableRaisingEvents = true;
                cmd.OutputDataReceived += (a, b) => addToConsole("P", b.Data, 0);
                cmd.ErrorDataReceived += (a, b) => addToConsole("P", b.Data, 1);
                cmd.Exited += (a, b) => addToConsole("P", "Package manager process exited.", 2);
                cmd.Start();
                ProList.Add(cmd);
                addToConsole("P", "Started the operation with new process (" + cmd.Id + ").", 2);
                cmd.BeginErrorReadLine();
                cmd.BeginOutputReadLine();
            }
            else
            {
                cmd.Start();
            }
            //cmd.WaitForExit();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            if (ProList.Count > 0)
            {
                foreach (Process p in ProList)
                {
                    try
                    {
                        p.Kill();
                    }
                    catch
                    {

                    }
                }
                ProList.Clear();
            }
            listView1.Items.Clear();
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = "/c npm install";
            cmd.StartInfo.WorkingDirectory = textBox1.Text;
            cmd.StartInfo.UseShellExecute = false;
            if (checkBox1.Checked)
            {
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.RedirectStandardError = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.RedirectStandardInput = false;
                cmd.EnableRaisingEvents = true;
                cmd.OutputDataReceived += (a, b) => addToConsole("P", b.Data, 0);
                cmd.ErrorDataReceived += (a, b) => addToConsole("P", b.Data, 1);
                cmd.Exited += (a, b) => addToConsole("P", "Package manager process exited.", 2);
                cmd.Start();
                ProList.Add(cmd);
                addToConsole("P", "Started the operation with new process (" + cmd.Id + ").", 2);
                cmd.BeginErrorReadLine();
                cmd.BeginOutputReadLine();
            }
            else
            {
                cmd.Start();
            }
            //cmd.WaitForExit();
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            if (ProList.Count > 0)
            {
                foreach (Process p in ProList)
                {
                    try
                    {
                        p.Kill();
                    }
                    catch
                    {

                    }
                }
                ProList.Clear();
            }
            listView1.Items.Clear();
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = "/c npm update";
            cmd.StartInfo.WorkingDirectory = textBox1.Text;
            cmd.StartInfo.UseShellExecute = false;
            if (checkBox1.Checked)
            {
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.RedirectStandardError = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.RedirectStandardInput = false;
                cmd.EnableRaisingEvents = true;
                cmd.OutputDataReceived += (a, b) => addToConsole("P", b.Data, 0);
                cmd.ErrorDataReceived += (a, b) => addToConsole("P", b.Data, 1);
                cmd.Exited += (a, b) => addToConsole("P", "Package manager process exited.", 2);
                cmd.Start();
                ProList.Add(cmd);
                addToConsole("P", "Started the operation with new process (" + cmd.Id + ").", 2);
                cmd.BeginErrorReadLine();
                cmd.BeginOutputReadLine();
            }
            else
            {
                cmd.Start();
            }
            //cmd.WaitForExit();
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            if (ProList.Count > 0)
            {
                foreach (Process p in ProList)
                {
                    try
                    {
                        p.Kill();
                    }
                    catch
                    {

                    }
                }
                ProList.Clear();
            }
            listView1.Items.Clear();
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = "/c yarn install --check-files";
            cmd.StartInfo.WorkingDirectory = textBox1.Text;
            cmd.StartInfo.UseShellExecute = false;
            if (checkBox1.Checked)
            {
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.RedirectStandardError = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.RedirectStandardInput = false;
                cmd.EnableRaisingEvents = true;
                cmd.OutputDataReceived += (a, b) => addToConsole("P", b.Data, 0);
                cmd.ErrorDataReceived += (a, b) => addToConsole("P", b.Data, 1);
                cmd.Exited += (a, b) => addToConsole("P", "Package manager process exited.", 2);
                cmd.Start();
                ProList.Add(cmd);
                addToConsole("P", "Started the operation with new process (" + cmd.Id + ").", 2);
                cmd.BeginErrorReadLine();
                cmd.BeginOutputReadLine();
            }
            else
            {
                cmd.Start();
            }
            //cmd.WaitForExit();
        }

        private void Button8_Click(object sender, EventArgs e)
        {
            if (ProList.Count > 0)
            {
                foreach (Process p in ProList)
                {
                    try
                    {
                        p.Kill();
                    }
                    catch
                    {

                    }
                }
                ProList.Clear();
            }
            listView1.Items.Clear();
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = "/c gradlew clean";
            cmd.StartInfo.WorkingDirectory = Path.Combine(textBox1.Text, "android");
            cmd.StartInfo.UseShellExecute = false;

            if (checkBox1.Checked)
            {
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.RedirectStandardError = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.RedirectStandardInput = false;
                cmd.EnableRaisingEvents = true;
                cmd.OutputDataReceived += (a, b) => addToConsole("CB", b.Data, 0);
                cmd.ErrorDataReceived += (a, b) => addToConsole("CB", b.Data, 1);
                cmd.Exited += (a, b) => addToConsole("CB", "Cleaning build process exited.", 2);
                cmd.Start();
                ProList.Add(cmd);
                addToConsole("CB", "Started the operation with new process (" + cmd.Id + ").", 2);
                cmd.BeginErrorReadLine();
                cmd.BeginOutputReadLine();
            }
            else
            {
                cmd.Start();
            }

        }

        private void addToConsole(string op, string data, int type = 0)
        {

            listView1.Invoke((MethodInvoker)delegate
            {
                ListViewItem li = new ListViewItem();
                li.Text = op + ": " + data;
                if (type == 0)
                {
                    li.BackColor = Color.LightGreen;
                    li.ForeColor = Color.Black;
                }
                else if (type == 1)
                {
                    li.BackColor = Color.LightPink;
                    li.ForeColor = Color.Black;
                }
                else if (type == 2)
                {
                    li.BackColor = Color.LightBlue;
                    li.ForeColor = Color.Black;
                }
                else if (type == 3)
                {
                    li.BackColor = Color.Yellow;
                    li.ForeColor = Color.Black;
                }
                else if (type == 4)
                {
                    li.BackColor = Color.Red;
                    li.ForeColor = Color.Black;
                }
                listView1.BeginUpdate();
                listView1.Items.Add(li);
                listView1.EnsureVisible(listView1.Items.Count - 1);
                listView1.EndUpdate();
            });
        }

        private void Button9_Click(object sender, EventArgs e)
        {
            foreach (Process p in Process.GetProcessesByName("adb"))
            {
                try
                {
                    p.Kill();
                }
                catch (Exception ec)
                {
                    MessageBox.Show(ec.Message);
                }

            }
        }

        private void Button11_Click(object sender, EventArgs e)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = emulatorp;
            cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            cmd.StartInfo.Arguments = "-avd " + comboBox1.Text + " -no-boot-anim -netspeed full -netdelay none";
            cmd.StartInfo.WorkingDirectory = textBox1.Text;
            cmd.StartInfo.UseShellExecute = false;
            if (checkBox1.Checked)
            {
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.RedirectStandardError = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.RedirectStandardInput = false;
                cmd.EnableRaisingEvents = true;
                cmd.OutputDataReceived += (a, b) => addToConsole("EM", b.Data, 0);
                cmd.ErrorDataReceived += (a, b) => addToConsole("EM", b.Data, 1);
                cmd.Exited += (a, b) => addToConsole("EM", "Emulator process exited.", 2);
                cmd.Start();
                addToConsole("EM", "Started the emulator with new process (" + cmd.Id + ").", 2);
                cmd.BeginErrorReadLine();
                cmd.BeginOutputReadLine();
            }
            else
            {
                cmd.Start();
            }
        }

        private void Button12_Click(object sender, EventArgs e)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = emulatorp;
            cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            cmd.StartInfo.Arguments = "-avd " + comboBox1.Text + " -wipe-data -no-boot-anim -netspeed full -netdelay none";
            cmd.StartInfo.WorkingDirectory = textBox1.Text;
            cmd.StartInfo.UseShellExecute = false;
            if (checkBox1.Checked)
            {
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.RedirectStandardError = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.RedirectStandardInput = false;
                cmd.EnableRaisingEvents = true;
                cmd.OutputDataReceived += (a, b) => addToConsole("EM", b.Data, 0);
                cmd.ErrorDataReceived += (a, b) => addToConsole("EM", b.Data, 1);
                cmd.Exited += (a, b) => addToConsole("EM", "Emulator process exited.", 2);
                cmd.Start();
                addToConsole("EM", "Started the emulator with new process (" + cmd.Id + ").", 2);
                cmd.BeginErrorReadLine();
                cmd.BeginOutputReadLine();
            }
            else
            {
                cmd.Start();
            }
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Timer2_Tick(object sender, EventArgs e)
        {
            Process[] p = Process.GetProcessesByName("adb");
            Process[] n = Process.GetProcessesByName("node");
            Process[] j = Process.GetProcessesByName("java");
            if (p.Length > 0)
            {
                label20.Text = "Android emulators: [" + p.Length + " adb running]";
                button9.Enabled = true;
            }
            else
            {
                label20.Text = "Android emulators: ";
                button9.Enabled = false;
            }

            for (int u = 0; u < ProList.Count; u++)
            {
                try
                {
                    if (ProList[u].HasExited) ProList.RemoveAt(u);
                }
                catch { }
            }

            label38.Text = ProList.Count.ToString() + " console processes running";

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

        List<Process> ProList = new List<Process>();

        private void Button13_Click(object sender, EventArgs e)
        {
            if (ProList.Count > 0)
            {
                foreach (Process p in ProList)
                {
                    try
                    {
                        p.Kill();
                    }
                    catch
                    {

                    }
                }
                ProList.Clear();
            }
            listView1.Items.Clear();
            button28.PerformClick();

            if (checkBox1.Checked)
            {
                Process cmd2 = new Process();
                cmd2.StartInfo.FileName = "cmd.exe";
                cmd2.StartInfo.Arguments = "/c npx react-native start";
                cmd2.StartInfo.WorkingDirectory = textBox1.Text;
                cmd2.StartInfo.UseShellExecute = false;
                cmd2.StartInfo.CreateNoWindow = true;
                cmd2.StartInfo.RedirectStandardError = true;
                cmd2.StartInfo.RedirectStandardOutput = true;
                cmd2.StartInfo.RedirectStandardInput = false;
                cmd2.EnableRaisingEvents = true;
                cmd2.OutputDataReceived += (a, b) => addToConsole("Node", b.Data, 3);
                cmd2.ErrorDataReceived += (a, b) => addToConsole("Node", b.Data, 4);
                cmd2.Exited += (a, b) => addToConsole("Node", "Node process exited.", 2);
                cmd2.Start();
                ProList.Add(cmd2);
                addToConsole("Node", "Started the Nod Server with new process (" + cmd2.Id + ").", 2);
                cmd2.BeginErrorReadLine();
                cmd2.BeginOutputReadLine();
            }


            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = "/c npx react-native run-android";
            cmd.StartInfo.WorkingDirectory = textBox1.Text;
            cmd.StartInfo.UseShellExecute = false;

            if (checkBox1.Checked)
            {
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.RedirectStandardError = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.RedirectStandardInput = false;
                cmd.EnableRaisingEvents = true;
                cmd.OutputDataReceived += (a, b) => addToConsole("RD", b.Data, 0);
                cmd.ErrorDataReceived += (a, b) => addToConsole("RD", b.Data, 1);
                cmd.Exited += (a, b) => addToConsole("RD", "Running debug process exited.", 2);
                cmd.Start();
                ProList.Add(cmd);
                addToConsole("RD", "Started the operation with new process (" + cmd.Id + ").", 2);
                cmd.BeginErrorReadLine();
                cmd.BeginOutputReadLine();

                listView1.Left = 14;
                listView1.Width = 1280;
                checkBox2.Location = new Point(13, 7);
                checkBox2.Checked = true;
                listView1.Columns[0].Width = 1246;
                foreach (Control c in this.Controls)
                {
                    if (c.Name != "listView1" && c.Name != "checkBox2" && c.Name != "label38")
                    {
                        c.Visible = false;
                    }
                }
                //cmd.WaitForExit();
            }
            else
            {
                cmd.Start();
            }
        }

        private void Button14_Click(object sender, EventArgs e)
        {
            if (ProList.Count > 0)
            {
                foreach (Process p in ProList)
                {
                    try
                    {
                        p.Kill();
                    }
                    catch
                    {

                    }
                }
                ProList.Clear();
            }
            listView1.Items.Clear();
            button28.PerformClick();

            if (checkBox1.Checked)
            {
                Process cmd2 = new Process();
                cmd2.StartInfo.FileName = "cmd.exe";
                cmd2.StartInfo.Arguments = "/c npx react-native start";
                cmd2.StartInfo.WorkingDirectory = textBox1.Text;
                cmd2.StartInfo.UseShellExecute = false;
                cmd2.StartInfo.CreateNoWindow = true;
                cmd2.StartInfo.RedirectStandardError = true;
                cmd2.StartInfo.RedirectStandardOutput = true;
                cmd2.StartInfo.RedirectStandardInput = false;
                cmd2.EnableRaisingEvents = true;
                cmd2.OutputDataReceived += (a, b) => addToConsole("Node", b.Data, 3);
                cmd2.ErrorDataReceived += (a, b) => addToConsole("Node", b.Data, 4);
                cmd2.Exited += (a, b) => addToConsole("Node", "Node process exited.", 2);
                cmd2.Start();
                ProList.Add(cmd2);
                addToConsole("Node", "Started the Nod Server with new process (" + cmd2.Id + ").", 2);
                cmd2.BeginErrorReadLine();
                cmd2.BeginOutputReadLine();
            }

            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = "/c npx react-native run-android --variant=release";
            cmd.StartInfo.WorkingDirectory = textBox1.Text;
            cmd.StartInfo.UseShellExecute = false;
            if (checkBox1.Checked)
            {
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.RedirectStandardError = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.RedirectStandardInput = false;
                cmd.EnableRaisingEvents = true;
                cmd.OutputDataReceived += (a, b) => addToConsole("RR", b.Data, 0);
                cmd.ErrorDataReceived += (a, b) => addToConsole("RR", b.Data, 1);
                cmd.Exited += (a, b) => addToConsole("RR", "Running release process exited.", 2);
                cmd.Start();
                ProList.Add(cmd);
                addToConsole("RR", "Started the operation with new process (" + cmd.Id + ").", 2);
                cmd.BeginErrorReadLine();
                cmd.BeginOutputReadLine();
            }
            else
            {
                cmd.Start();
            }

            //cmd.WaitForExit();
        }

        private void Button15_Click(object sender, EventArgs e)
        {
            if (ProList.Count > 0)
            {
                foreach (Process p in ProList)
                {
                    try
                    {
                        p.Kill();
                    }
                    catch
                    {

                    }
                }
                ProList.Clear();
            }
            listView1.Items.Clear();
            button28.PerformClick();
            if (checkBox1.Checked)
            {
                Process cmd2 = new Process();
                cmd2.StartInfo.FileName = "cmd.exe";
                cmd2.StartInfo.Arguments = "/c npx react-native start";
                cmd2.StartInfo.WorkingDirectory = textBox1.Text;
                cmd2.StartInfo.UseShellExecute = false;
                cmd2.StartInfo.CreateNoWindow = true;
                cmd2.StartInfo.RedirectStandardError = true;
                cmd2.StartInfo.RedirectStandardOutput = true;
                cmd2.StartInfo.RedirectStandardInput = false;
                cmd2.EnableRaisingEvents = true;
                cmd2.OutputDataReceived += (a, b) => addToConsole("Node", b.Data, 3);
                cmd2.ErrorDataReceived += (a, b) => addToConsole("Node", b.Data, 4);
                cmd2.Exited += (a, b) => addToConsole("Node", "Node process exited.", 2);
                cmd2.Start();
                ProList.Add(cmd2);
                addToConsole("Node", "Started the Nod Server with new process (" + cmd2.Id + ").", 2);
                cmd2.BeginErrorReadLine();
                cmd2.BeginOutputReadLine();
            }
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = "/c gradlew bundleRelease";
            cmd.StartInfo.WorkingDirectory = Path.Combine(textBox1.Text, "android");
            cmd.StartInfo.UseShellExecute = false;
            if (checkBox1.Checked)
            {
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.RedirectStandardError = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.RedirectStandardInput = false;
                cmd.EnableRaisingEvents = true;
                cmd.OutputDataReceived += (a, b) => addToConsole("BA", b.Data, 0);
                cmd.ErrorDataReceived += (a, b) => addToConsole("BA", b.Data, 1);
                cmd.Exited += (a, b) => addToConsole("BA", "App bundles building process exited.", 2);
                cmd.Start();
                ProList.Add(cmd);
                addToConsole("BA", "Started the operation with new process (" + cmd.Id + ").", 2);
                cmd.BeginErrorReadLine();
                cmd.BeginOutputReadLine();
            }
            else
            {
                cmd.Start();
            }
        }

        private void Button16_Click(object sender, EventArgs e)
        {
            Process.Start(Path.Combine(textBox1.Text, @"android\app\build\outputs"));
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            string atom = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "atom\\atom.exe");
            if (Directory.Exists(textBox1.Text))
            {
                button17.Enabled = true;
                if (File.Exists(atom))
                {
                    button27.Enabled = true;
                }
            }
            else
            {
                button17.Enabled = false;
                button27.Enabled = false;
            }

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

                    if (comboBox3.Text != "")
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
                if (items.Count == 1 && (string)json["objects"][0]["package"]["name"] == package)
                {
                    this.BeginInvoke(new MethodInvoker(() =>
                    {
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
                            if (version != (string)json["objects"][0]["package"]["version"])
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
                    this.BeginInvoke(new MethodInvoker(() =>
                    {
                        label29.Text = "[not found]";
                        label29.ForeColor = Color.Red;
                        label28.Text = "N/A";
                        label30.Text = "N/A";
                        if (radioButton1.Checked)
                        {
                            label28.ForeColor = Color.Black;
                        }
                        else if (radioButton2.Checked)
                        {
                            label28.ForeColor = Color.White;
                        }
                        
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
                this.BeginInvoke(new MethodInvoker(() =>
                {
                    label29.Text = "[error]";
                    label29.ForeColor = Color.Red;
                    label28.Text = "N/A";
                    label30.Text = "N/A";
                    if (radioButton1.Checked)
                    {
                        label28.ForeColor = Color.Black;
                    }
                    else if (radioButton2.Checked)
                    {
                        label28.ForeColor = Color.White;
                    }
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
            if (label28.Cursor == Cursors.Hand)
            {
                Process.Start(label28.Text);
            }
        }

        private void Button23_Click(object sender, EventArgs e)
        {
            if (ProList.Count > 0)
            {
                foreach (Process p in ProList)
                {
                    try
                    {
                        p.Kill();
                    }
                    catch
                    {

                    }
                }
                ProList.Clear();
            }
            listView1.Items.Clear();
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = "/c yarn add " + comboBox3.Text;
            cmd.StartInfo.WorkingDirectory = textBox1.Text;
            cmd.StartInfo.UseShellExecute = false;
            string package = comboBox3.Text;
            if (checkBox1.Checked)
            {
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.RedirectStandardError = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.RedirectStandardInput = false;
                cmd.EnableRaisingEvents = true;
                cmd.OutputDataReceived += (a, b) => addToConsole("P", b.Data, 0);
                cmd.ErrorDataReceived += (a, b) => addToConsole("P", b.Data, 1);
                cmd.Exited += (a, b) => { 
                    addToConsole("P", "Package manager process exited.", 2);
                    this.Invoke((MethodInvoker)delegate {
                        checkDirectory(textBox1.Text);
                        comboBox3.Text = package;
                    });
                };
                cmd.Start();
                ProList.Add(cmd);
                addToConsole("P", "Started the operation with new process (" + cmd.Id + ").", 2);
                cmd.BeginErrorReadLine();
                cmd.BeginOutputReadLine();
            }
            else
            {
                cmd.Start();
                cmd.WaitForExit();
                
                checkDirectory(textBox1.Text);
                comboBox3.Text = package;
            }
            
        }

        private void Button22_Click(object sender, EventArgs e)
        {
            if (ProList.Count > 0)
            {
                foreach (Process p in ProList)
                {
                    try
                    {
                        p.Kill();
                    }
                    catch
                    {

                    }
                }
                ProList.Clear();
            }
            listView1.Items.Clear();
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = "/c npm install " + comboBox3.Text;
            cmd.StartInfo.WorkingDirectory = textBox1.Text;
            cmd.StartInfo.UseShellExecute = false;
            string package = comboBox3.Text;
            if (checkBox1.Checked)
            {
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.RedirectStandardError = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.RedirectStandardInput = false;
                cmd.EnableRaisingEvents = true;
                cmd.OutputDataReceived += (a, b) => addToConsole("P", b.Data, 0);
                cmd.ErrorDataReceived += (a, b) => addToConsole("P", b.Data, 1);
                cmd.Exited += (a, b) => {
                    addToConsole("P", "Package manager process exited.", 2);
                    this.Invoke((MethodInvoker)delegate {
                        checkDirectory(textBox1.Text);
                        comboBox3.Text = package;
                    });
                };
                cmd.Start();
                ProList.Add(cmd);
                addToConsole("P", "Started the operation with new process (" + cmd.Id + ").", 2);
                cmd.BeginErrorReadLine();
                cmd.BeginOutputReadLine();
            }
            else
            {
                cmd.Start();
                cmd.WaitForExit();

                checkDirectory(textBox1.Text);
                comboBox3.Text = package;
            }
        }

        private void Button21_Click(object sender, EventArgs e)
        {
            if (ProList.Count > 0)
            {
                foreach (Process p in ProList)
                {
                    try
                    {
                        p.Kill();
                    }
                    catch
                    {

                    }
                }
                ProList.Clear();
            }
            listView1.Items.Clear();
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = "/c yarn remove " + comboBox3.Text;
            cmd.StartInfo.WorkingDirectory = textBox1.Text;
            cmd.StartInfo.UseShellExecute = false;
            string package = comboBox3.Text;
            if (checkBox1.Checked)
            {
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.RedirectStandardError = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.RedirectStandardInput = false;
                cmd.EnableRaisingEvents = true;
                cmd.OutputDataReceived += (a, b) => addToConsole("P", b.Data, 0);
                cmd.ErrorDataReceived += (a, b) => addToConsole("P", b.Data, 1);
                cmd.Exited += (a, b) => {
                    addToConsole("P", "Package manager process exited.", 2);
                    this.Invoke((MethodInvoker)delegate {
                        checkDirectory(textBox1.Text);
                        comboBox3.Text = package;
                    });
                };
                cmd.Start();
                ProList.Add(cmd);
                addToConsole("P", "Started the operation with new process (" + cmd.Id + ").", 2);
                cmd.BeginErrorReadLine();
                cmd.BeginOutputReadLine();
            }
            else
            {
                cmd.Start();
                cmd.WaitForExit();

                checkDirectory(textBox1.Text);
                comboBox3.Text = package;
            }
        }

        private void Button20_Click(object sender, EventArgs e)
        {
            if (ProList.Count > 0)
            {
                foreach (Process p in ProList)
                {
                    try
                    {
                        p.Kill();
                    }
                    catch
                    {

                    }
                }
                ProList.Clear();
            }
            listView1.Items.Clear();
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = "/c npm uninstall " + comboBox3.Text;
            cmd.StartInfo.WorkingDirectory = textBox1.Text;
            cmd.StartInfo.UseShellExecute = false;
            string package = comboBox3.Text;
            if (checkBox1.Checked)
            {
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.RedirectStandardError = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.RedirectStandardInput = false;
                cmd.EnableRaisingEvents = true;
                cmd.OutputDataReceived += (a, b) => addToConsole("P", b.Data, 0);
                cmd.ErrorDataReceived += (a, b) => addToConsole("P", b.Data, 1);
                cmd.Exited += (a, b) => {
                    addToConsole("P", "Package manager process exited.", 2);
                    this.Invoke((MethodInvoker)delegate {
                        checkDirectory(textBox1.Text);
                        comboBox3.Text = package;
                    });
                };
                cmd.Start();
                ProList.Add(cmd);
                addToConsole("P", "Started the operation with new process (" + cmd.Id + ").", 2);
                cmd.BeginErrorReadLine();
                cmd.BeginOutputReadLine();
            }
            else
            {
                cmd.Start();
                cmd.WaitForExit();

                checkDirectory(textBox1.Text);
                comboBox3.Text = package;
            }
        }

        private void Button18_Click(object sender, EventArgs e)
        {
            if (ProList.Count > 0)
            {
                foreach (Process p in ProList)
                {
                    try
                    {
                        p.Kill();
                    }
                    catch
                    {

                    }
                }
                ProList.Clear();
            }
            listView1.Items.Clear();
            //yarn upgrade left - pad--latest
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = "/c yarn upgrade " + comboBox3.Text + " --latest";
            cmd.StartInfo.WorkingDirectory = textBox1.Text;
            cmd.StartInfo.UseShellExecute = false;
            string package = comboBox3.Text;
            if (checkBox1.Checked)
            {
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.RedirectStandardError = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.RedirectStandardInput = false;
                cmd.EnableRaisingEvents = true;
                cmd.OutputDataReceived += (a, b) => addToConsole("P", b.Data, 0);
                cmd.ErrorDataReceived += (a, b) => addToConsole("P", b.Data, 1);
                cmd.Exited += (a, b) => {
                    addToConsole("P", "Package manager process exited.", 2);
                    this.Invoke((MethodInvoker)delegate {
                        checkDirectory(textBox1.Text);
                        comboBox3.Text = package;
                    });
                };
                cmd.Start();
                ProList.Add(cmd);
                addToConsole("P", "Started the operation with new process (" + cmd.Id + ").", 2);
                cmd.BeginErrorReadLine();
                cmd.BeginOutputReadLine();
            }
            else
            {
                cmd.Start();
                cmd.WaitForExit();

                checkDirectory(textBox1.Text);
                comboBox3.Text = package;
            }
        }

        private void Button19_Click(object sender, EventArgs e)
        {
            if (ProList.Count > 0)
            {
                foreach (Process p in ProList)
                {
                    try
                    {
                        p.Kill();
                    }
                    catch
                    {

                    }
                }
                ProList.Clear();
            }
            listView1.Items.Clear();
            //npm update --save package_name
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = "/c npm install --save " + comboBox3.Text;
            cmd.StartInfo.WorkingDirectory = textBox1.Text;
            cmd.StartInfo.UseShellExecute = false;
            string package = comboBox3.Text;
            if (checkBox1.Checked)
            {
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.RedirectStandardError = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.RedirectStandardInput = false;
                cmd.EnableRaisingEvents = true;
                cmd.OutputDataReceived += (a, b) => addToConsole("P", b.Data, 0);
                cmd.ErrorDataReceived += (a, b) => addToConsole("P", b.Data, 1);
                cmd.Exited += (a, b) => {
                    addToConsole("P", "Package manager process exited.", 2);
                    this.Invoke((MethodInvoker)delegate {
                        checkDirectory(textBox1.Text);
                        comboBox3.Text = package;
                    });
                };
                cmd.Start();
                ProList.Add(cmd);
                addToConsole("P", "Started the operation with new process (" + cmd.Id + ").", 2);
                cmd.BeginErrorReadLine();
                cmd.BeginOutputReadLine();
            }
            else
            {
                cmd.Start();
                cmd.WaitForExit();

                checkDirectory(textBox1.Text);
                comboBox3.Text = package;
            }
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

        private void button25_Click_1(object sender, EventArgs e)
        {
            if (ProList.Count > 0)
            {
                foreach (Process p in ProList)
                {
                    try
                    {
                        p.Kill();
                    }
                    catch
                    {

                    }
                }
                ProList.Clear();
            }
            listView1.Items.Clear();
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = "/c npx react-native start";
            cmd.StartInfo.WorkingDirectory = textBox1.Text;
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.RedirectStandardError = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.RedirectStandardInput = false;
            cmd.EnableRaisingEvents = true;
            cmd.OutputDataReceived += (a, b) => addToConsole("Node", b.Data, 3);
            cmd.ErrorDataReceived += (a, b) => addToConsole("Node", b.Data, 4);
            cmd.Exited += (a, b) => addToConsole("Node", "Node process exited.", 2);
            cmd.Start();
            ProList.Add(cmd);
            addToConsole("Node", "Started the Nod Server with new process (" + cmd.Id + ").", 2);
            cmd.BeginErrorReadLine();
            cmd.BeginOutputReadLine();
        }

        private void button27_Click_1(object sender, EventArgs e)
        {
            try
            {
                string atom = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "atom\\atom.exe");
                Process cmd = new Process();
                cmd.StartInfo.FileName = atom;
                cmd.StartInfo.Arguments = textBox1.Text;
                cmd.StartInfo.WorkingDirectory = textBox1.Text;
                cmd.StartInfo.UseShellExecute = false;
                cmd.Start();
            }
            catch
            {

            }

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void label30_Click(object sender, EventArgs e)
        {

        }

        private void label29_Click(object sender, EventArgs e)
        {

        }

        private void label32_Click(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                activateLightMood();
                RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ReactNativeCmdEase");
                if (key != null)
                {
                    key.SetValue("mood", "light");
                }
                //ComboBox3_TextChanged(null, EventArgs.Empty);
                if(label29.Text == "[error]")
                {
                    label29.ForeColor = Color.Red;
                }else if (label29.Text == "[not found]")
                {
                    label29.ForeColor = Color.Red;
                }
                else if (label29.Text == "[present]")
                {
                    label29.ForeColor = Color.Green;
                }

                if(label28.Cursor == Cursors.Hand)
                {
                    label28.ForeColor = Color.Blue;
                }
            }
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
            this.listView1.BackColor = Color.White;
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

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                activateDarkMood();
                RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ReactNativeCmdEase");
                if (key != null)
                {
                    key.SetValue("mood", "dark");
                }
                //ComboBox3_TextChanged(null, EventArgs.Empty);
                if (label29.Text == "[error]")
                {
                    label29.ForeColor = Color.Red;
                }
                else if (label29.Text == "[not found]")
                {
                    label29.ForeColor = Color.Red;
                }
                else if (label29.Text == "[present]")
                {
                    label29.ForeColor = Color.Green;
                }

                if (label28.Cursor == Cursors.Hand)
                {
                    label28.ForeColor = Color.Blue;
                }
            }
        }

        private void Center(Form form)
        {
            form.Location = new Point((Screen.PrimaryScreen.Bounds.Size.Width / 2) - (form.Size.Width / 2), (Screen.PrimaryScreen.Bounds.Size.Height / 2) - (form.Size.Height / 2));
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                this.Width = 1322;
                Center(this);
                checkBox2.Enabled = true;
            }
            else
            {
                this.Width = 815;
                Center(this);
                checkBox2.Checked = false;
                checkBox2.Enabled = false;
            }
        }

        private void button28_Click(object sender, EventArgs e)
        {
            foreach (Process p in Process.GetProcessesByName("java"))
            {
                try
                {
                    addToConsole("CLEAN", p.ProcessName + " is terminating to clean the memory" , 0);
                    p.Kill();
                    
                }
                catch(Exception ec)
                {
                    addToConsole("CLEAN", p.ProcessName + " - " +  ec.Message, 1);
                }

            }
            foreach (Process p in Process.GetProcessesByName("node"))
            {
                try
                {
                    addToConsole("CLEAN", p.ProcessName + " is terminating to clean the memory", 0);
                    p.Kill();
                }
                catch (Exception ec)
                {
                    addToConsole("CLEAN", p.ProcessName + " - " + ec.Message, 1);
                }

            }
            foreach (Process p in Process.GetProcessesByName("cmd"))
            {
                try
                {
                    addToConsole("CLEAN", p.ProcessName + " is terminating to clean the memory", 0);
                    p.Kill();
                }
                catch (Exception ec)
                {
                    addToConsole("CLEAN", p.ProcessName + " - " + ec.Message, 1);
                }

            }
            foreach (Process p in Process.GetProcessesByName("conhost"))
            {
                try
                {
                    addToConsole("CLEAN", p.ProcessName + " is terminating to clean the memory", 0);
                    p.Kill();
                }
                catch (Exception ec)
                {
                    addToConsole("CLEAN", p.ProcessName + " - " + ec.Message, 1);
                }

            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                //location 810 Size width 484, 1294, 640 173
                listView1.Left = 14;
                listView1.Width = 1280;
                checkBox2.Location = new Point(13, 7);
                listView1.Columns[0].Width = 1246;
                foreach (Control c in this.Controls)
                {
                    if (c.Name != "listView1" && c.Name != "checkBox2" && c.Name != "label38")
                    {
                        c.Visible = false;
                    }
                }
            }
            else
            {
                listView1.Left = 810;
                listView1.Width = 484;
                checkBox2.Location = new Point(640, 173);
                listView1.Columns[0].Width = 450;
                foreach (Control c in this.Controls)
                {
                    if (c.Name != "listView1" && c.Name != "checkBox2" && c.Name != "label38" && c.Name != "label10" && c.Name != "label14")
                    {
                        c.Visible = true;
                    }
                }
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label37_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

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
            foreach (string line in output.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                comboBox1.Items.Add(line);
                comboBox1.Text = line;
                if (!button11.Enabled) button11.Enabled = true;
                if (!button12.Enabled) button12.Enabled = true;
            }
        }

        private void Label14_Click(object sender, EventArgs e)
        {

        }
    }
}
