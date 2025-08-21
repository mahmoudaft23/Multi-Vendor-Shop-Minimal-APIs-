using Microsoft.AspNetCore.Mvc;

public static class HrEndpoints
{
    public static RouteGroupBuilder MapHrEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/vendorstatus", ([FromServices] FetureHR hr) =>
        {
            return Results.Ok(hr.AllVendors());
        });

        return group;
    }
}
