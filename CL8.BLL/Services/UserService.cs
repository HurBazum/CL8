using CL8.BLL.Infrastructure;
using CL8.BLL.Infrastructure.CustomExceptions;
using CL8.BLL.Services.SpecialInterfaces;
using CL8.DAL.Entities;
using CL8.Interfaces;
using CL8.UI.Infrastructure.Others;

namespace CL8.BLL.Services
{
    public class UserService(IRepository<User> repository) : IUserService
    {
        private readonly IRepository<User> _userRepository = repository;

        public async Task<IResponse<UserDto>> TryLoginAsync(string username, string? password)
        {
            var response = new BaseResponse<UserDto>();

            if(string.IsNullOrEmpty(password))
            {
                response.Message = "Type the password!";
                return response;
            }

            try
            {
                var user = await _userRepository.GetEntityAsync(u => u.Name == username);

                if(user == null)
                {
                    response.Message = "such user wasn't found";
                }
                else
                {
                    password = PasswordProtector.Protect(password);
                    var confirmed = Equals(password, user.Password);
                    switch(confirmed)
                    {
                        case true:

                            response.Value = Transformer.ToDto(user);

                            break;
                        case false:

                            response.Message = "Wrong password, dear. . .";

                            break;
                    }
                }
            }
            catch
            {
                response.Message = "error!";
            }

            return response;
        }

        public async Task<IResponse<UserDto>> TryRegisterUserAsync(string? username, string? email, string? password, string? confirmedPassword)
        {
            var response = new BaseResponse<UserDto>();

            if(string.IsNullOrEmpty(username))
            {
                response.Message = $"Fill {nameof(username)} field!";
                return response;
            }
            if(string.IsNullOrEmpty(password))
            {
                response.Message = $"Fill {nameof(password)} field!";
                return response;
            }
            if(string.IsNullOrEmpty(confirmedPassword))
            {
                response.Message = $"Fill {nameof(confirmedPassword)} field!";
                return response;
            }

            try
            {
                if(await _userRepository.GetEntityAsync(u => u.Email == email) != null)
                {
                    throw new CustomException($"User with {email} already exists!");
                }

                switch(Equals(password, confirmedPassword))
                {
                    case false:
                        response.Message = $"Passwords don't match!";
                        break;
                    case true:
                        password = PasswordProtector.Protect(password);
                        var u = new User() { Name = username, Password = password, Email = email };
                        var user = await _userRepository.AddEntityAsync(u);
                        response.Value = Transformer.ToDto(user);
                        break;
                }
            }
            catch(Exception ex)
            {
                response.Message = ex.Message;
            }

            return response;
        }
    }
}
