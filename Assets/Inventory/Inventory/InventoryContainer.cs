using UnityEngine;
using UnityEngine.Serialization;
using VoidexSoft.Inventory.DataCore;

namespace VoidexSoft.Inventory
{
    [CreateAssetMenu(fileName = "InventoryContainer", menuName = "VoidexSoft/Inventory/InventoryContainer")]
    public class InventoryContainer : DataEntity
    {
        public SlotHolder[] slots;

        public InventoryContainer()
        {
            slots = new SlotHolder[10];
        }

        protected override void Reset()
        {
            base.Reset();
            slots = new SlotHolder[10];
        }
    }
}