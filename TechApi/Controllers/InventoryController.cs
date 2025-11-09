using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Newtonsoft.Json;
using TechApi.Models;


namespace TechApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        [HttpPost]
        public ActionResult SaveInventoryData(Inventory InventoryDto)
        {
            SqlConnection connection = new SqlConnection
            {
                ConnectionString = "Server=localhost;Database=shopDb;Trusted_Connection=True;MultipleActiveResultSets=true;"
            };

            SqlCommand command = new SqlCommand
            {
                CommandText = "sp_SaveInventoryData",
                CommandType = System.Data.CommandType.StoredProcedure,
                Connection = connection
            };
            command.Parameters.AddWithValue("@ProductId", InventoryDto.ProductId);
            command.Parameters.AddWithValue("@ProductName", InventoryDto.ProductName);
            command.Parameters.AddWithValue("@StockAvailable", InventoryDto.StockAvailable);
            command.Parameters.AddWithValue("@ReorderStock", InventoryDto.ReorderStock);

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
            return Ok(new { message = "Inventory Data Saved!" });
        }


        [HttpDelete]
        public ActionResult DeleteInventoryData(int ProductId)
        {
            SqlConnection connection = new SqlConnection 
            {
                ConnectionString = "Server=localhost;Database=shopDb;Trusted_Connection=True;MultipleActiveResultSets=true;"
            };

            SqlCommand command = new SqlCommand
            {
                CommandText = "sp_DeleteInventoryDetails",
                CommandType = System.Data.CommandType.StoredProcedure,
                Connection = connection
            };
            connection.Open();
            command.Parameters.AddWithValue("@ProductId", ProductId);
            command.ExecuteNonQuery();
            connection.Close();
            return Ok();
        }

        [HttpPut]
        public ActionResult UpdateInventoryData(Inventory InventoryDto)
        {
            SqlConnection connection = new SqlConnection
            {
                ConnectionString = "Server=localhost;Database=shopDb;Trusted_Connection=True;MultipleActiveResultSets=true;"
            };

            SqlCommand command = new SqlCommand
            {
                CommandText = "sp_UpdateInventoryData",
                CommandType = System.Data.CommandType.StoredProcedure,
                Connection = connection
            };
            connection.Open();
            command.Parameters.AddWithValue("@ProductId", InventoryDto.ProductId);
            command.Parameters.AddWithValue("@ProductName", InventoryDto.ProductName);
            command.Parameters.AddWithValue("@StockAvailable", InventoryDto.StockAvailable);
            command.Parameters.AddWithValue("@ReorderStock", InventoryDto.ReorderStock);
            command.ExecuteNonQuery();
            connection.Close();
            return Ok();
        }


        [HttpGet]
        public ActionResult GetInventoryData()
        {
            SqlConnection connection = new SqlConnection
            {
                ConnectionString = "Server=localhost;Database=shopDb;Trusted_Connection=True;MultipleActiveResultSets=true;"
            };

            SqlCommand command = new SqlCommand
            {
                CommandText = "sp_GetInventoryData",
                CommandType = System.Data.CommandType.StoredProcedure,
                Connection = connection
            };
            connection.Open();

            List<InventoryDto> response = new List<InventoryDto>();
            using (SqlDataReader sqlDataReader = command.ExecuteReader())
            {
                while(sqlDataReader.Read())
                {
                    InventoryDto inventoryDto = new InventoryDto();
                    inventoryDto.ProductId = Convert.ToInt32(sqlDataReader["ProductId"]);
                    inventoryDto.ProductName = Convert.ToString(sqlDataReader["ProductName"]);
                    inventoryDto.StockAvailable = Convert.ToInt32(sqlDataReader["StockAvailable"]);
                    inventoryDto.ReorderStock = Convert.ToInt32(sqlDataReader["ReorderStock"]);

                    response.Add(inventoryDto);
                }
            }
            connection.Close();
            return Ok(JsonConvert.SerializeObject(response));
        }

    }
}
