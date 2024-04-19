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
    private readonly ApplicationDbContext _context;

    public UserRepository(UserManager<AppUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
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
        var updatedUser = await _userManager.UpdateAsync(user);
        
        return updatedUser.Succeeded;
    }

    public async Task DeleteAsync(string id)

    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
        
        if (user != null)
        {
            var meetingsToDelete = await _context.Meetings.Where(m => m != null && m.UserId == id).ToListAsync();
            _context.Meetings.RemoveRange(meetingsToDelete);
            
            await _userManager.DeleteAsync(user);
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task<bool> UserExists(string userId)
    {
        return await _userManager.Users.AnyAsync(u => u.Id == userId);
    }
    
    public async Task<bool> Save()
    {
        var saved = await _context.SaveChangesAsync();

        return saved > 0;
    }
    
    public void Detach(AppUser user)
    { 
        _context.Entry(user).State = EntityState.Detached;
    }
}