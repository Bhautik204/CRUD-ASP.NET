using CoffeeShop.Models;
namespace CoffeeShop.Services;
using System.Data;
using System.Data.SqlClient;

public class DatabaseService
{
    #region Configuration
    private readonly IConfiguration _configuration;
    public DatabaseService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    #endregion

    #region DeleteProcedure

    // Method to execute stored procedure that deletes an entry
    public void DeleteProcedure(string procedureName, string paramName, int id)
    {
        string connectionString = _configuration.GetConnectionString("ConnectionString");
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(procedureName, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(paramName, id);
                command.ExecuteNonQuery();
            }
        }
    }
    #endregion

    #region GetAllProcedure
    // Method to execute stored procedure that retrieves data
    public DataTable GetAllProcedure(string procedureName, Dictionary<string, object> parameters = null)
    {
        string connectionString = _configuration.GetConnectionString("ConnectionString");
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = procedureName;

                // Add any parameters to the command
                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        command.Parameters.AddWithValue(param.Key, param.Value);
                    }
                }

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    DataTable table = new DataTable();
                    table.Load(reader);
                    return table;
                }
            }
        }
    }
    #endregion

    #region GetUserDropDown
    // Method to retrieve User drop-down data
    public List<UserDropDownModel> GetUserDropDown()
    {
        string connectionString = _configuration.GetConnectionString("ConnectionString");
        List<UserDropDownModel> users = new List<UserDropDownModel>();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_User_DropDown";

            using (SqlDataReader reader = command.ExecuteReader())
            {
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);

                foreach (DataRow dataRow in dataTable.Rows)
                {
                    UserDropDownModel userDropDownModel = new UserDropDownModel
                    {
                        UserID = Convert.ToInt32(dataRow["UserID"]),
                        UserName = dataRow["UserName"].ToString()
                    };
                    users.Add(userDropDownModel);
                }
            }
        }
    
        return users;
    }
    #endregion

    #region GetCustomerDropDown
    // Method to retrieve Customer drop-down data
    public List<CustomerDropDownModel> GetCustomerDropDown()
    {
        string connectionString = _configuration.GetConnectionString("ConnectionString");
        List<CustomerDropDownModel> customers = new List<CustomerDropDownModel>();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Customer_DropDown";

            using (SqlDataReader reader = command.ExecuteReader())
            {
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);

                foreach (DataRow dataRow in dataTable.Rows)
                {
                    CustomerDropDownModel customerDropDownModel = new CustomerDropDownModel
                    {
                        CustomerID = Convert.ToInt32(dataRow["CustomerID"]),
                        CustomerName = dataRow["CustomerName"].ToString()
                    };
                    customers.Add(customerDropDownModel);
                }
            }
        }
    
        return customers;
    }
    #endregion

    #region GetProductDropDown
    // Method to retrieve Product drop-down data
    public List<ProductDropDownModel> GetProductDropDown()
    {
        string connectionString = _configuration.GetConnectionString("ConnectionString");
        List<ProductDropDownModel> products = new List<ProductDropDownModel>();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Product_DropDown"; // Update with your actual stored procedure name

            using (SqlDataReader reader = command.ExecuteReader())
            {
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);

                foreach (DataRow dataRow in dataTable.Rows)
                {
                    ProductDropDownModel productDropDownModel = new ProductDropDownModel
                    {
                        ProductID = Convert.ToInt32(dataRow["ProductID"]),
                        ProductName = dataRow["ProductName"].ToString()
                    };
                    products.Add(productDropDownModel);
                }
            }
        }

        return products;
    }
    #endregion

    #region GetOrderDropDown
    // Method to retrieve Order drop-down data
    public List<OrderDropDownModel> GetOrderDropDown()
    {
        string connectionString = _configuration.GetConnectionString("ConnectionString");
        List<OrderDropDownModel> orders = new List<OrderDropDownModel>();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Order_DropDown"; // Update with your actual stored procedure name

            using (SqlDataReader reader = command.ExecuteReader())
            {
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);

                foreach (DataRow dataRow in dataTable.Rows)
                {
                    OrderDropDownModel orderDropDownModel = new OrderDropDownModel
                    {
                        OrderID = Convert.ToInt32(dataRow["OrderID"]),
                        OrderDate = Convert.ToDateTime(dataRow["OrderDate"]) // Adjust property name and type as needed
                    };
                    orders.Add(orderDropDownModel);
                }
            }
        }

        return orders;
    }
    #endregion
}

