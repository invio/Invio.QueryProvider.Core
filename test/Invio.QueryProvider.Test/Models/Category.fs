namespace Invio.QueryProvider.Test.Models

type Category = {
    CategoryId : int
    CategoryName : string
    Description : string
    Picture : byte[]
} with
    static member Create id name description picture = {
        CategoryId = id;
        CategoryName = name;
        Description = description;
        Picture = picture
    }
