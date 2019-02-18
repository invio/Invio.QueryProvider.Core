namespace Invio.QueryProvider.Test.Models

open System

type Supplier = {
    SupplierId : int
    CompanyName : string
    ContactName : string
    ContactTitle : string
    Address : string
    City : string
    Region : string
    PostalCode : string
    Country : string
    Phone : string
    Fax : string
    HomePage : Nullable<Hyperlink>
} with
    static member Create id companyName contactName contactTitle address city region postalCode country phone fax homePage = {
        SupplierId = id;
        CompanyName = companyName;
        ContactName = contactName;
        ContactTitle = contactTitle;
        Address = address;
        City = city;
        Region = region;
        PostalCode = postalCode;
        Country = country;
        Phone = phone;
        Fax = fax;
        HomePage = homePage;
    }
