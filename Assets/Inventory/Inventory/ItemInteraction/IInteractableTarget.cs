using UnityEngine;

namespace VoidexSoft.Inventory.Interactions
{
    public interface IInteractableTarget
    {
        Transform MyTransform { get; }
        ItemInteraction[] Interactions { get; }
        void Interact(IInventoryHolder interactor = null);
        void InteractionSelect();
        void InteractionDeselect();
    }
}