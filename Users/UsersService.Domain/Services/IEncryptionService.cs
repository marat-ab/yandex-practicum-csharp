namespace UsersService.Domain.Services;

public interface IEncryptionService
{
    string CalcHash(string data);
}
