using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gateway_Gatekeeper.Validator
{
    public interface IValidator<in T>
    {
        Task<bool> IsValid(T message);
    }
}
