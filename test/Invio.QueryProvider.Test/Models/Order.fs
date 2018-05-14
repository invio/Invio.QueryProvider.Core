namespace Invio.QueryProvider.Test.Models

open System

type Order = {
    OrderId : int
    CustomerId : string
    EmployeeId : int Nullable
    OrderDate : DateTime
    RequiredDate : DateTime Nullable
    ShippedDate : DateTime Nullable
    ShipVia : int
    Freight : decimal
    ShipName : string
    ShipAddress : string
    ShipCity : string
    ShipRegion : string
    ShipPostalCode : string
    ShipCountry : string
} with
    static member Create id customerId employeeId orderDate requiredDate shippedDate shipVia freight shipName shipAddress shipCity shipRegion shipPostalCode shipCountry = {
        OrderId = id;
        CustomerId = customerId;
        EmployeeId = employeeId;
        OrderDate = orderDate;
        RequiredDate = requiredDate;
        ShippedDate = shippedDate;
        ShipVia = shipVia;
        Freight = freight;
        ShipName = shipName;
        ShipAddress = shipAddress;
        ShipCity = shipCity;
        ShipRegion = shipRegion;
        ShipPostalCode = shipPostalCode;
        ShipCountry = shipCountry;
    }
