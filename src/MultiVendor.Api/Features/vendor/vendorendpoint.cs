using Microsoft.AspNetCore.Mvc;

public static class VendorEndpoints
{
    public static RouteGroupBuilder MapVendorEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/addvendor", ([FromBody] VendorDto vendordto,[FromServices]FetureVendor vendor ) =>
        {       

           var result= vendor.addvendor(vendordto.OwnerUserId,vendordto.name,vendordto.description);
            return result ? Results.Ok("Created") : Results.Unauthorized();
        });


       


        return group;
    }
}
public record VendorDto(string OwnerUserId,string name, string description);
