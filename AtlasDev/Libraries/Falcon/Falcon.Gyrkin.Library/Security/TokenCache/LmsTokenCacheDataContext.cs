using System.Data.Entity;
using Thinktecture.IdentityModel.Web;

namespace Falcon.Web.Security.TokenCache.Providers
{
    public class LmsTokenCacheDataContext : DbContext
    {
        public LmsTokenCacheDataContext()
            : base("name=LmsDbContext")
        {
        }

        public DbSet<TokenCacheItem> Tokens { get; set; }
    }
}