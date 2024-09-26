using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Data; // Required for CommandType and DataTable
using System.Data.SqlClient; // Required for SqlConnection, SqlCommand, SqlDataReader
using CoffeeShop.Models; 
using CoffeeShop.Services;

namespace CoffeeShop.Controllers
{
    [CheckAccess]
    public class OrderDetailsController : Controller
    {
        private readonly DatabaseService _databaseService;
        private readonly IConfiguration _configuration;

        // Single constructor to inject both DatabaseService and IConfiguration
        public OrderDetailsController(DatabaseService databaseService, IConfiguration configuration)
        {
            _databaseService = databaseService;
            _configuration = configuration;
        }
        
        
        #region AddEdit

        public IActionResult OrderDetailsAddEdit(int OrderDetailID)
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");

            #region OrderDropDown

            ViewBag.OrderList = _databaseService.GetOrderDropDown();

            #endregion

            #region ProductDropDown

            ViewBag.ProductList = _databaseService.GetProductDropDown();

            #endregion

            #region UserDropDown

            ViewBag.UserList = _databaseService.GetUserDropDown();

            #endregion

            #region OrderDetailsByID

            OrderDetailsModel orderDetailsModel = new OrderDetailsModel();

            if (OrderDetailID > 0)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PR_OrderDetail_GetByID";
                        command.Parameters.AddWithValue("@OrderDetailID", OrderDetailID);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            DataTable table = new DataTable();
                            table.Load(reader);

                            if (table.Rows.Count > 0)
                            {
                                DataRow dataRow = table.Rows[0];
                                orderDetailsModel.OrderID = Convert.ToInt32(dataRow["OrderID"]);
                                orderDetailsModel.ProductID = Convert.ToInt32(dataRow["ProductID"]);
                                orderDetailsModel.Quantity = Convert.ToInt32(dataRow["Quantity"]);
                                orderDetailsModel.Amount = Convert.ToDouble(dataRow["Amount"]);
                                orderDetailsModel.TotalAmount = Convert.ToDouble(dataRow["TotalAmount"]);
                                orderDetailsModel.UserID = Convert.ToInt32(dataRow["UserID"]);
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

            return View("OrderDetailsAddEdit", orderDetailsModel);
            #endregion
        }

        #endregion
        
        #region SaveMethod
        public IActionResult OrderDetailsSave(OrderDetailsModel orderDetailsModel)
        {
            
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            if (orderDetailsModel.OrderDetailID == 0)
            {
                command.CommandText = "PR_OrderDetail_Insert";
            }
            else
            {
                command.CommandText = "PR_OrderDetail_Update";
                command.Parameters.Add("@OrderDetailID", SqlDbType.Int).Value = orderDetailsModel.OrderDetailID;
            }
            command.Parameters.Add("@OrderID", SqlDbType.Int).Value = orderDetailsModel.OrderID;
            command.Parameters.Add("@ProductID", SqlDbType.Int).Value = orderDetailsModel.ProductID;
            command.Parameters.Add("@Quantity", SqlDbType.Int).Value = orderDetailsModel.Quantity;
            command.Parameters.Add("@Amount", SqlDbType.Decimal).Value = orderDetailsModel.Amount;
            command.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value = orderDetailsModel.TotalAmount;
            command.Parameters.Add("@UserID", SqlDbType.Int).Value = orderDetailsModel.UserID;
            command.ExecuteNonQuery();
            return RedirectToAction("OrderDetailsAddEdit", new { OrderDetailID = orderDetailsModel.OrderDetailID });
            
        }
        #endregion
        
        public IActionResult OrderDetailsList()
        {
            DataTable orderdetailsTable = _databaseService.GetAllProcedure("PR_OrderDetail_GetAll");
            return View(orderdetailsTable );
        }
        
        public IActionResult Delete(int ID)
        {
            try
            {
                // Use the database service to delete the order detail
                _databaseService.DeleteProcedure("PR_OrderDetail_Delete", "OrderDetailID", ID);
                return Json(new { success = true, message = "OrderDetail deleted successfully." });
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547) // Foreign key violation error code in SQL Server
                {
                    return Json(new { success = false, message = "Unable to delete OrderDetail. The OrderDetail is referenced by other records." });
                }
                return Json(new { success = false, message = "An error occurred while deleting the OrderDetail." });
            }
        }
    }
}