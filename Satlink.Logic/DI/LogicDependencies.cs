using FluentValidation;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

using Satlink.Logic.CQRS.AemetValues.Queries;
using Satlink.Logic.CQRS.Behaviours;
using Satlink.Logic.CQRS.Requests.Validators;

namespace Satlink.Logic.DI
{
    public static class LogicDependencies
    {
        public static void RegisterLogicDependencies(this IServiceCollection services)
        {
            services.AddScoped<IAemetValuesService, AemetValuesService>();
            services.AddScoped<IRequestsService, RequestsService>();
            services.AddScoped<IAuthService, AuthService>();

            // FluentValidation: register all validators defined in the Logic assembly.
            services.AddValidatorsFromAssemblyContaining<CreateRequestCommandValidator>();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining<GetAemetValuesQueryHandler>();

                // Pipeline order (outermost → innermost):
                // ExceptionBehaviour → LoggingBehaviour → ValidationBehaviour → TransactionBehaviour → Handler
                cfg.AddOpenBehavior(typeof(ExceptionBehaviour<,>));
                cfg.AddOpenBehavior(typeof(LoggingBehaviour<,>));
                cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
                cfg.AddOpenBehavior(typeof(TransactionBehaviour<,>));
            });
        }
    }
}