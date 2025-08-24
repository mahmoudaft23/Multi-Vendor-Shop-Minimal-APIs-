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


        group.MapGet("/vendors/{id}", (string id ,[FromServices]FetureVendor vendor ) =>
        {       

           var result= vendor.getvendors(id);
            return Results.Ok(result);
        });
         group.MapGet("/vendor/{ownerid}/{id}", (string ownerid,string id ,[FromServices]FetureVendor vendor ) =>
        {       

           var result= vendor.getvendorbyid(ownerid,id);
            return Results.Json(result);
        });

        return group;
    }
}
public record VendorDto(string OwnerUserId,string name, string description);
