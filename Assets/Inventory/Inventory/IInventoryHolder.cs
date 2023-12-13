using UnityEngine;

namespace VoidexSoft.Inventory
{
    /// <summary>
    /// Reference to an inventory to use or interact with it
    /// </summary>
    public interface IInventoryHolder
    {
        Inventory Inventory { get; set; }
        Transform MyTransform { get; }
    }
}