using Lidgren.Network;
using System;
using System.Text;
using System.Threading;
using XWA_Multiplayer_Connector.Interfaces;
using static XWA_Multiplayer_Connector.Classes.Networking.Payloads.Client.ClientMessageTypes;
using static XWA_Multiplayer_Connector.Classes.Networking.Payloads.Server.ServerMessageTypes;

namespace XWA_Multiplayer_Connector.Classes.Networking
{
    class LidgrenClient : LidgrenObject, ILidgrenClient
    {
        //Fields

        /// <summary>
        /// The Lidgren client
        /// </summary>
        private readonly NetClient netClient;

        /// <summary>
        /// The connection back to the game logic
        /// </summary>
        private readonly ILidgrenClientEvents lidgrenClientEvents;

        /// <summary>
        /// The address to reach the server on
        /// </summary>
        private readonly string serveraddress;

        /// <summary>
        /// The port to reach the server on
        /// </summary>
        private readonly int serverport;

        //Constructor

        public LidgrenClient(string appIdentifier, ILidgrenConsoleOutputGUI lidgrenConsoleOutputGUI,
                            ILidgrenClientEvents lidgrenClientEvents, string serveraddress, int serverport)
                            : base(lidgrenConsoleOutputGUI)
        {
            //Store the injection
            this.lidgrenClientEvents = lidgrenClientEvents;
            this.serveraddress = serveraddress;
            this.serverport = serverport;

            //Create the NetClient
            NetPeerConfiguration config = new NetPeerConfiguration(appIdentifier);
            netClient = new NetClient(config);
            netClient.RegisterReceivedCallback(new SendOrPostCallback(HandleMessage));
            netClient.Start();

            //Log the message
            lidgrenConsoleOutputGUI.WriteConsole("Client started on Port: " + netClient.Port, LogLevel.Low);
        }

        //Interface Functions

        /// <summary>
        /// Returns the true if connected
        /// </summary>
        /// <returns></returns>
        bool ILidgrenClient.AreWeConnected()
        {
            return netClient.ConnectionStatus == NetConnectionStatus.Connected;
        }

        /// <summary>
        /// Attempt the connection to the server
        /// </summary>
        void ILidgrenClient.AttemptToConnectToServer()
        {
            if (netClient.ConnectionStatus == NetConnectionStatus.Disconnected)
            {
                //Log the message
                lidgrenConsoleOutputGUI.WriteConsole("Client attempting to connect", LogLevel.Low);

                //Start the connection
                netClient.Connect(serveraddress, serverport);
            }
        }

        /// <summary>
        /// Call on shutdown so a graceful message can be sent to the server
        /// </summary>
        void ILidgrenClient.Shutdown(string reason)
        {
            //Gracefully tell the server we are disconnecting
            netClient.Disconnect(reason);
        }

        /// <summary>
        /// Sends a message using the lidgren connection
        /// </summary>
        /// <param name="newMessageFromClient">Set to true when it is a new message from the client and not a reply</param>
        /// <param name="messageType">Based on the enum in the owner class (Note: limited to 256 types)</param>
        /// <param name="deliverymethod">The delivery method to use</param>
        /// <param name="feedback">This is required if it is a reply type message</param>
        /// <param name="sequenceChannel"></param>
        /// <param name="body"></param>
        void ILidgrenClient.SendMessage(bool newMessageFromClient,
                                   byte messageType,
                                   NetDeliveryMethod deliverymethod,
                                   int? feedback,
                                   int? sequenceChannel,
                                   string jsonBody)
        {
            //Log the message
            if (newMessageFromClient)
            {
                //Cast to the enum
                ClientMessageType clientMessageType = (ClientMessageType)messageType;

                //Log the message
                if (jsonBody == null)
                {
                    lidgrenConsoleOutputGUI.WriteConsole($"Sending new message with type {clientMessageType} and no body", LogLevel.High);
                }
                else
                {
                    lidgrenConsoleOutputGUI.WriteConsole($"Sending new message with type {clientMessageType} and body {jsonBody}", LogLevel.High);
                }
            }
            else
            {
                //Cast to the enum
                ServerMessageType serverMessageType = (ServerMessageType)messageType;

                lidgrenConsoleOutputGUI.WriteConsole($"Sending reply message with type {serverMessageType} and feedback {feedback} and body {jsonBody}", LogLevel.High);
            }

            //If there is a jsonBody then convert it to a byte array body, otherwise set to null
            byte[] body = jsonBody == null ? null : Encoding.UTF8.GetBytes(jsonBody);

            //Create a new message
            NetOutgoingMessage outgoingMessage = netClient.CreateMessage();

            //Write whether it is a new message from the client (true) or a reply to a message from the server (false)
            outgoingMessage.Write(newMessageFromClient);

            //Write true if a body exists to read
            outgoingMessage.Write(body != null);

            //Pad so that future reading is easily done
            outgoingMessage.WritePadBits();

            //Write the message type (note it's been squished to 8 bits therefore max 256 message types)
            outgoingMessage.Write(messageType);

            //If it is a reply message
            if (!newMessageFromClient)
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
                        netClient.SendMessage(outgoingMessage, deliverymethod, (int)sequenceChannel);
                    }
                    else
                    {
                        throw new Exception("Trying to send a message without specifying the required sequence channel");
                    }

