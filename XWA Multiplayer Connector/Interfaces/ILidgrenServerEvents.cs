using Lidgren.Network;
using static XWA_Multiplayer_Connector.Classes.Networking.Payloads.Client.ClientMessageTypes;
using static XWA_Multiplayer_Connector.Classes.Networking.Payloads.Server.ServerMessageTypes;

namespace XWA_Multiplayer_Connector.Interfaces
{
    interface ILidgrenServerEvents
    {
        /// <summary>
        /// A non banned IP has connected
        /// </summary>
        void NewConnectionApproved(NetConnection netConnection);

        /// <summary>
        /// Note the connection was not guaranteed to have been approved i.e. this connection might be foriegn to you and can therefore be ignored
        /// </summary>
        void LostConnection(NetConnection netConnection);

        /// <summary>
        /// A new message from the client
        /// </summary>
        void HandleNewMessage(NetConnection netConnection, ClientMessageType messageType, string jsonBody);

        /// <summary>
        /// A reply to a message originating from the server
        /// </summary>
        void HandleReplyMessage(NetConnection netConnection, ServerMessageType messageType, int feedback, string jsonBody);
    }
}
