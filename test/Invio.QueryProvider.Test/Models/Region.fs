namespace Invio.QueryProvider.Test.Models

type Region = {
    RegionId : int
    RegionDescription : string
} with
    static member Create id description = {
        RegionId = id;
        RegionDescription = description;
    }
