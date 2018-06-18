using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using Invio.QueryProvider.Test.Models;
using Xunit;

namespace Invio.QueryProvider.Test.CSharp {
    public abstract class QueryableTestBase {
        protected abstract IQueryable<Product> Products { get; }
        protected abstract IQueryable<Customer> Customers { get; }
        protected abstract IQueryable<Employee> Employees { get; }
        protected abstract IQueryable<Order> Orders { get; }
        protected abstract IQueryable<Shipper> Shippers { get; }
        protected abstract IQueryable<Supplier> Suppliers { get; }
        protected abstract IQueryable<Category> Categories { get; }

        private static ISet<Int32> ShipperIds { get; } = ImmutableHashSet.Create(1, 2, 3);

        [Fact]
        public void Enumerate_All() {
            var count = 0;
            var ids = ImmutableHashSet<Int32>.Empty;

            using (var enumerator = this.Shippers.GetEnumerator()) {
                while (enumerator.MoveNext()) {
                    count++;
                    ids = ids.Add(enumerator.Current.ShipperId);
                }
            }

            Assert.Equal(3, count);
            Assert.Equal(ShipperIds, ids);
        }

        //==================================================
        // Where Clause Tests
        //==================================================

        #region Where Clause Comparison Operators

        //--------------------------------------------------
        // Equality/Inequality Comparisons
        //--------------------------------------------------

        // See Northwind data
        private static Int32[] ProductIdsWithReorderLevelTen { get; } =
            { 1, 7, 16, 41, 51, 54, 67 };

        [Fact]
        public virtual void Where_Field_Eq_Literal() {
            var result = this.Products.Where(p => p.ReorderLevel == 10).ToList();

            AssertIsProductsWithReorderLevelTen(result);
        }

        [Fact]
        public virtual void Where_Field_Eq_ArithmeticExpression() {
            var result = this.Products.Where(p => p.ReorderLevel == 5 * 2).ToList();

            AssertIsProductsWithReorderLevelTen(result);
        }

        [Fact]
        public virtual void Where_Field_Eq_Local() {
            var val = 10;
            var result = this.Products.Where(p => p.ReorderLevel == val).ToList();

            AssertIsProductsWithReorderLevelTen(result);
        }

        [Theory]
        [InlineData(10)]
        public virtual void Where_Field_Eq_Argument(Int32 val) {
            var result = this.Products.Where(p => p.ReorderLevel == val).ToList();

            AssertIsProductsWithReorderLevelTen(result);
        }

        private const Int32 StaticMember_Ten = 10;
        [Fact]
        public virtual void Where_Field_Eq_StaticMember() {
            var result = this.Products.Where(p => p.ReorderLevel == StaticMember_Ten).ToList();

            AssertIsProductsWithReorderLevelTen(result);
        }

        private Int32 InstanceMember_Ten = 10;
        [Fact]
        public virtual void Where_Field_Eq_InstanceMember() {
            var results = this.Products.Where(p => p.ReorderLevel == InstanceMember_Ten).ToList();

            AssertIsProductsWithReorderLevelTen(results);
        }

        [Fact]
        public virtual void Where_Field_Eq_ImplicitConversion() {
            var val = 10L;
            var results = this.Products.Where(p => p.ReorderLevel == val).ToList();

            AssertIsProductsWithReorderLevelTen(results);
        }

        [Fact]
        public virtual void Where_Field_Eq_ExplicitConversion() {
            var val = 10.0000001M;
            var results = this.Products.Where(p => p.ReorderLevel == (Int32)val).ToList();

            AssertIsProductsWithReorderLevelTen(results);
        }

        private static void AssertIsProductsWithReorderLevelTen(List<Product> results) {
            AssertProductIdSet(ProductIdsWithReorderLevelTen, results);
        }

        private static Int32[] ProductIdsWithReorderLevelEqInStock { get; } =
            { 5, 17, 29, 53 };

        [Fact]
        public virtual void Where_Field_Eq_Field() {
            var results = this.Products.Where(p => p.ReorderLevel == p.UnitsInStock).ToList();

            AssertProductIdSet(ProductIdsWithReorderLevelEqInStock, results);
        }

        private static Int32[] ProductIdsWithUnitsOnOrder { get; } =
            { 2, 3, 11, 21, 31, 32, 37, 43, 45, 48, 49, 56, 64, 66, 68, 70, 74 };

        [Fact]
        public virtual void Where_Field_NotEq() {
            var results = this.Products.Where(p => p.UnitsOnOrder != 0).ToList();

            AssertProductIdSet(ProductIdsWithUnitsOnOrder, results);
        }

        //--------------------------------------------------
        // Less Than/Greater Than Comparisons
        //--------------------------------------------------

