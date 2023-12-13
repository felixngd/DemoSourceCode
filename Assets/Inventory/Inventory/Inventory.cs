using System;
using System.Collections.Generic;
using UnityEngine;
using VoidexSoft.Inventory.DataCore;
using VoidexSoft.Inventory.Items;
using VoidexSoft.Inventory.UI;

namespace VoidexSoft.Inventory
{
    public class Inventory : MonoBehaviour
    {
        public bool IsInitialized { get; protected set; }

        public IInventoryHolder InventoryHolder;
        public Action<int> OnChanged;
        public Action<ItemStack> OnItemAdded;
        public Action<ItemStack> OnItemRemoved;
        public Action<Inventory> OnDestroyed;
        public Action OnInitialized;

        [SerializeField]
        private InventoryContainer _inventoryContainer;
        public virtual InventoryContainer Container
        {
            get => _inventoryContainer;
            set => _inventoryContainer = value;
        }

        protected List<ItemStack> Content { get; set; }

        protected List<SlotHolder> Holders;
        public int MaxSlots
        {
            get => m_maxSlots;
            set
            {
                if (value < m_maxSlots || value == m_maxSlots) return;

                int oldCount = m_maxSlots;
                m_maxSlots = value;
                for (int i = oldCount; i < m_maxSlots; i++)
                {
                    Content.Add(null);
                    OnChanged?.Invoke(i);
                }
            }
        }
        private int m_maxSlots;

        /// <summary>
        /// Initialize the inventory.
        /// </summary>
        public virtual void Initialize(IInventoryHolder holder, bool resetContent, InventoryContainer config = null, bool silent = false)
        {
            // set grid
            if (config == null && Container != null) config = Container;
            else if (config != null) Container = config;
            else config = ScriptableObject.CreateInstance<InventoryContainer>();

            // set owner
            InventoryHolder = holder;

            // set slots
            Holders = new List<SlotHolder>();
            m_maxSlots = config.slots.Length > 0 ? config.slots.Length : 1;
            for (int i = 0; i < config.slots.Length; i++)
            {
                Holders.Add(config.slots[i]);
            }

            // set content
            if (resetContent)
            {
                // list must always be at capacity and all empty nodes must be null.
                Content = new List<ItemStack>();
                for (int i = 0; i < config.slots.Length; i++)
                {
                    Content.Add(null);
                }
            }

            if (!silent)
            {
                IsInitialized = true;
                OnInitialized?.Invoke();
            }
        }

        /// <summary>
        /// Initialize this inventory from an <see cref="SerializedInventory"/> which has references to the config and content stacks.
        /// </summary>
        public virtual void Initialize(IInventoryHolder holder, SerializedInventory state)
        {
            Container = (InventoryContainer)DatabaseManager.Get(state.dbId);
            Content = new List<ItemStack>();

            for (int i = 0; i < state.itemIds.Count; i++)
            {
                if (state.itemIds[i] == -1)
                {
                    Content.Add(null);
                    continue;
                }

                Content.Add(new ItemStack((ItemBase)DatabaseManager.Get(state.itemIds[i]), state.itemStackCounts[i]));
            }

            Initialize(holder, false, Container, silent:true);
            IsInitialized = true;
            OnInitialized?.Invoke();
        }

        /// <summary>
        /// Get the configuration and content of this Inventory converted into an <see cref="SerializedInventory"/> which can be serialized and deserialized into JSON.
        /// </summary>
        public virtual SerializedInventory ToSerializedInventory()
        {
            return new SerializedInventory(this, Content);
        }

        /// <summary>
        /// Read the content at the given index.
        /// </summary>
        public virtual ItemStack Get(int index)
        {
            return Content == null || Content[index] == null || Content[index].Source == null
                ? new ItemStack(null, 0) 
                : Content[index];
        }

        /// <summary>
        /// Searches the Inventory to see if it has an amount of a specific item by matching title.
        /// </summary>
        public virtual bool Contains(ItemBase item, int amount)
        {
            List<int> indices = new List<int>();
            int verifiedCount = 0;

            for (int i = 0; i < MaxSlots; i++)
            {
                if (Get(i).Source == null) continue;
                if (Get(i).Source.Title == item.Title)
                {
                    indices.Add(i);
                }
            }

            for (int i = 0; i < indices.Count; i++)
            {
                verifiedCount += Get(indices[i]).StackSize;
            }

            return verifiedCount >= amount;
        }

