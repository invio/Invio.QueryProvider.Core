namespace Invio.QueryProvider.Test.Models

type Shipper = {
    ShipperId : int
    CompanyName : string
    Phone : string
} with
    static member Create id companyName phone = {
        ShipperId = id;
        CompanyName = companyName;
        Phone = phone;
    }
