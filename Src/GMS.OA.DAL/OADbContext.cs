using System;
using GMS.Framework.DAL;
using GMS.Core.Config;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using GMS.OA.Contract;
using GMS.Core.Log;

namespace GMS.OA.DAL
{
    public class OADbContext : DbContextBase
    {
        public OADbContext()
            : base(CachedConfigContext.Current.DaoConfig.OA, new LogDbContext())
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<OADbContext>(null);
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Branch> Branchs { get; set; }
    }
}