        /// <summary>
        /// Get all slots with the matching slot holder
        /// </summary>
        public virtual int[] GetAllSlotIndexOfType(SlotHolder slotHolder)
        {
            List<int> results = new List<int>();
            for (int i = 0; i < Container.slots.Length; i++)
            {
                if (Container.slots[i] == slotHolder) results.Add(i);
            }
            return results.ToArray();
        }

        /// <summary>
        /// Get the first slot with the matching slotHolder
        /// </summary>
        public virtual int GetFirstSlotIndexOfType(SlotHolder slotHolder, bool mustBeEmpty)
        {
            for (int i = 0; i < Container.slots.Length; i++)
            {
                if (Container.slots[i] == slotHolder)
                {
                    if (mustBeEmpty && Content[i] == null) return i;
                    if (!mustBeEmpty) return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Count how many of the given item are in this inventory.
        /// </summary>
        /// <param name="item">The item you want to count.</param>
        /// <returns>Available stack size of the item in the inventory.</returns>
        public virtual int GetCountOfItem(ItemBase item)
        {
            return GetItemCount(item.Id);
        }

        /// <summary>
        /// Count how many of the given item are in this inventory.
        /// </summary>
        public virtual int GetCountOfItem(int itemId)
        {
            return DoCount(itemId);
        }

        /// <summary>
        /// Count the number of slots with no content in them.
        /// </summary>
        public virtual int GetEmptySlotCount()
        {
            int emptyCount = 0;
            foreach (ItemStack itemStack in Content)
            {
                if (itemStack == null) emptyCount++;
            }
            return emptyCount;
        }

        /// <summary>
        /// Get all content indexes using the specified Slot
        /// </summary>
        public virtual int[] GetAllIndexesWithSlot(SlotHolder slotHolder, bool includeNoRestriction)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < Container.slots.Length; i++)
            {
                if (Container.slots[i] == null)
                {
                    if (includeNoRestriction || slotHolder == null) result.Add(i);
                }
                else if (slotHolder != null && Container.slots[i].Id == slotHolder.Id)
                {
                    result.Add(i);
                }
            }
            return result.ToArray();
        }
        
        
        public virtual void Set(int index, ItemStack item)
        {
            Content[index] = item;
        }

        /// <summary>
        /// Add an item to this inventory, if possible.
        /// </summary>
        /// <returns>The amount that could NOT be added. A return of 0 is total success, the amount was fully added.</returns>
        public virtual int DoAdd(ItemStack item, bool tryToMerge = true)
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("Cannot Add to an uninitialized inventory.", this);
                return -1;
            }
            if (item == null || item.Source == null)
            {
                return -1;
            }
            if (item.StackSize == 0) item.StackSize = 1;

            int hotStack = item.StackSize;

            #region Merge Path
            if (tryToMerge && item.Source.maxStackSize > 1)
            {
                bool merging = true;
                while (merging)
                {
                    if (hotStack <= 0) return 0;

                    // Try to find a suitable slot to merge into, exit this loop if we can't.
                    int mergeIndex = GetIndexOfItemNotFull(item.Source.Id);
                    if (mergeIndex == -1)
                    {
                        merging = false;
                        continue;
                    }

                    // If found a matching slot, see how much doesn't fit.
                    int remain = GetMergeRemainder(
                        hotStack,
                        Content[mergeIndex].StackSize,
                        Content[mergeIndex].Source.maxStackSize);

                    // stash old data, for use as a delta.
                    ItemStack delta = new ItemStack(Content[mergeIndex].Source, Content[mergeIndex].StackSize);

                    // Modify the data on the server
                    if (remain > 0) Content[mergeIndex].StackSize = Content[mergeIndex].Source.maxStackSize;
                    else Content[mergeIndex].StackSize += hotStack;

                    // update the delta amount
                    delta.StackSize = Content[mergeIndex].StackSize - delta.StackSize;

                    // Update live stack count and flag a change on this index.
                    hotStack = remain;

                    if (Application.isBatchMode) OnChanged?.Invoke(mergeIndex);

                    // broadcast add.
                    OnItemAdded?.Invoke(delta);
                    UpdateSlot(mergeIndex, Content[mergeIndex].Source.Id, Content[mergeIndex].StackSize, true);
                }
            }
            #endregion

