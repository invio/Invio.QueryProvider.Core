namespace Invio.QueryProvider.Test.Models

type Customer = {
    CustomerId : string
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
} with
    static member Create id name contact contactTitle address city region postalCode country phone fax = {
        CustomerId = id;
        CompanyName = name;
        ContactName = contact;
        ContactTitle = contactTitle;
        Address = address;
        City = city;
        Region = region;
        PostalCode = postalCode;
        Country = country;
        Phone = phone;
        Fax = fax;
    }
