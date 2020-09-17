using System.Collections.Generic;
using XWA_Multiplayer_Connector.Enums;

namespace XWA_Multiplayer_Connector.Classes.Missions
{
    static class MissionReference
    {
        public static Dictionary<Battle, string[]> battleIDtoMissionName = new Dictionary<Battle, string[]>()
        {
            { Battle.Family_Business, new string[] { 
                                "Aeron's lesson Transport operation",
                                "Emon's lesson Weapons",
                                "Aeron's error Data recovery",
                                "Sticking it to the Viraxos Covert delivery",
                                "Black market Bacta Cargo transfer",
                                "Rebel rendezvous Aid to the Alliance",
                                "Nowhere to go? Escape Imperial attack"
            } },
            { Battle.Clearing_the_Way, new string[] {
                                "Convoy Attack",
                                "Rescue Uncle Antan (Family mission)",
                                "Reconnaissance of Imperial Task Force",
                                "Rescue Echo Base Prisoners",
                                "Recover Imperial Probe (Family mission)",
                                "Stop Resupply of ISD Corrupter",
                                "Destroy Imperial Sensor Net",
            } },
            { Battle.Secret_Weapons_of_the_Empire, new string[] {
                                "Flight Staff Transfer",
                                "Ensnare Imperial Prototypes",
                                "Kill K'Armyn Viraxo (Family mission)",
                                "Raid Production Facility",
                                "Defend CRS Liberty",
                                "Destroy Imperial Research Facility",
            } },
            { Battle.Over_the_Fence, new string[] {
                                "Liberate Slave Convoy",
                                "Supply Rebels with Warheads (Family mission)",
                                "Recon Imperial research center",
                                "Investigate Imperial Communications Array",
                                "Plant Listening Device (Family mission)",
                                "Rendezvous with defector",
                                "Scramble!",
            } },
            { Battle.The_Bothan_Connection, new string[] {
                                "Shipment to Mining Colony (Family mission)",
                                "Reconnaissance of Imperial Convoy",
                                "Mining Colony Under Siege Rescue Aeron (Family mission)",
                                "Capture the Freighter Suprosa",
                                "Abandon Rebel Base at Kothlis",
                                "Protect Imperial Computer",
            } },
            { Battle.Mustering_the_Fleet, new string[] {
                                "Protect Alliance-Smuggler Meeting",
                                "Attack Imperial Convoy",
                                "Break Emon Out of Brig (Family mission)",
                                "Protect Smuggler Retreat",
                                "Rescue Smugglers",
                                "Recover Family Data Core (Family mission)",
                                "Attack Pirate Base",
            } },
            { Battle.The_Darkest_Hour, new string[] {
                                "Meet with Bothan Delegation",
                                "Locate Mercenary Base (Family mission)",
                                "Raid Mercenary Base",
                                "Rescue Bothan Spies",
                                "Steal Imperial Shuttle",
                                "Escort Rebel Fleet",
                                "Family Reunion (Family mission)",
            } },
            { Battle.Battle_of_Endor, new string[] {
                                "Battle of Endor",
                                "That Thing's Operational",
                                "The Shield is Down",
                                "Death Star Tunnel Run",
            } },
        };
    }
}
