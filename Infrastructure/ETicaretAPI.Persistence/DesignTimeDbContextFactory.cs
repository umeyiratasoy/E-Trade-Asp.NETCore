
using ETicaretAPI.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ETicaretAPIDbContext>
    {
        public ETicaretAPIDbContext CreateDbContext(string[] args)
        {

            DbContextOptionsBuilder<ETicaretAPIDbContext> dbContextOptionsBuiler = new();
            dbContextOptionsBuiler.UseNpgsql(Configuration.ConnectionString);
            return new ETicaretAPIDbContext(dbContextOptionsBuiler.Options);
        }
    }
}
