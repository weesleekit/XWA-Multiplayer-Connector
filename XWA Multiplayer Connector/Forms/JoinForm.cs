using Lidgren.Network;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Windows.Forms;
using XWA_Multiplayer_Connector.Classes.Missions;
using XWA_Multiplayer_Connector.Classes.Models;
using XWA_Multiplayer_Connector.Classes.Networking;
using XWA_Multiplayer_Connector.Interfaces;
using static XWA_Multiplayer_Connector.Classes.Networking.LidgrenObject;
using static XWA_Multiplayer_Connector.Classes.Networking.Payloads.Client.ClientMessageTypes;
using static XWA_Multiplayer_Connector.Classes.Networking.Payloads.Server.ServerMessageTypes;

namespace XWA_Multiplayer_Connector.Forms
{
    public partial class JoinForm : Form, ILidgrenConsoleOutputGUI, ILidgrenClientEvents
    {
        //Constants

        private const string connectText = "Connect";
        private const string connectingText = "Connecting...";
        private const string disconnectText = "Disconnect";

        //Fields

        private readonly List<Mission> missions;

        private readonly Config config;

        private ILidgrenClient lidgrenClient = null;

        private LogLevel? logLevelVisible = null;

        //Constructor

        public JoinForm(List<Mission> missions, Config config)
        {
            //Standard setup
            InitializeComponent();

            //Store injection
            this.missions = missions;
            this.config = config;

            //Set up the UI
            buttonConnectDisconnect.Text = connectText;

            if (config.PlayerName != null)
            {
                textBoxPlayerName.Text = config.PlayerName;
            }

            if (config.ServerHostName != null)
            {
                textBoxHostName.Text = config.ServerHostName;
            }
        }

        //Interface Implementation

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

        void ILidgrenClientEvents.OnConnect()
        {
            //Log the message
            LogMessage("Connected to server");

            //Update the UI
            buttonConnectDisconnect.Text = disconnectText;
            buttonConnectDisconnect.Enabled = true;

            //Send the player name to the server

            //Create an outgoing payload
            var payloadModel = new Classes.Networking.Payloads.Client.SendName.OriginPayload
            {
                ClientPlayerName = config.PlayerName,
            };

            //Convert payload to json
            string jsonBody = JsonConvert.SerializeObject(payloadModel);

            //Send the network message
            SendNewMessage(ClientMessageType.SendName, jsonBody);
        }

        void ILidgrenClientEvents.OnDisconnect(string reason)
        {
            //Set the lidgren client to null
            lidgrenClient = null;

            //Update the UI
            buttonConnectDisconnect.Text = connectText;
            buttonConnectDisconnect.Enabled = true;
            textBoxHostName.Enabled = true;
            textBoxPlayerName.Enabled = true;
        }

