using Mapi.Dto;
using MAPI.Dto;
using MAPI.Model.DTO;

namespace MAPI.IServices
{
    public interface IUserRepository
    {
        bool IsUniqueUser(string username);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<UserDTO> Register(RegisterationRequestDTO registerationRequestDTO);
    }
}
