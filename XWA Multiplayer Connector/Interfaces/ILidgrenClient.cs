using Lidgren.Network;

namespace XWA_Multiplayer_Connector.Interfaces
{
    interface ILidgrenClient
    {
        void AttemptToConnectToServer();

        bool AreWeConnected();

        void Shutdown(string reason);

        void SendMessage(bool newMessageFromClient,
                        byte messageType,
                        NetDeliveryMethod deliverymethod,
                        int? feedback,
                        int? sequenceChannel,
                        string jsonBody);
    }
}
