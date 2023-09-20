using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class ApprovePhotoDto
    {
        public int  Id { get; set; }
        public string photoUrl { get; set; } 

        public string Username { get; set; }
        public bool IsPhotoApproved { get; set; }

    }
}