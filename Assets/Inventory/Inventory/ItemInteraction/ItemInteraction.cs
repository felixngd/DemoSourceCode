using VoidexSoft.Inventory.DataCore;

namespace VoidexSoft.Inventory.Interactions
{
    public class ItemInteraction
    {
        /// <summary>
        /// A base class for item interactions.
        /// </summary>
        public abstract class Interaction : DataEntity
        {
            public string interactName;

            /// <summary>
            /// Check if the interaction is valid.
            /// </summary>
            /// <param name="interactable"></param>
            /// <returns></returns>
            public abstract bool IsValid(IInteractableTarget interactable);

            /// <summary>
            /// Perform the interaction.
            /// </summary>
            /// <param name="interactable"></param>
            public abstract void DoInteract(IInteractableTarget interactable);
        }
    }
}