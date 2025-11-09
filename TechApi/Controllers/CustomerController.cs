using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data.SqlClient;
using TechApi.Models;

namespace TechApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        [HttpPost]
        public ActionResult SaveCustomerData(CustomerRequestDto CustomerRequestDto)
        {
            using SqlConnection connection = new SqlConnection
            {
                ConnectionString = "Server=localhost;Database=shopDb;Trusted_Connection=True;MultipleActiveResultSets=true;"
            };

            SqlCommand command = new SqlCommand
            {
                CommandText = "sp_SaveCustomerDetails",
                CommandType = System.Data.CommandType.StoredProcedure,
                Connection = connection
            };

            command.Parameters.AddWithValue("@CustomerId", CustomerRequestDto.CustomerId);
            command.Parameters.AddWithValue("@FirstName", CustomerRequestDto.FirstName);
            command.Parameters.AddWithValue("@LastName", CustomerRequestDto.LastName);
            command.Parameters.AddWithValue("@Email", CustomerRequestDto.Email);
            command.Parameters.AddWithValue("@Mobile", CustomerRequestDto.Mobile);
            command.Parameters.AddWithValue("@RegistrationDate", CustomerRequestDto.RegistrationDate);

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();

            return Ok(new { message = "Customer Data Saved!" });
        }

        [HttpPut]
        public ActionResult UpdateCustomerData(CustomerRequestDto customerRequestDto)
        {
            using SqlConnection connection = new SqlConnection
            {
                ConnectionString = "Server=localhost;Database=shopDb;Trusted_Connection=True;MultipleActiveResultSets=true;"
            };

            SqlCommand command = new SqlCommand
            {
                CommandText = "sp_UpdateCustomerDetails",
                CommandType = System.Data.CommandType.StoredProcedure,
                Connection = connection
            };

            command.Parameters.AddWithValue("@CustomerId", customerRequestDto.CustomerId);
            command.Parameters.AddWithValue("@FirstName", customerRequestDto.FirstName);
            command.Parameters.AddWithValue("@LastName", customerRequestDto.LastName);
            command.Parameters.AddWithValue("@Email", customerRequestDto.Email);
            command.Parameters.AddWithValue("@Mobile", customerRequestDto.Mobile);
            command.Parameters.AddWithValue("@RegistrationDate", customerRequestDto.RegistrationDate);

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();

            return Ok(new { message = "Customer Data Updated!" });
        }

        [HttpDelete("{customerId}")]
        public ActionResult DeleteCustomerData(int customerId)
        {
            using SqlConnection connection = new SqlConnection
            {
                ConnectionString = "Server=localhost;Database=shopDb;Trusted_Connection=True;MultipleActiveResultSets=true;"
            };

            using SqlCommand command = new SqlCommand
            {
                CommandText = "sp_DeleteCustomerDetails",
                CommandType = System.Data.CommandType.StoredProcedure,
                Connection = connection
            };

            command.Parameters.AddWithValue("@CustomerId", customerId);

            connection.Open();
            int rowsAffected = command.ExecuteNonQuery();
            connection.Close();

            if (rowsAffected > 0)
                return Ok(new { message = "Customer deleted successfully!" });
            else
                return NotFound(new { message = "Customer not found or already deleted." });
        }


        [HttpGet]
        public ActionResult GetCustomerData()
        {
            SqlConnection connection = new SqlConnection
            {
                ConnectionString = "Server=localhost;Database=shopDb;Trusted_Connection=True;MultipleActiveResultSets=true;"
            };

            SqlCommand command = new SqlCommand
            {
                CommandText = "sp_getCustomerDetails",
                CommandType = System.Data.CommandType.StoredProcedure,
                Connection = connection
            };
            connection.Open();

            List<CustomerDto> response = new List<CustomerDto>();
            using (SqlDataReader sqlDataReader = command.ExecuteReader())
            {
                while (sqlDataReader.Read())
                {
                    CustomerDto customerDto = new CustomerDto();
                    customerDto.CustomerId = Convert.ToInt32(sqlDataReader["CustomerId"]);
                    customerDto.FirstName = Convert.ToString(sqlDataReader["FirstName"]);
                    customerDto.LastName = Convert.ToString(sqlDataReader["LastName"]);
                    customerDto.Email = Convert.ToString(sqlDataReader["Email"]);
                    customerDto.Mobile = Convert.ToString(sqlDataReader["Mobile"]);
                    customerDto.RegistrationDate = Convert.ToDateTime(sqlDataReader["RegistrationDate"]);

                    response.Add(customerDto);
                }
            }
            connection.Close();
            return Ok(JsonConvert.SerializeObject(response));
        }
    }
}