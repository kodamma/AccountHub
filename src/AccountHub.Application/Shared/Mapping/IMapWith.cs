using AutoMapper;

namespace AccountHub.Application.Shared.Mapping
{
    public interface IMapWith<T>
    {
        void MapTo(Profile profile) => profile.CreateMap(typeof(T), GetType());
    }
}
