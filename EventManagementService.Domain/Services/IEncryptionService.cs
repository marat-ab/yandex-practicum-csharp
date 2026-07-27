using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagementService.Domain.Services;

public interface IEncryptionService
{
    string CalcHash(string data);
}
