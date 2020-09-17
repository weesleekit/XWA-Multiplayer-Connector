using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using XWA_Multiplayer_Connector.Interfaces;
using static XWA_Multiplayer_Connector.Classes.Networking.Payloads.Client.ClientMessageTypes;
using static XWA_Multiplayer_Connector.Classes.Networking.Payloads.Server.ServerMessageTypes;

namespace XWA_Multiplayer_Connector.Classes.Networking
{
    class LidgrenServer : LidgrenObject, ILidgrenServer
    {
        //Fields

        /// <summary>
        /// The lidgren object that is the server
        /// </summary>
        private readonly NetServer netServer;

        /// <summary>
        /// The output route for this server
        /// </summary>
        private readonly ILidgrenServerEvents lidgrenServerEvents;

        /// <summary>
        /// List of connected objects
        /// </summary>
        private readonly List<NetConnection> ConnectionList = new List<NetConnection>();

        //Constructor

        public LidgrenServer(string appIdentifier, ILidgrenConsoleOutputGUI lidgrenConsoleOutputGUI, 
                            ILidgrenServerEvents lidgrenServerEvents, int serverPort, int maxConnections)
                            : base(lidgrenConsoleOutputGUI)
        {
            //Store the injection
            this.lidgrenServerEvents = lidgrenServerEvents;

            //Create the server configuration
            NetPeerConfiguration netPeerConfiguration = new NetPeerConfiguration(appIdentifier)
            {
                Port = serverPort,
                MaximumConnections = maxConnections
            };

            //clientconfig.UseMessageRecycling = true;
            //clientconfig.ConnectionTimeout = 10; // ping timeout default 25 secs. Cannot be less than PingInterval
            //clientconfig.PingInterval = 3; // default is 4 secs            
            netPeerConfiguration.EnableMessageType(NetIncomingMessageType.ConnectionApproval);

            //Create the server
            netServer = new NetServer(netPeerConfiguration);
            netServer.RegisterReceivedCallback(new SendOrPostCallback(HandleMessage));
            netServer.Start();

            //Log the message
            lidgrenConsoleOutputGUI.WriteConsole("Server started on Port: " + netPeerConfiguration.Port, LogLevel.Low);
        }

        //Interface functions

        /// <summary>
        /// To be called frequently. Sends a message to each approved connectiont to check they are still there
        /// </summary>
        void ILidgrenServer.Beat()
        {
            //Recycle the message
            NetOutgoingMessage outgoingMessage;

            //Loop through each connected object sending it a beat
            foreach (NetConnection netConnection in ConnectionList)
            {
                //Log the message
                lidgrenConsoleOutputGUI.WriteConsole($"Sending beat to: {netConnection}", LogLevel.Everything);

                //Create the new message
                outgoingMessage = netServer.CreateMessage();

                //Write whether it is a new message from the server (true) or a reply to a message from the client (false)
                outgoingMessage.Write(true);

                //Write true if a body exists to read
                outgoingMessage.Write(false);

                //Pad so that future reading is easily done
                outgoingMessage.WritePadBits();

                //Write the message type (note it's been squished to 8 bits therefore max 256 message types)
                outgoingMessage.Write((byte)0);

                //Send the message
                netServer.SendMessage(outgoingMessage, netConnection, NetDeliveryMethod.Unreliable);
            }
        }

        /// <summary>
        /// Called when the application is closing down
        /// </summary>
        void ILidgrenServer.ShutDown(string reason)
        {
            //Log the message
            lidgrenConsoleOutputGUI.WriteConsole($"Server shut down with reason: {reason}", LogLevel.Low);

            netServer.Shutdown(reason);
        }

