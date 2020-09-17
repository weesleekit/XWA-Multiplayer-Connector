using Lidgren.Network;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using XWA_Multiplayer_Connector.Classes.Missions;
using XWA_Multiplayer_Connector.Classes.Models;
using XWA_Multiplayer_Connector.Classes.Networking;
using XWA_Multiplayer_Connector.Classes.Networking.Payloads.Server.New;
using XWA_Multiplayer_Connector.Interfaces;
using static XWA_Multiplayer_Connector.Classes.Networking.LidgrenObject;
using static XWA_Multiplayer_Connector.Classes.Networking.Payloads.Client.ClientMessageTypes;
using static XWA_Multiplayer_Connector.Classes.Networking.Payloads.Server.ServerMessageTypes;

namespace XWA_Multiplayer_Connector.Forms
{
    public partial class HostForm : Form, ILidgrenConsoleOutputGUI, ILidgrenServerEvents
    {
        //Constants

        private readonly Color loadedAwaiting = Color.Yellow;
        private readonly Color loadedSuccess = Color.Lime;
        private readonly Color loadedFailure = Color.PaleVioletRed;

        //Fields

        private readonly Config config;

        private readonly ILidgrenServer lidgrenServer;

        private readonly List<ClientPlayer> clientPlayers = new List<ClientPlayer>();

        private Mission selectedMission = null;

        private LogLevel? logLevelVisible = null;

        //Properties

        private Mission SelectedMission
        {
            get
            {
                return selectedMission;
            }
            set
            {
                if (value == null)
                {
                    textBoxMissionInfo.Text = "";
                    buttonSendTempTie.Enabled = false;
                }
                else
                {
                    textBoxMissionInfo.Text = $"Battle: {Environment.NewLine} {value.BattleName}" +
                                              Environment.NewLine +
                                              Environment.NewLine +
                                              $"Mission: {Environment.NewLine} {value.MissionName}" +
                                              Environment.NewLine +
                                              Environment.NewLine +
                                              $"Players: {Environment.NewLine} {value.PlayerNumber}";
                    buttonSendTempTie.Enabled = true;
                }

                selectedMission = value;
            }
        }

        //Constructor

        public HostForm(List<Mission> missions, Config config)
        {
            //Standard setup
            InitializeComponent();

            //Store injection
            this.config = config;

            //Add the missions to the UI
            listBoxMissionSelection.Items.AddRange(missions.ToArray());

            //Copy the mission files into the skirmish folder
            foreach (Mission mission in missions)
            {
                if (!mission.PrepHostFile(config.ExePath, out string destination, out string feedback))
                {
                    LogMessage($"Failure, mission {mission.FileName}, {mission.BattleName}, {mission.MissionName} did not copy in {feedback}");
                }
            }

            //Get the configuration of the server
            if (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["AppIdentifier"]))
            {
                LogMessage("Error, could not start server because AppIdentifier is null empty or whitespace");
                return;
            }
            if (!int.TryParse(ConfigurationManager.AppSettings["Port"], out int port))
            {
                LogMessage("Error, could not start server because Port could not be parsed from config");
                return;
            }
            if (!int.TryParse(ConfigurationManager.AppSettings["MaxConnections"], out int maxConnections))
            {
                LogMessage("Error, could not start server because MaxConnections could not be parsed from config");
                return;
            }

            //Start the server
            lidgrenServer = new LidgrenServer(ConfigurationManager.AppSettings["AppIdentifier"], this, 
                                              this, 
                                              port,
                                              maxConnections);

            //Display the public IP
            LogMessage($"Public IP address: {IPHelper.GetIPAddress()}");

