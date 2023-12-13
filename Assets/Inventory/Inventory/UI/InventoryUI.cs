using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace VoidexSoft.Inventory.UI
{
    /// <summary>
    /// This class manages the UI side of the <see cref="Inventory"/> by listening to a target for OnChanged events and refreshing the UI slot that changed.
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        public enum DragDropAction { Cancel, Move, Discard }
        public static Action OnAnyClickedItemChanged;
        public static Action OnAnyInventoryUiOpened;
        public static Action OnAnyInventoryUiClosed;
        public static ItemUI DragOrigin;
        public static ItemUI DragDestination;
        
        public static ItemUI ClickedItem
        {
            get => m_clickedItem;
            set
            {
                m_clickedItem = value;
                OnAnyClickedItemChanged?.Invoke();
            }
        }
        private static ItemUI m_clickedItem;
        
        
        public GridLayoutGroup GridUi;
        public SlotHolder[] slotHolders;

        [Header("Runtime Only")]
        public List<ItemUI> slots;
        public Inventory inventory;
        /// <summary>
        /// A map for translating inventory indexes to grid slot indexes and vice versa. KEY is Inventory Array Index, VALUE is Grid UI Slot Index.
        /// </summary>
        public Dictionary<int, int> IndexMap;

        public Action OnClosed;
        public Action OnOpened;

        private void Awake()
        {
            IndexMap = new Dictionary<int, int>();
            CanvasInventory.OnInventoryHolderSpawn += LoadInventoryOfHolder;
            
        }

        private void OnDestroy()
        {
            CanvasInventory.OnInventoryHolderSpawn -= LoadInventoryOfHolder;
        }

        private void OnDisable()
        {
            OnClosed?.Invoke();
            OnAnyInventoryUiClosed?.Invoke();
        }
        
        public static DragDropAction DetectDragDropAction()
        {
            if (DragOrigin != null && DragDestination != null) return DragDropAction.Move;
            if (DragOrigin != null && DragDestination == null) return DragDropAction.Discard;
            return DragDropAction.Cancel;
        }

        public virtual void LoadInventoryOfHolder(IInventoryHolder avatar)
        {
            SetTargetInventory(avatar.Inventory);
        }
        public virtual void SetTargetInventory(Inventory inv)
        {
            if (inv == null)
            {
                Debug.LogException(new Exception("Inventory was null when trying to set target inventory on UI."), this);
                return;
            }

            ClearUi();
            inv.OnInitialized += ReloadCurrentInventory; 
            inventory = inv;

            if (!inventory.IsInitialized)
            {
                return;
            }

            for (int indexInv = 0; indexInv < inv.MaxSlots; indexInv++)
            {
                foreach (SlotHolder s in slotHolders)
                {
                    if (inv.Container.slots[indexInv] == s)
                    {
                        AddNewSlot(indexInv);
                    }
                }
            }

            // Subscribe new
            inv.OnChanged += UpdateUi;
            inv.OnDestroyed += DestroyedCleanup;

        }
        private void ReloadCurrentInventory()
        {
            SetTargetInventory(inventory);
        }
        private void AddNewSlot(int inventoryIndex)
        {
            GameObject x = Instantiate(CanvasInventory.ItemSlotTemplate, GridUi.transform);
            ItemUI comp = x.GetComponentInChildren<ItemUI>();
            comp.ReferenceInventoryIndex = inventoryIndex;
            comp.Ui = this;
            slots.Add(comp);
            int slotIndex = slots.Count - 1;
            IndexMap.Add(inventoryIndex, slotIndex);
            UpdateUi(inventoryIndex);
        }

        public virtual void UpdateUi(int inventoryIndex)
        {
            if (!IndexMap.ContainsKey(inventoryIndex)) return;

            int slotIndex = IndexMap[inventoryIndex];

            slots[slotIndex].UpdateUi(inventory.Get(inventoryIndex), inventory.Container.slots[inventoryIndex]);
        }
        public virtual void ClearUi()
        {
            if (inventory != null) inventory.OnChanged -= UpdateUi;

            foreach (ItemUI item in slots) Destroy(item.slotOwnerObject);
            slots = new List<ItemUI>();
            IndexMap = new Dictionary<int, int>();
        }
        public void CloseUi()
        {
            OnAnyInventoryUiClosed?.Invoke();
            OnClosed?.Invoke();

            EventSystem.current.SetSelectedGameObject(null);
        }
        public void OpenUi()
        { 
            OnAnyInventoryUiOpened?.Invoke();
            OnOpened?.Invoke();

            EventSystem.current.SetSelectedGameObject(slots[0].gameObject);
        }
        
        protected virtual void DestroyedCleanup(Inventory inv)
        {
            if (inv != inventory) return;
            ClearUi();
        }
    }
}