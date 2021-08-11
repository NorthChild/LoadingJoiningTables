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

                var ordersQuery =
                    from order in db.Orders.Include( o=> o.Customer)
                    where order.Freight > 750
                    select order;
                foreach (var order in ordersQuery)
                {
                    if (order.Customer != null)
                        Console.WriteLine($" {order.Customer.ContactName} of {order.Customer.City} paid {order.Freight} for shipping");
                }

                var ordersQuery2 = db.Orders.Include(
                    o => o.Customer).Include(
                    c => c.OrderDetails).Where(
                    o => o.Freight > 750).Select(o => o);

                //Execute Query
                foreach (var o in ordersQuery2)
                {
                    Console.WriteLine($"Order {o.OrderId} was made by {o.Customer.CompanyName}");
                    foreach (var od in o.OrderDetails)
                    {
                        Console.WriteLine($"\t ProductId: {od.ProductId}");
                    }
                }
                var ordersQuery3 = db.Orders.Include(
                    o => o.Customer).Include(
                    c => c.OrderDetails).ThenInclude(
                    od => od.Product).Where(
                    o => o.Freight > 750);

                //Execute query
                foreach (var o in ordersQuery3)
                {
                    Console.WriteLine($"Order {o.OrderId} was made by {o.Customer.CompanyName}");
                    foreach (var od in o.OrderDetails)
                    {
                        Console.WriteLine($"\t ProductId: {od.ProductId} - Product: {od.Product.ProductName} - Quantity {od.Quantity}");
                    }
                }
            }
        }
    }
}
