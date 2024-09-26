using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Data; // Required for CommandType and DataTable
using System.Data.SqlClient; // Required for SqlConnection, SqlCommand, SqlDataReader
using CoffeeShop.Models; 
using CoffeeShop.Services;

namespace CoffeeShop.Controllers
{
    [CheckAccess]
    public class ProductController : Controller
    {
        private readonly DatabaseService _databaseService;
        private readonly IConfiguration _configuration;

        // Single constructor to inject both DatabaseService and IConfiguration
        public ProductController(DatabaseService databaseService, IConfiguration configuration)
        {
            _databaseService = databaseService;
            _configuration = configuration;
        }

        
        #region AddEdit
        public IActionResult ProductAddEdit(int ProductID)
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");

            #region UserDropDown
            ViewBag.UserList = _databaseService.GetUserDropDown();
            #endregion

            #region ProductByID
            ProductModel productModel = new ProductModel();

            if (ProductID > 0)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PR_Product_GetByID";
                        command.Parameters.AddWithValue("@ProductID", ProductID);
                
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            DataTable table = new DataTable();
                            table.Load(reader);

                            if (table.Rows.Count > 0)
                            {
                                DataRow dataRow = table.Rows[0];
                                productModel.ProductID = Convert.ToInt32(dataRow["ProductID"]);
                                productModel.ProductName = dataRow["ProductName"].ToString();
                                productModel.ProductCode = dataRow["ProductCode"].ToString();
                                productModel.ProductPrice = Convert.ToDouble(dataRow["ProductPrice"]);
                                productModel.Description = dataRow["Description"].ToString();
                                productModel.UserID = Convert.ToInt32(dataRow["UserID"]);
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
    
            return View("ProductAddEdit", productModel);
            #endregion
        }
        #endregion
        
        #region SaveMethod
        public IActionResult ProductSave(ProductModel productModel)
        {
            
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            if (productModel.ProductID == 0)
            {
                command.CommandText = "PR_Product_Insert";
            }
            else
            {
                command.CommandText = "PR_Product_Update";
                command.Parameters.Add("@ProductID", SqlDbType.Int).Value = productModel.ProductID;
            }
            command.Parameters.Add("@ProductName", SqlDbType.VarChar).Value = productModel.ProductName;
            command.Parameters.Add("@ProductCode", SqlDbType.VarChar).Value = productModel.ProductCode;
            command.Parameters.Add("@ProductPrice", SqlDbType.Decimal).Value = productModel.ProductPrice;
            command.Parameters.Add("@Description", SqlDbType.VarChar).Value = productModel.Description;
            command.Parameters.Add("@UserID", SqlDbType.Int).Value = productModel.UserID;
            command.ExecuteNonQuery();
            return RedirectToAction("ProductAddEdit", new { ProductID = productModel.ProductID });
            
        }
        #endregion
        
        #region TableList
        public IActionResult ProductsList()
        {
            DataTable productsTable = _databaseService.GetAllProcedure("PR_Product_GetAll");
            return View(productsTable);
        }
        #endregion
        
        #region DeleteOperation
        public IActionResult Delete(int ID)
        {
            try
            {
                // Use the database service to delete the order detail
                _databaseService.DeleteProcedure("PR_Product_Delete", "ProductID", ID);
                return Json(new { success = true, message = "Product deleted successfully." });
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547) // Foreign key violation error code in SQL Server
                {
                    return Json(new { success = false, message = "Unable to delete Product. The Product is referenced by other records." });
                }
                return Json(new { success = false, message = "An error occurred while deleting the Product." });
            }
        }
        #endregion
    }
}
