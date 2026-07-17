using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace EventManagementService.Domain.Services;

public class EncryptionService : IEncryptionService
{
    public string CalcHash(string data)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(data));
        
        var result = Convert.ToHexString(bytes);

        return result;
    }
}
