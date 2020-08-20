using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebLoginApplication.Models;

namespace WebLoginApplication.Services
{
    public interface IUserService
    {
        Task<UserManagerResponse> ForgetPasswordAsync(string email);

        Task<UserManagerResponse> ResetPasswordAsync(ResetPasswordViewModel model);
    }

    public class UserService : IUserService
    {
        private UserManager<IdentityUser> _userManger;
        private IConfiguration _configuration;
        private IMailService _mailService;

        public UserService(UserManager<IdentityUser> userManger, IConfiguration configuration, IMailService mailService)
        {
            _userManger = userManger;
            _configuration = configuration;
            _mailService = mailService;
        }

        public  async Task<UserManagerResponse> ForgetPasswordAsync(string email)
        {
            var user = await _userManger.FindByEmailAsync(email);
            if (user == null)
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "Nenhum usuario associado a este email"
                };

            var token = await _userManger.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Encoding.UTF8.GetBytes(token);
            var validToken = WebEncoders.Base64UrlEncode(encodedToken);

            string url = $"{_configuration["AppUrl"]}/ResetPassword?email={email}&toekn={validToken}";

            await _mailService.SendEmailAsync(email, "Reset Password", "<h1>Siga as instruções para resetar o password</h1>" +
                $"<p>Para resetar o seu password <a href='{url}'>Click aqui</a></p>");

            return new UserManagerResponse
            {
                IsSuccess = true,
                Message = "Reset password URL enviado para seu email com sucesso!"
            };
        }

        public async Task<UserManagerResponse> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            var user = await _userManger.FindByEmailAsync(model.Email);
            if (user == null)
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "Nenhum usuario associado a este email"
                };

            if (model.NewPassword != model.ConfirmPassword)
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "Password não corresponde à sua confirmação"
                };

            var decodedToken = WebEncoders.Base64UrlDecode(model.Token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await _userManger.ResetPasswordAsync(user, normalToken, model.NewPassword);

            if (result.Succeeded)
                return new UserManagerResponse
                {
                    Message = "Password foi resetado com sucesso ",
                    IsSuccess = true
                };
            return new UserManagerResponse
            {
                Message = "Algo deu errado", 
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description),
            };

        }
    }
}
