using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;

namespace Launcher
{
    public partial class Accueil : Form
    {
        static public Accueil Acc;
        // Je Fawk | 13 April 2014 | Adding varibles to hold the path of the game exe and folder
        public static string pathToGameExecutable;
        public static string pathToGameFolder;
        bool isConnected = false;

        public Accueil()
        {
            InitializeComponent();
            Acc = this;
            // Je Fawk | 13 April 2014 | Reading the game path that was written b the ServerLauncher
            // Explanation notes:
            //   Application.StartupPath.Remove(Application.StartupPath.LastIndexOf('\\')) + "\\GamePath.txt")
            //   * gets the application startup path
            //   * substracts the last \
            //   * adds the GamePath.txt to it
            using (System.IO.StreamReader streamReader = new StreamReader(Application.StartupPath.Remove(Application.StartupPath.LastIndexOf('\\')) + "\\GamePath.txt"))
            {
                pathToGameExecutable = streamReader.ReadLine();
                pathToGameFolder = streamReader.ReadLine();
            }
        }

        // Je Fawk | 13 April 2014 | Commenting Form1_Load as it's unused right now; 
        // If you plan on using it you must add the event again since it's deleted
        #region Old code
        //private void Form1_Load(object sender, EventArgs e)
        //{

        //}
        #endregion

        private void Disconnect(object sender, FormClosedEventArgs e)
        {
            Client.Close();
        }

        private void B_start_Click(object sender, EventArgs e)
        {
            string Username = T_username.Text.ToLower();
            string NCPass = T_password.Text.ToLower();
            // Je Fawk | 13 April 2014 | Making it fool proof encapsulating everything in the if statement
            if (Username.Length > 1 && NCPass.Length > 1 && textBox_serverIp.Text.Length > 8 && textBox_port.Text.Length > 0)
            {
                // Je Fawk | 13 April 2014 | Getting the server ip and port via interface
                // Putting the server IP from the textBox to Client.IP
                Client.IP = textBox_serverIp.Text.Trim();
                // Putting the server port from the textBox to Client.Port
                Client.Port = Convert.ToInt32(textBox_port.Text.Trim());
                Client.User = Username;

                // Connecting to the client now that we have all the credentials
                Client.Connect();

                SHA256Managed Sha = new SHA256Managed();
                string CP = Username + ":" + NCPass;
                byte[] Result = Sha.ComputeHash(UTF8Encoding.UTF8.GetBytes(CP));

                PacketOut Out = new PacketOut((byte)Opcodes.CL_START);
                Out.WriteString(Username);
                Out.WriteUInt32((uint)Result.Length);
                Out.Write(Result, 0, Result.Length);

                Client.SendTCP(Out);
                B_start.Enabled = false;

                // Je Fawk | 13 April 2014 | Setting the isConnected to true in case the user checks the realm status
                isConnected = true;
                // Je Fawk | 13 April 2014 | Minimizing the form that gets in the way
                this.WindowState = FormWindowState.Minimized;
            }
            else
            {
                MessageBox.Show("Credentials are too short.");
            }
        }

        public void ReceiveStart()
        {
            B_start.Enabled = true;
        }

        public void Print(string Message)
        {
            // Je Fawk | 13 April 2014 | Modified from \n\r to Environment.NewLine for greater compatibility (don't ask, just seems right to me ;) )
            T_Log.Text += Message + Environment.NewLine;
            #region Old code
            //T_Log.Text += Message + "\n" + "\r";
            #endregion
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Client.Language = L_selection.Text;
            Print("Selection de : " + Client.Language);
            Client.UpdateLanguage();
        }

        private void b_realms_Click(object sender, EventArgs e)
        {
            // Je Fawk | 13 April 2014 | Checking if the application is connected before verifying the realms status
            if (isConnected)
            {
                Client.UpdateRealms();
            }
            else
            {
                MessageBox.Show("Please connect to the server first");
            }
            #region Old code
            //Client.UpdateRealms();
            #endregion
        }

        public void ClearRealms()
        {
            Realms.Rows.Clear();
        }

        public void AddRealm(string Name, bool Online, uint Players, uint Destruction, uint Order)
        {
            Realms.Rows.Add(Name, Online ? "true" : "false", "" + Players, "" + Destruction, "" + Order);
        }
    }
}
