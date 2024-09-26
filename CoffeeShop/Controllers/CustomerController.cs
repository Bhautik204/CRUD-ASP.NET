using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Data; // Required for CommandType and DataTable
using System.Data.SqlClient;// Required for SqlConnection, SqlCommand, SqlDataReader
using CoffeeShop.Models;
using CoffeeShop.Services; 

namespace CoffeeShop.Controllers
{
    [CheckAccess]
    public class CustomerController : Controller
    {
        
        private readonly DatabaseService _databaseService;
        private readonly IConfiguration _configuration;

        // Single constructor to inject both DatabaseService and IConfiguration
        public CustomerController(DatabaseService databaseService, IConfiguration configuration)
        {
            _databaseService = databaseService;
            _configuration = configuration;
        }

        #region AddEdit
        public IActionResult CustomerAddEdit(int CustomerID)
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            
            #region UserDropDown
            ViewBag.UserList = _databaseService.GetUserDropDown();
            #endregion
            
            #region CustomerByID
        
            CustomerModel customerModel = new CustomerModel();

            if (CustomerID > 0)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PR_Customer_GetByID";
                        command.Parameters.AddWithValue("@CustomerID", CustomerID);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            DataTable table = new DataTable();
                            table.Load(reader);

                            if (table.Rows.Count > 0)
                            {
                                DataRow dataRow = table.Rows[0];
                                customerModel.CustomerName = dataRow["CustomerName"].ToString();
                                customerModel.HomeAddress = dataRow["HomeAddress"].ToString();
                                customerModel.Email = dataRow["Email"].ToString();
                                customerModel.MobileNo = dataRow["MobileNo"].ToString();
                                customerModel.GSTNo = dataRow["GSTNO"].ToString();
                                customerModel.CityName = dataRow["CityName"].ToString();
                                customerModel.PinCode = dataRow["PinCode"].ToString();
                                customerModel.NetAmount = Convert.ToDouble(dataRow["NetAmount"]);
                                customerModel.UserID = Convert.ToInt32(dataRow["UserID"]);
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

            return View("CustomerAddEdit", customerModel);
            #endregion
        }
        #endregion
        
        #region SaveMethod
        public IActionResult CustomerSave(CustomerModel customerModel)
        {
            
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            if (customerModel.CustomerID == 0)
            {
                command.CommandText = "PR_Customer_Insert";
            }
            else
            {
                command.CommandText = "PR_Customer_Update";
                command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = customerModel.CustomerID;
            }
            command.Parameters.Add("@CustomerName", SqlDbType.VarChar).Value = customerModel.CustomerName;
            command.Parameters.Add("@HomeAddress", SqlDbType.VarChar).Value = customerModel.HomeAddress;
            command.Parameters.Add("@Email", SqlDbType.VarChar).Value = customerModel.Email;
            command.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = customerModel.MobileNo;
            command.Parameters.Add("@GSTNo", SqlDbType.VarChar).Value = customerModel.GSTNo;
            command.Parameters.Add("@CityName", SqlDbType.VarChar).Value = customerModel.CityName;
            command.Parameters.Add("@PinCode", SqlDbType.VarChar).Value = customerModel.PinCode;
            command.Parameters.Add("@NetAmount", SqlDbType.VarChar).Value = customerModel.NetAmount;
            command.Parameters.Add("@UserID", SqlDbType.Int).Value = customerModel.UserID;
            command.ExecuteNonQuery();
            return RedirectToAction("CustomerAddEdit", new { CustomerID = customerModel.CustomerID });
            
        }
        #endregion

        #region Customer List
        public IActionResult CustomersList()
        {
            DataTable customersTable = _databaseService.GetAllProcedure("PR_Customer_GetAll");
            return View(customersTable);
        }
        #endregion

        public IActionResult Delete(int ID)
        {
            try { 
            // Use the database service to delete the order detail
            _databaseService.DeleteProcedure("PR_Customer_Delete", "CustomerID", ID);
                return Json(new { success = true, message = "Customer deleted successfully." });
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547) // Foreign key violation error code in SQL Server
                {
                    return Json(new { success = false, message = "Unable to delete Customer. The Customer is referenced by other records." });
                }
                return Json(new { success = false, message = "An error occurred while deleting the Customer." });
            }
        }
    }
}