using AirandWebAPI.Services.Contract;
using AirandWebAPI.Core;
using AirandWebAPI.Core.Domain;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using System;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using AirandWebAPI.Helpers;
using AirandWebAPI.Models.Auth;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirandWebAPI.Models;

namespace AirandWebAPI.Services.Concrete
{

    public class UserService : IUserService
    {
        private IUnitOfWork _unitOfWork;
        private readonly AppSettings _appSettings;
        private ISmsService _smsService;

        public UserService(IUnitOfWork unitOfWork, IOptions<AppSettings> appSettings, ISmsService smsService)
        {
            _unitOfWork = unitOfWork;
            _appSettings = appSettings.Value;
            _smsService = smsService;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {
            var user = _unitOfWork.Users.SingleOrDefault(x => x.Username == model.Username);

            // return null if user not found
            if (user == null) return null;

            if (!VerifyPasswordHash(model.Password, user.PasswordHash, user.PasswordSalt)) return null;

            // authentication successful so generate jwt token
            var token = generateJwtToken(user);

            return new AuthenticateResponse(user, token);
        }

        public IEnumerable<User> GetAll()
        {
            return _unitOfWork.Users.GetAll();
        }

        public User GetById(int id)
        {
            return _unitOfWork.Users.SingleOrDefault(x => x.Id == id);
        }

        // helper methods

        public async Task<AuthenticateResponse> Create(User user, string password)
        {
            var token = generateJwtToken(user);
            var existingUser = _unitOfWork.Users.SingleOrDefault(x => x.Username == user.Username);

            if (existingUser != null) return new AuthenticateResponse(existingUser, token);

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.DateCreated = DateTime.UtcNow.AddHours(1);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _unitOfWork.Users.Add(user);
            await _unitOfWork.Complete();

            return new AuthenticateResponse(user, token);
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }
        private string generateJwtToken(User user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim("id", user.Id.ToString())
                    }),
                Expires = DateTime.UtcNow.AddYears(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<bool> CheckPhone(string phone)
        {
            var user = _unitOfWork.Users.Find(x => x.Phone.Equals(phone)).FirstOrDefault();

            if (user != null)
            {
                var otp = _unitOfWork.Otps.Find(x => x.UserId.Equals(user.Id)).FirstOrDefault();
                string code = GenerateOtp();
                string message = $"Airand: Your OTP is {code}. Complete verification using this OTP";
                SmsBody smsBody = new SmsBody("Airand", user.Phone, message);
                if (otp != null)
                {
                    otp.Code = code;
                    otp.isUsed = false;
                    await _smsService.SendAsync(smsBody);
                    await _unitOfWork.Complete();
                    return true;
                }
                otp = new Otp();
                otp.Code = code;
                otp.UserId = user.Id;
                otp.DateCreated = DateTime.Now;
                _unitOfWork.Otps.Add(otp);
                await _unitOfWork.Complete();
                await _smsService.SendAsync(smsBody);
                

                return true;
            }
            return false;
        }

        public async Task<bool> VerifyPhone(VerifyPhoneModel model)
        {
            var user = _unitOfWork.Users.Find(x => x.Phone.Equals(model.Phone)).FirstOrDefault();
            if (user != null)
            {
                var otp = _unitOfWork.Otps.Find(x => x.UserId.Equals(user.Id) && x.Code.Equals(model.Otp)).FirstOrDefault();
                if(otp == null || otp.isUsed) return false;

                otp.isUsed = true;
                user.isVerified = true;
                await _unitOfWork.Complete();
                return true;
            }
            return false;
        }

        private List<User> _users = new List<User>
        {
            new User { Id = 1, FirstName = "Test", LastName = "User", Username = "test" }
        };

        private string GenerateOtp()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}