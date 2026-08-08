using System.Security.Cryptography;
using System.Text;

namespace UsersService.Domain.Services;

public class EncryptionService : IEncryptionService
{
    public string CalcHash(string data)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(data));

        var result = Convert.ToHexString(bytes);

        return result;
    }
}
