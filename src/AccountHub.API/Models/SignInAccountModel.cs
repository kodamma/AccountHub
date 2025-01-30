using AccountHub.Application.CQRS.Commands.Authentication.Login;
using AccountHub.Application.Shared.Mapping;
using AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace AccountHub.API.Models
{
    public class SignInAccountModel : IMapWith<LoginCommand>
    {
        [MinLength(6)]
        [MaxLength(254)]
        [EmailAddress]
        public required string Email { get; set; }
        [MinLength(10)]
        [MaxLength(45)]
        public required string Password { get; set; }

        public void MapTo(Profile profile) 
            => profile.CreateMap<SignInAccountModel, LoginCommand>();
    }
}
