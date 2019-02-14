namespace Invio.QueryProvider.Test.Models

open System

type Employee = {
    EmployeeId : int
    LastName : string
    FirstName : string
    Title : string
    TitleOfCourtesy : string
    BirthDate : DateTime Nullable
    HireDate : DateTime Nullable
    Address : string
    City : string
    Region : string
    PostalCode : string
    Country : string
    HomePhone : PhoneNumber
    Extension : string
    Photo : byte[]
    Notes : string
    ReportsTo : int Nullable
    PhotoPath : Uri
} with
    static member Create id lastName firstName title titleOfCourtesy birthDate hireDate address city region postalCode country homePhone extension photo notes reportsTo photoPath = {
        EmployeeId = id;
        LastName = lastName;
        FirstName = firstName;
        Title = title;
        TitleOfCourtesy = titleOfCourtesy;
        BirthDate = birthDate;
        HireDate = hireDate;
        Address = address;
        City = city;
        Region = region;
        PostalCode = postalCode;
        Country = country;
        HomePhone = homePhone;
        Extension = extension;
        Photo = photo;
        Notes = notes;
        ReportsTo = reportsTo;
        PhotoPath = photoPath;
    }
