using System.Reflection;
using AutoMapper;
using Ordering.Application.Common.Mapping;

namespace Ordering.Application.Common.Models;

public class MappingProfile
{
    public MappingProfile()
    {
        ApplyMappingFromAssembly(Assembly.GetExecutingAssembly());
    }

    public void ApplyMappingFromAssembly(Assembly assembly)
    {
        var mapFromTypes = typeof(IMapFrom<>);
        const string mappingMethodName = nameof(IMapFrom<object>.Mapping);
        
        bool HasInterface(Type t) => t.IsGenericType && t.GetGenericTypeDefinition() == mapFromTypes;

        var types = assembly.GetExportedTypes()
            .Where(t => t.GetInterfaces().Any(HasInterface)).ToList();

        var argumentTypes = new Type[] { typeof(Profile) };

        foreach (var type in types)
        {
            var instance= Activator.CreateInstance(type);
            var methodInfo = type.GetMethod(mappingMethodName);

            if (methodInfo != null)
            {
                methodInfo.Invoke(instance, new object?[]{this});
            }
            else
            {
                var interfaces = type.GetInterfaces().Where(HasInterface).ToList();
                if(interfaces.Count<=0) continue;

                foreach (var @interface in interfaces)
                {
                    var interfaceMethodInfo = @interface.GetMethod(mappingMethodName, argumentTypes);
                    ArgumentNullException.ThrowIfNull(interfaceMethodInfo);
                    
                    interfaceMethodInfo.Invoke(instance, new object?[]{this});
                }
            }
        }
    }
}