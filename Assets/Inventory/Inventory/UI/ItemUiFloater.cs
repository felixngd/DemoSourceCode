using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace VoidexSoft.Inventory.UI
{
    /// <summary>
    /// The 'floating' thing that follows the mouse cursor when you are dragging an item around in the Canvas.
    /// </summary>
    public class ItemUiFloater : MonoBehaviour
    {
        public TMP_Text stackSizeText;
        public Image myImage;

        public virtual void Set(Sprite sprite, string text)
        {
            myImage.sprite = sprite;
            stackSizeText.text = text;
        }
    }
}