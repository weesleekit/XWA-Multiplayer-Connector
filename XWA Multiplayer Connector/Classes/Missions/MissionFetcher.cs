using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace XWA_Multiplayer_Connector.Classes.Missions
{
    public static class MissionFetcher
    {
        //Constants

        private const string xwaCoopFilesFolder = "XWA Coop Files";

        //Public Methods

        public static List<Mission> FetchMissions()
        {
            List<Mission> missions = new List<Mission>();

            //Get all the mission files
            string[] coopFileNames = Directory.GetFiles(xwaCoopFilesFolder);

            //Go through each of them
            foreach (string fileName in coopFileNames)
            {
                //Split out to try and get the filetype
                string[] splitForExtension = fileName.Split('.');

                //If it's not the expected format then exit with exception
                if (splitForExtension.Length != 2)
                {
                    Console.WriteLine($"Unable to handle filename (too many or not enough periods): {fileName}");
                    continue;
                }

                //If it isn't the skm type then move on (so we don't double add the mission)
                if (splitForExtension[1] != "skm")
                {
                    //Don't need to write a console item as we were expecting .tie files to make their way into the file names list
                    continue;
                }

                //Check to see make sure there is a corresponding tie file
                if (!coopFileNames.Contains(splitForExtension[0] + ".tie"))
                {
                    Console.WriteLine($"Unable to add filename (missing the corresponding .tie file): {fileName}");
                    continue;
                }

                //Create a new mission based on the filename
                Mission mission = new Mission(splitForExtension[0]);

                //Add it to the list
                missions.Add(mission);
            }

            //Return the mission list
            return missions;
        }
    }
}
