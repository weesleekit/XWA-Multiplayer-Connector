using Lidgren.Network;

namespace XWA_Multiplayer_Connector.Interfaces
{
    interface ILidgrenServer
    {
        /// <summary>
        /// To be called periodically
        /// </summary>
        void Beat();

        /// <summary>
        /// To be called when the server is to be shut down
        /// </summary>
        void ShutDown(string reason);

        /// <summary>
        /// Send a message using the server
        /// </summary>
        void SendMessage(bool newMessageFromServer,
                        byte messageType,
                        NetConnection netConnection,
                        NetDeliveryMethod deliverymethod,
                        int? feedback,
                        int? sequenceChannel,
                        string jsonBody);
    }
}