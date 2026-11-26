
using Microsoft.EntityFrameworkCore;

namespace ITArizaTakip.Data
{
    public class DatabaseTest
    {
        private readonly AppDbContext _context;

        public DatabaseTest(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> TestConnectionAsync()
        {
            return await _context.Database.CanConnectAsync();
        }
    }
}