            #region Open Slot Path
            while (true)
            {
                // Find the first open slot. If there isn't one, or there's no amount left, we're done.
                int openSlot = GetValidNullIndex(item.Source.slotHolder);
                if (openSlot == -1 || hotStack <= 0) return hotStack;

                // Otherwise we can add a fresh new item to the list.
                Content[openSlot] = item;
                Content[openSlot].StackSize = 0;

                // And figure out if it fits. (duh, it does, but whatever, maybe StackSize > MaxStackSize because reasons?)
                int remain = GetMergeRemainder(
                    hotStack,
                    Content[openSlot].StackSize,
                    Content[openSlot].Source.maxStackSize);

                // Then fill up the open slot
                Content[openSlot].StackSize = remain > 0
                    ? Content[openSlot].Source.maxStackSize
                    : hotStack;

                // And update the hot stack, flag a change and try another loop
                hotStack = remain;
                OnItemAdded?.Invoke(Content[openSlot]);
                UpdateSlot(openSlot, item.Source.Id, Content[openSlot].StackSize, true);
            }

            #endregion
        }

        /// <summary>
        /// Move an item from one slot to another slot, even between <see cref="Inventory"/>.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="target"></param>
        /// <param name="fromIndex"></param>
        /// <param name="toIndex"></param>
        /// <returns></returns>
        public virtual int DoMove(Inventory origin, Inventory target, int fromIndex, int toIndex)
        {
            if (!IsInitialized) return -1;
            if (origin == target && fromIndex == toIndex) return -1;
            if (origin.Content[fromIndex] == null) return -1;

            // try merge - is goal empty?
            if (target.Content[toIndex] != null)
            {
                // not empty apparently, so are they different items? if so we can try to swap them.
                if (origin.Content[fromIndex].Source != target.Content[toIndex].Source)
                {
                    // is the goal slot holder None or Same as the origin item going there?
                    if (target.Holders[toIndex] != null &&
                        target.Holders[toIndex] != origin.Content[fromIndex].Source.slotHolder)
                    {
                        return -1;
                    }

                    // is the origin slot holder None or Same as goal item going there?
                    if (origin.Holders[fromIndex] != null &&
                        origin.Holders[fromIndex] != target.Content[toIndex].Source.slotHolder)
                    {
                        return -1;
                    }

                    ItemStack originCache = origin.Content[fromIndex];
                    ItemStack goalCache = target.Content[toIndex];

                    target.Content[toIndex] = originCache;
                    origin.Content[fromIndex] = goalCache;

                    // for client
                    target.UpdateSlot(toIndex, target.Content[toIndex].Source.Id, target.Content[toIndex].StackSize, false);
                    origin.UpdateSlot(fromIndex, origin.Content[fromIndex].Source.Id, origin.Content[fromIndex].StackSize, false);

                    return 0;
                }

                // alright, apparently not empty - but they're the same type! sooo see how many are left if origin stack merged into goal stack
                int remain = GetMergeRemainder(
                    origin.Content[fromIndex].StackSize,
                    target.Content[toIndex].StackSize,
                    target.Content[toIndex].Source.maxStackSize);

                // they're the same type, so simply set the stack sizes on both slots
                if (remain <= 0)
                {
                    // if theres none remaining, the merge was 100% success
                    target.Content[toIndex].StackSize += origin.Content[fromIndex].StackSize;
                    origin.Content[fromIndex] = null;

                    target.UpdateSlot(toIndex, target.Content[toIndex].Source.Id, target.Content[toIndex].StackSize, false);
                    origin.UpdateSlot(fromIndex, -1, 0, false);

                    return 0;
                }

                // otherwise, there is a remainder so max out the goal stack and set the origin stack size to the remainder.
                target.Content[toIndex].StackSize = target.Content[toIndex].Source.maxStackSize;
                origin.Content[fromIndex].StackSize = remain;

                // for client
                target.UpdateSlot(toIndex, target.Content[toIndex].Source.Id, target.Content[toIndex].StackSize, false);
                origin.UpdateSlot(fromIndex, origin.Content[fromIndex].Source.Id, origin.Content[fromIndex].StackSize, false);

                return remain;
            }

            // otherwise, direct move to empty cell.
            // does the slot accept this property?
            if (target.Holders[toIndex] == null || target.Holders[toIndex] == origin.Content[fromIndex].Source.slotHolder)
            {
                target.Content[toIndex] = origin.Content[fromIndex];
                origin.Content[fromIndex] = null;

                // for client
                target.UpdateSlot(toIndex, target.Content[toIndex].Source.Id, target.Content[toIndex].StackSize, false);
                origin.UpdateSlot(fromIndex, -1, 0, false);
                return 0;
            }

            return origin.Content[fromIndex].StackSize;
        }

