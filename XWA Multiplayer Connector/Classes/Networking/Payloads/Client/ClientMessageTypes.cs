using Lidgren.Network;
using System;

namespace XWA_Multiplayer_Connector.Classes.Networking.Payloads.Client
{
    class ClientMessageTypes
    {
        /* Documentation on Delivery Methods
        Unreliable              This is just UDP. Messages can be lost or received more than once.
                                Messages may not be received in the same order as they were sent.

        UnreliableSequenced     Using this delivery method messages can still be lost; but you're protected against duplicated messages. 
                                If a message arrives late; that is, if a message sent after this one has already been received - it will be dropped. 
                                This means you will never receive "older" data than what you already have received.

        ReliableUnordered       This delivery method ensures that every message sent will be eventually received.
                                It does not however guarantee what order they will be received in; late messages may be delivered before newer ones.

        ReliableSequenced       This delivery method is similar to UnreliableSequenced; except that it guarantees that SOME messages will be received
                                - if you only send one message - it will be received.
                                If you sent two messages quickly, and they get reordered in transit, only the newest message will be received 
                                - but at least ONE of them will be received.

        ReliableOrdered         This delivery method guarantees that messages will always be received in the exact order they were sent.
        */

        public static void DetermineMethodandChannel(ClientMessageType messageType, out NetDeliveryMethod deliverymethod, out int? sequenceChannel)
        {
            //Determine the delivery type and if needed, the sequence channel
            sequenceChannel = null;

            switch (messageType)
            {
                case ClientMessageType.Beat:
                    throw new Exception("Shouldn't be sending beats this way (Specific beat function that recycles message)");

                case ClientMessageType.SendName:
                    deliverymethod = NetDeliveryMethod.ReliableOrdered;
                    sequenceChannel = 0;
                    break;

                default:
                    throw new Exception("Could not determine datatype");
            }
        }

        public enum ClientMessageType
        {
            /// <summary>
            /// Note this is "reserved" as zero and has to be the same on both client and server
            /// </summary>
            Beat = 0,
            SendName,
        }
    }
}