        private static Int32[] ProductIdsWithReorderLevelLessThanTen { get; } =
            {
                4, 5, 8, 9, 10, 12, 13, 14, 15, 17, 18, 19, 20, 21, 24, 26, 28, 29, 39, 42, 46, 47,
                53, 59, 60, 62, 63, 65, 71, 72, 73, 74
            };

        [Fact]
        public virtual void Where_Field_LessThan_Literal() {
            var results = this.Products.Where(p => p.ReorderLevel < 10).ToList();

            AssertProductIdSet(ProductIdsWithReorderLevelLessThanTen, results);
        }

        private static Int32[] ProductIdsWithReorderLevelLessThanOrEqualToTen { get; } =
            ProductIdsWithReorderLevelTen.Concat(ProductIdsWithReorderLevelLessThanTen)
                .OrderBy(id => id)
                .ToArray();

        [Fact]
        public virtual void Where_Field_LessThanOrEqualTo_Literal() {
            var results = this.Products.Where(p => p.ReorderLevel <= 10).ToList();

            AssertProductIdSet(ProductIdsWithReorderLevelLessThanOrEqualToTen, results);
        }

        private static Int32[] ProductIdsWithReorderLevelGreaterThanTen { get; } =
            {
                2, 3, 6, 11, 22, 23, 25, 27, 30, 31, 32, 33, 34, 35, 36, 37, 38, 40, 43, 44, 45, 48,
                49, 50, 52, 55, 56, 57, 58, 61, 64, 66, 68, 69, 70, 75, 76, 77
            };

        [Fact]
        public virtual void Where_Field_GreaterThan_Literal() {
            var results = this.Products.Where(p => p.ReorderLevel > 10).ToList();

            AssertProductIdSet(ProductIdsWithReorderLevelGreaterThanTen, results);
        }

        private static Int32[] ProductIdsWithReorderLevelGreaterThanOrEqualToTen { get; } =
            ProductIdsWithReorderLevelTen.Concat(ProductIdsWithReorderLevelGreaterThanTen)
                .OrderBy(id => id)
                .ToArray();

        [Fact]
        public virtual void Where_Field_GreaterThanOrEqualTo_Literal() {
            var results = this.Products.Where(p => p.ReorderLevel >= 10).ToList();

            AssertProductIdSet(ProductIdsWithReorderLevelGreaterThanOrEqualToTen, results);
        }

        private static Int32[] ProductIdsWithLowUnitsInStock { get; } =
            { 2, 3, 11, 21, 30, 31, 32, 37, 43, 45, 48, 49, 56, 64, 66, 68, 70, 74 };

        private static Expression<Func<Product, Boolean>>[] LowUnitsInStockExpressions { get; } = {
            p => p.UnitsInStock < p.ReorderLevel,
            p => p.ReorderLevel > p.UnitsInStock
        };

        public static IEnumerable<Object[]> LowUnitsInStockArguments { get; } =
            LowUnitsInStockExpressions.Select(expression => new Object[] { expression });

        [Theory]
        [MemberData(nameof(LowUnitsInStockArguments))]
        public virtual void Where_Field_CompareWith_Field(Expression<Func<Product, Boolean>> predicate) {
            var results = this.Products.Where(predicate).ToList();

            AssertProductIdSet(ProductIdsWithLowUnitsInStock, results);
        }

        #endregion

        #region Where Clause Boolean Operations

        private static Int32[] ProductIdsOfDiscontinued { get; } =
            { 5, 9, 17, 24, 28, 29, 42, 53 };

        [Fact]
        public virtual void Where_Field_BooleanAsCondition() {
            var results = this.Products.Where(p => p.Discontinued).ToList();

            AssertProductIdSet(ProductIdsOfDiscontinued, results);
        }

        // !A
        [Fact]
        public virtual void Where_NegateExpression() {
            var results = this.Products.Where(p => !(p.UnitsInStock >= p.ReorderLevel)).ToList();

            AssertProductIdSet(ProductIdsWithLowUnitsInStock, results);
        }

        private static Int32[] ProductIdsLowStockCategoryEight { get; } =
            { 30, 37, 45 };

        // A && B
        [Fact]
        public virtual void Where_Conjunction() {
            var results =
                this.Products
                    .Where(p =>
                        p.UnitsInStock < p.ReorderLevel &&
                        p.CategoryId == 8)
                    .ToList();

            AssertProductIdSet(ProductIdsLowStockCategoryEight, results);
        }

        private static Int32[] ProductIdsAcceptableStockCategoryEight { get; } =
            { 10, 13, 18, 36, 40, 41, 46, 58, 73 };

        // !A && B
        [Fact]
        public virtual void Where_Conjunction_NegateExpression() {
            var results =
                this.Products
                    .Where(p =>
                        !(p.UnitsInStock < p.ReorderLevel) &&
                        p.CategoryId == 8)
                    .ToList();

            AssertProductIdSet(ProductIdsAcceptableStockCategoryEight, results);
        }

