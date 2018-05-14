namespace Invio.QueryProvider.Test.Models

open System

type OrderDetail = {
    OrderId : int
    ProductId : int
    UnitPrice : decimal
    Quantity : int
    Discount : double
} with
    static member Create orderId productId unitPrice quantity discount = {
        OrderId = orderId;
        ProductId = productId;
        UnitPrice = unitPrice;
        Quantity = quantity;
        Discount = discount;
    }
