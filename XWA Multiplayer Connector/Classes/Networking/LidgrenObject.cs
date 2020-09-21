using Lidgren.Network;
using XWA_Multiplayer_Connector.Interfaces;

namespace XWA_Multiplayer_Connector.Classes.Networking
{
    public abstract class LidgrenObject
    {
        //Delegate

        public delegate void lidgrenEventHandler(int feedback, string jsonBody);

        //Enum

        /// <summary>
        /// Note, exceptions are always logged
        /// </summary>
        public enum LogLevel
        {
            /// <summary>
            /// Only outputs server starting/shutting down and clients connecting/disconnecting
            /// </summary>
            Low = 0,

            /// <summary>
            /// Outputs problem messages e.g. banned connection attempts and invalid messages
            /// </summary>
            Medium = 1,

            /// <summary>
            /// Outputs every message (except beats)
            /// </summary>
            High = 2,

            /// <summary>
            /// Outputs even the beats
            /// </summary>
            Everything = 3,
        }

        //Fields

        /// <summary>
        /// The GUI for this server
        /// </summary>
        protected readonly ILidgrenConsoleOutputGUI lidgrenConsoleOutputGUI;

        //Constructor

        public LidgrenObject(ILidgrenConsoleOutputGUI lidgrenConsoleOutputGUI)
        {
            this.lidgrenConsoleOutputGUI = lidgrenConsoleOutputGUI;
        }

        //Protected Methods

        /// <summary>
        /// If cannot read string then returns empty string
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected string TryToReadString(NetIncomingMessage message)
        {
            try
            {
                return message.ReadString();
            }
            catch //(Exception ex)
            {
                return "";
            }
        }

        /// <summary>
        /// If cannot read byte then returns zero
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected byte TryToReadByte(NetIncomingMessage message)
        {
            try
            {
                return message.ReadByte();
            }
            catch //(Exception ex)
            {
                return 0;
            }
        }
    }
}
