using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class SRGroupConnection // tracks connection between users in a group
    {
        public SRGroupConnection() // default constructor required by EF --> KEY!!!
        {}

        public SRGroupConnection(string connectionId, string username)
        {
            this.connectionId = connectionId;
            this.username = username;
        }
        
        public string connectionId { get; set; }
        public string username { get; set; }
        
    }
}