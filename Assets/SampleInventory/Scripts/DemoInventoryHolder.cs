using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using VoidexSoft.Inventory;
using VoidexSoft.Inventory.UI;

namespace SampleInventory.Scripts
{
    public class DemoInventoryHolder : MonoBehaviour, IInventoryHolder
    {
        [AssetSelector()]
        public VoidexSoft.Inventory.Items.ItemBag starterBag;
        
        public Button openInventoryButton;
        public GameObject inventoryPanel;

        public Inventory Inventory
        {
            get => _inventory;
            set => _inventory = value;
        }
        [SerializeField] private Inventory _inventory;

        public Transform MyTransform => transform;
        
        private void Start()
        {
            Inventory.Initialize(this, true);
            
            for (int i = 0; i < starterBag.items.Length; i++)
            {
                if (starterBag.items[i] == null) continue;
                Inventory.DoAdd(new ItemStack(starterBag.items[i], starterBag.amounts[i]));
            }
            
            
            openInventoryButton.onClick.AddListener(OpenInventory);
        }
        
        public void OpenInventory()
        {
            CanvasInventory.OnInventoryHolderSpawn.Invoke(this);
            inventoryPanel.SetActive(true);
        }
    }
}