        /// <summary>
        /// Sends a message using the lidgren connection
        /// </summary>
        /// <param name="newMessageFromServer">Set to true when it is a new message from the server and not a reply</param>
        /// <param name="messageType">Based on the enum in the child class (Note: limited to 256 types)</param>
        /// <param name="netConnection">The connection to send the message to</param>
        /// <param name="deliverymethod">The delivery method to use</param>
        /// <param name="feedback">This is required if it is a reply type message</param>
        /// <param name="sequenceChannel"></param>
        /// <param name="body"></param>
        void ILidgrenServer.SendMessage(bool newMessageFromServer,
                                   byte messageType,
                                   NetConnection netConnection,
                                   NetDeliveryMethod deliverymethod,
                                   int? feedback,
                                   int? sequenceChannel,
                                   string jsonBody)
        {
            //Log the message
            if (newMessageFromServer)
            {
                //Cast to the enum
                ServerMessageType serverMessageType = (ServerMessageType)messageType;

                //Log the message
                if (jsonBody == null)
                {
                    lidgrenConsoleOutputGUI.WriteConsole($"Sending new message to {netConnection} with type {serverMessageType} and no body", LogLevel.High);
                }
                else
                {
                    lidgrenConsoleOutputGUI.WriteConsole($"Sending new message to {netConnection} with type {serverMessageType} and body {jsonBody}", LogLevel.High);
                }
            }
            else
            {
                //Cast to the enum
                ClientMessageType clientMessageType = (ClientMessageType)messageType;

                //Log the message
                if (jsonBody == null)
                {
                    lidgrenConsoleOutputGUI.WriteConsole($"Sending reply message to {netConnection} with type {clientMessageType} and feedback {feedback} and no body", LogLevel.High);
                }
                else
                {
                    lidgrenConsoleOutputGUI.WriteConsole($"Sending reply message to {netConnection} with type {clientMessageType} and feedback {feedback} and body {jsonBody}", LogLevel.High);
                }
            }

            //If there is a jsonBody then convert it to a byte array body, otherwise set to null
            byte[] body = jsonBody == null ? null : Encoding.UTF8.GetBytes(jsonBody);

            //Create a new message
            NetOutgoingMessage outgoingMessage = netServer.CreateMessage();

            //Write whether it is a new message from the server (true) or a reply to a message from the client (false)
            outgoingMessage.Write(newMessageFromServer);

            //Write true if a body exists to read
            outgoingMessage.Write(body != null);

            //Pad so that future reading is easily done
            outgoingMessage.WritePadBits();

            //Write the message type (note it's been squished to 8 bits therefore max 256 message types)
            outgoingMessage.Write(messageType);

            //If it is a reply message
            if (!newMessageFromServer)
            {
                if (feedback != null)
                {
                    //Write the feedback (note again 8 bit so max 256 types)
                    outgoingMessage.Write((byte)feedback);
                }
                else
                {
                    throw new Exception("Feedback not provided but is necessary for reply type message!");
                }
            }

            //If a body exists
            if (body != null)
            {
                //Write the body length
                outgoingMessage.Write(body.Length);

                //Write the body
                outgoingMessage.Write(body);
            }

            /*
            There is an override for SendMessage() which takes an integer called 'sequenceChannel' 
            - this can be used for certain delivery methods, namely UnreliableSequenced, ReliableSequenced and ReliableOrdered.

            The sequence channel is a number between 0 and (NetConstants.NetChannelsPerDeliveryMethod - 1) - currently 31. 
            The reason for this limitation is to reduce the amount of overhead per message.Note that there are this amount of channels per delivery method, not in total.

            Messages sent in a certain sequence channel will be dropped/withheld independently of messages sent in a different sequence channel.
            */

            switch (deliverymethod)
            {
                case NetDeliveryMethod.UnreliableSequenced:
                case NetDeliveryMethod.ReliableSequenced:
                case NetDeliveryMethod.ReliableOrdered:
                    if (sequenceChannel != null)
                    {
                        netServer.SendMessage(outgoingMessage, netConnection, deliverymethod, (int)sequenceChannel);
                    }
                    else
                    {
                        throw new Exception("Trying to send a message without specifying the required sequence channel");
                    }

                    break;

                default:
                    netServer.SendMessage(outgoingMessage, netConnection, deliverymethod);
                    break;
            }
        }

        //Private Methods

