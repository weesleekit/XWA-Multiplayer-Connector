using static XWA_Multiplayer_Connector.Classes.Networking.Payloads.Client.ClientMessageTypes;
using static XWA_Multiplayer_Connector.Classes.Networking.Payloads.Server.ServerMessageTypes;

namespace XWA_Multiplayer_Connector.Interfaces
{
    interface ILidgrenClientEvents
    {
        void OnConnect();

        void OnDisconnect(string reason);

        void HandleNewMessage(ServerMessageType messageType, string jsonBody);

        void HandleReplyMessage(ClientMessageType messageType, int feedback, string jsonBody);
    }
}
