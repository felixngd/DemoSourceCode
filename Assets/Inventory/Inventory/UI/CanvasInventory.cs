using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VoidexSoft.Inventory.UI
{
    public static class CanvasInventory
    {
        public static GameObject ItemSlotTemplate;
        
        public static GameObject RuntimeItemTemplate;
        
        public static GameObject InventoryUiTemplate;
        
        public static bool CanDragItemUI = true;
        
        public static Canvas MainCanvas;

        /// <summary>
        /// A global event that is fired when an inventory holder is spawned into the world.
        /// </summary>
        public static Action<IInventoryHolder> OnInventoryHolderSpawn;
        
        public static Action<ItemUI> OnMoveItemBegin;

        public static Action<ItemUI> OnMoveItemCancel;

        public static Action<ItemUI> OnMoveItemEnd;

        public static Action<ItemUI> OnSlotSelected;
        
        public static void Initialize(Canvas canvas, GameObject itemSlot, GameObject itemRuntime, GameObject inventoryTemplate, bool canDragPlugs)
        {
            ItemSlotTemplate = itemSlot;
            RuntimeItemTemplate = itemRuntime;
            CanDragItemUI = canDragPlugs;
            MainCanvas = canvas;
            InventoryUiTemplate = inventoryTemplate;
        }

        /// <summary>
        /// Spawns a new item into the world. Creates a wrapper object from the template, spawns the prefab as a child and assigns the correct properties.
        /// </summary>
        public static GameObjectItem SpawnWorldItem(ItemBase item, Vector3 pos, int stackSize)
        {
            if (item == null || item.prefab == null)
            {
                Debug.LogError("Failed SpawnWorldItem(). The input Source item or prefab object was null when trying to SpawnWorldItem().");
                return null;
            }

            GameObject wrapper = Object.Instantiate(RuntimeItemTemplate, pos, Quaternion.identity);
            GameObjectItem itemComponent = wrapper.GetComponent<GameObjectItem>();

            if (itemComponent == null)
            {
                Debug.LogError("Failed SpawnWorldItem(). The RuntimeItemTemplate did not have a GameObjectItem component attached.");
                return null;
            }

            
            itemComponent.Initialize(item, stackSize);
            
            return itemComponent;
        }
        
        public static GameObject SpawnInventoryUI(Inventory target)
        {
            // Spawn the UI, check the top level object, then dig deeper if there is nothing found.
            var go = Object.Instantiate(InventoryUiTemplate, MainCanvas.transform);
            var ui = go.GetComponent<InventoryUI>();
            if (ui == null) ui = go.GetComponentInChildren<InventoryUI>();
        
            // Bind the UI to the given inventory.
            ui.SetTargetInventory(target);
            return go;
        }
        

        /// <summary>
        /// Move an item from one inventory to another. Returns the amount of items that CAN NOT be moved.
        /// </summary>
        public static int TryMoveItem(Inventory target, ItemStack stack)
        {
            if (stack != null && target != null) return target.DoAdd(stack);
            Debug.LogError("Failed TryMoveItem(). The input Source item or Prefab object was null when trying to TryMoveItem().");
            return -1;
        }
    }
}