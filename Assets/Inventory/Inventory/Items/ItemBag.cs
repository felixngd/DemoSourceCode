using UnityEngine;
using VoidexSoft.Inventory.DataCore;

namespace VoidexSoft.Inventory.Items
{
    [CreateAssetMenu(fileName = "ItemBag", menuName = "VoidexSoft/Inventory/Items/ItemBag")]
    public class ItemBag : DataEntity
    {
        public ItemBase[] items;
        public int[] amounts;

        protected virtual bool HasError()
        {
            if (items.Length == 0 || amounts.Length != items.Length)
            {
                Debug.LogError("There is errors with the data", this);
                return true;
            }

            return false;
        }

        public ItemStack GetBag()
        {
            if (HasError()) return null;

            int index = Random.Range(0, items.Length);
            return new ItemStack(items[index], amounts[index]);
        }
    }
}