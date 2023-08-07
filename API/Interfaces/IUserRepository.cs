using API.DTOs;
using API.Entities;

namespace API.Interfaces
{
  public interface IUserRepository
  {
    void Update(AppUser user);
    Task<bool> AsyncSaveAll();
    Task<IEnumerable<AppUser>> AsyncGetUsers();
    Task<AppUser> AsyncGetUserByID(int id);
    Task<AppUser> AsyncGetUserByUsername(string username);
    Task<IEnumerable<MemberDto>> AsyncGetMembers();
    Task<MemberDto> AsyncGetMember(string username);
  }
}