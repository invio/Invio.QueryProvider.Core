namespace Invio.QueryProvider.Test.Models

open System

type Product = {
    ProductId : int
    ProductName : string
    SupplierId : int
    CategoryId : int
    QuantityPerUnit : string
    UnitPrice : decimal
    UnitsInStock : int
    UnitsOnOrder : int
    ReorderLevel : int
    Discontinued : bool
} with
    static member Create id name supplierId categoryId quantityPerUnit unitPrice unitsInStock unitsOnOrder reorderLevel discontinued = {
        ProductId = id;
        ProductName = name;
        SupplierId = supplierId;
        CategoryId = categoryId;
        QuantityPerUnit = quantityPerUnit;
        UnitPrice = unitPrice;
        UnitsInStock = unitsInStock;
        UnitsOnOrder = unitsOnOrder;
        ReorderLevel = reorderLevel;
        Discontinued = discontinued;
    }
