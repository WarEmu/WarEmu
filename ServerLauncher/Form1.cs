#region Libraries
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
#endregion

namespace ServerLauncher
{
    public partial class Form1 : Form
    {
        public Process AccountCacher;
        public Process LauncherServer;
        public Process LobbyServer;
        public Process WorldServer;
        public Process GameLauncher;
        static public string pathToGameExecutable;
        static public string pathToGameFolder;

        public Form1()
        {
            InitializeComponent();
            // Je Fawk | 13 April 2014 | Getting the file path saved setting
            pathToGameExecutable = Properties.Settings.Default.setting_pathToGameExecutable;
            textBox_path.Text = pathToGameExecutable;
            // Worse case scenario for the length - C:\WAR.exe = 9 characters
            if (pathToGameExecutable.Length > 9)
            {
                pathToGameFolder = pathToGameExecutable.Remove(pathToGameExecutable.IndexOf("WAR.exe"));
            }

        }
        #region [Event] Button start on click (Click)
        private void B_start_Click(object sender, EventArgs e)
        {
            // Je Fawk | 13 April 2014 | Verifying that the path is valid
            if (textBox_path.Text.Length > 9)
            {

                B_start.Enabled = false;

                // Je Fawk | 13 April 2014 | Creating the game path file 
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Application.StartupPath + "\\GamePath.txt"))
                {
                    file.WriteLine(pathToGameExecutable);
                    file.WriteLine(pathToGameFolder);
                }

                if (StartAccountCheckBox.Checked)
                {
                    AccountCacher = new Process();
                    AccountCacher.StartInfo.FileName = "AccountCacher.exe";
                    AccountCacher.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                    AccountCacher.Start();

                    Thread.Sleep(500);
                }

                if (StartLauncherCheckBox.Checked)
                {
                    LauncherServer = new Process();
                    LauncherServer.StartInfo.FileName = "LauncherServer.exe";
                    LauncherServer.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                    LauncherServer.Start();

                    Thread.Sleep(500);
                }

                if (StartLobbyCheckBox.Checked)
                {
                    LobbyServer = new Process();
                    LobbyServer.StartInfo.FileName = "LobbyServer.exe";
                    LobbyServer.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                    LobbyServer.Start();

                    Thread.Sleep(500);
                }

                if (StartWorldCheckBox.Checked)
                {
                    WorldServer = new Process();
                    WorldServer.StartInfo.FileName = "WorldServer.exe";
                    WorldServer.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                    WorldServer.Start();
                    // Je Fawk | 13 April 2014 | Added to wait for the console to appear before starting the Launcher.exe
                    Thread.Sleep(500);
                }

                // Je Fawk | 13 April 2014 | Starting the Launcher.exe from \bin\Release\Launcher\Launcher.exe
                #region Starting the Warhammer Online Launcher
                GameLauncher = new Process();
                GameLauncher.StartInfo.FileName = Application.StartupPath + "\\Launcher\\Launcher.exe";
                GameLauncher.Start();
                #endregion

                B_start.Enabled = true;

            }
            else
            {
                MessageBox.Show("The path is too small.");
            }
        }
        #endregion

        #region [Event] Button stop on click (Click)
        private void B_stop_Click(object sender, EventArgs e)
        {
            try
            {
                if( WorldServer != null ) WorldServer.Kill();
            }
            catch (Exception) { }
            
            try
            {
                if (LobbyServer != null) LobbyServer.Kill();
            }
            catch (Exception) { }
            
            try
            {
                if (LauncherServer != null) LauncherServer.Kill();
            }
            catch (Exception) { }
            
            try
            {
                if (AccountCacher != null) AccountCacher.Kill();
            }
            catch (Exception) { }
        }
        #endregion

        // Je Fawk | 13 April 2014 | Browse button click event
        #region [Event] Button browse for path on click (Click)
        private void button_browse_Click(object sender, EventArgs e)
        {
            bool foundGameExecutable = false;
            bool savedSettings = true;

            OpenFileDialog openFileDialog_executablePath = new OpenFileDialog();

            // Reading the OpenFileDialog button
            DialogResult result = openFileDialog_executablePath.ShowDialog();
            if (result == DialogResult.OK)
            {
                // Getting the full file path
                string fileName = openFileDialog_executablePath.FileName;
                // Splitting the path by \ in order to try and find if the user actually has war.exe in the path
                string[] pathSplit = openFileDialog_executablePath.FileName.Split('\\');

                foreach (string pathSplitPart in pathSplit)
                {
                    #region Comparing the splitted string lower case to war.exe
                    if (String.Compare(pathSplitPart.ToLower(), "war.exe") == 0)
                    {
                        foundGameExecutable = true;
                    }
                    #endregion
                }

                // If the game executable was found
                if (foundGameExecutable)
                {
                    #region <Try> Save the settings
                    try
                    {
                        Properties.Settings.Default.setting_pathToGameExecutable = openFileDialog_executablePath.FileName;
                        //Properties.Settings.Default["setting_pathToGameExecutable"] = openFileDialog_executablePath.FileName; -- hmmmm
                        Properties.Settings.Default.Save();
                    }
                    #endregion
                    #region <Catch>
                    catch (Exception exp)
                    {
                        savedSettings = false;
                        MessageBox.Show("Error while trying to save the path (the application settings): " + exp.Message);
                    }
                    #endregion
                    #region If the settings were saved successfully announce the user
                    if (savedSettings)
                    {
                        pathToGameExecutable = openFileDialog_executablePath.FileName;
                        pathToGameFolder = pathToGameExecutable.Remove(pathToGameExecutable.IndexOf("WAR.exe"));
                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(Application.StartupPath + "\\GamePath.txt"))
                        {
                            file.WriteLine(pathToGameExecutable);
                            file.WriteLine(pathToGameFolder);
                        }
                        textBox_path.Text = pathToGameExecutable;
                    }
                    #endregion
                }
                if (!foundGameExecutable)
                {
                    MessageBox.Show("The path does not point to WAR.exe");
                }
            }
        }
        #endregion

        // Je Fawk | 13 April 2014 | Form closing event that will kill all the processes
        #region [Event] Form on closing (Closing)
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (WorldServer != null) WorldServer.Kill();
            }
            catch (Exception) { }

            try
            {
                if (LobbyServer != null) LobbyServer.Kill();
            }
            catch (Exception) { }

            try
            {
                if (LauncherServer != null) LauncherServer.Kill();
            }
            catch (Exception) { }

            try
            {
                if (AccountCacher != null) AccountCacher.Kill();
            }
            catch (Exception) { }
        }
        #endregion
    }
}
