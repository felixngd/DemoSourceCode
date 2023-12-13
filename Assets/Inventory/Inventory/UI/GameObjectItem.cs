using UnityEngine;

namespace VoidexSoft.Inventory.UI
{
    /// <summary>
    /// Represents a <see cref="ItemBase"/> stack in the world space.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class GameObjectItem : InteractableGameObject
    {
        public ItemStack Data;
        
        private int _itemId;
        
        public override void Interact(IInventoryHolder interactor = null)
        {
            Debug.Log("Interacting with " + Data.Source.Title);
        }
        
        public void Initialize(ItemBase data, int stackSize)
        {
            Data = new ItemStack(data, stackSize);
            _itemId = data.Id;
        }
    }
}