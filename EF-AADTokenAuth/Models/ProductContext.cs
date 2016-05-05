using System.Data.Common;
using System.Data.Entity;

namespace EF_AADTokenAuth.Models
{
    public class ProductContext: DbContext
    {
        public ProductContext() : base()
        {

        }

        public ProductContext(DbConnection existingConnection, bool contextOwnsConnection) : base(existingConnection, contextOwnsConnection)
        {

        }

        public DbSet<Product> Products { get; set; }
    }
}