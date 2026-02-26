using BluClinicsApi.Data;
using BluClinicsApi.Dtos;
using BluClinicsApi.Entitys;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BluClinicsApi.Models
{
    public class RolesService
    {
        private readonly ApplicationDbContext dbContext;
        public RolesService(ApplicationDbContext applicationDbContext)
        {
            dbContext = applicationDbContext;
        }

        public async Task<Roles> SaveRole(Roles roles)
        {
            var isExisted = await dbContext.Roles.FirstOrDefaultAsync(e => e.RoleName.ToLower().Trim() == roles.RoleName.ToLower().Trim());
            if (isExisted != null)
            {
                throw new InvalidOperationException("Role name already exists.");
            }
            var newRole = await dbContext.Roles.AddAsync(roles);
            await dbContext.SaveChangesAsync();
            return newRole.Entity;
        }

        public async Task<Roles> GetByRoleById(RoleGetByIdDto roleGetByIdDto)
        {
            var existedRoleData = await dbContext.Roles.FirstOrDefaultAsync(e => e.RoleId == roleGetByIdDto.RoleId);
            return existedRoleData!;
        }

        public async Task<List<Roles>> GetRolesData()
        {
            List<Roles> roleList = await dbContext.Roles.Where(e=>e.IsActive=="Active").AsNoTracking().ToListAsync();
            return roleList;
        }

        public async Task<Roles?> UpdateStatus( RoleUpdateDto roles)
        {
            var roleData = await dbContext.Roles.FirstOrDefaultAsync(r => r.RoleId == roles.RoleId);
            if (roleData == null)
            {
                throw new InvalidOperationException("Role not found.");
            }
            roleData.IsActive = roles.IsActive;
            roleData.UpdatedDate = DateTime.UtcNow;
            await dbContext.SaveChangesAsync();
            return roleData;
        }
    }
}