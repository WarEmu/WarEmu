
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

namespace ServerLauncher
{
    public partial class Form1 : Form
    {
        public Process AccountCacher;
        public Process LauncherServer;
        public Process LobbyServer;
        public Process WorldServer;

        public Form1()
        {
            InitializeComponent();
        }

        private void B_start_Click(object sender, EventArgs e)
        {
            B_start.Enabled = false;

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
            }

            B_start.Enabled = true;
        }

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
    }
}
