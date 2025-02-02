using CL8.Interfaces;
using CL8.BLL.Infrastructure;

namespace CL8.BLL.Services.SpecialInterfaces
{
    public interface IUserService
    {
        public Task<IResponse<UserDto>> TryLoginAsync(string username, string? password = null);
        public Task<IResponse<UserDto>> TryRegisterUserAsync(string? username, string? email, string? password, string? confirmedPassword);
    }
}