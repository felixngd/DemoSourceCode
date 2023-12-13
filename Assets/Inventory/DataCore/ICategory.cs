using System;
using System.Collections.Generic;

namespace VoidexSoft.Inventory.DataCore
{
    public interface ICategory
    {
        /// <summary>
        /// The title of the category.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// The type of the source data.
        /// </summary>
        public Type SourceType { get; set; }
        /// <summary>
        /// The content of the category.
        /// </summary>
        public List<DataEntity> Content { get; set; }
        /// <summary>
        /// Add an entity to the category.
        /// </summary>
        /// <param name="entity"></param>
        public void AddEntity(DataEntity entity);
        /// <summary>
        /// Remove an entity from the category.
        /// </summary>
        /// <param name="key"></param>
        public void RemoveEntity(int key);
        /// <summary>
        /// Cleanup the category.
        /// </summary>
        public void Cleanup();
    }
}