        private static Int32[] ProductIdsOutOfStockOrExpensive { get; } =
            { 5, 17, 29, 31, 38, 53 };

        // !(A && B)
        [Fact]
        public virtual void Where_Negate_Conjunction() {
            var results =
                this.Products
                    .Where(p =>
                        !(p.UnitsInStock > 0 &&
                            p.UnitPrice < 100))
                    .ToList();

            AssertProductIdSet(ProductIdsOutOfStockOrExpensive, results);
        }

        // A || B
        [Fact]
        public virtual void Where_Disjunction() {
            var results =
                this.Products
                    .Where(p =>
                        p.UnitsInStock == 0 ||
                        p.UnitPrice >= 100)
                    .ToList();

            AssertProductIdSet(ProductIdsOutOfStockOrExpensive, results);
        }

        // A || !B
        [Fact]
        public virtual void Where_Disjunction_NegateExpression() {
            var results =
                this.Products
                    .Where(p =>
                        p.UnitsInStock == 0 ||
                        !(p.UnitPrice < 100))
                    .ToList();

            AssertProductIdSet(ProductIdsOutOfStockOrExpensive, results);
        }

        private static Int32[] ProductIdsOutOfStockNotOnOrder { get; } =
            { 5, 17, 29, 53 };

        // !(A || B)
        [Fact]
        public virtual void Where_Negate_Disjunction() {
            var results =
                this.Products
                    .Where(p => !(p.UnitsInStock > 0 || p.UnitsOnOrder > 0))
                    .ToList();

            AssertProductIdSet(ProductIdsOutOfStockNotOnOrder, results);
        }

        private static Int32[] ProductIdsDiscontinuedCategoryOneOrTwo { get; } =
            { 5, 24 };

        // A && (B || C)
        [Fact]
        public virtual void Where_ConjunctionWithDisjunction() {
            var results =
                this.Products
                    .Where(p => p.Discontinued == true && (p.CategoryId == 1 || p.CategoryId == 2))
                    .ToList();

            AssertProductIdSet(ProductIdsDiscontinuedCategoryOneOrTwo, results);
        }

        private static Int32[] ProductIdsDiscontinuedCategoryOne { get; } =
            { 24 };

        private static Int32[] ProductIdsCategoryTwo { get; } =
            { 3, 4, 5, 6, 8, 15, 44, 61, 63, 65, 66, 77 };

        // A && B || C
        [Fact]
        public virtual void Where_DisjunctionWithConjunction() {
            var results =
                this.Products
                    .Where(p => p.Discontinued == true && p.CategoryId == 1 || p.CategoryId == 2)
                    .ToList();

            AssertProductIdSet(
                ProductIdsDiscontinuedCategoryOne
                    .Concat(ProductIdsCategoryTwo)
                    .OrderBy(id => id)
                    .ToArray(),
                results);
        }

        #endregion

        #region Where Clause Collection Contains

        private static Int32[] ProductIdsCategoryOne { get; } =
            { 1, 2, 24, 34, 35, 38, 39, 43, 67, 70, 75, 76 };

        private static Int32[] ProductIdsCategoryOneOrTwo { get; } =
            ProductIdsCategoryOne.Concat(ProductIdsCategoryTwo).OrderBy(id => id).ToArray();

        [Fact]
        public virtual void Where_LiteralArray_Contains() {
            var results = this.Products.Where(p => new[] { 1, 2 }.Contains(p.CategoryId)).ToList();

            AssertProductIdSet(
                ProductIdsCategoryOneOrTwo,
                results
            );
        }

        [Fact]
        private void Where_LocalArray_Contains() {
            var categories = new[] { 1, 2 };

            var results = this.Products.Where(p => categories.Contains(p.CategoryId)).ToList();

            AssertProductIdSet(
                ProductIdsCategoryOneOrTwo,
                results
            );
        }

        private Int32[] InstanceMember_CategoryIdsOneAndTwo { get; } = new[] { 1, 2 };

        [Fact]
        public virtual void Where_InstanceArray_Contains() {
            var results =
                this.Products
                    .Where(p => InstanceMember_CategoryIdsOneAndTwo.Contains(p.CategoryId))
                    .ToList();

            AssertProductIdSet(
                ProductIdsCategoryOneOrTwo,
                results
            );
        }

        private static Int32[] StaticMember_CategoryIdsOneAndTwo { get; } = new[] { 1, 2 };

        [Fact]
        public virtual void Where_StaticArray_Contains() {
            var results =
                this.Products
                    .Where(p => StaticMember_CategoryIdsOneAndTwo.Contains(p.CategoryId))
                    .ToList();

            AssertProductIdSet(
                ProductIdsCategoryOneOrTwo,
                results
            );
        }

        [Fact]
        public virtual void Where_Set_Contains() {
            var categories = ImmutableHashSet.Create(1, 2);

            var results = this.Products.Where(p => categories.Contains(p.CategoryId)).ToList();

            AssertProductIdSet(
                ProductIdsCategoryOneOrTwo,
                results
            );
        }

