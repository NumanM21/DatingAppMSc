using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
  // This class allows us to control what information about AppUser we want to return on API requests 
  public class MemberDto
  {
    public int Id { get; set; }

    public string Username { get; set; }

    public int Age { get; set; }

    public string KnownAs { get; set; }

    public DateTime UserCreated { get; set; }

    public DateTime LastActive { get; set; }
    public string UserGender { get; set; }

    public string Introduction { get; set; }

    public string LookingFor { get; set; }

    public string UserInterests { get; set; }

    public string City { get; set; }

    public string Country { get; set; }

    public string photoUrl { get; set; } // only set to main photo

    public List<PhotoDto> Photos { get; set; }


  }
}