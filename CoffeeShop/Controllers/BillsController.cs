using CoffeeShop.Models; // Required for SqlConnection, SqlCommand, SqlDataReader
using CoffeeShop.Services;
using Microsoft.AspNetCore.Mvc;
using System.Data; // Required for CommandType and DataTable
using System.Data.SqlClient;

namespace CoffeeShop.Controllers
{
    [CheckAccess]
    public class BillsController : Controller
    {
        private readonly DatabaseService _databaseService;
        private readonly IConfiguration _configuration;

        // Single constructor to inject both DatabaseService and IConfiguration
        public BillsController(DatabaseService databaseService, IConfiguration configuration)
        {
            _databaseService = databaseService;
            _configuration = configuration;
        }

        #region AddEdit
        public IActionResult BillsAddEdit(int BillID)
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");

            #region OrderDropDown
            ViewBag.OrderList = _databaseService.GetOrderDropDown();
            #endregion

            #region UserDropDown
            ViewBag.UserList = _databaseService.GetUserDropDown();
            #endregion

            #region BillByID

            BillsModel billsModel = new BillsModel();

            if (BillID > 0)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PR_Bill_GetByID";
                        command.Parameters.AddWithValue("@BillID", BillID);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            DataTable table = new DataTable();
                            table.Load(reader);

                            if (table.Rows.Count > 0)
                            {
                                DataRow dataRow = table.Rows[0];
                                billsModel.BillNumber = dataRow["BillNumber"].ToString();
                                billsModel.BillDate = Convert.ToDateTime(dataRow["BillDate"]);
                                billsModel.OrderID = Convert.ToInt32(dataRow["OrderID"]);
                                billsModel.TotalAmount = Convert.ToDouble(dataRow["TotalAmount"]);
                                billsModel.Discount = dataRow["Discount"] != DBNull.Value ? Convert.ToDouble(dataRow["Discount"]) : (double?)null;
                                billsModel.NetAmount = Convert.ToDouble(dataRow["NetAmount"]);
                                billsModel.UserID = Convert.ToInt32(dataRow["UserID"]);
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

            return View("BillsAddEdit", billsModel);
            #endregion
        }
        #endregion

        #region SaveMethod
        public IActionResult BillsSave(BillsModel billsModel)
        {

            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            if (billsModel.BillID == 0)
            {
                command.CommandText = "PR_Bill_Insert";
            }
            else
            {
                command.CommandText = "PR_Bill_Update";
                command.Parameters.Add("@BillID", SqlDbType.Int).Value = billsModel.BillID;
            }
            command.Parameters.Add("@BillNumber", SqlDbType.VarChar).Value = billsModel.BillNumber;
            command.Parameters.Add("@BillDate", SqlDbType.DateTime).Value = billsModel.BillDate;
            command.Parameters.Add("@OrderID", SqlDbType.Int).Value = billsModel.OrderID;
            command.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value = billsModel.TotalAmount;
            command.Parameters.Add("@Discount", SqlDbType.Decimal).Value = (object)billsModel.Discount ?? DBNull.Value;
            command.Parameters.Add("@NetAmount", SqlDbType.Decimal).Value = billsModel.NetAmount;
            command.Parameters.Add("@UserID", SqlDbType.Int).Value = billsModel.UserID;

            command.ExecuteNonQuery();
            return RedirectToAction("BillsAddEdit", new { BillID = billsModel.BillID });

        }
        #endregion



        public IActionResult BillsList()
        {
            DataTable billsTable = _databaseService.GetAllProcedure("PR_Bill_GetAll");
            return View(billsTable);
        }

        public IActionResult Delete(int ID)
        {
            try
            {
                // Use the database service to delete the order detail
                _databaseService.DeleteProcedure("PR_Bill_Delete", "BillID", ID);
                return Json(new { success = true, message = "Bills deleted successfully." });
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547) // Foreign key violation error code in SQL Server
                {
                    return Json(new { success = false, message = "Unable to delete Bill. The Bill is referenced by other records." });
                }
                return Json(new { success = false, message = "An error occurred while deleting the Bill." });
            }
        }


    }
}