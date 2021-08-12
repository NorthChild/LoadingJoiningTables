using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;


namespace NorthwindData
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new NorthwindContext())
            {

                // EAGER LOADING
                // QUERY SYNTAX
                var ordersQuery =
                    from order in db.Orders.Include (o => o.Customer) // the include statement will make this an eager loading
                    where order.Freight > 750
                    select order;

                foreach (var order in ordersQuery)
                {
                    if (order.Customer != null)
                    {
                        //Console.WriteLine($"{order.Customer.CompanyName} of {order.Customer.City} paid {order.Freight} for shipping");
                    }

                }
                Console.WriteLine(" ");



                // METHOD SYNTAX
                var ordersQuery2 =
                    db.Orders.Include(o => o.Customer).Include(c => c.OrderDetails).Where(o => o.Freight > 750).Select(o => o);

                foreach (var res in ordersQuery2)
                {
                    //Console.WriteLine($"Order {res.OrderId} was made by {res.Customer.CompanyName}");

                    foreach (var od in res.OrderDetails)
                    {
                        //Console.WriteLine($"ProductId: {od.ProductId}");
                    }

                }
                Console.WriteLine(" ");

                // expanding on the last query
                var orderQuery3 =
                    db.Orders.Include(o => o.Customer).Include(c => c.OrderDetails).ThenInclude(od => od.Product).Where(o => o.Freight > 750);

                foreach (var res in orderQuery3)
                {
                    //Console.WriteLine($"Order {res.OrderId} was made by {res.Customer.CompanyName}");

                    foreach (var od in res.OrderDetails)
                    {
                        //Console.WriteLine($"ProductID: {od.ProductId} - Product: {od.Product.ProductName} - Quantity: {od.Quantity}");
                        

                    }
                }
                Console.WriteLine(" ");



                // QUERY SYNTAX ON JOINS AND ANONYMOUS TYPE OBJECTS
                var orderQueryUsingJoing =
                    from order in db.Orders
                    where order.Freight > 750
                    join customer in db.Customers on order.CustomerId equals customer.CustomerId
                    select new { CustomerContactName = customer.ContactName, City = customer.City, Freight = order.Freight };

                foreach (var res in orderQueryUsingJoing)
                {
                    //Console.WriteLine($"{res.CustomerContactName} of {res.City} paid {res.Freight} for shipping");
                }
                //Console.WriteLine(" ");



                // ANOTHER EXAMPLE OF QUERY USING ANONYMOUS OBJ
                var orderCustomerBerlinParisQuery =
                    from o in db.Orders
                    join c in db.Customers on o.CustomerId equals c.CustomerId
                    where c.City == "Berlin" || c.City == "Paris"
                    select new {o.OrderId, c.CompanyName};

                foreach (var item in orderCustomerBerlinParisQuery)
                {
                    //Console.WriteLine($"OrderID: {item.OrderId} was ordered by {item.CompanyName}");
                }
                Console.WriteLine(" ");








                // QUERY AND METHOD SYNTAX EXERCISE

                // 1.1 
                Console.WriteLine("QUESTION 1.1 - METHOD SYNTAX");

                var customerOrderParisLondonMethodSyntax =
                    db.Customers.Where(c => c.City == "London" || c.City == "Paris");

                foreach (var res in customerOrderParisLondonMethodSyntax)
                {
                    Console.WriteLine($"CustomerID: {res.CustomerId} Company Name: {res.CompanyName} Address: {res.Address}, {res.City}, {res.Country}");
                }

                //
                Console.WriteLine("QUESTION 1.1 - QUERY SYNTAX");
                var customerOrderParisLondonQuerySyntax =
                    from c in db.Customers
                    where c.City == "London" || c.City == "Paris"
                    select c;

                foreach (var res in customerOrderParisLondonQuerySyntax)
                {
                    Console.WriteLine($"CustomerID: {res.CustomerId} Company Name: {res.CompanyName} Address: {res.Address}, {res.City}, {res.Country}");
                }
                Console.WriteLine(" ");





                // 1.2
                Console.WriteLine("QUESTION 1.2 - METHOD SYNTAX");

                var storedInBottleMethodSyntax =
                    db.Products.Where(p => p.QuantityPerUnit.Contains("bottle"));

                foreach (var el in storedInBottleMethodSyntax) Console.WriteLine($"{el.ProductName} - {el.QuantityPerUnit}");

                //
                Console.WriteLine("QUESTION 1.2 - QUERY SYNTAX");

                var storedInBottleQuerySyntax =
                    from p in db.Products
                    where p.QuantityPerUnit.Contains("bottle")                   
                    select p;

                foreach (var el in storedInBottleQuerySyntax) Console.WriteLine($"{el.ProductName} - {el.QuantityPerUnit}");
                Console.WriteLine(" ");




                // 1.3
                Console.WriteLine("QUESTION 1.3 - METHOD SYNTAX");

                var abovePlusSupNameAndCountryMethod =
                    db.Products.Include(c => c.Supplier).Where(p => p.QuantityPerUnit.Contains("bottle"));


                foreach (var el in abovePlusSupNameAndCountryMethod) Console.WriteLine($"{el.Supplier.CompanyName}, {el.Supplier.Country}: {el.ProductName} - {el.QuantityPerUnit}");

                //
                Console.WriteLine("QUESTION 1.3 - QUERY SYNTAX");

                var abovePlusSupNameAndCountryQuery =
                   from p in db.Products
                   where p.QuantityPerUnit.Contains("bottle")
                   join s in db.Suppliers on p.SupplierId equals s.SupplierId
                   select p;

                foreach (var el in abovePlusSupNameAndCountryQuery) Console.WriteLine($"{el.Supplier.CompanyName}, {el.Supplier.Country}: {el.ProductName} - {el.QuantityPerUnit}");
                Console.WriteLine(" ");





                // 1.4
                Console.WriteLine("QUESTION 1.4 - METHOD SYNTAX");

                var howManyCategoriesMethod = 
                    db.Products.Include(p => p.Category).GroupBy(c => c.Category.CategoryName).Select( includedGroup => new {category = includedGroup.Key, count = includedGroup.Count() }).OrderByDescending(p => p.count);

                foreach (var el in howManyCategoriesMethod) Console.WriteLine($"{el.category} - {el.count}");



                //
                Console.WriteLine("QUESTION 1.4 - QUERY SYNTAX");

                var howManyCategoriesQuery =
                    from p in db.Products
                    join c in db.Categories on p.CategoryId equals c.CategoryId
                    group p by c.CategoryName into newGroup
                    select new { Category = newGroup.Key, NumOfProducts = newGroup.Count() };

                foreach (var el in howManyCategoriesQuery) Console.WriteLine($" {el.Category} {el.NumOfProducts}");
                Console.WriteLine(" ");





                // 1.5
                Console.WriteLine("QUESTION 1.5 - METHOD SYNTAX");

                //var ukEmployeesWithFullDetailsMethod =
                //    db.Employees.Include(e => e.FirstName).Include(e => e.Title).Include(e => e.FirstName).Include(e => e.LastName).Include(e => e.Address).Include(e => e.City).Include(e => e.Country).Include(e => e.Region).Include(e => e.PostalCode).Select(db.Employees);

                var ukEmployeesWithFullDetailsMethod =
                    db.Employees.Where(c => c.Country == "Uk");


                foreach (var res in ukEmployeesWithFullDetailsMethod)
                {
                    Console.WriteLine($"FullName: {res.Title}. {res.FirstName} {res.LastName} Address: {res.Address}, {res.City}, {res.Country}, {res.Region} {res.PostalCode}");
                }


                //
                Console.WriteLine("QUESTION 1.5 - QUERY SYNTAX");

                var ukEmployeesWithFullDetailsQuery =
                    from e in db.Employees
                    where e.Country == "Uk"
                    select e;

                foreach (var res in ukEmployeesWithFullDetailsQuery)
                {
                    Console.WriteLine($"FullName: {res.Title}. {res.FirstName} {res.LastName} Address: {res.Address}, {res.City}, {res.Country}, {res.Region} {res.PostalCode}");
                }
                Console.WriteLine(" ");



                // 1.6
                Console.WriteLine("QUESTION 1.6 - METHOD SYNTAX");





                //
                Console.WriteLine("QUESTION 1.6 - QUERY SYNTAX");

                Console.WriteLine(" ");








                // 1.7
                Console.WriteLine("QUESTION 1.7 - METHOD SYNTAX");

                var ordersWithFreightHigherThanAndFromUkOrUsaMethod =
                    db.Orders.Where(c => c.Freight > 100).Where(d => d.ShipCountry.Contains("USA") || d.ShipCountry.Contains("UK")).Count() ;


                Console.WriteLine($"Orders over 100 from uk or usa: {ordersWithFreightHigherThanAndFromUkOrUsaMethod}");


                //
                Console.WriteLine("QUESTION 1.7 - QUERY SYNTAX");

                var ordersWithFreightHigherThanAndFromUkOrUsaQuery =
                    (from p in db.Orders
                    where p.Freight > 100 && (p.ShipCountry.Contains("UK") || p.ShipCountry.Contains("USA"))
                    select p).Count();

                Console.WriteLine($"Orders over 100 from uk or usa: {ordersWithFreightHigherThanAndFromUkOrUsaQuery}");
                Console.WriteLine(" ");












                // 1.8
                Console.WriteLine("QUESTION 1.8 - METHOD SYNTAX");





                //
                Console.WriteLine("QUESTION 1.8 - QUERY SYNTAX");















                // 1.9
                Console.WriteLine("QUESTION 1.9 - METHOD SYNTAX");





                //
                Console.WriteLine("QUESTION 1.9 - QUERY SYNTAX");









                //var prodGroupedByCategory =

                //    from product in db.Products
                //    join category in db.Categories on product.CategoryId equals category.CategoryId
                //    group product by category.CategoryName into newGroup
                //    select new { Category = newGroup.Key, NumOfProd = newGroup.Count() };


                //foreach (var result in prodGroupedByCategory)
                //{
                //    Console.WriteLine($"{result.Category} - {result.NumOfProd}");
                //}


                ////Q1.1 Query Syntax
                //var londonParisQuery =
                // from customer in db.Customers
                // where customer.City == "London" || customer.City == "Paris"
                // select customer;

                ////Q1.1 Method Syntax
                //var londonParisQuery2 =
                //   db.Customers.Where(c => c.City == "Paris" || c.City == "London");

                //foreach (var c in londonParisQuery2)
                //{
                //    Console.WriteLine($"{c.CompanyName} is located in {c.City},{c.Country}");
                //}

                ////Q1.2 Query Syntax
                //var bottleQuery =
                //from product in db.Products
                //where product.QuantityPerUnit.Contains("bottle")
                //select product;

                //foreach (var product in bottleQuery)
                //{
                //    Console.WriteLine($"Products which are contained in bottles are {product.ProductName}. Quantity Per Unit: {product.QuantityPerUnit}");
                //}

                ////Q1.2 Method Syntax


                //var bottleQuery2 =
                //    db.Products.Where(p => p.QuantityPerUnit.Contains("bottle"));

                //foreach (var p in bottleQuery2)
                //{
                //    Console.WriteLine($"Products which are contained in bottles are {p.ProductName}. Quantity Per Unit: {p.QuantityPerUnit}");
                //}

                ////Q1.3 Query Syntax
                //var bottleQueryJoin =
                //from product in db.Products
                //join supplier in db.Suppliers on product.SupplierId equals supplier.SupplierId
                //where product.QuantityPerUnit.Contains("bottle")
                //select new { productName = product.ProductName, supplierName = supplier.CompanyName, quantityPerUnit = product.QuantityPerUnit };

                //foreach (var result in bottleQueryJoin)
                //{
                //    Console.WriteLine($"Product:{result.productName}. Company Name: {result.supplierName}. Quantity per Unit: {result.quantityPerUnit}");
                //}

                ////Q1.3 Method Syntax

                //var bottleQueryJoin2 =
                //    db.Products.Where(p => p.QuantityPerUnit.Contains("Bottle")).Include(s => s.Supplier);

                //foreach (var p in bottleQueryJoin2)
                //{
                //    Console.WriteLine($"Product:{p.ProductName}. Company Name: {p.CompanyName}. Quantity per Unit: {p.QuantityPerUnit}");
                //}

                ////1.4 Query Syntax //how many products are there in each category

                //var prodGroupedByCategory =
                //     from product in db.Products
                //     join category in db.Categories on product.CategoryId equals category.CategoryId
                //     group product by category.CategoryName into newGroup
                //     select new { Category = newGroup.Key, NumOfProd = newGroup.Count() };


                //foreach (var result in prodGroupedByCategory)
                //{
                //    Console.WriteLine($"{result.Category} - {result.NumOfProd}");
                //}


                ////1.4 Method Syntax 

                //var prodGroupedByCategory2 =
                //    db.Products.Include(c => c.Category).ToList().GroupBy(C => C.Category.CategoryName);

                //foreach (var result in prodGroupedByCategory2)
                //{
                //    Console.WriteLine($"{result.Key} - {result.Count()}");
                //}

                ////1.5 Query Syntax

                //var employeesUKQuery =
                //from emp in db.Employees
                //where emp.Country == "UK"
                //select new { firstName = emp.FirstName, lastName = emp.LastName, Country = emp.Country };

                //foreach (var result in employeesUKQuery)
                //{
                //    Console.WriteLine($"{result.firstName} {result.lastName} - {result.Country}");
                //}

                ////1.5 Method syntax

                //var employeesUKQuery2 =
                //    db.Employees.Where(e => e.Country == "UK");


                //foreach (var result in employeesUKQuery2)
                //{
                //    Console.WriteLine($"{result.FirstName} {result.LastName} - {result.Country}");
                //}

                ////1.6 Query Syntax

                //var manyjoinsQuery1 =
                //    from r in db.Regions
                //    join t in db.Territories on r.RegionId equals t.RegionId
                //    join et in db.EmployeeTerritories on t.TerritoryId equals et.TerritoryId
                //    join o in db.Orders on et.EmployeeId equals o.EmployeeId
                //    join od in db.OrderDetails on o.OrderId equals od.OrderId
                //    group od by new { r.RegionId, r.RegionDescription } into g
                //    where g.Sum(od => od.Quantity * od.UnitPrice * (decimal)(1 - od.Discount)) > 1000000
                //    select new
                //    {
                //        g.Key.RegionId,
                //        Region = g.Key.RegionDescription,
                //        SalesTotal = Math.Round(g.Sum(od => od.Quantity * od.UnitPrice * (decimal)(1 - od.Discount)), 2)
                //    };

                ////1.6 Method Syntax (gets very messy!!)

                //var query1_6m = db.Regions
                //    .Include(r => r.Territories)
                //    .ThenInclude(t => t.EmployeeTerritories)
                //    .ThenInclude(et => et.Employee)
                //    .ThenInclude(e => e.Orders)
                //    .ThenInclude(o => o.OrderDetails)
                //    .Select(r => new { r.RegionId, r.RegionDescription, r.Territories });

                //                foreach (var region in query1_6m)
                //                {
                //                    decimal sum = 0;

                //                    foreach (var territory in region.Territories)
                //                    {
                //                        foreach (var employeeTerritory in territory.EmployeeTerritories)
                //                        {
                //                            foreach (var order in employeeTerritory.Employee.Orders)
                //                            {
                //                                foreach (var orderDetails in order.OrderDetails)
                //                                {
                //                                    sum += orderDetails.UnitPrice * orderDetails.Quantity * (decimal)(1 - orderDetails.Discount);
                //                                }
                //                            }
                //                        }
                //                    }

                //                    if (sum > 1000000)
                //                    {
                //                        Console.WriteLine($"RegionId: {region.RegionId}, Region: {region.RegionDescription}, Sales Total: {Math.Round(sum, 2)}");
                //                    }
                //                }


                //1.7 Query Syntax
                //var moreThan100OrdersUKUSQuery =
                //    (from order in db.Orders
                //     where order.Freight > 100 && (order.ShipCountry == "UK" || order.ShipCountry == "USA")
                //     select order).Count();

                //Console.WriteLine($"{moreThan100OrdersUKUSQuery}");


                ////1.7 method Syntax
                //var moreThan100OrdersUKUSQuery2 =
                //    db.Orders.Where(x => x.Freight > 100).Where(y => y.ShipCountry.Contains("USA") || y.ShipCountry.Contains("UK")).Count();

                //Console.WriteLine($"No. of Order >100 from US or UK {moreThan100OrdersUKUSQuery2}");


                ////1.8 Query Syntax
                //var query1_8q =
                //(
                //    from o in db.Orders
                //    join od in db.OrderDetails on o.OrderId equals od.OrderId
                //    orderby o.OrderDetails.Sum(od => (float)od.UnitPrice * od.Quantity * od.Discount) descending
                //    select new { o.OrderId, Discount = o.OrderDetails.Sum(od => (float)od.UnitPrice * od.Quantity * od.Discount) }
                //)
                //.First();
                ////1.8 Method Syntax

                //var query1_8m = db.Orders
                //    .Include(o => o.OrderDetails)
                //    .OrderByDescending(o => o.OrderDetails.Sum(od => (float)od.UnitPrice * od.Quantity * od.Discount))
                //    .Select(o => new { o.OrderId, Discount = o.OrderDetails.Sum(od => (float)od.UnitPrice * od.Quantity * od.Discount) })
                //    .First();

                ////3.1
                //var query3_1q =
                //    from e in db.Employees
                //    join s in db.Employees on e.ReportsTo equals s.EmployeeId into grp
                //    from g in grp.DefaultIfEmpty()
                //    select new
                //    {
                //        e.EmployeeId,
                //        Name = e.FirstName + ' ' + e.LastName,
                //        SupervisorId = g.EmployeeId,
                //        Supervisor = g != null ? g.FirstName + ' ' + g.LastName : "No-one"
                //    };

                ////3.2
                //var query3_2q =
                //   from s in db.Suppliers
                //   join p in db.Products on s.SupplierId equals p.SupplierId
                //   join od in db.OrderDetails on p.ProductId equals od.ProductId
                //   group od by new { s.SupplierId, Supplier = s.CompanyName } into g
                //   where g.Sum(od => od.Quantity * od.UnitPrice * (decimal)(1 - od.Discount)) > 10000
                //   select new { g.Key.SupplierId, g.Key.Supplier, TotalSales = g.Sum(od => od.Quantity * od.UnitPrice * (decimal)(1 - od.Discount)) };
            }
        }

    }
        }
 

