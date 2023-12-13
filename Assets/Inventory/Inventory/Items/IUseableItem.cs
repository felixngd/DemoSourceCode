namespace VoidexSoft.Inventory.Items
{
    public interface IUseableItem
    {
        int Id { get; }
        float UseCooldownTime { get; set; }
        void Use(IInventoryHolder holder);
    }
}