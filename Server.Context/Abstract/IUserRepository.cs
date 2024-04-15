using Server.Model.Models;

namespace Server.Context.Abstract;

public interface IUserRepository
{
    Task<User> GetUserByIdAsync(int id);
    Task<User> GetUserByEmailAsync(string email);
    Task<List<User>> GetUsersAsync();
    Task<User> AddAsync(User user);
    Task<User> UpdateAsync(User user);
    Task DeleteAsync(int id);

    bool UserExists(int userId);
}