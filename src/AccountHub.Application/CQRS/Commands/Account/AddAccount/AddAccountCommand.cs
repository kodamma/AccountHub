using AccountHub.Application.CQRS.Extensions;
using AccountHub.Application.Shared.Mapping;
using AccountHub.Application.Shared.ResultHelper;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using AccountEntity = AccountHub.Domain.Entities.Account;

namespace AccountHub.Application.CQRS.Commands.Account.AddAccount
{
    public class AddAccountCommand : ICommand<Result<Guid>>, IMapWith<AccountEntity>
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required DateOnly Birthdate { get; set; }
        public IFormFile? Avatar { get; set; }

        public void MapTo(Profile profile)
            => profile.CreateMap<AddAccountCommand, AccountEntity>()
                .ForMember(d => d.AvatarURL, s => s.Ignore())
                .ForMember(d => d.EmailConfirmed, s => s.Ignore())
                .ForMember(d => d.Locked, s => s.Ignore())
                .ForMember(d => d.LockedCount, s => s.Ignore())
                .ForMember(d => d.LockedEnd, s => s.Ignore());
    }
}
