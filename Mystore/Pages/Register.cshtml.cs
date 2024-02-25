using __NameSpace__.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mystore.Pages.Clients;
using System.Data.SqlClient;

namespace Mystore.Pages
{
    public class RegisterModel : PageModel
    {

        private readonly string _connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Mystore;Integrated Security=True";
        public string Msg = "";
        [BindProperty]
        public string fieldAccount { get; set; }

        [BindProperty]
        public string fieldPwd { get; set; }

        public void OnGet()
        {
        }


        public async Task<IActionResult> OnPost()
        {
            try
            {
                Users user = await GetUserFromDatabaseAsync();

                if (user != null)
                {
                    //await SignInUserAsync(user.UserName);
                    Msg = $"User Existed!!";
                    return Page();
                }
                else
                {
                    await CreateUserToDatabaseAsync();
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"SQL Exception: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Log the exception using a logging framework
                Console.WriteLine("Exception: " + ex.ToString());
            }

            Msg = $"User Created!!";
            return Redirect("/Login");
        }

        private async Task CreateUserToDatabaseAsync()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                String sql = "INSERT INTO Users " + "(UserName, Password) VALUES " + "(@UserName, @Password);";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserName", fieldAccount);
                    command.Parameters.AddWithValue("@Password", fieldPwd);                 

                    command.ExecuteNonQuery();
                }
            }
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
    }
}