        /// <summary>
        /// remove, destroy and obliterate an item from this &lt;see cref="Inventory"/&gt;.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual int DoErase(int index)
        {
            if (!IsInitialized) return -1;

            ItemStack data = Content[index];
            Content[index] = null;
            OnChanged?.Invoke(index);
            OnItemRemoved?.Invoke(data);
            UpdateSlot(index, -1, 0, false);
            return 0;
        }
        
        /// <summary>
        /// Finds item's matching the Id and removes an amount
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amountToRemove"></param>
        /// <returns></returns>
        public virtual int DoTake(ItemBase item, int amountToRemove)
        {
            if (!IsInitialized) return -1;

            int removedSoFar = 0;
            int[] contentIndexes = GetAllIndexOfItem(item.Id);
            foreach (int index in contentIndexes)
            {
                if (removedSoFar >= amountToRemove) break;
                int amountRemainingToRemove = amountToRemove - removedSoFar;

                if (Content[index].StackSize > amountRemainingToRemove)
                {
                    Content[index].StackSize -= amountRemainingToRemove;
                    OnChanged?.Invoke(index);
                    OnItemRemoved?.Invoke(new ItemStack(Content[index].Source, amountRemainingToRemove));
                    UpdateSlot(index, item.Id, Content[index].StackSize, false);
                    removedSoFar = amountToRemove;
                    break;
                }

                removedSoFar += Content[index].StackSize;
                DoErase(index);
            }

            return amountToRemove - removedSoFar;
        }

        /// <summary>
        /// Take an amount of items from a specific slot.
        /// </summary>
        /// <param name="inventoryIndex"></param>
        /// <param name="amountToRemove"></param>
        /// <returns></returns>
        public virtual int DoTake(int inventoryIndex, int amountToRemove)
        {
            if (!IsInitialized) return -1;

            if (amountToRemove == Content[inventoryIndex].StackSize)
            {
                DoErase(inventoryIndex);
                return 0;
            }

            if (amountToRemove < Content[inventoryIndex].StackSize)
            {
                int itemId = Content[inventoryIndex].Source.Id;

                Content[inventoryIndex].StackSize -= amountToRemove;
                OnChanged?.Invoke(inventoryIndex);
                OnItemRemoved?.Invoke(new ItemStack(Content[inventoryIndex].Source, amountToRemove));
                UpdateSlot(inventoryIndex, itemId, Content[inventoryIndex].StackSize, false);
                return 0;
            }
            
            return amountToRemove;
        }

        /// <summary>
        /// Count how many of the given item are in this inventory.
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public virtual int DoCount(int itemId)
        {
            if (!IsInitialized) return -1;

            return GetItemCount(itemId);
        }

        /// <summary>
        /// Discard, or 'drop' an item by spawning it's Prefab and removing it's data from the <see cref="Inventory"/> class.
        /// </summary>
        public virtual GameObjectItem DoDiscard(int index)
        {
            if (!IsInitialized) return null;

            // spawn new
            GameObjectItem result = CanvasInventory.SpawnWorldItem(Content[index].Source, InventoryHolder.MyTransform.position, Content[index].StackSize);

            // kill, flag and return.
            Content[index] = null;
            if (Application.isBatchMode) OnChanged?.Invoke(index);
            OnItemRemoved?.Invoke(result.Data);
            UpdateSlot(index, -1, 0, true);
            return result;
        }

