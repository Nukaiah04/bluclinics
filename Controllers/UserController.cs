using BluClinicsApi.Dtos;
using BluClinicsApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace BluClinicsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService userService;
        public UserController(UserService userService)
        {
            this.userService = userService;
        }

        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var allUsers = await userService.GetAll();
                return Ok(new { status = true, message = "Users found.", data = allUsers });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { status = false, message = "Error occured while gettingg user data.", error = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPost("GetById")]
        public async Task<IActionResult> GetUserById([FromBody] GetUserByIdDto getUserByIdDto)
        {
            try
            {
                var userData = await userService.GetUserData(getUserByIdDto);
                return Ok(new { status = true, message = "User found successfully", data = userData });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { status = false, message = ex.InnerException?.Message ?? ex.Message, data = new { } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = false, message = "Error occured while gettingg user data.", error = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPost("ActiveInactiveUser")]
        public async Task<IActionResult> InActiveUser([FromBody] InActiveUserDto inActiveUserDto)
        {
            try
            {
                var userData = await userService.UserInactive(inActiveUserDto);
                return Ok(new { status = true, message = "User status updated", data = userData });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { status = false, message = ex.InnerException?.Message ?? ex.Message, data = new { } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = false, message = "Error occurred while changing the status", error = ex.InnerException?.Message ?? ex.Message });
            }
        }
    }
}