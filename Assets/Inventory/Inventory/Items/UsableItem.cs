using UnityEngine;

namespace VoidexSoft.Inventory.Items
{
    public abstract class UseableItem : ItemBase, IUseableItem
    {
        [SerializeField]
        private float m_useCooldown;
        public float UseCooldownTime { get => m_useCooldown; set => m_useCooldown = value; }

        public abstract void Use(IInventoryHolder holder);
    }
}