        #endregion

        #region Where Clause String Comparisons

        private static String[] CustomerIdsTitleStartsWithAssistant { get; } =
            { "FOLIG", "RATTC", "RICAR" };

        [Fact]
        public virtual void Where_String_StartsWith() {
            var results =
                this.Customers.Where(c => c.ContactTitle.StartsWith("Assistant")).ToList();

            AssertCustomerIdSet(
                CustomerIdsTitleStartsWithAssistant,
                results
            );
        }

        private static String[] CustomerIdsTitleEndsWithAssistant { get; } =
            { "FAMIA", "LAUGB", "MEREP", "MORGK", "QUEEN", "THECR", "WILMK" };

        [Fact]
        public virtual void Where_String_EndsWith() {
            var results =
                this.Customers.Where(c => c.ContactTitle.EndsWith("Assistant")).ToList();

            AssertCustomerIdSet(
                CustomerIdsTitleEndsWithAssistant,
                results
            );
        }

        private static String[] CustomerIdsTitleContainsAssistant { get; } =
            CustomerIdsTitleStartsWithAssistant
                .Concat(CustomerIdsTitleEndsWithAssistant)
                .OrderBy(id => id)
                .ToArray();

        [Fact]
        public virtual void Where_String_Contains() {
            var results =
                this.Customers.Where(c => c.ContactTitle.Contains("Assistant")).ToList();

            AssertCustomerIdSet(
                CustomerIdsTitleContainsAssistant,
                results
            );
        }

        private static String[] CustomerIdsAddressContainsAveAfterPositionTen { get; } =
            { "HILAA", "LILAS", "WHITC" };

        [Fact]
        public virtual void Where_String_IndexOf() {
            var results = this.Customers.Where(c => c.Address.IndexOf("Ave") > 10).ToList();

            AssertCustomerIdSet(
                CustomerIdsAddressContainsAveAfterPositionTen,
                results
            );
        }

        private static String[] CustemrIdsWithLongAddressFields { get; } =
            { "HILAA", "LILAS", "OCEAN" };

        [Fact]
        public virtual void Where_String_Length() {
            var results = this.Customers.Where(c => c.Address.Length > 30).ToList();

            AssertCustomerIdSet(
                CustemrIdsWithLongAddressFields,
                results
            );
        }

        // TODO: write tests for explicit Ordinal and OrdinalIgnoreCase comparisons, which should
        // be prefered since with the methods tested here the collations will be the environment
        // defaults, which could differ between the execution environment and the query engine
        // environment.

        #endregion

        #region Where Clause Nullable HasValue

        private static Int32[] EmployeeIdsReportsToNull { get; } =
            { 2 };

        private static Int32[] EmployeeIdsReportsToNotNull { get; } =
            { 1, 3, 4, 5, 6, 7, 8, 9 };

        [Fact]
        public virtual void Where_Nullable_HasValue() {
            var results = this.Employees.Where(e => e.ReportsTo.HasValue).ToList();

            AssertEmployeeIdSet(
                EmployeeIdsReportsToNotNull,
                results
            );
        }

        [Fact]
        public virtual void Where_Nullable_NotHasValue() {
            var results = this.Employees.Where(e => !e.ReportsTo.HasValue).ToList();

            AssertEmployeeIdSet(
                EmployeeIdsReportsToNull,
                results
            );
        }

        [Fact]
        public virtual void Where_Nullable_NotEqualsNull() {
            var results = this.Employees.Where(e => e.ReportsTo != null).ToList();

            AssertEmployeeIdSet(
                EmployeeIdsReportsToNotNull,
                results
            );
        }

        [Fact]
        public virtual void Where_Nullable_EqualsNull() {
            var results = this.Employees.Where(e => e.ReportsTo == null).ToList();

            AssertEmployeeIdSet(
                EmployeeIdsReportsToNull,
                results
            );
        }

        private static Int32[] EmployeeIdsNotReportingToEmployeeTwo { get; } =
            { 2, 6, 7, 9 };

        // Unlike .Net, in most SQL engines null != 5 will NOT return true, it will return
        // null which evaluates to false. QueryProvider implementations should generate
        // appropriate null checks in order to achieve consistent behavior.
        [Fact]
        public virtual void Where_Nullable_NotEqualValue() {
            var results = this.Employees.Where(e => e.ReportsTo != 2).ToList();

            AssertEmployeeIdSet(
                EmployeeIdsNotReportingToEmployeeTwo,
                results
            );
        }

