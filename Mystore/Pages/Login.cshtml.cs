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
using Mystore.Pages.Clients;
using System.Data.SqlClient;
// sample --> https://gist.github.com/isdaviddong/8c42a0e226a33131d5af6ab4514e960e
#endregion

namespace __NameSpace__.Pages
{

    public class Users
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class LoginModel : PageModel
    {
        private readonly string _connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Mystore;Integrated Security=True";
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

        public async Task<IActionResult> OnPostLogin()
        {
            try
            {
                Users user = await GetUserFromDatabaseAsync();

                if (user != null)
                {
                    await SignInUserAsync(user.UserName);

                    return Redirect("/index");
                }
            }
            catch (Exception ex)
            {
                // Log the exception using a logging framework
                Console.WriteLine("Exception: " + ex.ToString());
            }

            Msg = $"User Not Exist!!";
            return Page();
        }

        private async Task<Users> GetUserFromDatabaseAsync()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string sql = "SELECT * FROM Users WHERE UserName=@UserName AND Password=@Password";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserName", fieldAccount);
                    command.Parameters.AddWithValue("@Password", fieldPwd);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            Users user = new Users
                            {
                                UserId = reader.GetInt32(0),
                                UserName = reader.GetString(1),
                                Password = reader.GetString(2),
                            };

                            return user;
                        }
                    }
                }
            }

            return null;
        }

        private async Task SignInUserAsync(string userName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim("FullName", userName),
                new Claim(ClaimTypes.Role, "nobody"),
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }
    }
}
