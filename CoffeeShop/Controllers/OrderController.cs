using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Data; // Required for CommandType and DataTable
using System.Data.SqlClient;// Required for SqlConnection, SqlCommand, SqlDataReader
using CoffeeShop.Models;
using CoffeeShop.Services; 

namespace CoffeeShop.Controllers
{
    [CheckAccess]
    public class OrderController : Controller
    {

        private readonly DatabaseService _databaseService;
        private readonly IConfiguration _configuration;

        // Single constructor to inject both DatabaseService and IConfiguration
        public OrderController(DatabaseService databaseService, IConfiguration configuration)
        {
            _databaseService = databaseService;
            _configuration = configuration;
        }

        #region AddEdit
        public IActionResult OrderAddEdit(int OrderID)
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            
            #region UserDropDown
            ViewBag.UserList = _databaseService.GetUserDropDown();
            #endregion
            
            #region Customer Drop-Down
            ViewBag.CustomerList = _databaseService.GetCustomerDropDown();
            #endregion
            
            #region OrderByID
        
            OrderModel orderModel = new OrderModel();

            if (OrderID > 0)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PR_Order_GetByID";
                        command.Parameters.AddWithValue("@OrderID", OrderID);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            DataTable table = new DataTable();
                            table.Load(reader);

                            if (table.Rows.Count > 0)
                            {
                                DataRow dataRow = table.Rows[0];
                                orderModel.OrderDate = Convert.ToDateTime(dataRow["OrderDate"]);
                                orderModel.CustomerID = Convert.ToInt32(dataRow["CustomerID"]);
                                orderModel.PaymentMode = dataRow["PaymentMode"].ToString();
                                orderModel.TotalAmount = Convert.ToDouble(dataRow["TotalAmount"]);
                                orderModel.ShippingAddress = dataRow["ShippingAddress"].ToString();
                                orderModel.UserID = Convert.ToInt32(dataRow["UserID"]);
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
            return View("OrderAddEdit", orderModel);
            #endregion
        }
        #endregion
        
        #region SaveMethod
        public IActionResult OrderSave(OrderModel orderModel)
        {
            
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            if (orderModel.OrderID == 0)
            {
                command.CommandText = "PR_Order_Insert";
            }
            else
            {
                command.CommandText = "PR_Order_Update";
                command.Parameters.Add("@OrderID", SqlDbType.Int).Value = orderModel.OrderID;
            }
            command.Parameters.Add("@OrderDate", SqlDbType.DateTime).Value = orderModel.OrderDate;
            command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = orderModel.CustomerID;
            command.Parameters.Add("@PaymentMode", SqlDbType.VarChar).Value = orderModel.PaymentMode;
            command.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value = orderModel.TotalAmount;
            command.Parameters.Add("@ShippingAddress", SqlDbType.VarChar).Value = orderModel.ShippingAddress;
            command.Parameters.Add("@UserID", SqlDbType.Int).Value = orderModel.UserID;

            command.ExecuteNonQuery();
            return RedirectToAction("OrderAddEdit", new { OrderID = orderModel.OrderID });
            
        }
        #endregion
        
        public IActionResult OrdersList()
        {
            DataTable ordersTable = _databaseService.GetAllProcedure("PR_Order_GetAll");
            return View(ordersTable);
        }
        
        public IActionResult Delete(int ID)
        {
            try
            {
                // Use the database service to delete the order detail
                _databaseService.DeleteProcedure("PR_Order_Delete", "OrderID", ID);
                return Json(new { success = true, message = "Order deleted successfully." });
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547) // Foreign key violation error code in SQL Server
                {
                    return Json(new { success = false, message = "Unable to delete Order. The Order is referenced by other records." });
                }
                return Json(new { success = false, message = "An error occurred while deleting the Order." });
            }
        }
        
        
    }
}