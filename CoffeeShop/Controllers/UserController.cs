using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Data; // Required for CommandType and DataTable
using System.Data.SqlClient;// Required for SqlConnection, SqlCommand, SqlDataReader
using CoffeeShop.Models;
using CoffeeShop.Services;

namespace CoffeeShop.Controllers
{
    public class UserController : Controller
    {
        private readonly DatabaseService _databaseService;
        private readonly IConfiguration _configuration;

        // Single constructor to inject both DatabaseService and IConfiguration
        public UserController(DatabaseService databaseService, IConfiguration configuration)
        {
            _databaseService = databaseService;
            _configuration = configuration;
        }

       #region AddEdit

       #region UserByID
       public IActionResult UserAddEdit(int UserID)
       {
           string connectionString = this._configuration.GetConnectionString("ConnectionString");
           UserModel userModel = new UserModel();

           if (UserID > 0)
           {
               using (SqlConnection connection = new SqlConnection(connectionString))
               {
                   connection.Open();
                   using (SqlCommand command = connection.CreateCommand())
                   {
                       command.CommandType = CommandType.StoredProcedure;
                       command.CommandText = "PR_User_GetByID";
                       command.Parameters.AddWithValue("@UserID", UserID);

                       using (SqlDataReader reader = command.ExecuteReader())
                       {
                           DataTable table = new DataTable();
                           table.Load(reader);

                           if (table.Rows.Count > 0)
                           {
                               DataRow dataRow = table.Rows[0];
                               userModel.UserName = dataRow["UserName"].ToString();
                               userModel.Email = dataRow["Email"].ToString();
                               userModel.Password = dataRow["Password"].ToString();
                               userModel.MobileNo = dataRow["MobileNo"].ToString();
                               userModel.Address = dataRow["Address"].ToString();
                               userModel.IsActive = Convert.ToBoolean(dataRow["IsActive"]);
                           }
                       }
                   }
               }
               ViewBag.IsEdit = true; // Set flag to indicate edit mode
           }
           else
           {
               ViewBag.IsEdit = false; // Set flag to indicate add mode
           }

           return View("UserAddEdit", userModel);
       }
       #endregion
       
       #endregion
        
       #region SaveMethod
        public IActionResult UserSave(UserModel userModel)
        {
            
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            if (userModel.UserID == 0)
            {
                command.CommandText = "PR_User_Insert";
            }
            else
            {
                command.CommandText = "PR_User_Update";
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = userModel.UserID;
            }
            command.Parameters.Add("@UserName", SqlDbType.VarChar).Value = userModel.UserName;
            command.Parameters.Add("@Email", SqlDbType.VarChar).Value = userModel.Email;
            command.Parameters.Add("@Password", SqlDbType.VarChar).Value = userModel.Password;
            command.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = userModel.MobileNo;
            command.Parameters.Add("@Address", SqlDbType.VarChar).Value = userModel.Address;
            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = userModel.IsActive;
            command.ExecuteNonQuery();
            return RedirectToAction("UserAddEdit", new { ProductID = userModel.UserID });
            
        }

        #endregion

        #region User List
        public IActionResult UsersList()
        {
            DataTable usersTable = _databaseService.GetAllProcedure("PR_User_GetAll");
            return View(usersTable);
        }
        #endregion

        #region User Delete
        public IActionResult Delete(int ID)
        {
            try
            {
                _databaseService.DeleteProcedure("PR_User_Delete", "UserID", ID);
                return Json(new { success = true, message = "User deleted successfully." });
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547) // Foreign key violation error code in SQL Server
                {
                    return Json(new { success = false, message = "Unable to delete user. The user is referenced by other records." });
                }
                return Json(new { success = false, message = "An error occurred while deleting the user." });
            }
        }
        #endregion

        #region Login
        public IActionResult UserLogin(UserLoginModel userLoginModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string connectionString = _configuration.GetConnectionString("ConnectionString");
                    SqlConnection sqlConnection = new SqlConnection(connectionString);
                    sqlConnection.Open();
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.CommandText = "PR_User_Login";
                    sqlCommand.Parameters.Add("@UserName", SqlDbType.VarChar).Value = userLoginModel.UserName;
                    sqlCommand.Parameters.Add("@Password", SqlDbType.VarChar).Value = userLoginModel.Password;
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    DataTable dataTable = new DataTable();
                    dataTable.Load(sqlDataReader);
                    if (dataTable.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dataTable.Rows)
                        {
                            HttpContext.Session.SetString("UserID", dr["UserID"].ToString());
                            HttpContext.Session.SetString("UserName", dr["UserName"].ToString());
                        }

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return RedirectToAction("Login", "User");
                    }

                }
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
            }

            return RedirectToAction("Login");
        }
        #endregion

        #region Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "User");
        }
        #endregion

        public IActionResult Login()
        {
            return View();

        }

        #region Register User
        public IActionResult UserRegister(UserRegisterModel userRegisterModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string connectionString = this._configuration.GetConnectionString("ConnectionString");
                    SqlConnection sqlConnection = new SqlConnection(connectionString);
                    sqlConnection.Open();
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.CommandText = "PR_User_Register";
                    sqlCommand.Parameters.Add("@UserName", SqlDbType.VarChar).Value = userRegisterModel.UserName;
                    sqlCommand.Parameters.Add("@Password", SqlDbType.VarChar).Value = userRegisterModel.Password;
                    sqlCommand.Parameters.Add("@Email", SqlDbType.VarChar).Value = userRegisterModel.Email;
                    sqlCommand.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = userRegisterModel.MobileNo;
                    sqlCommand.Parameters.Add("@Address", SqlDbType.VarChar).Value = userRegisterModel.Address;
                    sqlCommand.ExecuteNonQuery();
                    return RedirectToAction("Login", "User");
                }
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Register");
            }
            return RedirectToAction("Register");
        }
        #endregion
        public IActionResult Register()
        {
            return View();

        }
    }
}

