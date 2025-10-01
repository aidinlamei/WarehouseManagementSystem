using Microsoft.EntityFrameworkCore;

namespace WarehouseManagement.Infrastructure.Data
{
    public static class DatabaseTester
    {
        public static async Task TestConnection(string connectionString)
        {
            try
            {
                var options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseSqlServer(connectionString)
                    .Options;

                using var context = new AppDbContext(options);
                var canConnect = await context.Database.CanConnectAsync();

                if (canConnect)
                    Console.WriteLine("✅ Connection successful!");
                else
                    Console.WriteLine("❌ Connection failed!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Connection error: {ex.Message}");
            }
        }
    }
}