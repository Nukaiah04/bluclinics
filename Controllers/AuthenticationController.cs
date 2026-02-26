using System.Net;
using System.Net.Mail;
using BluClinicsApi.Data;
using BluClinicsApi.Dtos;
using BluClinicsApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BluClinicsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AuthenticationService authentication;
        private readonly IWebHostEnvironment _env;
        public AuthenticationController(AuthenticationService authenticationService, IWebHostEnvironment env)
        {
            authentication = authenticationService;
            _env = env;
        }


        [HttpPost("Register")]
        public async Task<IActionResult> UserRegister([FromBody] RegisterDto registerDto)
        {
            try
            {
                var userData = await authentication.Registration(registerDto);
                var FirstName = AesHelper.Decrypt(userData.FirstName);
                var LastName = AesHelper.Decrypt(userData.LastName);
                return Ok(new { status = true, message = "Registered Successfully", data = new {userData.UhId, userData.UserId, FirstName, LastName } });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { status = false, message = ex.InnerException?.Message ?? ex.Message, data = new { } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = false, message = "Error occurred while updating role status.", data = new { }, error = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> UserLogin([FromBody] EmailLoginDto emailLoginDto)
        {
            try
            {
                var userData = await authentication.Login(emailLoginDto);
                var Email = AesHelper.Decrypt(userData.Email);
                var MobileNumber = AesHelper.Decrypt(userData.Mobile);
                var FirstName = AesHelper.Decrypt(userData.FirstName);
                var LastName = AesHelper.Decrypt(userData.LastName);
                var userJsonData = new { userData.UserId, userData.RoleId, FirstName, LastName, Email, MobileNumber };
                return Ok(new { status = true, message = "Login successful", data = userJsonData });
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message == "Email not found.")
                {
                    return NotFound(new { status = false, message = ex.InnerException?.Message ?? ex.Message, data = new { } });
                }
                return Conflict(new { status = false, message = ex.InnerException?.Message ?? ex.Message, data = new { } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = false, message = "Error occurred while login.", data = new { }, error = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> UserChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                var isUpdated = await authentication.ChagePassword(changePasswordDto);
                return Ok(new { status = true, message = "Password updated", data = new { isUpdated.UserId, isUpdated.UpdatedBy, isUpdated.UpdatedDate } });
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message == "No user found.")
                {
                    return NotFound(new { status = false, message = ex.InnerException?.Message ?? ex.Message, data = new { } });
                }
                return Conflict(new { status = false, message = ex.InnerException?.Message ?? ex.Message, data = new { } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = false, message = "Error occurred while login.", data = new { }, error = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPost("GenerateOtp")]
        public async Task<IActionResult> SendEmailOtp([FromBody] GenerateOtpDto generateOtpDto)
        {
            try
            {
                var OTP = await authentication.OTPGenearate(generateOtpDto);
                if (generateOtpDto.Type == "Email")
                {
                    var fromEmail = "bluhealth@bluecloudsoftech.com";
                    var ToEmail = generateOtpDto.Email!;
                    var appPassword = "gqvzqbkjphfrwvmx";

                    SmtpClient client = new SmtpClient("smtp.office365.com", 587);
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(fromEmail, appPassword);

                    var OtpTemplate = Path.Combine(_env.ContentRootPath, "Helpers", "OTPTemplate.html");
                    var html = await System.IO.File.ReadAllTextAsync(OtpTemplate);
                    html = html.Replace("{OTP}", OTP);

                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress(fromEmail);
                    mail.To.Add(ToEmail);
                    mail.Subject = "OTP";
                    mail.IsBodyHtml = true;
                    mail.Body = html;

                    await client.SendMailAsync(mail);
                    return Ok(new
                    {
                        status = true,
                        message = $"OTP sent to {(generateOtpDto.Type == "Email" ? "Email" : "Mobile")}.",
                        data = new
                        {
                            OTP
                        }
                    });
                }
                else
                {

                    var smsService = new Fast2SMSService();
                    var response = await smsService.SendOtpAsync(generateOtpDto.Mobile!, OTP);
                    return Ok(new
                    {
                        status = true,
                        message = $"OTP sent to {(generateOtpDto.Type == "Email" ? "Email" : "Mobile")}.",
                        data = new
                        {
                            OTP
                        }
                    });
                }

            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { status = false, message = ex.InnerException?.Message ?? ex.Message, data = new { } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = false, message = "Error occurred while send OTP.", data = new { }, error = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPost("VerifyOtp")]
        public async Task<IActionResult> VerifyEmailOtp([FromBody] VerifyEmailOtpDto verifyEmailOtpDto)
        {
            try
            {
                var OtpData = await authentication.VerifyEmailOtp(verifyEmailOtpDto);
                return Ok(new { status = true, message = "Otp verified successfully", data = OtpData });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { status = false, message = ex.InnerException?.Message ?? ex.Message, data = new { } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = false, message = "Error occurred while verify OTP.", data = new { }, error = ex.InnerException?.Message ?? ex.Message });
            }
        }
    }
}