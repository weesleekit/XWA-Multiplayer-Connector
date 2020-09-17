namespace XWA_Multiplayer_Connector.Classes.Networking.Payloads.Server.New
{
    class PrepMission
    {
        public enum Feedback
        {
            Success,
            Failure,
        }

        public string MissionFileName { get; set; }
    }
}
