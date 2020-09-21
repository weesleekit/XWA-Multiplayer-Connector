namespace XWA_Multiplayer_Connector.Classes.Networking.Payloads.Client
{class SendName
    {
        public enum Feedback
        {
            ThatsACoolNameBro,
        }

        public class OriginPayload
        {
            public string ClientPlayerName { get; set; }
        }

        public class ReplyPayload
        {

        }
    }
}