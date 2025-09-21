public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/login", (LoginDto dto, AuthService auth, JwtService jwt) =>
        {
             var result = auth.VerifyLogin(dto.email, dto.passwordHash);
              var token = jwt.GenerateJwtToken(dto.email);   
    return result.Check
        ? Results.Ok(new { token })
        : Results.Unauthorized();
        })
        .AddEndpointFilter(async (context, next) =>
        {
            var dto = context.GetArgument<LoginDto>(0); 

            if (string.IsNullOrWhiteSpace(dto.email) || string.IsNullOrWhiteSpace(dto.passwordHash))
            {
                return Results.BadRequest("Email or password is required");
            }

            
            return await next(context);
        });

        return group;
    }
}

public record LoginDto(string email, string passwordHash);
