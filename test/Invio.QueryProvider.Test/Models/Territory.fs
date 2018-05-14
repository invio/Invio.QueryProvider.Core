namespace Invio.QueryProvider.Test.Models

type Territory = {
    TerritoryId : string
    TerritoryDescription : string
    RegionId : int
} with
    static member Create id description regionId = {
        TerritoryId = id;
        TerritoryDescription = description;
        RegionId = regionId;
    }
