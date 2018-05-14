namespace Invio.QueryProvider.Test

open System
open System.IO
open System.Text

open Invio.QueryProvider.Test.CsvReader
open Invio.QueryProvider.Test.Models

type private Local = { Nothing : unit }

type Data() =
    let assembly = typedefof<Local>.Assembly

    let toString = function
        | null -> null
        | (value : obj) when (value :? string) -> value :?> string
        | value -> value.ToString()

    let toDecimal = function
        | (value : obj) when (value :? decimal) -> value :?> decimal
        | (value : obj) when (value :? double) -> (decimal (value :?> double))
        | (value : obj) when (value :? single) -> (decimal (value :?> single))
        | (value : obj) when (value :? int) -> (decimal (value :?> int))
        | (value : obj) when (value :? uint32) -> (decimal (value :?> uint32))
        | (value : obj) when (value :? int64) -> (decimal (value :?> int64))
        | (value : obj) when (value :? uint64) -> (decimal (value :?> uint64))
        | (value : obj) when (value :? int16) -> (decimal (value :?> int16))
        | (value : obj) when (value :? uint16) -> (decimal (value :?> uint16))
        | (value : obj) when (value :? sbyte) -> (decimal (value :?> sbyte))
        | (value : obj) when (value :? byte) -> (decimal (value :?> byte))
        | null -> failwith "A null value cannot be converted to a decimal."
        | value -> failwithf "The specified value of type %s could not be converted to a decimal." (value.GetType().Name)

    let toDouble = function
        | (value : obj) when (value :? decimal) -> (double (toDecimal value))
        | (value : obj) when (value :? double) -> (value :?> double)
        | (value : obj) when (value :? single) -> (double (value :?> single))
        | (value : obj) when (value :? int) -> (double (value :?> int))
        | (value : obj) when (value :? uint32) -> (double (value :?> uint32))
        | (value : obj) when (value :? int64) -> (double (value :?> int64))
        | (value : obj) when (value :? uint64) -> (double (value :?> uint64))
        | (value : obj) when (value :? int16) -> (double (value :?> int16))
        | (value : obj) when (value :? uint16) -> (double (value :?> uint16))
        | (value : obj) when (value :? sbyte) -> (double (value :?> sbyte))
        | (value : obj) when (value :? byte) -> (double (value :?> byte))
        | null -> failwith "A null value cannot be converted to a double."
        | value -> failwithf "The specified value of type %s could not be converted to a double." (value.GetType().Name)

    let lazyCategories = lazy (
        use stream = assembly.GetManifestResourceStream("Invio.QueryProvider.Test.Data.Category.csv")
        use reader = new StreamReader(stream, Encoding.UTF8)
        readCsvData reader
        |> Seq.mapi
            (fun ix data ->
                match data with
                | [|id;name;description;picture|] ->
                    Category.Create
                        (id :?> int)
                        (toString name)
                        (toString description)
                        (picture :?> byte[])
                | _ ->
                    raise (
                        new InvalidDataException(
                            sprintf "Unexpected column count (row #: %i, columns: %i)." ix data.Length)))
        |> ResizeArray<Category>)

    let lazyCustomers = lazy (
        use stream = assembly.GetManifestResourceStream("Invio.QueryProvider.Test.Data.Customer.csv")
        use reader = new StreamReader(stream, Encoding.UTF8)
        readCsvData reader
        |> Seq.mapi
            (fun ix data ->
                match data with
                | [|id;name;contact;contactTitle;address;city;region;postalCode;country;phone;fax|] ->
                    Customer.Create
                        (toString id)
                        (toString name)
                        (toString contact)
                        (toString contactTitle)
                        (toString address)
                        (toString city)
                        (toString region)
                        (toString postalCode)
                        (toString country)
                        (toString phone)
                        (toString fax)
                | _ ->
                    raise (
                        new InvalidDataException(
                            sprintf "Unexpected column count (row #: %i, columns: %i)." ix data.Length)))
        |> ResizeArray<Customer>)

    let lazyEmployees = lazy (
        use stream = assembly.GetManifestResourceStream("Invio.QueryProvider.Test.Data.Employee.csv")
        use reader = new StreamReader(stream, Encoding.UTF8)
        readCsvData reader
        |> Seq.mapi
            (fun ix data ->
                match data with
                | [|id;lastName;firstName;title;titleOfCourtesy;birthDate;hireDate;address;city;region;postalCode;country;homePhone;extension;photo;notes;reportsTo;photoPath|] ->
                    Employee.Create
                        (id :?> int)
                        (toString lastName)
                        (toString firstName)
                        (toString title)
                        (toString titleOfCourtesy)
                        (birthDate :?> Nullable<DateTime>)
                        (hireDate :?> Nullable<DateTime>)
                        (toString address)
                        (toString city)
                        (toString region)
                        (toString postalCode)
                        (toString country)
                        (toString homePhone)
                        (toString extension)
                        (photo :?> byte[])
                        (toString notes)
                        (reportsTo :?> Nullable<int>)
                        (match photoPath with null -> null | _ -> new Uri(toString photoPath))
                | _ ->
                    raise (
                        new InvalidDataException(
                            sprintf "Unexpected column count (row #: %i, columns: %i)." ix data.Length)))
        |> ResizeArray<Employee>)

    let lazyEmployeeTerritories = lazy (
        use stream = assembly.GetManifestResourceStream("Invio.QueryProvider.Test.Data.EmployeeTerritory.csv")
        use reader = new StreamReader(stream, Encoding.UTF8)
        readCsvData reader
        |> Seq.mapi
            (fun ix data ->
                match data with
                | [|employeeId;territoryId|] ->
                    EmployeeTerritory.Create
                        (employeeId :?> int)
                        (toString territoryId)
                | _ ->
                    raise (
                        new InvalidDataException(
                            sprintf "Unexpected column count (row #: %i, columns: %i)." ix data.Length)))
        |> ResizeArray<EmployeeTerritory>)

    let lazyOrders = lazy (
        use stream = assembly.GetManifestResourceStream("Invio.QueryProvider.Test.Data.Order.csv")
        use reader = new StreamReader(stream, Encoding.UTF8)
        readCsvData reader
        |> Seq.mapi
            (fun ix data ->
                match data with
                | [|id;customerId;employeeId;orderDate;requiredDate;shippedDate;shipVia;freight;shipName;shipAddress;shipCity;shipRegion;shipPostalCode;shipCountry|] ->
                    Order.Create
                        (id :?> int)
                        (toString customerId)
                        (employeeId :?> Nullable<int>)
                        (orderDate :?> DateTime)
                        (requiredDate :?> Nullable<DateTime>)
                        (shippedDate :?> Nullable<DateTime>)
                        (shipVia :?> int)
                        (toDecimal freight)
                        (toString shipName)
                        (toString shipAddress)
                        (toString shipCity)
                        (toString shipRegion)
                        (toString shipPostalCode)
                        (toString shipCountry)
                | _ ->
                    raise (
                        new InvalidDataException(
                            sprintf "Unexpected column count (row #: %i, columns: %i)." ix data.Length)))
        |> ResizeArray<Order>)

    let lazyOrderDetails = lazy (
        use stream = assembly.GetManifestResourceStream("Invio.QueryProvider.Test.Data.OrderDetail.csv")
        use reader = new StreamReader(stream, Encoding.UTF8)
        readCsvData reader
        |> Seq.mapi
            (fun ix data ->
                match data with
                | [|orderId;productId;untiPrice;quantity;discount|] ->
                    OrderDetail.Create
                        (orderId :?> int)
                        (productId :?> int)
                        (toDecimal untiPrice)
                        (quantity :?> int)
                        (toDouble discount)
                | _ ->
                    raise (
                        new InvalidDataException(
                            sprintf "Unexpected column count (row #: %i, columns: %i)." ix data.Length)))
        |> ResizeArray<OrderDetail>)

    let lazyProducts = lazy (
        use stream = assembly.GetManifestResourceStream("Invio.QueryProvider.Test.Data.Product.csv")
        use reader = new StreamReader(stream, Encoding.UTF8)
        readCsvData reader
        |> Seq.mapi
            (fun ix data ->
                match data with
                | [|productId;productName;supplierId;categoryId;quantityPerUnit;unitPrice;unitsInStock;unitsOnOrder;reorderLevel;discontinued|] ->
                    Product.Create
                        (productId :?> int)
                        (toString productName)
                        (supplierId :?> int)
                        (categoryId :?> int)
                        (toString quantityPerUnit)
                        (toDecimal unitPrice)
                        (unitsInStock :?> int)
                        (unitsOnOrder :?> int)
                        (reorderLevel :?> int)
                        ((discontinued :?> int) = 1)
                | _ ->
                    raise (
                        new InvalidDataException(
                            sprintf "Unexpected column count (row #: %i, columns: %i)." ix data.Length)))
        |> ResizeArray<Product>)

    let lazyRegions = lazy (
        use stream = assembly.GetManifestResourceStream("Invio.QueryProvider.Test.Data.Region.csv")
        use reader = new StreamReader(stream, Encoding.UTF8)
        readCsvData reader
        |> Seq.mapi
            (fun ix data ->
                match data with
                | [|regionId;regionDescription|] ->
                    Region.Create
                        (regionId :?> int)
                        (toString regionDescription)
                | _ ->
                    raise (
                        new InvalidDataException(
                            sprintf "Unexpected column count (row #: %i, columns: %i)." ix data.Length)))
        |> ResizeArray<Region>)

    let lazyShippers = lazy (
        use stream = assembly.GetManifestResourceStream("Invio.QueryProvider.Test.Data.Shipper.csv")
        use reader = new StreamReader(stream, Encoding.UTF8)
        readCsvData reader
        |> Seq.mapi
            (fun ix data ->
                match data with
                | [|shipperId;companyName;phone|] ->
                    Shipper.Create
                        (shipperId :?> int)
                        (toString companyName)
                        (toString phone)
                | _ ->
                    raise (
                        new InvalidDataException(
                            sprintf "Unexpected column count (row #: %i, columns: %i)." ix data.Length)))
        |> ResizeArray<Shipper>)

    let lazySuppliers = lazy (
        use stream = assembly.GetManifestResourceStream("Invio.QueryProvider.Test.Data.Supplier.csv")
        use reader = new StreamReader(stream, Encoding.UTF8)
        readCsvData reader
        |> Seq.mapi
            (fun ix data ->
                match data with
                | [|supplierId;companyName;contactName;contactTitle;address;city;region;postalCode;country;phone;fax;homePage|] ->
                    Supplier.Create
                        (supplierId :?> int)
                        (toString companyName)
                        (toString contactName)
                        (toString contactTitle)
                        (toString address)
                        (toString city)
                        (toString region)
                        (toString postalCode)
                        (toString country)
                        (toString phone)
                        (toString fax)
                        (toString homePage)
                | _ ->
                    raise (
                        new InvalidDataException(
                            sprintf "Unexpected column count (row #: %i, columns: %i)." ix data.Length)))
        |> ResizeArray<Supplier>)

    let lazyTerritories = lazy (
        use stream = assembly.GetManifestResourceStream("Invio.QueryProvider.Test.Data.Territory.csv")
        use reader = new StreamReader(stream, Encoding.UTF8)
        readCsvData reader
        |> Seq.mapi
            (fun ix data ->
                match data with
                | [|territoryId;territoryDescription;regionId|] ->
                    Territory.Create
                        (toString territoryId)
                        (toString territoryDescription)
                        (regionId :?> int)
                | _ ->
                    raise (
                        new InvalidDataException(
                            sprintf "Unexpected column count (row #: %i, columns: %i)." ix data.Length)))
        |> ResizeArray<Territory>)

    member this.Categories = lazyCategories.Force()
    member this.Customers = lazyCustomers.Force()
    member this.Employees = lazyEmployees.Force()
    member this.EmployeeTerritories = lazyEmployeeTerritories.Force()
    member this.Orders = lazyOrders.Force()
    member this.OrderDetails = lazyOrderDetails.Force()
    member this.Products = lazyProducts.Force()
    member this.Regions = lazyRegions.Force()
    member this.Shippers = lazyShippers.Force()
    member this.Suppliers = lazySuppliers.Force()
    member this.Territories = lazyTerritories.Force()
