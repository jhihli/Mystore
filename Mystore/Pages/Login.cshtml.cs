using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

#region ForSignIn
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
// sample --> https://gist.github.com/isdaviddong/8c42a0e226a33131d5af6ab4514e960e
#endregion

namespace __NameSpace__.Pages
{
    public class LoginModel : PageModel
    {
        public string Msg = "";

        [BindProperty]
        public string fieldAccount { get; set; }
        [BindProperty]
        public string fieldPwd { get; set; }

        public IActionResult OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                HttpContext.SignOutAsync();
                return Redirect("/Login");
            }
            return null;
        }


        public IActionResult OnPostLogin()
        {

            #region SignIn
            var claims = new List<Claim>
                {
                    //use email or LINE user ID as login name
                    new Claim(ClaimTypes.Name, fieldAccount),
                    //other data
                    new Claim("FullName",fieldAccount),
                    new Claim(ClaimTypes.Role, "nobody"),
                };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
            };

            HttpContext.SignInAsync(
               CookieAuthenticationDefaults.AuthenticationScheme,
               new ClaimsPrincipal(claimsIdentity),
               authProperties);
            #endregion

            //Msg = $"fieldAccount: {fieldAccount} fieldPwd: {fieldPwd} login:{User.Identity.IsAuthenticated}";
            return Redirect("/index");
        }
    }
}
