using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server.Context.Abstract;
using Server.Context.Context;
using Server.Model.Dtos;
using Server.Model.Dtos.User;
using Server.Model.Models;

namespace Server.Context.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManager<AppUser> _userManager;

    public UserRepository(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<AppUser?> GetUserByIdAsync(string id)
    {
        return await _userManager.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
    }

    public async Task<AppUser> GetUserByEmailAsync(string email)
    {
        if (email == null) throw new ArgumentNullException(nameof(email));

        return await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email) ?? throw new InvalidOperationException();
    }

    public async Task<List<AppUser>> GetUsersAsync()
    {
        return await _userManager.Users.ToListAsync();
    }

    public async Task<CreateUserResponse> AddAsync(UserRegisterDto model)
    {
        var appUser = new AppUser()
        {
            UserName = model.Email,
            Id = Guid.NewGuid().ToString(),
            Name = model.Name,
            Surname = model.Surname,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            ProfilePictureUrl = model.ProfilePictureUrl
        };
        
        var createdUser = await _userManager.CreateAsync(appUser, model.Password);
        
        CreateUserResponse response = new() { Succeeded = createdUser.Succeeded };
        
        if (createdUser.Succeeded)
        {
            var roleResult = await _userManager.AddToRoleAsync(appUser, "User");
            
            if(roleResult.Succeeded)
                response.Message = "Kullanıcı başarıyla oluşturulmuştur.";
            else
            {
                response.Message = "Rol atanamadi.";
            }
        }
        else
            foreach (var error in createdUser.Errors)
                response.Message += $"{error.Code} - {error.Description}\n";

        return response;
    }

    public async Task<bool> UpdateAsync(AppUser user)
    {
        await _userManager.UpdateAsync(user);
        return await Save();
    }

    public async Task DeleteAsync(int id)
    {
        // var user = await _context.Users.FindAsync(id);
        //
        // if (user != null)
        // {
        //     // var meetingsToDelete = _context.Meetings.Where(m => m.UserId == id);
        //     // _context.Meetings.RemoveRange(meetingsToDelete);
        //     _context.Users.Remove(user);
        //     await _context.SaveChangesAsync();
        // }
    }

    public bool UserExists(int userId)
    {
        // return _context.Users.Any(u => u.Id == userId);
        return true;
    }
    
    public async Task<bool> Save()
    {
        // var saved = await _context.SaveChangesAsync();

        return true;
    }
}