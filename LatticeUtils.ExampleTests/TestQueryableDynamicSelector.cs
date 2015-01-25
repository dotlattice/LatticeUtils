using Effort;
using Effort.DataLoaders;
using LatticeUtils.ExampleTests.Examples;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LatticeUtils.ExampleTests
{
    public class TestQueryableDynamicSelector
    {
        [Test]
        public void Northwind_SimpleDynamicSelectMatchesNormalSelect()
        {
            using (var connection = Effort.DbConnectionFactory.CreateTransient())
            using (var context = new NorthwindContext(connection, contextOwnsConnection: true))
            {
                var dynamicSelectQueryable = new QueryableDynamicSelector().SelectProperties(context.Customers, new HashSet<string> { "CustomerID", "ContactName" });
                var dynamicSelectSql = dynamicSelectQueryable.ToString();

                var normalSelectQueryable = context.Customers.Select(c => new { c.CustomerID, c.ContactName });
                var normalSelectSql = normalSelectQueryable.ToString();

                Assert.AreEqual(normalSelectSql, dynamicSelectSql);
            }
        }

        #region Entity Framework

        private class NorthwindContext : DbContext
        {
            static NorthwindContext()
            {
                System.Data.Entity.Database.SetInitializer<NorthwindContext>(null);
            }

            public NorthwindContext()
                : base()
            {
                Initialize();
            }

            public NorthwindContext(DbConnection existingConnection, bool contextOwnsConnection)
                : base(existingConnection, contextOwnsConnection)
            {
                Initialize();
            }

            private void Initialize()
            {
                Configuration.ProxyCreationEnabled = false;
                Configuration.LazyLoadingEnabled = false;
            }

            public DbSet<Customer> Customers { get; set; }
        }

        private class Customer
        {
            public string CustomerID { get; set; }
            public string CompanyName { get; set; }
            public string ContactName { get; set; }
            public string ContactTitle { get; set; }
            public string Address { get; set; }
            public string City { get; set; }
            public string Region { get; set; }
            public string PostalCode { get; set; }
            public string Country { get; set; }
            public string Phone { get; set; }
            public string Fax { get; set; }
        }

        #endregion
    }
}
