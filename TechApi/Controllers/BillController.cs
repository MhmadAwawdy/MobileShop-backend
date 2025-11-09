using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data.SqlClient;
using TechApi.Models;

namespace TechApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillController : ControllerBase
    {
        private readonly string _connectionString = "Server=localhost;Database=shopDb;Trusted_Connection=True;MultipleActiveResultSets=true;";

        [HttpPost]
        public ActionResult SaveBill(BillRequestDto billRequest)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(_connectionString);
                connection.Open();

                string billNumber = GenerateBillNumber();

                decimal subtotal = billRequest.BillItems.Sum(item => item.Price * item.Quantity);
                decimal taxAmount = subtotal * (billRequest.TaxPercentage / 100);
                decimal totalAmount = subtotal + taxAmount;

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "sp_SaveBill";
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@BillNumber", billNumber);
                    command.Parameters.AddWithValue("@CustomerId", billRequest.CustomerId);
                    command.Parameters.AddWithValue("@BillDate", billRequest.BillDate);
                    command.Parameters.AddWithValue("@Subtotal", subtotal);
                    command.Parameters.AddWithValue("@TaxPercentage", billRequest.TaxPercentage);
                    command.Parameters.AddWithValue("@TaxAmount", taxAmount);
                    command.Parameters.AddWithValue("@TotalAmount", totalAmount);

                    command.ExecuteNonQuery();
                }

                foreach (var item in billRequest.BillItems)
                {
                    using SqlCommand itemCommand = new SqlCommand();
                    itemCommand.Connection = connection;
                    itemCommand.CommandText = "sp_SaveBillItem";
                    itemCommand.CommandType = System.Data.CommandType.StoredProcedure;

                    itemCommand.Parameters.AddWithValue("@BillNumber", billNumber);
                    itemCommand.Parameters.AddWithValue("@ProductId", item.ProductId);
                    itemCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                    itemCommand.Parameters.AddWithValue("@Price", item.Price);
                    itemCommand.Parameters.AddWithValue("@Total", item.Price * item.Quantity);

                    itemCommand.ExecuteNonQuery();
                }

                connection.Close();

                return Ok(new
                {
                    message = "Bill saved successfully!",
                    billNumber = billNumber,
                    totalAmount = totalAmount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error saving bill: {ex.Message}" });
            }
        }

        [HttpGet]
        public ActionResult GetAllBills()
        {
            try
            {
                using SqlConnection connection = new SqlConnection(_connectionString);
                SqlCommand command = new SqlCommand
                {
                    CommandText = "sp_GetAllBills",
                    CommandType = System.Data.CommandType.StoredProcedure,
                    Connection = connection
                };

                connection.Open();

                List<BillDto> bills = new List<BillDto>();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        BillDto bill = new BillDto
                        {
                            BillNumber = Convert.ToString(reader["BillNumber"]),
                            CustomerId = Convert.ToInt32(reader["CustomerId"]),
                            CustomerName = Convert.ToString(reader["CustomerName"]),
                            BillDate = Convert.ToDateTime(reader["BillDate"]),
                            Subtotal = Convert.ToDecimal(reader["Subtotal"]),
                            TaxPercentage = Convert.ToDecimal(reader["TaxPercentage"]),
                            TaxAmount = Convert.ToDecimal(reader["TaxAmount"]),
                            TotalAmount = Convert.ToDecimal(reader["TotalAmount"])
                        };
                        bills.Add(bill);
                    }
                }

                connection.Close();
                return Ok(JsonConvert.SerializeObject(bills));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error retrieving bills: {ex.Message}" });
            }
        }

        [HttpGet("{billNumber}")]
        public ActionResult GetBillDetails(string billNumber)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(_connectionString);
                connection.Open();

                BillDto bill = null;

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "sp_GetBillByNumber";
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@BillNumber", billNumber);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            bill = new BillDto
                            {
                                BillNumber = Convert.ToString(reader["BillNumber"]),
                                CustomerId = Convert.ToInt32(reader["CustomerId"]),
                                CustomerName = Convert.ToString(reader["CustomerName"]),
                                BillDate = Convert.ToDateTime(reader["BillDate"]),
                                Subtotal = Convert.ToDecimal(reader["Subtotal"]),
                                TaxPercentage = Convert.ToDecimal(reader["TaxPercentage"]),
                                TaxAmount = Convert.ToDecimal(reader["TaxAmount"]),
                                TotalAmount = Convert.ToDecimal(reader["TotalAmount"])
                            };
                        }
                    }
                }

                if (bill == null)
                {
                    return NotFound(new { message = "Bill not found" });
                }

                using (SqlCommand itemCommand = new SqlCommand())
                {
                    itemCommand.Connection = connection;
                    itemCommand.CommandText = "sp_GetBillItems";
                    itemCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    itemCommand.Parameters.AddWithValue("@BillNumber", billNumber);

                    using (SqlDataReader reader = itemCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            BillItemDto item = new BillItemDto
                            {
                                BillItemId = Convert.ToInt32(reader["BillItemId"]),
                                BillNumber = Convert.ToString(reader["BillNumber"]),
                                ProductId = Convert.ToInt32(reader["ProductId"]),
                                ProductName = Convert.ToString(reader["ProductName"]),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                                Price = Convert.ToDecimal(reader["Price"]),
                                Total = Convert.ToDecimal(reader["Total"])
                            };
                            bill.BillItems.Add(item);
                        }
                    }
                }

                connection.Close();
                return Ok(JsonConvert.SerializeObject(bill));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error retrieving bill details: {ex.Message}" });
            }
        }

        [HttpDelete("{billNumber}")]
        public ActionResult DeleteBill(string billNumber)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(_connectionString);
                SqlCommand command = new SqlCommand
                {
                    CommandText = "sp_DeleteBill",
                    CommandType = System.Data.CommandType.StoredProcedure,
                    Connection = connection
                };

                command.Parameters.AddWithValue("@BillNumber", billNumber);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                connection.Close();

                if (rowsAffected > 0)
                    return Ok(new { message = "Bill deleted successfully!" });
                else
                    return NotFound(new { message = "Bill not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error deleting bill: {ex.Message}" });
            }
        }

        private string GenerateBillNumber()
        {
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            Random random = new Random();
            int randomNum = random.Next(1000, 9999);
            return $"BILL-{timestamp}{randomNum}";
        }
    }
}