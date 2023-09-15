using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class SRGroupConnection // tracks connection between users in a group
    {
        // default constructor required by EF --> KEY!!!
        public SRGroupConnection() 
        {}

        public SRGroupConnection(string connectionId, string username)
        {
            this.ConnectionId = connectionId;
            this.Username = username;
        }
        
        [Key]
        public string ConnectionId { get; set; }
        public string Username { get; set; }
        
    }
}