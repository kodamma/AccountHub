using AutoMapper;
using System.Reflection;

namespace AccountHub.Application.Shared.Mapping
{
    public class AssemblyMappingProfile : Profile
    {
        public AssemblyMappingProfile(Assembly assembly) 
            => ApplyMappingFromAssembly(assembly);
        private void ApplyMappingFromAssembly(Assembly assembly)
        {
            var types = assembly.GetExportedTypes().Where(t 
                => t.GetInterfaces().Any(i 
                => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapWith<>))).ToArray();

            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type);
                var methodInfo = type.GetMethod("MapTo");
                methodInfo?.Invoke(instance, [this]);
            }
        }
    }
}
