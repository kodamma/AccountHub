using AccountHub.Application.CQRS.Extensions;
using AccountHub.Application.Responses;
using AutoMapper;
using Kodamma.Common.Base.Mapping;
using Kodamma.Common.Base.ResultHelper;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;
using AccountEntity = AccountHub.Domain.Entities.Account;

namespace AccountHub.Application.CQRS.Commands.Account.AddAccount
{
    public class AddAccountCommand : ICommand<Result<SignUpAccountResponse>>, IMapWith<AccountEntity>
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required DateOnly Birthdate { get; set; }
        public IFormFile? Avatar { get; set; }
        public required string Country { get; set; }
        public bool Agree { get; set; }
        public string IpAddress = null!;

        public void MapTo(Profile profile)
            => profile.CreateMap<AddAccountCommand, AccountEntity>()
                .ForMember(d => d.IpAddress, s => s.MapFrom(x => x.IpAddress))
                .ForMember(d => d.AvatarURL, s => s.Ignore())
                .ForMember(d => d.PasswordSalt, s => s.Ignore())
                .ForMember(d => d.PasswordHash, s => s.Ignore())
                .ForMember(d => d.EmailConfirmed, s => s.Ignore())
                .ForMember(d => d.Locked, s => s.Ignore())
                .ForMember(d => d.LockedCount, s => s.Ignore())
                .ForMember(d => d.LockedEnd, s => s.Ignore());
    }
}
