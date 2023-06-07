namespace Devpool.CQRS.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class TransactionCommandHandlerDecoratorAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Class)]
public class LoggingCommandHandlerDecoratorAttribute : Attribute
{
}