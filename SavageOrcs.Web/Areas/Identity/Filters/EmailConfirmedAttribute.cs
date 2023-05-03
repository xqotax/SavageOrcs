
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SavageOrcs.BusinessObjects;

public class EmailConfirmedAttribute : Attribute, IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var userManager = context.HttpContext.RequestServices.GetService<UserManager<User>>();

        // Check if user is authenticated
        if (context.HttpContext.User.Identity.IsAuthenticated)
        {
            var user = await userManager.GetUserAsync(context.HttpContext.User);

            // Check if user email is confirmed
            if (user != null && !user.EmailConfirmed)
            {
                context.Result = new RedirectToActionResult("Main", "Map", null);
            }
        }
    }
}
