using Server.Model.Dtos;
using Server.Model.Dtos.User;
using Server.Model.Models;

namespace Server.Context.Abstract;

public interface IUserRepository
{
    Task<AppUser?> GetUserByIdAsync(string id);
    Task<AppUser> GetUserByEmailAsync(string email);
    Task<List<AppUser>> GetUsersAsync();
    Task<CreateUserResponse> AddAsync(UserRegisterDto model);
    Task<bool> UpdateAsync(AppUser user);
    Task DeleteAsync(string id);

    Task<bool> UserExists(string userId);

    void Detach(AppUser user);
}