        /// <summary>
        /// Split a stack in half. One empty slot is required.
        /// </summary>
        /// <param name="index">Index of this <see cref="Content"/> to split.</param>
        /// <returns>TRUE if successfully split and created a new item with half the stack size, FALSE if failure and no action was taken.</returns>
        public virtual bool DoSplit(int index)
        {
            if (!IsInitialized) return false;

            int openSlot = GetValidNullIndex(Content[index].Source.slotHolder);
            if (openSlot == -1)
            {
                Debug.LogError("Inventory has no space to split stack");
                return false;
            }

            // Figure out the new slot size. (Rounding doesn't matter with this approach)
            int originalAmountInSlot = Content[index].StackSize;
            int amountRemovedFromSlot = originalAmountInSlot / 2;
            int amountRemainingInSlot = originalAmountInSlot - amountRemovedFromSlot;

            // Try adding it.
            int oudex = DoAdd(new ItemStack(Content[index].Source, amountRemovedFromSlot), false);
            if (oudex == -1) return false;

            // Success, so subtract the new slot amount from the original stack.
            Content[index].StackSize = amountRemainingInSlot;
            
            UpdateSlot(index, Content[index].Source.Id, Content[index].StackSize, false);
            return true;
        }

        /// <summary>
        /// Use an item.
        /// </summary>
        /// <param name="index">Index of this <see cref="Content"/> to Use.</param>
        /// <returns>TRUE if successfully used. False otherwise.</returns>
        public virtual bool DoUse(int index)
        {
            if (!IsInitialized) return false;

            IUseableItem useable = (IUseableItem)Content[index].Source;
            if (useable == null) return false;

            useable.Use(InventoryHolder);

            if (Application.isBatchMode) OnChanged?.Invoke(index);
            return true;
        }

        
        public virtual void UpdateSlot(int index, int itemId, int amount, bool flagAddRemove)
        {
            if (amount == 0 || itemId == -1)
            {
                if (Content[index] != null && flagAddRemove)
                {
                    OnItemRemoved?.Invoke(Content[index]);
                }
                Content[index] = null;
                OnChanged?.Invoke(index);
                return;
            }

            // if not then it must be an increase, decrease or a new item.
            ItemStack newData = new ItemStack((ItemBase)DatabaseManager.Get(itemId), amount);
            ItemStack oldData = Content[index] == null || Content[index].Source == null
                ? null
                : new ItemStack(Content[index].Source, Content[index].StackSize);


            // is the slot currently holding something?
            if (oldData != null && oldData.Source != null)
            {
                // slot had something so it must be increase or decrease
                if (newData.StackSize < oldData.StackSize)
                {
                    Content[index] = newData;
                    Content[index].StackSize = amount;

                    int delta = oldData.StackSize - newData.StackSize;
                    if (flagAddRemove) OnItemRemoved?.Invoke(new ItemStack(oldData.Source, delta));
                }
                else
                {
                    Content[index] = newData;
                    Content[index].StackSize = amount;

                    int delta = newData.StackSize - oldData.StackSize;
                    if (flagAddRemove) OnItemAdded?.Invoke(new ItemStack(oldData.Source, delta));
                }
            }
            else
            {
                Content[index] = newData;
                Content[index].StackSize = amount;
                if (flagAddRemove) OnItemAdded?.Invoke(Content[index]);
            }
            OnChanged?.Invoke(index);
        }



        /// <summary>
        /// Looks for the first open slot with the matching restriction and returns it's index.
        /// </summary>
        protected virtual int GetValidNullIndex(SlotHolder slotHolder)
        {
            for (int i = 0; i < Content.Count; i++)
            {
                if (Content[i] == null && (Holders[i] == slotHolder || Holders[i] == null)) return i;
            }
            return -1;
        }

