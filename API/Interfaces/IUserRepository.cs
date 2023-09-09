using API.DTOs;
using API.Entities;
using API.ExternalHelpers;

namespace API.Interfaces
{
  public interface IUserRepository
  {
    void Update(AppUser user);
    Task<bool> AsyncSaveAll();
    Task<IEnumerable<AppUser>> AsyncGetUsers();
    Task<AppUser> AsyncGetUserByID(int id);
    Task<AppUser> AsyncGetUserByUsername(string username);
    Task<PaginationList<MemberDto>> AsyncGetMembers(ParameterFromUser parameterFromUser);
    Task<MemberDto> AsyncGetMember(string username);
  }
}