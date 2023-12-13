using UnityEngine;
using VoidexSoft.Inventory.Interactions;

namespace VoidexSoft.Inventory.UI
{
    public interface IInteractableUi
    {
        Transform MyTransform { get; }
        ItemInteraction[] Interactions { get; }
        void Interact(IInventoryHolder interactor = null);
        void InteractionSelect();
        void InteractionDeselect();
    }
}