namespace Cargotruck.Server.Services
{
    public interface IColumnNamesService
    {
        List<string> GetCargoesColumnNames();
        List<string> GetExpensesColumnNames();
        List<string> GetMonthlyExpensesColumnNames();
        List<string> GetRoadsColumnNames();
        List<string> GetTasksColumnNames();
        List<string> GetTrucksColumnNames();
        List<string> GetWarehousesColumnNames();
    }
}