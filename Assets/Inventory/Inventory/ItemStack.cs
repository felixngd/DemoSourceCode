using VoidexSoft.Inventory.DataCore;

namespace VoidexSoft.Inventory
{
    public class ItemStack
    {
        public ItemStack(ItemBase item, int count)
        {
            Source = item;
            StackSize = count;
        }
        public ItemStack(int vaultId, int count)
        {
            Source = (ItemBase) DatabaseManager.Get(vaultId);
            StackSize = count;
        }

        public ItemBase Source;
        public int StackSize;

        public virtual void Reset()
        {
            Source = null;
            StackSize = 0;
        }

        /// <summary>
        /// Returns the total value of the stack.
        /// </summary>
        /// <returns></returns>
        public virtual int GetTotalValue()
        {
            return Source.value * StackSize;
        }
    }
}