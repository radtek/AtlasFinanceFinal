using System.Linq;
using System.Security.Claims;

namespace Falcon.Gyrkin.Library.Security
{
    /// <summary>
    /// Custom Claims Authorization Manager
    /// </summary>
    public class CustomAuthorizationManager : ClaimsAuthorizationManager
    {
        /// <summary>
        /// checks authorization for the subject in the specified context to perform the specified action on the specified resource.
        /// </summary>
        /// <param name="context">The authorization context that contains the subject, resource, and action for which authorization is to be checked.</param>
        /// <returns>
        /// true if the subject is authorized to perform the specified action on the specified resource; otherwise, false.
        /// </returns>
        public override bool CheckAccess(AuthorizationContext context)
        {
            string resource = context.Resource.Count > 0 ? context.Resource.First().Value : string.Empty;
            string action = context.Action.Count > 0 ? context.Action.First().Value : string.Empty;
            //You can do your custom claims check here.
            //lets say if we want to check the role of the user in a department before 
            return context.Principal.HasClaim(action, resource);
        }
    }
}