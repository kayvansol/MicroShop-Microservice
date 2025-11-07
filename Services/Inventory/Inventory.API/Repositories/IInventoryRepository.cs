namespace Inventory.API.Repositories
{
    public interface IInventoryRepository
    {
        Task<bool> IsThereEmptyInventory(int OrderId);
    }
}
