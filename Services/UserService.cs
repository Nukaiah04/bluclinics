using BluClinicsApi.Data;
using BluClinicsApi.Dtos;
using BluClinicsApi.Entitys;
using Microsoft.EntityFrameworkCore;

namespace BluClinicsApi.Models
{
    public class UserService
    {
        private readonly ApplicationDbContext dbContext;
        public UserService(ApplicationDbContext applicationDbContext)
        {
            dbContext = applicationDbContext;
        }

        public async Task<Object> GetAll()
        {
            var userData = await dbContext.Users.Select(e=>new
            {
                 e.UserId,
                 e.UhId,
                 FirstName = AesHelper.Decrypt(e.FirstName),
                 LastName = AesHelper.Decrypt(e.LastName),
                 Email = AesHelper.Decrypt(e.Email),
                 Mobile = AesHelper.Decrypt(e.Mobile),
                 e.RoleId,
                 e.ImageUrl,
                 e.IsActive,
                 e.CreatedBy,
                 e.CreatedDate,
                 e.UpdatedBy,
                 e.UpdatedDate
            }).ToListAsync();
            return userData;
        }

        public async Task<object> GetUserData(GetUserByIdDto getUserByIdDto)
        {
            var userData = await dbContext.Users.Where(e => e.UserId == getUserByIdDto.UserId).Select(e => new {
                 e.UserId,
                 e.UhId,
                 FirstName = AesHelper.Decrypt(e.FirstName),
                 LastName = AesHelper.Decrypt(e.LastName),
                 Email = AesHelper.Decrypt(e.Email),
                 Mobile = AesHelper.Decrypt(e.Mobile),
                 e.RoleId,
                 e.ImageUrl,
                 e.IsActive,
                 e.CreatedBy,
                 e.CreatedDate,
                 e.UpdatedBy,
                 e.UpdatedDate
                 }).FirstOrDefaultAsync();
            if (userData == null)
            {
                throw new InvalidOperationException("No user found.");
            }

            return userData!;
        }


        public async Task<Users> UserInactive(InActiveUserDto inActiveUserDto)
        {
            var userData = await dbContext.Users.FirstOrDefaultAsync(e=>e.UserId == inActiveUserDto.UserId);
            if (userData == null)
            {
                throw new InvalidOperationException("No user found.");
            }
            userData!.IsActive = inActiveUserDto.status;
            await dbContext.SaveChangesAsync();
            return userData;
        }
    }
}