            //Display the local IPs
            //https://stackoverflow.com/questions/6803073/get-local-ip-address
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    LogMessage($"Local IP address: {ip}");
                }
            }
        }

        //Interface Implementation

        void ILidgrenServerEvents.HandleNewMessage(NetConnection netConnection, ClientMessageType messageType, string jsonBody)
        {
            try
            {
                //Try and find the client player
                ClientPlayer clientPlayer = clientPlayers.Find(x => x.netConnection == netConnection);

                //If not found then report error
                if (clientPlayer == null)
                {
                    throw new Exception("Can't find client object from net connection");
                }

                //Switch on the message type and have it handled by specific methods so as to not clutter this area
                switch (messageType)
                {
                    case ClientMessageType.SendName:
                        SendName(clientPlayer, jsonBody);
                        break;

                    default:
                        throw new Exception($"Unhandled message type {messageType}");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Error handling reply message type {messageType} from {netConnection.RemoteEndPoint}, error: {ex.Message}");
            }
        }

        void ILidgrenServerEvents.HandleReplyMessage(NetConnection netConnection, ServerMessageType messageType, int feedback, string jsonBody)
        {
            try
            {
                //Try and find the client player
                ClientPlayer clientPlayer = clientPlayers.Find(x => x.netConnection == netConnection);

                //If not found then report error
                if (clientPlayer == null)
                {
                    throw new Exception("Can't find client object from net connection");
                }

                //Switch on the message type and have it handled by specific methods so as to not clutter this area
                switch (messageType)
                {
                    case ServerMessageType.PrepMission:
                        PrepMission(clientPlayer, feedback, jsonBody);
                        break;

                    default:
                        throw new Exception($"Unhandled message type {messageType}");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Error handling reply message type {messageType} with feedback {feedback} from {netConnection.RemoteEndPoint}, error: {ex.Message}");
            }
        }

        void ILidgrenServerEvents.LostConnection(NetConnection netConnection)
        {
            //Try and find a player connected
            ClientPlayer clientPlayer = clientPlayers.Find(x => x.netConnection == netConnection);

            //If found
            if (clientPlayer != null)
            {
                //Remove it from the list
                clientPlayers.Remove(clientPlayer);

                //Remove it from the UI
                colourableListBoxClients.RemoveItem(clientPlayer);
            }
        }

        void ILidgrenServerEvents.NewConnectionApproved(NetConnection netConnection)
        {
            //Create the player
            ClientPlayer clientPlayer = new ClientPlayer(netConnection);

            //Add it to the list
            clientPlayers.Add(clientPlayer);

            //Add it to the UI
            colourableListBoxClients.AddItemWithColour(clientPlayer);
        }

        void ILidgrenConsoleOutputGUI.WriteConsole(string message, LogLevel messageLevel)
        {
            //If the log level visible hasn't been found yet
            if (logLevelVisible == null)
            {
                //Try and parse the config
                if (Enum.TryParse(ConfigurationManager.AppSettings["LogLevelVisible"], out LogLevel parsedLogLevel))
                {
                    //On success, set the level
                    logLevelVisible = parsedLogLevel;
                }
                else
                {
                    //On failure, log it and assume a level
                    LogMessage("Could not parse LogLevelVisible from config, assuming a level of High");
                    logLevelVisible = LogLevel.High;
                }
            }

            //If the visiblity is set high enough to be able to see this message then output it
            if (logLevelVisible >= messageLevel)
            {
                LogMessage(message);
            }
        }

        //Events

        /// <summary>
        /// Server beat every x seconds
        /// </summary>
        private void TimerBeat_Tick(object sender, EventArgs e)
        {
            if (lidgrenServer != null)
            {
                lidgrenServer.Beat();
            }
        }

        /// <summary>
        /// The user selects a mission
        /// </summary>
        private void ListBoxMissionSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            //If nothing selected
            if (listBoxMissionSelection.SelectedItem == null)
            {
                //Set it to null
                SelectedMission = null;
            }
            else
            {
                //Otherwise assign the mission
                SelectedMission = (Mission)listBoxMissionSelection.SelectedItem;
            }
        }

        /// <summary>
        /// The close the application
        /// </summary>
        private void HostForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Shut down the server gracefully
            lidgrenServer.ShutDown("User closed server");

            //Show the parent form
            Owner.Show();
        }

        /// <summary>
        /// The user wants to both load the temp tie mission file and also send this to all the clients
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonSendTempTie_Click(object sender, EventArgs e)
        {   
            if (SelectedMission == null)
            {
                LogMessage($"Select a mission first!");
                return;
            }

            //Try and Copy in the mission file
            if (SelectedMission.CopyInTempTieFile(config.ExePath, out string feedback))
            {
                LogMessage($"Success on local machine");
            }
            else
            {
                //On failure, write to the console
                LogMessage($"Failure, unable to copy temp.tie. {feedback}");

                //And exit this method
                return;
            }

            //On success write to the console
            LogMessage("Successfully loaded mission on local machine");

            //Create an outgoing payload
            var payloadModel = new PrepMission
            {
                MissionFileName = selectedMission.FileName,
            };

            //Convert payload to json
            string jsonBody = JsonConvert.SerializeObject(payloadModel);

            //Send the payload to each client
            foreach (ClientPlayer clientPlayer in clientPlayers)
            {
                //Log who it is being sent to
                LogMessage($"Sending to {clientPlayer.Name}");

                //Set the player as red on the UI
                colourableListBoxClients.ChangeItemColour(clientPlayer, loadedAwaiting);

                //Send the network instruction
                SendNewMessage(clientPlayer.netConnection, ServerMessageType.PrepMission, jsonBody);
            }

            colourableListBoxClients.Refresh();
        }

        //Handler Methods for replies from Server Messages

        /// <summary>
        /// The server has told the client to load the mission
        /// </summary>
        private void PrepMission(ClientPlayer clientPlayer, int feedback, string jsonBody)
        {
            //Just getting rid of the Message check. We don't use the body but it's nice to standardise the parameters.
            //I think in time, they should be moved to a standard type of event where feedback and body are included in every even if not used.
            if (jsonBody == null)
            {

            }

            switch((PrepMission.Feedback)feedback)
            {
                case Classes.Networking.Payloads.Server.New.PrepMission.Feedback.Success:

                    //Set the player in the listbox to green
                    colourableListBoxClients.ChangeItemColour(clientPlayer, loadedSuccess);

                    break;

                case Classes.Networking.Payloads.Server.New.PrepMission.Feedback.Failure:
                    
                    //Set the player in the listbox to red
                    colourableListBoxClients.ChangeItemColour(clientPlayer, loadedFailure);
                    break;

                default:
                    throw new Exception("Unhandled enum");
            }

            //Refresh the UI
            colourableListBoxClients.Refresh();
        }

        //Handler Methods for new Client Messages

        /// <summary>
        /// The client has told the server it has a name
        /// </summary>
        private void SendName(ClientPlayer clientPlayer, string jsonBody)
        {
            //Deserialise the incoming payload
            var incomingPayload = JsonConvert.DeserializeObject<Classes.Networking.Payloads.Client.New.SendName>(jsonBody);

            //Update the player with the new name
            clientPlayer.Name = incomingPayload.ClientPlayerName;

            //Refresh the UI
            colourableListBoxClients.Refresh();

            //Reply to the client just for fun
            SendReplyMessage(clientPlayer.netConnection, ClientMessageType.SendName, (int)Classes.Networking.Payloads.Client.New.SendName.Feedback.ThatsACoolNameBro);
        }

        //Private Methods

        private void LogMessage(string message)
        {
            //If the form has already been disposed of
            if (textBoxLog.IsDisposed)
            {
                //Just exit without writing message
                return;
            }

            textBoxLog.AppendText(DateTime.Now.ToString());
            textBoxLog.AppendText(" ");
            textBoxLog.AppendText(message);
            textBoxLog.AppendText(Environment.NewLine);
        }

        /// <summary>
        /// Called when creating a new message from the server
        /// </summary>
        private void SendNewMessage(NetConnection netConnection, ServerMessageType messageType, string jsonBody = null)
        {
            //Determine how to send the data
            DetermineMethodandChannel(messageType, out NetDeliveryMethod deliverymethod, out int? sequenceChannel);

            //Send the message
            lidgrenServer.SendMessage(true, (byte)messageType, netConnection, deliverymethod, null, sequenceChannel, jsonBody);
        }

        /// <summary>
        /// Called when replying to messages recieved from the client
        /// </summary>
        private void SendReplyMessage(NetConnection netConnection, ClientMessageType messageType, int feedback, string jsonBody = null)
        {
            //Determine how to send the data
            DetermineMethodandChannel(messageType, out NetDeliveryMethod deliverymethod, out int? sequenceChannel);

            //Send the message
            lidgrenServer.SendMessage(false, (byte)messageType, netConnection, deliverymethod, feedback, sequenceChannel, jsonBody);
        }
    }
}
