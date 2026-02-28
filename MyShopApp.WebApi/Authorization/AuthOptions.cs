using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MyShopApp.WebApi.Authorization
{
    public static class AuthOptions
    {
        public const string ISSUER = "MyAuthServer"; // издатель токена
        public const string AUDIENCE = "MyAuthClient"; // потребитель токена
        const string KEY = "mysupersecret_secretsecretsecretkey!123";   // ключ для шифрации
        public const int LIFETIME = 60; // время жизни токена - 60 минут

        /// <summary>
        /// Получить семмитричный ключ шифрования
        /// </summary>
        /// <returns></returns>
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
        }
    }
}
