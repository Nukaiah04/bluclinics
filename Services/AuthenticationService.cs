using System.Runtime.Intrinsics.Arm;
using BluClinicsApi.Data;
using BluClinicsApi.Dtos;
using BluClinicsApi.Entitys;
using BluClinicsApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BluClinicsApi.Models
{
    public class AuthenticationService
    {
        private readonly ApplicationDbContext dbContext;
        public AuthenticationService(ApplicationDbContext applicationDbContext)
        {
            dbContext = applicationDbContext;
        }

        public async Task<Users> Registration(RegisterDto registerDto)
        {
            string email = AesHelper.Encrypt(registerDto.Email.ToLower().Trim());
            string mobile = AesHelper.Encrypt(registerDto.Mobile.Trim());
            var existedUser = await dbContext.Users.FirstOrDefaultAsync(e => e.Email == email || e.Mobile == mobile);
            if (existedUser != null)
            {
                if (existedUser.Email == email)
                    throw new InvalidOperationException("Email already exists.");

                if (existedUser.Mobile == mobile)
                    throw new InvalidOperationException("Mobile number already exists.");
            }
            string hashedPassword = PasswordHelpers.HashPassword(registerDto.Password);
            using var transaction = await dbContext.Database.BeginTransactionAsync();

            string prefix = registerDto.RoleId == 4 ? "DOC" : "UHID";

            var idRecord = await dbContext.IDGenerator.FromSqlRaw($"SELECT * FROM IDGenerator WITH (UPDLOCK, ROWLOCK) WHERE IdPrefix = '{prefix}'").FirstOrDefaultAsync();

            if (idRecord == null)
            {
                throw new Exception($"{prefix} configuration missing in IDGenerator table.");
            }
        
            string today = DateTime.Now.ToString("yyyyMMdd");

            if (idRecord.DateValue != today)
            {
                idRecord.DateValue = today;
                idRecord.LastNumber = 0;
                idRecord.UpdatedAt = DateTime.UtcNow;
            }

            idRecord.LastNumber += 1;
            int nextNumber = idRecord.LastNumber;
            string newId = $"{prefix}{today}{nextNumber}"; 
            Users newUser = new Users
            {
                UhId = newId,
                FirstName = AesHelper.Encrypt(registerDto.FirstName.Trim()),
                LastName = AesHelper.Encrypt(registerDto.LastName.Trim()),
                Email = email,
                Mobile = mobile,
                Password = hashedPassword,
                RoleId = registerDto.RoleId,
                IsActive = "Active",
                CreatedBy = registerDto.CreatedBy,
                CreatedDate = DateTime.UtcNow,
                UpdatedBy = registerDto.CreatedBy,
                UpdatedDate = DateTime.UtcNow
            };

            dbContext.Users.Add(newUser);
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            return newUser;
        }


        public async Task<Users> Login(EmailLoginDto emailLoginDto)
        {
            var Email = AesHelper.Encrypt(emailLoginDto.Email.ToLower().Trim());
            var isExisted = await dbContext.Users.FirstOrDefaultAsync(e => e.Email == Email);
            if (isExisted == null)
            {
                throw new InvalidOperationException("Email not found.");
            }
            if (!PasswordHelpers.VerifyPassword(emailLoginDto.Password, isExisted.Password))
            {
                throw new InvalidOperationException("Incorrect password.");
            }
            return isExisted;
        }

        public async Task<Users> ChagePassword(ChangePasswordDto changePasswordDto)
        {
            var existedUser = await dbContext.Users.FirstOrDefaultAsync(e => e.UserId == changePasswordDto.UserId);
            if (existedUser == null)
            {
                throw new InvalidOperationException("No user found.");
            }
            if (!PasswordHelpers.VerifyPassword(changePasswordDto.CurrentPassword, existedUser!.Password))
            {
                throw new InvalidOperationException("Incorrect current password.");
            }
            existedUser.Password = PasswordHelpers.HashPassword(changePasswordDto.NewPassword);
            existedUser.UpdatedDate = DateTime.UtcNow;
            existedUser.UpdatedBy = existedUser.UserId;
            await dbContext.SaveChangesAsync();
            return existedUser;
        }

        public async Task<string> OTPGenearate(GenerateOtpDto generateOtpDto)
        {
            var encryptEmail = AesHelper.Encrypt(generateOtpDto.Email!);
            var encryptMobile = AesHelper.Encrypt(generateOtpDto.Mobile!);
            IQueryable<Users> query = dbContext.Users;
            if (generateOtpDto.Type == "Email")
            {
                query = query.Where(e => e.Email == encryptEmail);
            }
            else
            {
                query = query.Where(e => e.Mobile == encryptMobile);
            }
            var userData = await query.FirstOrDefaultAsync();

            if (userData != null)
            {
                throw new InvalidOperationException(
                    $"{(generateOtpDto.Type == "Email" ? "Email" : "Mobile")} already existed."
                );
            }
            var random = new Random();
            string otp = random.Next(100000, 999999).ToString();
            OTPS oTPS = new OTPS
            {
                Type = generateOtpDto.Type == "Email" ? "Email" : "Mobile",
                Email = generateOtpDto.Type == "Email" ? encryptEmail : "",
                Mobile = generateOtpDto.Type == "Mobile" ? encryptMobile : "",
                Otp = otp,
                ExpireDate = DateTime.UtcNow.AddMinutes(10)
            };
            await dbContext.OTPS.AddAsync(oTPS);
            await dbContext.SaveChangesAsync();
            return otp;
        }

        public async Task<OTPS> VerifyEmailOtp(VerifyEmailOtpDto verifyEmailOtpDto)
        {
            var Email = AesHelper.Encrypt(verifyEmailOtpDto.Email!.Trim());
            var Mobile = AesHelper.Encrypt(verifyEmailOtpDto.Mobile!.Trim());
            IQueryable<OTPS> query = dbContext.OTPS;
            if (verifyEmailOtpDto.Type == "Email")
            {
                query = query.Where(e => e.Email == Email && e.Otp == verifyEmailOtpDto.Otp);
            }
            else
            {
                query = query.Where(e => e.Mobile == Mobile && e.Otp == verifyEmailOtpDto.Otp);
            }
            var otpData = await query.FirstOrDefaultAsync();
            if (otpData == null)
            {
                throw new InvalidOperationException("OTP not matched.");
            }
            if (otpData.ExpireDate < DateTime.UtcNow)
            {
                throw new InvalidOperationException("OTP got expired.");
            }
            dbContext.OTPS.Remove(otpData);
            await dbContext.SaveChangesAsync();
            return otpData;
        }
    }
}