        // Similar problem as above, in SQL !null -> null. Getting this right in a query provider
        // required equality checks to take the form (A = value && A is not null) for
        // literals/parameters that are not null, and (A = B || A is null && B is null)
        [Fact]
        public virtual void Where_Nullable_Negated_EqualValue() {
            var results = this.Employees.Where(e => !(e.ReportsTo == 2)).ToList();

            AssertEmployeeIdSet(
                EmployeeIdsNotReportingToEmployeeTwo,
                results
            );
        }

        #endregion

        //==================================================
        // OrderBy Tests
        //==================================================

        #region OrderBy Tests

        private static Int32[] ShipperIdsOrderedByNameAsc { get; } =
            { 3, 1, 2 };

        private static Int32[] ShipperIdsOrderedByNameDesc { get; } =
            { 2, 1, 3 };

        private static Int32[] SupplierIdsNorthAmericanOrderedByCountryAndName { get; } =
            { 29, 25, 16, 3, 19, 2 };

        [Fact]
        public virtual void OrderBy_Field_Asc() {
            var results =
                this.Shippers.OrderBy(s => s.CompanyName).ToList();

            Assert.Equal(
                ShipperIdsOrderedByNameAsc,
                results.Select(s => s.ShipperId).ToArray()
            );
        }

        [Fact]
        public virtual void OrderBy_Field_Desc() {
            var results =
                this.Shippers.OrderByDescending(s => s.CompanyName).ToList();

            Assert.Equal(
                ShipperIdsOrderedByNameDesc,
                results.Select(s => s.ShipperId).ToArray()
            );
        }

        private static String[] UsaAndCanada { get; } = { "USA", "Canada" };

        [Fact]
        public virtual void OrderBy_MultipleFields() {
            var results =
                this.Suppliers
                    .Where(s => UsaAndCanada.Contains(s.Country))
                    .OrderBy(s => s.Country)
                    .ThenBy(s => s.CompanyName)
                    .ToList();

            Assert.Equal(
                SupplierIdsNorthAmericanOrderedByCountryAndName,
                results.Select(s => s.SupplierId).ToArray()
            );
        }

        private static Int32[] ProductIdsByValueInStock { get; } = {
            52, 42, 23, 57, 64, 56, 22
        };

        [Fact]
        public virtual void OrderBy_Expression() {
            var results =
                this.Products
                    .Where(p => p.CategoryId == 5)
                    .OrderBy(p => p.UnitPrice * p.UnitsInStock)
                    .ToList();

            Assert.Equal(
                ProductIdsByValueInStock,
                results.Select(c => c.ProductId).ToArray()
            );
        }

        #endregion

        //==================================================
        // Single/First/Any/All/Take/Skip/Distinct/Count Tests
        //==================================================

        #region Single/First Tests

        private static Shipper ShipperTwo { get; } =
            new Shipper(2, "United Package", "(503) 555-3199");

        [Fact]
        public virtual void Single_ById_Exists() {
            var result = this.Shippers.Single(s => s.ShipperId == 2);

            Assert.Equal(
                ShipperTwo,
                result
            );
        }

        [Fact]
        public virtual void Single_ById_Missing() {
            Assert.Throws<InvalidOperationException>(
                () => this.Shippers.Single(s => s.ShipperId == 9999)
            );
        }

        [Fact]
        public virtual void Single_MutlipleResults() {
            Assert.Throws<InvalidOperationException>(
                () => this.Shippers.Single()
            );
        }

        [Fact]
        public virtual void SingleOrDefault_ById_Exists() {
            var result = this.Shippers.SingleOrDefault(s => s.ShipperId == 2);

            Assert.Equal(
                ShipperTwo,
                result
            );
        }
        [Fact]
        public virtual void SingleOrDefault_ById_Missing() {
            var result = this.Shippers.SingleOrDefault(s => s.ShipperId == 9999);

            Assert.Null(result);
        }

        [Fact]
        public virtual void First_SingleResult() {
            var result = this.Shippers.First(s => s.ShipperId == 2);

            Assert.Equal(
                ShipperTwo,
                result
            );
        }

        [Fact]
        public virtual void First_MultipleResults() {
            var result = this.Shippers.OrderBy(s => s.Phone).First();

            Assert.Equal(
                ShipperTwo,
                result
            );
        }

        [Fact]
        public virtual void FirstOrDefault_MultipleResults() {
            var result = this.Shippers.OrderBy(s => s.Phone).FirstOrDefault();

            Assert.Equal(
                ShipperTwo,
                result
            );
        }

        [Fact]
        public virtual void FirstOrDefault_NoResults() {
            var result = this.Shippers.FirstOrDefault(s => s.ShipperId == 9999);

            Assert.Null(result);
        }

        #endregion

        #region Any/All Tests

        [Fact]
        public virtual void Any_ConditionTrue() {
            var result = this.Products.Any(p => p.UnitPrice > 100.0m);

            Assert.True(result);
        }

        [Fact]
        public virtual void Any_ConditionFalse() {
            var result = this.Products.Any(p => p.UnitPrice > 300.0m);

            Assert.False(result);
        }

