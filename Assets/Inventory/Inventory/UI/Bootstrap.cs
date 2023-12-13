using System;
using UnityEngine;
using UnityEngine.UI;

namespace VoidexSoft.Inventory.UI
{
    public class Bootstrap : MonoBehaviour
    {
        public GameObject itemSlotPrefab;
        public GameObject itemRuntime;
        public Canvas canvas;
        public GameObject inventoryTemplate;


        private void Awake()
        {
            CanvasInventory.Initialize(canvas, itemSlotPrefab, itemRuntime, inventoryTemplate, true);
        }
    }
}