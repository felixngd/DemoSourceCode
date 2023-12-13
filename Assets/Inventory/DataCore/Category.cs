using System;
using System.Collections.Generic;
using UnityEngine;

namespace VoidexSoft.Inventory.DataCore
{
    [Serializable]
    public class Category : ICategory
    { 
        public string Title
        {
            get => SourceType.Name;
            set => Debug.LogWarning("Cannot set Title to this category.");
        }
        public Type SourceType
        {
            get => Type.GetType(_typeName);
            set => _typeName = value.AssemblyQualifiedName;
        }
        [SerializeField] [HideInInspector]
        private string _typeName = typeof(DataEntity).AssemblyQualifiedName;
        
        public List<DataEntity> Content
        {
            get => _data;
            set => _data = value;
        }
        [SerializeField]
        private List<DataEntity> _data = new List<DataEntity>();

        public Category(Type t)
        {
            SourceType = t;
        }

        public void AddEntity(DataEntity data)
        {
            Content.Add(data);
        }
        public void RemoveEntity(int key)
        {
            for (int i = 0; i < Content.Count; i++)
            {
                if (Content[i].Id == key)
                {
                    Content.RemoveAt(i);
                }
            }
        }
        public void Cleanup()
        {
            Content.RemoveAll(x => x == null);
        }
    }
}