        [Fact]
        public virtual void All_ConditionTrue() {
            var result =
                this.Products.All(p => p.UnitsInStock > 0 || p.UnitsOnOrder > 0 || p.Discontinued);

            Assert.True(result);
        }

        [Fact]
        public virtual void All_ConditionFalse() {
            var result =
                this.Products.All(p => p.UnitsInStock > 0);

            Assert.False(result);
        }

        #endregion

        #region Take/Skip Tests

        private static Int32[] ProductIdsTenMostExpensive { get; } =
            { 38, 29, 9, 20, 18, 59, 51, 62, 43, 28 };

        [Fact]
        public virtual void Take() {
            var results =
                this.Products.OrderByDescending(p => p.UnitPrice).Take(5).ToList();

            AssertProductIdList(
                ProductIdsTenMostExpensive.Take(5).ToArray(),
                results
            );
        }

        [Fact]
        public virtual void Skip_Take() {
            var results =
                this.Products.OrderByDescending(p => p.UnitPrice).Skip(5).Take(5).ToList();

            AssertProductIdList(
                ProductIdsTenMostExpensive.Skip(5).Take(5).ToArray(),
                results
            );
        }

        [Fact]
        public virtual void Take_Skip() {
            var results =
                this.Products.OrderByDescending(p => p.UnitPrice).Take(10).Skip(5).ToList();

            AssertProductIdList(
                ProductIdsTenMostExpensive.Skip(5).Take(5).ToArray(),
                results
            );
        }

        [Fact]
        public virtual void Take_Take() {
            var results =
                this.Products.OrderByDescending(p => p.UnitPrice).Take(10).Take(5).ToList();

            AssertProductIdList(
                ProductIdsTenMostExpensive.Take(5).ToArray(),
                results
            );
        }

        private static Int32[] EmployeeIdsAfterTheFiveOldest { get; } =
            { 7, 6, 3, 9 };

        [Fact]
        public virtual void Skip() {
            var result =
                this.Employees.OrderBy(e => e.BirthDate).Skip(5).ToList();

            AssertEmployeeIdList(
                EmployeeIdsAfterTheFiveOldest,
                result
            );
        }

        [Fact]
        public virtual void Skip_Skip() {
            var result =
                this.Employees.OrderBy(e => e.BirthDate).Skip(2).Skip(3).ToList();

            AssertEmployeeIdList(
                EmployeeIdsAfterTheFiveOldest,
                result
            );
        }

        // TODO: test TakeLast/SkipLast
        // TODO: test TakeWhile/SkipWhile? probably not supported by most SQL engines

        #endregion

        #region Distinct Tests

        private static String[] SupplierCountries { get; } = {
            "Australia",
            "Brazil",
            "Canada",
            "Denmark",
            "Finland",
            "France",
            "Germany",
            "Italy",
            "Japan",
            "Netherlands",
            "Norway",
            "Singapore",
            "Spain",
            "Sweden",
            "UK",
            "USA",
        };

        [Fact]
        public virtual void Distinct_Scalar() {
            var results = this.Suppliers.Select(s => s.Country).Distinct().ToList();

            Assert.Equal(
                SupplierCountries,
                results.OrderBy(country => country).ToArray()
            );
        }

        private static Tuple<String, String>[] SupplierRegions { get; } = {
            Tuple.Create("NSW", "Australia"),
            Tuple.Create("Victoria", "Australia"),
            Tuple.Create((String)null, "Brazil"),
            Tuple.Create("Québec", "Canada"),
            Tuple.Create((String)null, "Denmark"),
            Tuple.Create((String)null, "Finland"),
            Tuple.Create((String)null, "France"),
            Tuple.Create((String)null, "Germany"),
            Tuple.Create((String)null, "Italy"),
            Tuple.Create((String)null, "Japan"),
            Tuple.Create((String)null, "Netherlands"),
            Tuple.Create((String)null, "Norway"),
            Tuple.Create((String)null, "Singapore"),
            Tuple.Create("Asturias", "Spain"),
            Tuple.Create((String)null, "Sweden"),
            Tuple.Create((String)null, "UK"),
            Tuple.Create("LA", "USA"),
            Tuple.Create("MA", "USA"),
            Tuple.Create("MI", "USA"),
            Tuple.Create("OR", "USA"),
        };

        [Fact]
        public virtual void Distinct_Anonymous() {
            var results =
                this.Suppliers.Select(s => new { s.Region, s.Country }).Distinct().ToList();

            Assert.Equal(
                SupplierRegions,
                results.OrderBy(loc => loc.Country)
                    .ThenBy(loc => loc.Region)
                    .Select(loc => Tuple.Create(loc.Region, loc.Country))
                    .ToArray()
            );
        }

        #endregion

        #region Count Tests

        [Fact]
        public virtual void Count_Total() {
            var result = this.Suppliers.Count();

            Assert.Equal(29, result);
        }

