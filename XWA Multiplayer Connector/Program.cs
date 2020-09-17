using System;
using System.Collections.Generic;
using System.Windows.Forms;
using XWA_Multiplayer_Connector.Classes.Missions;
using XWA_Multiplayer_Connector.Forms;

namespace XWA_Multiplayer_Connector
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Attempt to fetch the missions
            List<Mission> missions = MissionFetcher.FetchMissions();

            //If there are no missions then close the application
            if (missions.Count == 0)
            {
                MessageBox.Show("No missions found, aborting");
                return;
            }

            //Create the main 
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new XWAMainMenu(missions));
        }
    }
}
