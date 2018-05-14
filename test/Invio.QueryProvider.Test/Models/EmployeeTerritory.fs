namespace Invio.QueryProvider.Test.Models

type EmployeeTerritory = {
    EmployeeId : int
    TerritoryId : string
} with
    static member Create employeeId territoryId = {
        EmployeeId = employeeId;
        TerritoryId = territoryId;
    }

