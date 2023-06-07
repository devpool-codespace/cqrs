using Devpool.CQRS.Abstractions;
using Devpool.CQRS.CommandDecorators;
using Microsoft.Extensions.DependencyInjection;

namespace Devpool.CQRS;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCQRS(this IServiceCollection services)
    {
        services.AddTransient<ICommandDispatcher, CommandDispatcher>();
        services.AddTransient<IQueryDispatcher, QueryDispatcher>();

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        services.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());


        var attributeTypes = assemblies
            .SelectMany(x => x.GetTypes())
            .SelectMany(t => t.GetCustomAttributes(false))
            .Where(x => x.GetType().Name.EndsWith("CommandHandlerDecoratorAttribute"))
            .Select(x => x.GetType())
            .ToList();
        
        var decoratorTypes = assemblies.SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.Name.EndsWith("CommandHandlerDecorator`1")
                           && type.GetInterfaces().Any(i => i.IsGenericType
                                                            && i.GetGenericTypeDefinition() ==
                                                            typeof(ICommandHandler<>)))
            .ToList();
        
        
        foreach (var attribute in attributeTypes)
        {
            var decoratorName = attribute.Name.Replace("Attribute", "");
        
            var decoratorType = decoratorTypes.FirstOrDefault(t => t.Name == decoratorName + "`1");
        
            if (decoratorType == null)
                throw new Exception($"Decorator {decoratorName} not found");
        
            var handlerTypes = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.GetInterfaces().Any(i =>
                                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<>))
                            && t.GetCustomAttributes(attribute, true).Length > 0);
        
            foreach (var handlerType in handlerTypes)
            {
                var handlerInterface = handlerType.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<>));
                var decoratorType1 = decoratorType.MakeGenericType(handlerInterface.GetGenericArguments());
                services.Decorate(handlerInterface, decoratorType1);
            }
        }
        

        


        services.Decorate(typeof(ICommandHandler<>), typeof(LoggingCommandHandlerDecorator<>));
        services.Decorate(typeof(ICommandHandler<>), typeof(ExceptionHandlingCommandHandlerDecorator<>));

        return services;
    }
}