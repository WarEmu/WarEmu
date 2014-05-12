using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Launcher
{
    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {            
            if (!Client.Connect())  //Temponary fix for the realmlist to work

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Accueil());


        }
    }
}
