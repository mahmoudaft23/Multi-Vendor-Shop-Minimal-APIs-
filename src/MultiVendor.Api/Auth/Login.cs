

public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/login", (LoginDto dto, AuthService auth) =>
        {
            if (string.IsNullOrEmpty(dto.email) || string.IsNullOrEmpty(dto.passwordHash))
                return Results.BadRequest("Email or password is required");

            bool validLogin = auth.VerifyLogin(dto.email, dto.passwordHash);

            return validLogin ? Results.Ok("Loginصصص OK") : Results.Unauthorized();
        });

        return group;
    }
}
public record LoginDto(string email, string passwordHash);
