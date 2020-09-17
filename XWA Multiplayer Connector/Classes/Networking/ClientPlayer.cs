using Lidgren.Network;

namespace XWA_Multiplayer_Connector.Classes.Networking
{
    class ClientPlayer
    {
        //Fields

        /// <summary>
        /// The Lidgren connection
        /// </summary>
        public readonly NetConnection netConnection;

        /// <summary>
        /// The email address (null if not logged in)
        /// </summary>
        public string Name { get; set; }

        //Constructor

        public ClientPlayer(NetConnection netConnection)
        {
            //Store the network connection
            this.netConnection = netConnection;

            //Make up a name for them in the meantime (whilst we wait for a name)
            Name = netConnection.RemoteEndPoint.ToString();
        }

        //Overrides

        public override string ToString()
        {
            return Name;
        }
    }
}
