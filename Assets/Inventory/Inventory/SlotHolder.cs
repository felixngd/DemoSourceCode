using UnityEngine;
using VoidexSoft.Inventory.DataCore;
using VoidexSoft.Inventory.Editor;

namespace VoidexSoft.Inventory
{
    /// <summary>
    /// A slot is designed for a curtain item, for example a sword in a character inventory
    /// </summary>
    [CreateAssetMenu(fileName = "SlotHolder", menuName = "VoidexSoft/Inventory/SlotHolder")]
    public class SlotHolder : DataEntity
    {
        [IconPreview]
        public Sprite icon;
    }
}