using System;
using System.Collections.Generic;
using System.Text;

namespace Office2PDF.MQ
{
    [System.Serializable]
    public abstract class Messages
    {
        public Messages(int MessageEmergency)
        {
            this.MessageEmergency = MessageEmergency;
        }

        public abstract string MessageCatalogID { get; }

        public int MessageEmergency { get; set; }

        public string MessageID { get; set; }

        public bool Serialized { get; set; }

        public string[] ParticipativeIds { get; set; }

        public string Queues { get; set; }

        public string Exchange { get; set; }
    }
}
