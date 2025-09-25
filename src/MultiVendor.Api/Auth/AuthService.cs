public class AuthService
{
    private readonly UserRepository _repo;
    private readonly HrEmployeeRepository _repoHR;

    public AuthService(UserRepository repo,HrEmployeeRepository repoHR )
    {
        _repo = repo;
        _repoHR=repoHR;
    }

   

public LoginResponse VerifyLogin(string email, string passwordHash)
{
    if (email.StartsWith("HR", StringComparison.OrdinalIgnoreCase))
    {
        bool check = _repoHR.VerifyCredentials(email, passwordHash);

        return new LoginResponse(
            check,
            new[]
            {
                new { rel = "vendorstatus", href = "/api/v1/auth/HR/stute/vendor" }
            },
            "HR"
        );
    }
    else
    {
        bool check = _repo.VerifyCredentials(email, passwordHash);

        return new LoginResponse(
            check,
            new[]
            {
                new { rel = "profile", href = "/api/users/me" }
            },
            "sss"
        );
    }
}


    public bool VerifyByEmail(string email)
    {
        return _repo.UserExistsByEmail(email);
    }
}
public record LoginResponse(bool Check, object Links,string role );