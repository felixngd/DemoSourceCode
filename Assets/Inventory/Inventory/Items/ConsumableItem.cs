using UnityEngine;

namespace VoidexSoft.Inventory.Items
{
    [CreateAssetMenu(fileName = "ConsumableItem", menuName = "VoidexSoft/Inventory/Items/ConsumableItem")]
    public class ConsumableItem : UseableItem
    {
        
        public override void Use(IInventoryHolder holder)
        {
            holder.Inventory.DoTake(this, 1);
            
            //Do something with the item, for example, increase health and play a sound
        }
    }
}