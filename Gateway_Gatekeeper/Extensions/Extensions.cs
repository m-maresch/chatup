using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gateway_Gatekeeper.Validator;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Gateway_Gatekeeper.Extensions
{
    public static class Extensions
    {
        public static int AsInt(this long val) => (int)val;

        public static Task<bool> IsValid<T>(this T obj) => MultiValidator<T>.IsValid(obj);

        public static async Task<bool> PublishWhenValid<T>(this T message, IPublishEndpoint publishEndpoint) where T : class 
        {
            if (await message.IsValid())
            {
                await publishEndpoint.Publish(message);
                return true;
            }
            return false;
        }
    }
}
