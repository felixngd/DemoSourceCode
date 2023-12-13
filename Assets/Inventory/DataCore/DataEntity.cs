using Sirenix.OdinInspector;
using UnityEngine;

namespace VoidexSoft.Inventory.DataCore
{
    //TODO Create Editor UI to manage the DataEntity
    public abstract class DataEntity : ScriptableObject
    {
        [SerializeField, InlineButton("GenerateId", "Generate Id")]
        private int _id;
        
        [SerializeField] 
        private string _title = string.Empty;
        public string Title { get => _title; set => _title = value; }

        [TextArea]
        [SerializeField] 
        private string _description;
        public string Description { get => _description; set => _description = value; }
        
        public Sprite GetDataIcon => GetIcon();

        protected virtual void Reset()
        {
            Title = $"Untitled-{System.DateTime.Now.TimeOfDay.TotalMilliseconds}";
            Description = string.Empty;
        }

        public int Id
        {
            get => _id;
            set => _id = value;
        }
        /// <summary>
        /// Get the icon to use in the Editor for this Entity.
        /// </summary>
        /// <returns></returns>
        protected virtual Sprite GetIcon()
        {
            return null;
        }
        
        private void GenerateId()
        {
            _id = RandomId.Generate();
        }
    }
}