        void ILidgrenClientEvents.HandleNewMessage(ServerMessageType messageType, string jsonBody)
        {
            try
            {
                //Switch on the message type and have it handled by specific methods so as to not clutter this area
                switch (messageType)
                {
                    case ServerMessageType.PrepMission:
                        PrepMission(jsonBody);
                        break;

                    default:
                        throw new Exception($"Unhandled message type {messageType}");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Error handling reply message type {messageType}, error: {ex.Message}");
            }
        }

        void ILidgrenClientEvents.HandleReplyMessage(ClientMessageType messageType, int feedback, string jsonBody)
        {
            try
            {
                //Switch on the message type and have it handled by specific methods so as to not clutter this area
                switch (messageType)
                {
                    case ClientMessageType.SendName:
                        SendName(feedback, jsonBody);
                        break;

                    default:
                        throw new Exception($"Unhandled message type {messageType}");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Error handling reply message type {messageType} with feedback {feedback}, error: {ex.Message}");
            }
        }

        //Events

        private void ButtonConnectDisconnect_Click(object sender, EventArgs e)
        {
            if (lidgrenClient == null)
            {
                AttemptToConnect();
            }
            else
            {
                Disconnect();
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                AttemptToConnect();
            }
        }

        private void JoinForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //If the client still exists
            if (lidgrenClient != null)
            {
                //Close the server
                lidgrenClient.Shutdown("User closed form");
            }

            //Show the parent form
            Owner.Show();
        }

        //Handler Methods for replies from Client Messages

        /// <summary>
        /// The server has replied to the client after recieving the name
        /// </summary>
        private void SendName(int feedback, string jsonBody)
        {
            //Just getting rid of the Message check. We don't use the body but it's nice to standardise the parameters.
            //I think in time, they should be moved to a standard type of event where feedback and body are included in every even if not used.
            if (jsonBody == null)
            {

            }

            //Just a silly example handler for dealing with server reply messages
            switch ((Classes.Networking.Payloads.Client.SendName.Feedback)feedback)
            {
                case Classes.Networking.Payloads.Client.SendName.Feedback.ThatsACoolNameBro:
                    LogMessage("Server likes your name");
                    break;

                default:
                    throw new Exception("Unhandled enum");
            }
        }

        //Handler Methods for new Server Messages

        /// <summary>
        /// The server has told the client to load the mission
        /// </summary>
        private void PrepMission(string jsonBody)
        {
            //Deserialise the incoming payload
            var incomingPayload = JsonConvert.DeserializeObject<Classes.Networking.Payloads.Server.PrepMission.OriginPayload>(jsonBody);

            //Try and find the mission
            Mission mission = missions.Find(x => x.FileName == incomingPayload.MissionFileName);

            //If the mission was not found
            if (mission == null)
            {
                //Log the Failure
                LogMessage($"Failed to prepare mission {incomingPayload.MissionFileName} Could not find mission");

                //Reply failure to server
                SendReplyMessage(ServerMessageType.PrepMission, (int)Classes.Networking.Payloads.Server.PrepMission.Feedback.Failure);
            }

            //Try to copy in the file
            if (mission.CopyInTempTieFile(config.ExePath, out string feedback))
            {
                //Log the success
                LogMessage($"Successfully prepared mission {mission.BattleName} - {mission.MissionName}");

                //Reply success to server
                SendReplyMessage(ServerMessageType.PrepMission, (int)Classes.Networking.Payloads.Server.PrepMission.Feedback.Success);
            }
            else
            {
                //Log the Failure
                LogMessage($"Failed to prepare mission {mission.BattleName} - {mission.MissionName}. Feedback: {feedback}");

                //Reply failure to server
                SendReplyMessage(ServerMessageType.PrepMission, (int)Classes.Networking.Payloads.Server.PrepMission.Feedback.Failure);
            }
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

        private void AttemptToConnect()
        {
            //Check there isn't already a networking object
            if (lidgrenClient != null)
            {
                LogMessage("Error, new attempt to connect to server but there already exists an connection attempt");
            }

            //Check the hostname and playername
            if (string.IsNullOrWhiteSpace(textBoxHostName.Text))
            {
                LogMessage("Host name is null, empty or whitespace");
                return;
            }
            if (string.IsNullOrWhiteSpace(textBoxPlayerName.Text))
            {
                LogMessage("Player name is null, empty or whitespace");
                return;
            }

            //Store the names in the config (so it can be written to disk on application close)
            config.ServerHostName = textBoxHostName.Text;
            config.PlayerName = textBoxPlayerName.Text;

            //Lock the UI
            textBoxPlayerName.Enabled = false;
            textBoxHostName.Enabled = false;
            buttonConnectDisconnect.Enabled = false;

            //Update the button text
            buttonConnectDisconnect.Text = connectingText;

            //Get the configuration of the client
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

            //Create the client
            lidgrenClient = new LidgrenClient(ConfigurationManager.AppSettings["AppIdentifier"], this,
                                              this, config.ServerHostName, port);

            //Try to connect to the server
            lidgrenClient.AttemptToConnectToServer();
        }

        private void Disconnect()
        {
            LogMessage("Disconnecting from server");
            lidgrenClient.Shutdown("User chose to disconnect");
        }

        /// <summary>
        /// Called when creating a new message from the server
        /// </summary>
        private void SendNewMessage(ClientMessageType messageType, string jsonBody = null)
        {
            //Determine how to send the data
            DetermineMethodandChannel(messageType, out NetDeliveryMethod deliverymethod, out int? sequenceChannel);

            //Send the message
            lidgrenClient.SendMessage(true, (byte)messageType, deliverymethod, null, sequenceChannel, jsonBody);
        }

        /// <summary>
        /// Called when replying to messages recieved from the client
        /// </summary>
        private void SendReplyMessage(ServerMessageType messageType, int feedback, string jsonBody = null)
        {
            //Determine how to send the data
            DetermineMethodandChannel(messageType, out NetDeliveryMethod deliverymethod, out int? sequenceChannel);

            //Send the message
            lidgrenClient.SendMessage(false, (byte)messageType, deliverymethod, (byte)feedback, sequenceChannel, jsonBody);
        }
    }
}
