using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Windows.Forms;
using XWA_Multiplayer_Connector.Classes.Missions;
using XWA_Multiplayer_Connector.Classes.Models;

namespace XWA_Multiplayer_Connector.Forms
{
    public partial class XWAMainMenu : Form
    {
        //Constants

        private const string subFolderAndFilePath = @"\XWA Multiplayer Connector\config.json";

        //Fields

        private readonly FileInfo configFileInfo;

        private readonly List<Mission> missions;

        private readonly Config config = new Config();

        //Constructor

        public XWAMainMenu(List<Mission> missions)
        {
            InitializeComponent();

            //Store injection
            this.missions = missions;

            //Try and load a previously saved configuration
            configFileInfo = new FileInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + subFolderAndFilePath);

            //If the config exists then load it
            if (configFileInfo.Exists)
            {
                try
                {
                    //Read the file
                    string fileContents = File.ReadAllText(configFileInfo.FullName);

                    //Convert into the model
                    Config loadedConfig = JsonConvert.DeserializeObject<Config>(fileContents);

                    //If its not an empty string and the file exists
                    if (!string.IsNullOrWhiteSpace(loadedConfig.ExePath) && File.Exists(loadedConfig.ExePath))
                    {
                        //Store the whole config
                        config = loadedConfig;

                        //Build the path
                        FileInfo exeFileInfo = new FileInfo(loadedConfig.ExePath);

                        //Set the path in case they want to change it later
                        openFileDialog1.InitialDirectory = exeFileInfo.Directory.FullName;
                    }
                }
                catch
                {
                    //Don't need to do anything on catch
                }
            }

            //If the path isn't known
            if (config.ExePath == null)
            {
                //Block out the buttons
                buttonHostServer.Enabled = false;
                buttonJoinServer.Enabled = false;
            }
        }

        //Events

        private void ButtonHostServer_Click(object sender, EventArgs e)
        {
            HostForm hostForm = new HostForm(missions, config);
            Hide();
            hostForm.Show(this);
        }

        private void ButtonJoinServer_Click(object sender, EventArgs e)
        {
            JoinForm joinForm = new JoinForm(missions, config);
            Hide();
            joinForm.Show(this);
        }

        private void ButtonSetup_Click(object sender, EventArgs e)
        {
            //Ask the user to find the exe file
            DialogResult result = openFileDialog1.ShowDialog(this);

            //If they've selected a file
            if (result == DialogResult.OK)
            {
                try
                {
                    //Update the config
                    config.ExePath = openFileDialog1.FileName;

                    //Write the config file
                    WriteConfigFile();

                    //Enable the buttons
                    buttonHostServer.Enabled = true;
                    buttonJoinServer.Enabled = true;

                    //Play a jolly sound
                    SoundPlayer simpleSound = new SoundPlayer(@".\Audio\Great News.wav");
                    simpleSound.Play();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating config", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void XWAMainMenu_FormClosed(object sender, FormClosedEventArgs e)
        {
            WriteConfigFile();
        }

        //Private Methods

        private void WriteConfigFile()
        {
            //Create the payload
            string output = JsonConvert.SerializeObject(config);

            //Create the directory (if it doesn't already exist)
            configFileInfo.Directory.Create();

            //Write the config to disk for future loading
            File.WriteAllText(configFileInfo.FullName, output);
        }
    }
}