                    break;

                default:
                    netClient.SendMessage(outgoingMessage, deliverymethod);
                    break;
            }
        }

        //Private Functions

        /// <summary>
        /// Activated when a message is recieved
        /// </summary>
        private void HandleMessage(object peer)
        {
            //Wrap the message handling in a try catch
            try
            {
                //The Lidgren variable that is used to handle incoming messages
                NetIncomingMessage incomingMessage = ((NetClient)peer).ReadMessage();

                //If there is not a message then exit
                if (incomingMessage == null)
                {
                    return;
                }

                switch (incomingMessage.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        HandleMessageData(incomingMessage);
                        break;

                    case NetIncomingMessageType.StatusChanged:
                        HandleMessageStatusChanged(incomingMessage);
                        break;

                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.VerboseDebugMessage:
                        //Ignore all debug messages regardless of log level
                        break;

                    default:
                        //Do no processing to the message

                        //Log the message
                        lidgrenConsoleOutputGUI.WriteConsole($"Message ignored, type: {incomingMessage.MessageType}", LogLevel.Medium);

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
        /// Handles data messages (reads the message and outputs to interface)
        /// </summary>
        /// <param name="incomingMessage"></param>
        private void HandleMessageData(NetIncomingMessage incomingMessage)
        {
            //Determine if it is a new message from the client network object (true) or a reply to a message from the server (false)
            bool newMessage = incomingMessage.ReadBoolean();

            //Determine if there is a body to read
            bool bodyToRead = incomingMessage.ReadBoolean();

            //Consume the pad bits
            incomingMessage.ReadPadBits();

            //Determine the message type
            int messageType = incomingMessage.ReadByte();

            //If a beat then do nothing
            if (messageType == 0)
            {
                //Log the message
                lidgrenConsoleOutputGUI.WriteConsole($"Received beat", LogLevel.Everything);

                //Abort further processing
                return;
            }

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

            //Handle the message type accordingly
            if (newMessage)
            {
                //Cast to the enum
                ServerMessageType serverMessageType = (ServerMessageType)messageType;

                //Log the message
                if (jsonBody == null)
                {
                    lidgrenConsoleOutputGUI.WriteConsole($"Received new message with type {serverMessageType} and no body", LogLevel.High);
                }
                else
                {
                    lidgrenConsoleOutputGUI.WriteConsole($"Received new message with type {serverMessageType} and body {jsonBody}", LogLevel.High);
                }

                //Activate the event
                lidgrenClientEvents.HandleNewMessage(serverMessageType, jsonBody);
            }
            else
            {
                //Cast to the enum
                ClientMessageType clientMessageType = (ClientMessageType)messageType;

                //Log the message
                if (jsonBody == null)
                {
                    lidgrenConsoleOutputGUI.WriteConsole($"Received reply message with type {clientMessageType} and feedback {feedback} and no body", LogLevel.High);
                }
                else
                {
                    lidgrenConsoleOutputGUI.WriteConsole($"Received reply message with type {clientMessageType} and feedback {feedback} and body {jsonBody}", LogLevel.High);
                }

                if (feedback == null)
                {
                    //Log the message
                    lidgrenConsoleOutputGUI.WriteConsole($"Aborting processing of message type {clientMessageType} as feedback is null", LogLevel.Medium);

                    //Don't handle any further
                    return;
                }

                //Activate the event
                lidgrenClientEvents.HandleReplyMessage(clientMessageType, (int)feedback, jsonBody);
            }
        }

        /// <summary>
        /// Handles lidgren status changed messages
        /// </summary>
        /// <param name="incomingMessage"></param>
        private void HandleMessageStatusChanged(NetIncomingMessage incomingMessage)
        {
            //Read the message type
            NetConnectionStatus netConnectionStatus = (NetConnectionStatus)TryToReadByte(incomingMessage);

            //Switch on the net connection status
            switch (netConnectionStatus)
            {
                case NetConnectionStatus.Connected:

                    //Log the message
                    lidgrenConsoleOutputGUI.WriteConsole("Successfully connected", LogLevel.Low);

                    //Activate the event
                    lidgrenClientEvents.OnConnect();

                    break;

                case NetConnectionStatus.Disconnected:

                    //Try to read reason
                    string reason = TryToReadString(incomingMessage);

                    //Log the message
                    lidgrenConsoleOutputGUI.WriteConsole($"Disconnected, reason: {reason}", LogLevel.Low);

                    //Activate the event
                    lidgrenClientEvents.OnDisconnect(reason);

                    break;

                default:

                    //Log the message
                    lidgrenConsoleOutputGUI.WriteConsole($"Status changed to: {netConnectionStatus}", LogLevel.Medium);

                    break;
            }
        }
    }
}
