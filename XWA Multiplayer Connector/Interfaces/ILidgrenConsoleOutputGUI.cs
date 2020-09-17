using static XWA_Multiplayer_Connector.Classes.Networking.LidgrenObject;

namespace XWA_Multiplayer_Connector.Interfaces
{
    interface ILidgrenConsoleOutputGUI
    {
        /// <summary>
        /// Outputs to the console
        /// </summary>
        void WriteConsole(string message, LogLevel logLevel);
    }
}