        [Fact]
        public virtual void Count_Filtered() {
            var result = this.Suppliers.Count(s => s.Country == "USA");

            Assert.Equal(4, result);
        }

        [Fact]
        public virtual void Count_Distinct() {
            var result = this.Suppliers.Select(s => s.Country).Distinct().Count();

            Assert.Equal(SupplierCountries.Length, result);
        }

        #endregion

        //==================================================
        // Select Tests
        //==================================================

        #region Select Tests

        //--------------------------------------------------
        // POCO
        //--------------------------------------------------

        private static Product ProductFourteen { get; } =
            new Product(14, "Tofu", 6, 7, "40 - 100 g pkgs.", 23.25m, 35, 0, 0, false);

        [Fact]
        public virtual void Select_Model() {
            var result = this.Products.Single(p => p.ProductId == 14);

            Assert.Equal(ProductFourteen, result);
        }

        //--------------------------------------------------
        // Anonymous Types
        //--------------------------------------------------

        //   Simple

        [Fact]
        public virtual void Select_AnonymousType_Simple() {
            var result =
                this.Products
                    .Where(p => p.ProductId == 14)
                    .Select(p => new { Id = p.ProductId, Name = p.ProductName })
                    .Single();

            Assert.Equal(ProductFourteen.ProductId, result.Id);
            Assert.Equal(ProductFourteen.ProductName, result.Name);
        }

        //   Complex

        [Fact]
        public virtual void Select_AnonymousType_Complex() {
            var result =
                this.Products
                    .Where(p => p.ProductId == 14)
                    .Select(p =>
                        new {
                            Id = p.ProductId,
                            Name = p.ProductName,
                            StockInformation = new {
                                p.UnitsInStock,
                                p.UnitsOnOrder,
                                p.ReorderLevel
                            }
                        })
                    .Single();

            Assert.Equal(ProductFourteen.ProductId, result.Id);
            Assert.Equal(ProductFourteen.ProductName, result.Name);
            Assert.NotNull(result.StockInformation);
            Assert.Equal(ProductFourteen.UnitsInStock, result.StockInformation.UnitsInStock);
            Assert.Equal(ProductFourteen.UnitsOnOrder, result.StockInformation.UnitsOnOrder);
            Assert.Equal(ProductFourteen.ReorderLevel, result.StockInformation.ReorderLevel);
        }

        //   Composite w/ POCO

        [Fact]
        public virtual void Select_AnonymousType_Composite() {
            var result =
                this.Products
                    .Where(p => p.ProductId == 14)
                    .Select(p =>
                        new {
                            Product = p,
                            NeedsOrdering = p.UnitsInStock < p.ReorderLevel
                        })
                    .Single();

            Assert.Equal(ProductFourteen, result.Product);
            Assert.False(result.NeedsOrdering);
        }

        //--------------------------------------------------
        // Scalar Values
        //--------------------------------------------------

        [Fact]
        public virtual void Select_Scalar_Decimal() {
            var result =
                this.Products
                    .Where(p => p.ProductId == 14)
                    .Select(p => p.UnitPrice)
                    .Single();

            Assert.Equal(ProductFourteen.UnitPrice, result);
        }

        [Fact]
        public virtual void Select_Scalar_Uri() {
            var result =
                this.Employees
                    .Where(e => e.EmployeeId == 2)
                    .Select(e => e.PhotoPath)
                    .Single();

            Assert.Equal(new Uri("http://accweb/emmployees/fuller.bmp"), result);
        }

        //--------------------------------------------------
        // Arithmetic Expression
        //--------------------------------------------------

        [Fact]
        public virtual void Select_Arithmetic() {
            var result =
                this.Products
                    .Where(p => p.ProductId == 14)
                    .Select(p => p.UnitPrice * p.UnitsInStock)
                    .Single();

            Assert.Equal(ProductFourteen.UnitPrice * ProductFourteen.UnitsInStock, result);
        }

        //--------------------------------------------------
        // String Functions
        //--------------------------------------------------

        //   Concat

        [Fact]
        public virtual void Select_Concat() {
            var result =
                this.Products
                    .Where(p => p.ProductId == 14)
                    .Select(p => String.Concat(p.ProductName, " (", p.QuantityPerUnit, ")"))
                    .Single();

            Assert.Equal($"{ProductFourteen.ProductName} ({ProductFourteen.QuantityPerUnit})", result);
        }

        //   Trim

        [Fact]
        public virtual void Select_TrimStart() {
            var result =
                this.Suppliers
                    .Where(p => p.SupplierId == 2)
                    .Select(p => p.HomePage.TrimStart('#'))
                    .Single();

            Assert.Equal("CAJUN.HTM#", result);
        }

