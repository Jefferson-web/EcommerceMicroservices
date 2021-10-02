using Dapper;
using Discount.Grpc.Models;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.Grpc.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly IConfiguration _configuration;
        enum ResultType { FAILURE = 0, SUCCESS = 1 };

        public DiscountRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
        private IDbConnection Connection {
            get {
                var db = new NpgsqlConnection
                    (_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
                db.Open();
                return db;
            }
        }

        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            using (var db = Connection)
            {
                string query = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES(@ProductName, @Description, @Amount)";

                var parameters = new
                {
                    @ProductName = coupon.ProductName, @Description = coupon.Description, @Amount = coupon.Amount
                };

                int affected = await db.ExecuteAsync(query, parameters);
                if (affected == 0) {
                    return false;
                }
                return true;
            }
        }

        public async Task<bool> DeleteDicount(string ProductName)
        {
            using (var db = Connection)
            {
                string query = "DELETE FROM Coupon WHERE ProductName = @ProductName";

                var affected = await db.ExecuteAsync(query, new { @ProductName = ProductName });

                if (affected == 0) {
                    return false;
                }
                return true;
            }   
        }

        public async Task<Coupon> GetDiscount(string ProductName)
        {
            using (var db = Connection)
            {
                string query = "SELECT * FROM Coupon WHERE ProductName = @ProductName";

                var coupon = await db.QueryFirstOrDefaultAsync<Coupon>(query, new { @ProductName = ProductName });

                if (coupon == null) {
                    return new Coupon
                    {
                        ProductName = "No Discount", Amount = 0, Description = "No Discount Desc"
                    };
                }
                return coupon;
            }
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            using (var db = Connection)
            {
                string query = "UPDATE Coupon SET ProductName=@ProductName, Description=@Description, Amount=@Amount WHERE Id = @Id";
                var parameters = new
                {
                    @ProductName = coupon.ProductName, 
                    @Description = coupon.Description, 
                    @Amount = coupon.Amount, 
                    @Id = coupon.Id
                };
                var affected = await db.ExecuteAsync(query, parameters);
                if (affected == 0) {
                    return false;
                }
                return true;
            }
        }
    }
}
