using UnityEngine;
using VoidexSoft.Inventory.Interactions;

namespace VoidexSoft.Inventory.UI
{
    public abstract class InteractableGameObject : MonoBehaviour
    {
        public static InteractableGameObject CurrentSelected;
        public bool IsSelected { get; protected set; }
        public int InteractionPriority { get; protected set; }
        public ItemInteraction[] Interactions { get; protected set; }
        public abstract void Interact(IInventoryHolder interactor = null);
        public virtual void InteractionSelect(IInventoryHolder interactor)
        {
            if (CurrentSelected != null) CurrentSelected.InteractionDeselect(interactor);
            IsSelected = true;
        }
        public virtual void InteractionDeselect(IInventoryHolder interactor = null)
        {
            IsSelected = false;
        }
    }
    
}