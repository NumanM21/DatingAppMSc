using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class ApprovePhotoDto
    {
        public int  PhotoId { get; set; }
        public string PhotoURL { get; set; }

        public string Username { get; set; }
        public bool IsPhotoApproved { get; set; }

    }
}