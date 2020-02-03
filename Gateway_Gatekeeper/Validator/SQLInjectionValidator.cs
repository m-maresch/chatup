using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Gateway_Gatekeeper.Validator
{
    public class SQLInjectionValidator<T> : AbstractValidator<T>, Validator.IValidator<T>
    {
        public SQLInjectionValidator()
        {
            var stringProperties = typeof(T).GetProperties()
                .Where(t => t.PropertyType == typeof(string))
                .ToList();

            foreach (var stringProperty in stringProperties)
            {
                RuleFor(t => stringProperty.GetValue(t))
                    .Must(o => o == null ||
                               !new[] {"SELECT", "INSERT", "UPDATE", "DELETE", "CREATE", "DROP", ";", "*"}
                                   .Any(s => o.ToString()
                                       .ToUpper()
                                       .Contains(s)));
            }
        }

        public async Task<bool> IsValid(T message)
        {
            return (await this.ValidateAsync(message)).IsValid;
        }
    }
}
