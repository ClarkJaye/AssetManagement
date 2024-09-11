using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class ProfileAccessAuth : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Your custom authorization logic goes here
        // You can access the current HttpContext through the 'context.HttpContext' property
        // Perform any necessary checks or validations based on your requirements
        // Set the 'context.Result' property to an appropriate IActionResult if authorization fails

        // Example: Check if the user is authenticated

        // Authorization succeeded
        //context.Result = new OkResult();
        var currentUrl = context.HttpContext.Request.Path;

        var Usern = context.HttpContext.Session.GetString("UserName");

        if (!currentUrl.Value.Contains("Login") && currentUrl.Value!= "/" && String.IsNullOrEmpty(Usern))
        {
            context.Result = new RedirectToActionResult("Login", "Users", null);
        }


    
        
    }   
}
