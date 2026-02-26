using BluClinicsApi.Dtos;
using BluClinicsApi.Entitys;
using BluClinicsApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace BluClinicsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly RolesService rolesService;
        public RolesController(RolesService rolesService)
        {
            this.rolesService = rolesService;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateRole([FromBody] Roles roles)
        {
            try
            {
                var newRoleData = await rolesService.SaveRole(roles);
                return Ok(new { status = true, message = "Role created successfully", data = newRoleData });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { status = false, message = ex.InnerException?.Message ?? ex.Message, data = new { } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = false, message = "Error occurred while creating new role", data = new { }, error = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPost("GetById")]
        public async Task<IActionResult> GetRoleById([FromBody] RoleGetByIdDto roleGetByIdDto)
        {
            try
            {
                var existedRoleData = await rolesService.GetByRoleById(roleGetByIdDto);
                Console.WriteLine(existedRoleData.CreatedDate);
                if (existedRoleData == null)
                {
                    return NotFound(new { status = false, message = "Role not found", data = existedRoleData });
                }
                return Ok(new { status = true, message = "Role found", data = existedRoleData });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = false, message = "Error occurred while getting role", data = new { }, error = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllRoles()
        {
            try
            {
                var roleData = await rolesService.GetRolesData();
                if (!roleData.Any())
                {
                    return NotFound(new { status = false, message = "No roles found", data = roleData });
                }
                return Ok(new { status = true, message = "Roles found", data = roleData });
            }

            catch (Exception ex)
            {
                return StatusCode(500, new { status = false, message = "Error occurred while getting roles", data = new { }, error = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPost("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus([FromBody] RoleUpdateDto roleUpdateDto)
        {
            try
            {
                var updatedData = await rolesService.UpdateStatus(roleUpdateDto);
                return Ok(new { status = true, message = "Status updated successfully", data = updatedData });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { status = false, message = ex.InnerException?.Message ?? ex.Message, data = new { } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = false, message = "Error occurred while updating role status.", data = new { }, error = ex.InnerException?.Message ?? ex.Message });
            }
        }
    }
}