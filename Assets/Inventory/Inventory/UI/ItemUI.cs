using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VoidexSoft.Inventory.Interactions;

namespace VoidexSoft.Inventory.UI
{
    /// <summary>
    /// Represents a single item in the UI. Takes control of the interaction with the item.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class ItemUI : Selectable, IInteractableUi, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler, ISubmitHandler
    {
        public Transform MyTransform => transform;
        public ItemInteraction[] Interactions
        {
            get
            {
                if (GetReferenceItemData() == null || GetReferenceItemData().ExtraInteractions == null) return Array.Empty<ItemInteraction>();
                return GetReferenceItemData().ExtraInteractions;
            }
        }

        [Header("Item UI References")]
        public GameObject slotOwnerObject;
        public Image myTypeImage;
        public Image myItemImage;
        public Image myHighlight;
        public Image myEngagedBorder;
        public GameObject stackSizeBox;
        public TMP_Text stackSizeText;

        private Color _oriBgColor;

        /// <summary>
        /// The UI hosting this Plug.
        /// </summary>
        public InventoryUI Ui { get; set; }
        /// <summary>
        /// A reference to the index of the item in the <see cref="Inventory"/> of the <see cref="InventoryUI"/> hosting this Item UI
        /// </summary>
        public int ReferenceInventoryIndex { get; set; }

        /// <summary>
        /// A reference to the <see cref="ItemBase"/> in the database.
        /// </summary>
        public virtual ItemBase GetReferenceItemData()
        {
            return Ui?.inventory?.Get(ReferenceInventoryIndex)?.Source;
        }

        protected override void Awake()
        {
            base.Awake();
            _oriBgColor = myTypeImage == null ? Color.white : myTypeImage.color;
            HighlightOff();
        }
        
        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (!CanvasInventory.CanDragItemUI
                || eventData.button != PointerEventData.InputButton.Left
                || Ui.inventory.Get(ReferenceInventoryIndex) == null 
                || Ui.inventory.Get(ReferenceInventoryIndex).StackSize == 0) return;

            // can't have a tooltip if you're dragging.
            //Todo Hide the tooltip here.
            
            InventoryUI.DragOrigin = this;

            CanvasInventory.OnMoveItemBegin?.Invoke(this);
        } 
        
        public virtual void OnDrag(PointerEventData eventData)
        {
            if (!CanvasInventory.CanDragItemUI 
                || eventData.button != PointerEventData.InputButton.Left) return;

        }
        
        public virtual void OnDrop(PointerEventData eventData)
        {
            if (!CanvasInventory.CanDragItemUI
                || eventData.button != PointerEventData.InputButton.Left) return;

            InventoryUI.DragDestination = this;
            CanvasInventory.OnMoveItemEnd?.Invoke(this);
        }
        
        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (!CanvasInventory.CanDragItemUI
                || eventData.button != PointerEventData.InputButton.Left) return;

            //Todo handle drag event here.
        }
        
        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            //Todo Show the tooltip here.
            HighlightOn();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            //Todo Hide the tooltip here
            HighlightOff();
        }
        
        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            //Todo Show the tooltip here.
            HighlightOn();
            CanvasInventory.OnSlotSelected?.Invoke(this);
        }
        
        public override void OnDeselect(BaseEventData eventData)
        {   
            base.OnDeselect(eventData);
            //Todo Hide the tooltip here.
            HighlightOff();
        }

        
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (InventoryUI.ClickedItem == null && GetReferenceItemData() == null) return;

            // if you right click, we just interact.
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                Interact();
                return;
            }

            Engage(eventData);
        }
        
        public virtual void OnSubmit(BaseEventData eventData)
        {
            Engage(eventData);
        }

        /// <summary>
        /// Entry point for generally trying to engage a slot.
        /// </summary>
        protected virtual void Engage(BaseEventData _)
        {
            // If you clicked the same object.
            if (InventoryUI.ClickedItem == this)
            {
                SetAsNotEngaged();
                CanvasInventory.OnMoveItemCancel?.Invoke(this);
            }

            // if you clicked this first.
            else if (InventoryUI.ClickedItem == null)
            {
                SetAsEngaged();
                CanvasInventory.OnMoveItemBegin?.Invoke(this);
            }
            
            else if (InventoryUI.ClickedItem != null && InventoryUI.ClickedItem != this)
            {
                InventoryUI.DragOrigin = InventoryUI.ClickedItem;
                InventoryUI.DragDestination = this;
                //todo handle drag event here.
                CanvasInventory.OnMoveItemEnd?.Invoke(this);
            }
        }



        public virtual void SetAsEngaged()
        {
            if (InventoryUI.ClickedItem != null) InventoryUI.ClickedItem.SetAsNotEngaged();
            InventoryUI.ClickedItem = this;
            myEngagedBorder.gameObject.SetActive(true);

            //Todo Show the tooltip here.
        }
        public virtual void SetAsNotEngaged()
        {
            if (InventoryUI.ClickedItem == this) InventoryUI.ClickedItem = null;
            myEngagedBorder.gameObject.SetActive(false);

            //Todo Hide the tooltip here.
        }



        /// <summary>
        /// Interact with this ItemUI
        /// </summary>
        public virtual void Interact(IInventoryHolder interactor = null)
        {
            //TODO: do something
        }
        public virtual void InteractionSelect()
        {
        }
        public virtual void InteractionDeselect()
        {            
        }


        /// <summary>
        /// Updates the visuals on the Item UI to represent the given ItemStack.
        /// </summary>
        public virtual void UpdateUi(ItemStack content, SlotHolder slotHolder)
        {
            string stack = string.Empty;
            Sprite itemImg = null;

            if (content != null)
            {
                if (content.Source != null) itemImg = content.Source.UiIcon;
                if (stackSizeBox != null) stackSizeBox.SetActive(content.StackSize > 1);
                stack = content.StackSize > 1 ? content.StackSize.ToString() : string.Empty;
            }

            // push the results to the ui
            if (myItemImage != null)
            {
                SetItemSprite(itemImg);
                myItemImage.color = itemImg == null ? Color.clear : Color.white;
            }

            if (myTypeImage != null)
            {
                SetTypeSprite(slotHolder == null ? null : slotHolder.icon);
                myTypeImage.color = slotHolder == null || itemImg != null ? Color.clear : _oriBgColor;
            }

            SetStackSizeText(stack);
        }

        public virtual void SetItemSprite(Sprite sprite)
        {
            if (myItemImage == null) return;

            myItemImage.sprite = sprite;
            myItemImage.color = sprite == null ? Color.clear : Color.white;
        }        
        public virtual Sprite GetItemSprite()
        {
            return myItemImage == null ? null : myItemImage.sprite;
        }

        public virtual void SetTypeSprite(Sprite sprite)
        {
            if (myTypeImage != null) myTypeImage.sprite = sprite;
        }
        public virtual Sprite GetTypeSprite()
        {
            return myTypeImage == null ? null : myTypeImage.sprite;
        }

        public virtual void SetStackSizeText(string text)
        {
            if (stackSizeText == null) return;

            if (text == string.Empty) stackSizeBox.SetActive(text != string.Empty);
            stackSizeText.text = text;
        }
        public virtual string GetStackSizeText()
        {
            return stackSizeText == null ? string.Empty : stackSizeText.text;
        }

        public virtual void HighlightOn()
        {
            myHighlight.gameObject.SetActive(true);
        }
        public virtual void HighlightOff()
        {
            myHighlight.gameObject.SetActive(false);
        }
    }
}