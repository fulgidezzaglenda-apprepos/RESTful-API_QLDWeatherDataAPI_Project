using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using QLDEducationalWeatherDataAPI.Models.Repositories;
using System.Data;
using QLDEducationalWeatherDataAPI.Models;

namespace QLDEducationalWeatherDataAPI.AttributeTags
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(validOn: AttributeTargets.Method | AttributeTargets.Class)]
    public class ApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        /// <summary>
        /// 
        /// </summary>
        private UserRoles[] _requiredRole;

        /// <summary>
        /// 
        /// </summary>
        public UserRoles[] RequiredRole
        {
            get { return _requiredRole; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roles"></param>
        public ApiKeyAttribute(params UserRoles[] roles)
        {
            _requiredRole = roles;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //Tries to get the apikwy from the Http request  headers. If it can fine and apikey value it
            //will output it in a stringvalues data rtype. If it cannot find it, the statement will run and
            // send an error responcse to the suer.
            if (context.HttpContext.Request.Headers.TryGetValue("apiKey", out var key) == false)
            {
                //Create a HTTP response result and dill it out to give the user feedback.
                context.Result = new ContentResult
                {
                    StatusCode = 401,
                    Content = "No API Key provided!"
                };

                return;
            }

            //The apikey will be in a stringvalues class and will be nested inside curly braces when it is firstly retrieved
            //(e.g. {1431-3443-566554665}. We need to convert it to a string and remove the braces to use it in our validation method.
            var validKey = key.ToString().Trim('{', '}');
            //Request the user repository fron the services. This version is an alternative to asking for it in
            //our constructor. This is good if you only need to in one place whichfoes not always run.
            var userRepo = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
            //Run tye authenticated check o the provided key to see if it mnatched one of the allowed roles.
            if (userRepo.AuthenticateUser(validKey, RequiredRole) == null)
            {
                //Create a HTTP responese result and fill it out to give the user feedback.
                context.Result = new ContentResult
                {
                    StatusCode = 403,
                    Content = "User's API Key provided is not valid for this operation!"
                };

                return;
            }

            //Update tge login time for the succesful user.
            userRepo.UpdateLastLogin(validKey);
            //Pass the data onto the next item in the path. This will normally be the intended endpoint
            //for controller, Or it might be another filter attribute.
            await next();
        }
    }
}
