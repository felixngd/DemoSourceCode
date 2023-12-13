using UnityEngine;

namespace VoidexSoft.Inventory.Editor
{
    [System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.GenericParameter | System.AttributeTargets.Property)]
    public class IconPreviewAttribute : PropertyAttribute
    {
        public IconPreviewAttribute() { }
    }
}