using UnityEngine;
using UnityEngine.Serialization;
using VoidexSoft.Inventory.DataCore;
using VoidexSoft.Inventory.Editor;
using VoidexSoft.Inventory.Interactions;
using ItemRarityColors = VoidexSoft.Inventory.Utilities.ItemRarityColors;

namespace VoidexSoft.Inventory
{
    public abstract class ItemBase : DataEntity
    {
        public ItemRarity itemRarity;
         public SlotHolder slotHolder;
        
        [SerializeField, IconPreview]
        private Sprite _icon;
        public Sprite UiIcon { get => _icon; set => _icon = value; }

        public GameObject prefab;
        public int maxStackSize;
        public int value;

        public ItemInteraction[] ExtraInteractions;


        protected override void Reset()
        {
            base.Reset();
            Description = "Lorum ipsum.";

            itemRarity = ItemRarity.Common;
            slotHolder = null;
            _icon = null;
            prefab = null;
            maxStackSize = 999;
            value = 1;
        }

        public virtual Color GetRarityColor()
        {
            return ItemRarityColors.RarityColors[(int) itemRarity];
        }
        public virtual string GetTitle()
        {
            string x = ColorUtility.ToHtmlStringRGB(GetRarityColor());
            return $"<color=#{x}>{Title}</color>";
        }
        public virtual string GetShortDescription()
        {
            return Description;
        }
        public virtual string GetLongDescription()
        {
            string restriction = slotHolder == null ? "<color=white>Generic Item</color>" : $"<color=white>{slotHolder.Title}</color>";
            return $"{GetShortDescription()}\n" +
                   $"{restriction}\n" +
                   $"<color=white> ${value}</color>";
        }
        protected override Sprite GetIcon()
        {
            return UiIcon;
        }
    }
}