public class AuthService
{
    private readonly UserRepository _repo;

    public AuthService(UserRepository repo)
    {
        _repo = repo;
    }

    public bool VerifyLogin(string email, string passwordHash)
    {
        return _repo.VerifyCredentials(email, passwordHash);
    }
    public bool VerifyByEmail(string email)
    {
        return _repo.UserExistsByEmail(email);
    }
}
