using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FluentValidation;

namespace Gateway_Gatekeeper.Validator
{
    public class DataValidator<T> : AbstractValidator<T>, Validator.IValidator<T> 
    {
        public DataValidator()
        {
            var idProperties = typeof(T).GetProperties()
                .Where(t => t.PropertyType == typeof(int) &&
                            t.Name.ToUpper().EndsWith("ID"))
                .ToList();

            foreach (var idProperty in idProperties)
            {
                RuleFor(t => idProperty.GetValue(t))
                    .NotNull()
                    .Must(o => (int)o >= 0);
            }
        }
        public async Task<bool> IsValid(T message)
        {
            return (await this.ValidateAsync(message)).IsValid;
        }
    }
}
