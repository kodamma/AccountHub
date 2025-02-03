using AccountHub.Application.CQRS.Extensions;
using AccountHub.Application.Responses;
using AccountHub.Application.Shared.Mapping;
using AccountHub.Application.Shared.ResultHelper;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AccountEntity = AccountHub.Domain.Entities.Account;

namespace AccountHub.Application.CQRS.Commands.Account.AddAccount
{
    public class AddAccountCommand : ICommand<Result<SignUpAccountResponse>>, IMapWith<AccountEntity>
    {
        [MinLength(1)]
        [MaxLength(35)]
        public required string Username { get; set; }
        [MinLength(6)]
        [MaxLength(254)]
        [EmailAddress]
        public required string Email { get; set; }
        [MinLength(10)]
        [MaxLength(45)]
        [PasswordPropertyText]
        public required string Password { get; set; }
        public required DateOnly Birthdate { get; set; }
        public IFormFile? Avatar { get; set; }

        public void MapTo(Profile profile)
            => profile.CreateMap<AddAccountCommand, AccountEntity>()
                .ForMember(d => d.AvatarURL, s => s.Ignore())
                .ForMember(d => d.PasswordSalt, s => s.Ignore())
                .ForMember(d => d.PasswordHash, s => s.Ignore())
                .ForMember(d => d.EmailConfirmed, s => s.Ignore())
                .ForMember(d => d.Locked, s => s.Ignore())
                .ForMember(d => d.LockedCount, s => s.Ignore())
                .ForMember(d => d.LockedEnd, s => s.Ignore());
    }
}
