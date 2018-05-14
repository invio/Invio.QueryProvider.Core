using System.Linq;
using Invio.QueryProvider.Test.Models;

namespace Invio.QueryProvider.Test.CSharp {
    public class ListAsQueryableTest : QueryableTestBase {
        private Data testData { get; } = new Data();
        protected override IQueryable<Product> Products => this.testData.Products.AsQueryable();
        protected override IQueryable<Customer> Customers => this.testData.Customers.AsQueryable();
        protected override IQueryable<Employee> Employees => this.testData.Employees.AsQueryable();
        protected override IQueryable<Order> Orders => this.testData.Orders.AsQueryable();
        protected override IQueryable<Shipper> Shippers => this.testData.Shippers.AsQueryable();
        protected override IQueryable<Supplier> Suppliers => this.testData.Suppliers.AsQueryable();
        protected override IQueryable<Category> Categories => this.testData.Categories.AsQueryable();
    }
}
