using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.Models;

namespace Gateway_Gatekeeper
{
    public class Config
    {
        public static string ClientId { get; set; } = "";
        public static string AppAccessScope { get; set; } = "scope.appaccess";
        public static string ClientSecret { get; set; }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource(AppAccessScope, "Gateway_Gatekeeper")
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            GenerateSecret();
            
            return new List<Client>
            {
                new Client
                {
                    ClientId = ClientId,
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = new List<Secret>
                    {
                        new Secret(ClientSecret.Sha256())
                    },
                    AllowedScopes = { AppAccessScope }
                }
            };
        }

        public static void GenerateSecret()
        {
            char[] chars = new char[62];
            chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[20];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder(20);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            ClientSecret = result.ToString();
        }
    }
}