        /// <summary>
        /// Looks for an item in this inventory by the Item's DB Key.
        /// </summary>
        protected virtual int GetIndexOfItem(int itemId, int startIndex = 0)
        {
            for (int i = startIndex; i < Content.Count; i++)
            {
                if (Content[i] == null) continue;
                if (Content[i].Source.Id == itemId) return i;
            }
            return -1;
        }

        /// <summary>
        /// Find every index that contains the item with a given title.
        /// </summary>
        protected virtual int[] GetAllIndexOfItem(int itemId)
        {
            List<int> t = new List<int>();
            int curIndex = 0;
            while (curIndex < Content.Count)
            {
                if (Content[curIndex] != null && Content[curIndex].Source.Id == itemId) t.Add(curIndex);
                curIndex++;
            }

            return t.ToArray();
        }

        /// <summary>
        /// Find the first index of an item that is not full.
        /// </summary>
        protected virtual int GetIndexOfItemNotFull(int itemId, int startIndex = 0)
        {
            for (int i = startIndex; i < Content.Count; i++)
            {
                if (Content[i] == null || Content[i].Source == null) continue;
                if (Content[i].Source.Id == itemId && Content[i].StackSize < Content[i].Source.maxStackSize)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Shorthand math method to get the amount of items leftover after merging two stacks.
        /// </summary>
        /// <returns>How many should remain at the origin because they don't fit in the destination.</returns>
        protected virtual int GetMergeRemainder(int fromSize, int destinationSize, int maxSize)
        {
            int spaceAvailable = maxSize - destinationSize;
            return Mathf.Clamp(fromSize - spaceAvailable, 0, fromSize);
        }

        /// <summary>
        /// Find the total quantity of an item in this Inventory by scanning all of the slots.
        /// </summary>
        /// <returns>The total stack count of the item in the entire inventory.</returns>
        protected virtual int GetItemCount(int itemId)
        {
            int result = 0;
            foreach (ItemStack x in Content)
            {
                if (x == null) continue;
                if (x.Source.Id == itemId)
                {
                    result += x.StackSize;
                }
            }
            return result;
        }


        /// <summary>
        /// Local Read only. Check if any slot with the given slot holder is empty.
        /// </summary>
        /// <param name="slotHolder"></param>
        /// <returns>True if a slot of the slot holder type is empty and available.</returns>
        public virtual bool HasAnyAvaiableSlot(SlotHolder slotHolder)
        {
            return GetValidNullIndex(slotHolder) != -1;
        }

        /// <summary>
        /// Check if the given stack can completely fit into the inventory.
        /// </summary>
        /// <param name="stack">The content to merge in</param>
        /// <returns>True if the stack fits. False otherwise.</returns>
        public virtual bool StackCanFit(ItemStack stack)
        {
            if (!IsInitialized) return false;
            if (stack == null || stack.Source == null) return false;
            if (stack.StackSize == 0) stack.StackSize = 1;
            int hotStack = stack.StackSize;
            
            // If it can stack, "pour" it into the inventory.
            if (stack.Source.maxStackSize > 1)
            {
                int index = 0;
                while (index < Content.Count)
                {
                    // Try to find a suitable slot to merge into
                    int mergeIndex = GetIndexOfItemNotFull(stack.Source.Id, index);

                    // -1 means there isn't one. We can't merge. If hotStack flattens, we can fit it all.
                    if (mergeIndex == -1 || hotStack <= 0)
                    {
                        index = int.MaxValue;
                        continue;
                    }

                    // If found a matching slot, see how much doesn't fit.
                    int remain = GetMergeRemainder(
                        hotStack,
                        Content[mergeIndex].StackSize,
                        Content[mergeIndex].Source.maxStackSize);
                    
                    // Update rolling stack count
                    hotStack = remain;
                    index++;
                }
            }

            // If we emptied the stack, we know it fits.
            if (hotStack <= 0) return true;

            // Otherwise, try to find an open slot. If there isn't one, we can't make it fit.
            return GetValidNullIndex(stack.Source.slotHolder) != -1;
        }

        protected virtual void OnDestroy()
        {
            OnDestroyed?.Invoke(this);
        }
    }
}