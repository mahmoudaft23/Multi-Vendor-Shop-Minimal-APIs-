using Microsoft.AspNetCore.Mvc;

public static class HrEndpoints
{
    public static RouteGroupBuilder MapHrEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/vendorstatus", ([FromServices] FetureHR hr) =>
        {
            return Results.Ok(hr.AllVendors());
        });


       group.MapPatch("/vendorstatus/{id}", 
    (string id, [FromBody] StatusDto statusdto, [FromServices] FetureHR hr) =>
    {
        bool result = hr.updatestuts(id, statusdto.status);
        return result ? Results.Ok("done") : Results.Unauthorized();
    });


        return group;
    }
}
public record StatusDto(string status );
