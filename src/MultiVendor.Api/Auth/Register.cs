


public static class RegisterEndpoints
{
    public static RouteGroupBuilder MapRegisterEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/register", (registerDto dto, AuthService auth,CreatuserService Creatusernew ) =>
        {
            if (string.IsNullOrEmpty(dto.email) || string.IsNullOrEmpty(dto.passwordHash))
                return Results.BadRequest("Email or password is required");

            bool validLogin = auth.VerifyByEmail(dto.email);
            if(validLogin==true ){
            return Results.Conflict("Email used");

            }
            
             var cheackuder  =Creatusernew.Creatuser (dto.displayName,dto.email,dto.passwordHash, dto.chosserole );
                 return cheackuder ? Results.Ok("Created") : Results.Unauthorized();
        });

        return group;
    }
}
public record registerDto(string displayName,string email, string passwordHash,string  chosserole );