        [Fact]
        public virtual void Select_TrimEnd() {
            var result =
                this.Suppliers
                    .Where(p => p.SupplierId == 2)
                    .Select(p => p.HomePage.TrimEnd('#'))
                    .Single();

            Assert.Equal("#CAJUN.HTM", result);
        }

        [Fact]
        public virtual void Select_Trim() {
            var result =
                this.Suppliers
                    .Where(p => p.SupplierId == 2)
                    .Select(p => p.HomePage.Trim('#'))
                    .Single();

            Assert.Equal("CAJUN.HTM", result);
        }

        //   Substring

        [Fact]
        public virtual void Select_Substring() {
            var result =
                this.Suppliers
                    .Where(p => p.SupplierId == 6)
                    .Select(p => p.HomePage.Substring(p.HomePage.IndexOf('#')))
                    .Single();

            Assert.Equal("#http://www.microsoft.com/accessdev/sampleapps/mayumi.htm#", result);
        }

        //   Change Case

        [Fact]
        public virtual void Select_ToUpper() {
            var result =
                this.Suppliers
                    .Where(p => p.SupplierId == 2)
                    .Select(p => p.Address.ToUpper())
                    .Single();

            Assert.Equal("P.O. BOX 78934", result);
        }

        [Fact]
        public virtual void Select_ToLower() {
            var result =
                this.Suppliers
                    .Where(p => p.SupplierId == 2)
                    .Select(p => p.Address.ToLower())
                    .Single();

            Assert.Equal("p.o. box 78934", result);
        }

        //   Length

        [Fact]
        public virtual void Select_StringLength() {
            var result =
                this.Suppliers
                    .Where(p => p.SupplierId == 2)
                    .Select(p => p.Country.Length)
                    .Single();

            Assert.Equal(3, result);
        }

        #endregion

        //==================================================
        // Group By Tests
        //==================================================

        // TODO: Implement group by tests

        //==================================================
        // Join Tests
        //==================================================

        #region Join Tests

        private static String[] CustomerIdsWithOutstandingOrdersViaShipperOne { get; } =
            { "CACTU", "LEHMS", "LILAS" };

        [Fact]
        public virtual void Join_InnerJoin() {
            var results =
                this.Customers
                    .Join(
                        this.Orders,
                        c => c.CustomerId,
                        o => o.CustomerId,
                        (Customer, Order) => new { Customer, Order })
                    .Where(row => row.Order.ShippedDate == null && row.Order.ShipVia == 1)
                    .Select(row => row.Customer)
                    .Distinct()
                    .ToList();

            AssertCustomerIdSet(
                CustomerIdsWithOutstandingOrdersViaShipperOne,
                results
            );
        }

        private static String[] CustomerIdsWithNoOrders { get; } = { "FISSA", "PARIS" };

        [Fact]
        public virtual void Join_LeftJoin() {
            var results =
                this.Customers
                    .GroupJoin(
                        this.Orders,
                        c => c.CustomerId,
                        o => o.CustomerId,
                        (Customer, orders) => new { Customer, Orders = orders.DefaultIfEmpty() })
                    .SelectMany(row => row.Orders.Select(Order => new { row.Customer, Order }))
                    .Where(row => row.Order == null)
                    .Select(row => row.Customer)
                    .ToList();

            AssertCustomerIdSet(
                CustomerIdsWithNoOrders,
                results
            );
        }

        #endregion

        //==================================================
        // Sub-query Tests
        //==================================================

        // TODO: Implement sub-query tests

        //==================================================
        // Helper Functions
        //==================================================

        private static void AssertProductIdSet(
            Int32[] expectedProductIds,
            IList<Product> actualProducts) {

            Assert.Equal(
                expectedProductIds,
                actualProducts.OrderBy(p => p.ProductId).Select(p => p.ProductId).ToArray()
            );
        }

        private static void AssertProductIdList(
            Int32[] expectedProductIds,
            IList<Product> actualProducts) {

            Assert.Equal(
                expectedProductIds,
                actualProducts.Select(p => p.ProductId).ToArray()
            );
        }

        private static void AssertEmployeeIdSet(
            Int32[] expectedEmployeeIds,
            IList<Employee> actualEmployees) {

            Assert.Equal(
                expectedEmployeeIds,
                actualEmployees.OrderBy(p => p.EmployeeId).Select(p => p.EmployeeId).ToArray()
            );
        }

        private static void AssertEmployeeIdList(
            Int32[] expectedEmployeeIds,
            IList<Employee> actualEmployees) {

            Assert.Equal(
                expectedEmployeeIds,
                actualEmployees.Select(p => p.EmployeeId).ToArray()
            );
        }

        private static void AssertCustomerIdSet(
            String[] expectedCustomerIds,
            List<Customer> actualCustomers) {

            Assert.Equal(
                expectedCustomerIds,
                actualCustomers.OrderBy(p => p.CustomerId).Select(p => p.CustomerId).ToArray()
            );
        }
    }
}
