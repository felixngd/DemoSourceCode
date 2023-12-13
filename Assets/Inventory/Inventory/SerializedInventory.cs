using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace VoidexSoft.Inventory
{
    /// <summary>
    /// For serializing and deserializing an <see cref="Inventory"/> and its content.
    /// </summary>
    [Serializable]
    public partial class SerializedInventory
    {  
        /// <summary>
        /// The Vault Index ID of the Inventory Configuration.
        /// </summary>
        public int dbId;
        /// <summary>
        /// A list of IDs to identify items in each slot.
        /// </summary>
        public List<int> itemIds;
        /// <summary>
        /// A list of int's to indicate the stack size in each slot.
        /// </summary>
        public List<int> itemStackCounts;

        public SerializedInventory(Inventory source, List<ItemStack> content)
        {
            dbId = source.Container.Id;
            itemIds = new List<int>();
            itemStackCounts = new List<int>();

            foreach (ItemStack t in content)
            {
                itemIds.Add(t != null ? t.Source.Id : -1);
                itemStackCounts.Add(t != null ? t.StackSize : 0);
            }
        }
        
        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
        
        public static SerializedInventory FromJson(string json)
        {
            return JsonConvert.DeserializeObject<SerializedInventory>(json);
        }
    }
}