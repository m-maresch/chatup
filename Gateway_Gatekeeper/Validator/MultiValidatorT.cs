using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gateway_Gatekeeper.Validator
{
    public static class MultiValidator<T>
    {
        private static readonly List<IValidator<T>> Validators;

        static MultiValidator()
        {
            if (Validators == null)
            {
                Validators = new List<IValidator<T>>();
            }
            if (!Validators.Any())
            {
                foreach (var type in MultiValidator.Validators)
                {
                    Type genericType = type.MakeGenericType(typeof(T));
                    object instance = Activator.CreateInstance(genericType);
                    Validators.Add((IValidator<T>)instance);
                }
            }
        }

        public static async Task<bool> IsValid(T message)
        {
            bool result = true;
            foreach (var validator in Validators)
            {
                if (!(await validator.IsValid(message)))
                {
                    result = false;
                }
            }
            return result;
        }
    }
}
