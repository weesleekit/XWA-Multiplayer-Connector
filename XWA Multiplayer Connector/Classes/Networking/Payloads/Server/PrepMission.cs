namespace XWA_Multiplayer_Connector.Classes.Networking.Payloads.Server
{
    class PrepMission
    {
        public enum Feedback
        {
            Success,
            Failure,
        }

        public class OriginPayload
        {
            public string MissionFileName { get; set; }
        }

        public class ReplyPayload
        {

        }
    }
}
