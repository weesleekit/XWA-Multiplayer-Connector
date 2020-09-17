using System;
using System.IO;
using XWA_Multiplayer_Connector.Enums;

namespace XWA_Multiplayer_Connector.Classes.Missions
{
    public class Mission
    {
        //Constants

        /// <summary>
        /// This is the folder to find the skirmish folder from the XWA exe file
        /// </summary>
        private const string skirmishFolderSubPath = @"\SKIRMISH\";


        //Fields

        /// <summary>
        /// The coop mission file path (without extensions)
        /// </summary>
        public readonly string missionFilePathWithoutExtension;

        /// <summary>
        /// The file name (without extension)
        /// </summary>
        public string FileName { get; }

        public Battle Battle { get; }
        public int MissionNumber { get; }
        public int PlayerNumber { get; }
        public string BattleName { get; }
        public string MissionName { get; }

        //Constructor

        public Mission(string missionFilePathWithoutExtension)
        {
            //Store the path
            this.missionFilePathWithoutExtension = missionFilePathWithoutExtension;

            //Find and store the filename
            FileInfo missionFileInfo = new FileInfo(missionFilePathWithoutExtension);
            FileName = missionFileInfo.Name;

            //Check for both file types to be there
            if (!File.Exists(missionFilePathWithoutExtension + ".skm"))
            {
                throw new Exception($"Missing skm file from {FileName}");
            }

            if (!File.Exists(missionFilePathWithoutExtension + ".tie"))
            {
                throw new Exception($"Missing tie file from {FileName}");
            }

            //Parse the filename

            if (FileName.Length != 8 && FileName.Length != 9)
            {
                throw new Exception($"Incorrect length on {FileName}");
            }

            if (!int.TryParse(FileName.Substring(1, 1), out int battleNumber))
            {
                throw new Exception($"Could not parse battle number on {FileName}");
            }

            if (!int.TryParse(FileName.Substring(3, 1), out int missionNumber))
            {
                throw new Exception($"Could not parse battle number on {FileName}");
            }

            //This is ugly I know, I am too stubborn to learn regex
            if (FileName.Length == 8)
            {
                if (!int.TryParse(FileName.Substring(5, 1), out int playerNumber))
                {
                    throw new Exception($"Could not parse battle number on {FileName}");
                }
                PlayerNumber = playerNumber;
            }
            else if (FileName.Length == 9)
            {
                if (!int.TryParse(FileName.Substring(5, 2), out int playerNumber))
                {
                    throw new Exception($"Could not parse battle number on {FileName}");
                }
                PlayerNumber = playerNumber;
            }
            else
            {
                throw new Exception("Unexpected number of characters");
            }

            Battle = (Battle)battleNumber;
            MissionNumber = missionNumber;

            //Lookup the descriptions
            
            BattleName = Battle.ToString().Replace('_',' ');
            MissionName = MissionReference.battleIDtoMissionName[Battle][MissionNumber - 1];
        }

        // Public Methods

        /// <summary>
        /// Copies in the skm file so that the host can select the mission.
        /// </summary>
        /// <param name="exePath">The exe path used to determine where to create the file</param>
        /// <param name="destination">The full file path for the skm file for easy cleanup later</param>
        /// <param name="feedback">The error message text (only set on a failure; when it returns false)</param>
        /// <returns>Return indicates success</returns>
        public bool PrepHostFile(string exePath, out string destination, out string feedback)
        {
            //Get the exe directory
            FileInfo missionFileInfo = new FileInfo(exePath);
            
            //Determine the destination
            destination = missionFileInfo.DirectoryName + skirmishFolderSubPath + BattleName + " - " + MissionName + ".skm" ;

            //Determine the source
            string source = missionFilePathWithoutExtension + ".skm";

            try
            {
                File.Copy(source, destination, true);
            }
            catch (Exception ex)
            {
                feedback = ex.Message;
                return false;
            }

            feedback = "";
            return true;
        }

        public bool CopyInTempTieFile(string exePath, out string feedback)
        {
            //Get the exe directory
            FileInfo missionFileInfo = new FileInfo(exePath);

            //Determine the destination
            string destination = missionFileInfo.DirectoryName + skirmishFolderSubPath + "temp.tie";

            //Determine the source
            string source = missionFilePathWithoutExtension + ".tie";

            try
            {
                File.Copy(source, destination, true);
            }
            catch (Exception ex)
            {
                feedback = ex.Message;
                return false;
            }

            feedback = "";
            return true;
        }

        //Public Overrides

        public override string ToString()
        {
            return $"{BattleName} - {MissionName} - {PlayerNumber} Players";
        }
    }
}