        /// <summary>
        /// Activated when a message is recieved
        /// </summary>
        private void HandleMessage(object peer)
        {
            //Wrap the message handling in a try catch
            try
            {
                //The Lidgren variable that is used to handle incoming messages
                NetIncomingMessage incomingMessage = ((NetServer)peer).ReadMessage();

                //If there is not a message then exit
                if (incomingMessage == null)
                {
                    return;
                }

                //Attempt to find the connection in the list
                NetConnection netConnection = ConnectionList.Find(x => x == incomingMessage.SenderConnection);

                //Process the message according to its type
                switch (incomingMessage.MessageType)
                {
                    case NetIncomingMessageType.ConnectionApproval:
                        //Only handle the message if the connection has not been approved
                        if (netConnection == null)
                        {
                            HandleMessageConnectionApproval(incomingMessage);
                        }
                        else
                        {
                            //Log the message
                            lidgrenConsoleOutputGUI.WriteConsole($"Connection {incomingMessage.SenderConnection.RemoteEndPoint} trying to gain approval but already approved", LogLevel.Medium);
                        }
                        break;

                    case NetIncomingMessageType.Data:
                        //Only handle the message if the connection has been approved
                        if (netConnection != null)
                        {
                            HandleMessageData(netConnection, incomingMessage);
                        }
                        else
                        {
                            //Log the message
                            lidgrenConsoleOutputGUI.WriteConsole($"Connection {incomingMessage.SenderConnection.RemoteEndPoint} trying to send data but not approved", LogLevel.Medium);
                        }
                        break;

                    case NetIncomingMessageType.StatusChanged:
                        //Always handle whether approved or not
                        HandleMessageStatusChanged(netConnection, incomingMessage);
                        break;

                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.VerboseDebugMessage:
                        //Ignore all debug messages regardless of log level
                        break;

                    default:
                        //Do no processing to the message

                        //Log the message
                        if (incomingMessage.SenderConnection != null)
                        {
                            lidgrenConsoleOutputGUI.WriteConsole($"Connection {incomingMessage.SenderConnection.RemoteEndPoint} sent message but ignored, type: {incomingMessage.MessageType}", LogLevel.Medium);
                        }
                        else
                        {
                            lidgrenConsoleOutputGUI.WriteConsole($"Connection null sent message but ignored, type: {incomingMessage.MessageType}", LogLevel.Medium);
                        }

                        break;
                }                
            }
            catch (Exception ex)
            {
                //Log the message
                lidgrenConsoleOutputGUI.WriteConsole($"Message handling. Generic error: {ex.Message}", LogLevel.Medium);
            }
        }

        /// <summary>
        /// Handle the object wishing to have connection approval
        /// </summary>
        private void HandleMessageConnectionApproval(NetIncomingMessage incomingMessage)
        {
            //Is the IP banned
            if (IsBanned(incomingMessage))
            {
                //Log the message
                lidgrenConsoleOutputGUI.WriteConsole($"Banned IP rejected: {incomingMessage.SenderConnection.RemoteEndPoint}", LogLevel.Medium);

                //Deny the connection (Note: could delete this reply and ignore them if they're trying to spam us with connections)
                incomingMessage.SenderConnection.Deny("Banned IP");

                //Return
                return;
            }

            //Otherwise Approve the connection
            incomingMessage.SenderConnection.Approve();

            //Add the connection to the list that need beat
            ConnectionList.Add(incomingMessage.SenderConnection);

            //Log the message
            lidgrenConsoleOutputGUI.WriteConsole($"Client connected: {incomingMessage.SenderConnection.RemoteEndPoint}", LogLevel.Low);

            //Output the network object
            lidgrenServerEvents.NewConnectionApproved(incomingMessage.SenderConnection);
        }

        /// <summary>
        /// Checks to see if the ip address is in the banned table and returns true if found
        /// </summary>
        private bool IsBanned(NetIncomingMessage incomingMessage)
        {
            //Just to get rid of warning message
            if (incomingMessage == null)
            {

            }

            return false;
        }

