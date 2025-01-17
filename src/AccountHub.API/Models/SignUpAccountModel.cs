using AccountHub.Application.CQRS.Commands.Account.AddAccount;
using AccountHub.Application.Shared.Mapping;
using AutoMapper;

namespace AccountHub.API.Models
{
    public class SignUpAccountModel : IMapWith<AddAccountCommand>
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required DateOnly Birthdate { get; set; }
        public IFormFile? Avatar { get; set; }

        public void MapTo(Profile profile)
            => profile.CreateMap<SignUpAccountModel, AddAccountCommand>();
    }
}
