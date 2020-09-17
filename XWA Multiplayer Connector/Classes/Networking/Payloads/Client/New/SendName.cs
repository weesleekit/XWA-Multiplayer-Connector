namespace XWA_Multiplayer_Connector.Classes.Networking.Payloads.Client.New
{
    class SendName
    {
        public enum Feedback
        {
            ThatsACoolNameBro,
        }
        public string ClientPlayerName { get; set; }
    }
}