        /// <summary>
        /// Reads the message and sends it to the output interface
        /// </summary>
        private void HandleMessageData(NetConnection netConnection, NetIncomingMessage incomingMessage)
        {
            //Determine if it is a new message from the client (true) or a reply to a message from the server (false)
            bool newMessage = incomingMessage.ReadBoolean();

            //Determine if there is a body to read
            bool bodyToRead = incomingMessage.ReadBoolean();

            //Consume the pad bits
            incomingMessage.ReadPadBits();

            //Determine the message type
            int messageType = incomingMessage.ReadByte();

            int? feedback = null;
            //If it is a reply message
            if (!newMessage)
            {
                //Read the feedback
                feedback = incomingMessage.ReadByte();
            }

            string jsonBody = null;
            //If there is a body
            if (bodyToRead)
            {
                int bodylength = incomingMessage.ReadInt32();

                //Read the body
                byte[] bodyBytes = incomingMessage.ReadBytes(bodylength);

                //Decode the body
                jsonBody = Encoding.UTF8.GetString(bodyBytes);
            }

            //If new message
            if (newMessage)
            {
                //Cast to the enum
                ClientMessageType clientMessageType = (ClientMessageType)messageType;

                //Log the message
                if (jsonBody == null)
                {
                    lidgrenConsoleOutputGUI.WriteConsole($"Received new message from {netConnection} with type {clientMessageType} and no body", LogLevel.High);
                }
                else
                {
                    lidgrenConsoleOutputGUI.WriteConsole($"Received new message from {netConnection} with type {clientMessageType} and body {jsonBody}", LogLevel.High);
                }

                //Activate the event
                lidgrenServerEvents.HandleNewMessage(netConnection, clientMessageType, jsonBody);
            }
            else
            {
                //Cast to the enum
                ServerMessageType serverMessageType = (ServerMessageType)messageType;

                //Log the message
                if (jsonBody == null)
                {
                    lidgrenConsoleOutputGUI.WriteConsole($"Received reply message from {netConnection} with type {serverMessageType} and feedback {feedback} and no body", LogLevel.High);
                }
                else
                {
                    lidgrenConsoleOutputGUI.WriteConsole($"Received reply message from {netConnection} with type {serverMessageType} and feedback {feedback} and body {jsonBody}", LogLevel.High);
                }

                if (feedback == null)
                {
                    //Log the message
                    lidgrenConsoleOutputGUI.WriteConsole($"Aborting processing of message from {netConnection} with type {serverMessageType} as feedback is null", LogLevel.Medium);
                    
                    //Don't handle any further
                    return;
                }

                //Activate the event
                lidgrenServerEvents.HandleReplyMessage(netConnection, serverMessageType, (int)feedback, jsonBody);
            }
        }

        /// <summary>
        /// Handles status changed messages (currently only interested in disconnection messages)
        /// </summary>
        /// <param name="netConnection"></param>
        /// <param name="incomingMessage"></param>
        private void HandleMessageStatusChanged(NetConnection netConnection, NetIncomingMessage incomingMessage)
        {
            //If the connection wasn't authorised
            if (netConnection == null)
            {
                //Read the message type
                NetConnectionStatus netConnectionStatus = (NetConnectionStatus)TryToReadByte(incomingMessage);

                //Log the message
                lidgrenConsoleOutputGUI.WriteConsole($"Null connection has had status changed: {netConnectionStatus}", LogLevel.Medium);

                //Take no more action
                return;
            }

            //Continuing, the netConnection is known, therefore it was an approved connection

            //Switch on the net connection status. Should this instead be the net connection status read from the incoming message?
            switch (netConnection.Status)
            {
                //On disconnection
                case NetConnectionStatus.Disconnected:
                case NetConnectionStatus.Disconnecting:

                    //Remove it from the list
                    ConnectionList.Remove(netConnection);

                    //Try to read the reason
                    string reason = TryToReadString(incomingMessage);

                    //Log the message
                    lidgrenConsoleOutputGUI.WriteConsole($"Object {netConnection} disconnected, reason: {reason}", LogLevel.Low);

                    //Activate the event
                    lidgrenServerEvents.LostConnection(netConnection);

                    break;


                default:

                    //Log the message
                    lidgrenConsoleOutputGUI.WriteConsole($"Object {netConnection} status changed to: {netConnection.Status}", LogLevel.Medium);

                    break;
            }
        }
    }
}