using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using System.Linq;
#endif
using UnityEngine;

namespace VoidexSoft.Inventory.DataCore
{
    [CreateAssetMenu(fileName = "New Database", menuName = "VoidexSoft/Database", order = -9999)]
    public class Database : ScriptableObject//, ISerializationCallbackReceiver
    {
        public const string DATABASE_NAME = "Database";
        public Dictionary<int, DataEntity> Data = new Dictionary<int, DataEntity>();

        [SerializeField] private List<int> _dataKeys = new List<int>();
        [SerializeField] private List<DataEntity> _dataVals = new List<DataEntity>();

        [SerializeField] private List<Category> _staticCategories = new List<Category>();

        public virtual void OnBeforeSerialize()
        {
            _dataKeys.Clear();
            _dataVals.Clear();
            foreach (KeyValuePair<int, DataEntity> d in Data)
            {
                _dataKeys.Add(d.Key);
                _dataVals.Add(d.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            Data = new Dictionary<int, DataEntity>();
            for (int i = 0; i < _dataKeys.Count; i++)
            {
                Data.Add(_dataKeys[i], _dataVals[i]);
            }
        }

        public DataEntity Get(int id)
        {
            if (Data.TryGetValue(id, out DataEntity result))
            {
                return result;
            }

            return null;
        }

        public List<T> GetAll<T>()
        {
            List<T> result = new List<T>();
            foreach (KeyValuePair<int, DataEntity> d in Data)
            {
                if (d.Value is T t)
                {
                    result.Add(t);
                }
            }

            return result;
        }

        public virtual void Add(DataEntity data, bool generateNewId = true)
        {
            if (data == null) return;

            int id = generateNewId ? RandomId.Generate() : data.Id;
            if (Data.ContainsKey(id)) return;
            data.Id = id;
            Data.Add(id, data);

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(data);
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        public virtual void Remove(DataEntity data)
        {
            if (Data.ContainsKey(data.Id))
            {
                Data.Remove(data.Id);
            }
        }

        public virtual void Remove(int id)
        {
            if (Data.ContainsKey(id))
            {
                Data.Remove(id);
            }
        }

        public virtual Category GetCategory<T>() where T : DataEntity
        {
            foreach (Category x in _staticCategories)
            {
                if (x.SourceType == typeof(T))
                {
                    return x;
                }
            }

            return null;
        }

        public virtual Category GetCategory(Type t)
        {
            foreach (Category x in _staticCategories)
            {
                if (x.SourceType == t)
                {
                    return x;
                }
            }

            return null;
        }


        public virtual List<Category> GetAllCategories()
        {
            return _staticCategories;
        }


        public virtual void SetStaticGroup(Category identifier)
        {
            List<Category> categoriesToRemove = new List<Category>();
            foreach (Category x in _staticCategories)
            {
                if (x.SourceType == identifier.SourceType)
                {
                    categoriesToRemove.Add(x);
                }
            }

            foreach (Category categoryToRemove in categoriesToRemove)
            {
                _staticCategories.Remove(categoryToRemove);
            }

            _staticCategories.Add(identifier);
        }

        public virtual void ClearData()
        {
            Data.Clear();
            _dataKeys.Clear();
            _dataVals.Clear();
        }

        public virtual void ClearStaticCategories()
        {
            _staticCategories.Clear();
        }

        public virtual int GetEntityCount()
        {
            return Data.Count;
        }

        #region Editor

#if UNITY_EDITOR
        [Button]
        public void ReloadAsset()
        {
            //load all scriptable assets of type DataEntity
            DataEntity[] assets = UnityEditor.AssetDatabase.FindAssets("t:VoidexSoft.Inventory.DataCore.DataEntity")
                .Select(x => UnityEditor.AssetDatabase.GUIDToAssetPath(x))
                .Select(x => UnityEditor.AssetDatabase.LoadAssetAtPath<DataEntity>(x))
                .ToArray();
            //fill to keys and values
            Data.Clear();
            _dataKeys.Clear();
            _dataVals.Clear();
            foreach (DataEntity asset in assets)
            {
                Data.TryAdd(asset.Id, asset);
                _dataKeys.Add(asset.Id);
                _dataVals.Add(asset);
            }
            //save
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
            
        }
#endif

        #endregion
    }
}