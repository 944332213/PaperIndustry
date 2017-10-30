using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Lvshi.PaperProducts.DataAccessLayer.DataBase;
using Lvshi.PaperProducts.Model.Recombination;
using Lvshi.PaperProducts.Model.Table;

namespace Lvshi.PaperProducts.BusinessLogicLayer.Implement
{
    /// <summary>
    /// 栏目类型业务逻辑
    /// </summary>
    public class ColumnTypeLogic
    {
        #region 静态化

        private static readonly List<ModelType> _sList;
        private static readonly object Lock = new object();

        internal static bool IsEstablish;

        protected static ColumnTypeDao Dao => new ColumnTypeDao();

        static ColumnTypeLogic()
        {
            _sList = new List<ModelType>();
        }

        internal static void SetList()
        {
            if (_sList.Count > 0)
            {
                return;
            }
            lock (Lock)
            {
                if (_sList.Count > 0)
                {
                    return;
                }
                var dataTable = Dao.SelectAll();
                if (dataTable == null || dataTable.Rows.Count <= 0)
                {
                    return;
                }
                _sList.AddRange(dataTable.ToEntityList(dr => new ModelType
                {
                    Parent = new ModelType { Id = DataConvert.TryParse<int>(dr["ParentId"].ToString()) ?? 0, },
                }, TypeData.GetPropertys<ModelType>(), new StringPermitImplement(false, "Parent")));
                _sList.Remove(null);
                IsEstablish = false;
            }
        }

        /// <summary>
        /// 确定关系
        /// </summary>
        internal static void EstablishRelationship()
        {
            if (_sList.Count <= 0)
            {
                SetList();
            }
            if (_sList.Count <= 0 || IsEstablish)
            {
                return;
            }
            lock (Lock)
            {
                if (_sList.Count <= 0 || IsEstablish)
                {
                    return;
                }
                Func<ModelType, ModelType> fnEstablish = m =>
                {
                    if (m.Parent.Id > 0)
                    {
                        m.Parent = _sList.FirstOrDefault(item => item.Id == m.Parent.Id);
                    }
                    return m;
                };
                foreach (var m in _sList)
                {
                    fnEstablish(m);
                }
                IsEstablish = true;
            }
        }

        /// <summary>
        /// 增删改成功需调用该方法
        /// </summary>
        protected static void Initialization()
        {
            lock (Lock)
            {
                _sList.Clear();
            }
        }

        public static List<ModelType> List
        {
            get
            {
                if (_sList.Count <= 0)
                {
                    SetList();
                    EstablishRelationship();
                }
                return _sList;
            }
        }

        #endregion


        public static ModelType GetModel(int id)
        {
            if (id <= 0)
            {
                return null;
            }
            return List.FirstOrDefault(item => item.Status == EnumStatus.Value.Normal && item.Id == id);
        }

        public static ModelType GetModel(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }
            return
                List.FirstOrDefault(
                    item =>
                        item.Status == EnumStatus.Value.Normal &&
                        (key.Equals(item.EnName, StringComparison.CurrentCultureIgnoreCase) ||
                         key.Equals(item.Name, StringComparison.CurrentCultureIgnoreCase)));
        }

        /// <summary>
        /// 获取父级
        /// </summary>
        /// <param name="id"></param>
        /// <returns>若已为顶级则返回自身</returns>
        public static ModelType GetParent(int id)
        {
            var model = GetModel(id);
            if (model == null)
            {
                return null;
            }
            if (model.Level == EnumColumnTypeLevel.Value.One)
            {
                return model;
            }
            if (model.Parent.Id <= 0)
            {
                return null;
            }
            return GetModel(model.Parent.Id);
        }

        /// <summary> 
        /// 获取指定层级父级
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parentLevel"></param>
        /// <returns>
        /// 若已为顶级则返回自身
        /// 若自身已高于指定层级则返回null
        /// </returns>
        public static ModelType GetParent(int id, EnumColumnTypeLevel.Value parentLevel)
        {
            var model = GetModel(id);
            if (model == null || model.Level < parentLevel)
            {
                return null;
            }
            if (model.Level == parentLevel)
            {
                return model;
            }
            if (model.Parent.Id <= 0)
            {
                return null;
            }
            return GetParent(model.Parent.Id, parentLevel);
        }

        /// <summary> 
        /// 获取顶级层级父级
        /// </summary>
        /// <param name="id"></param>
        /// <returns>若已为顶级则返回自身</returns>
        public static ModelType GetTopParent(int id)
        {
            return GetParent(id, EnumColumnTypeLevel.Value.One);
        }

        public static List<ModelType> GetListParent(int parentId = 0)
        {
            return
                List.Where(
                    item =>
                        item.Status == EnumStatus.Value.Normal &&
                        (parentId <= 0 && item.Level == EnumColumnTypeLevel.Value.One ||
                         parentId > 0 && item.Parent.Id == parentId)).OrderByDescending(item => item.Rank).ToList();
        }

        public static List<ModelType> GetListNavigation()
        {
            return List.Where(item => item.Status == EnumStatus.Value.Normal && (item.Character & EnumCharacter.Value.Recommend) != 0).ToList();
        }

        public static List<ModelType> GetListIndex()
        {
            return List.Where(item => item.Status == EnumStatus.Value.Normal && (item.Character & EnumCharacter.Value.Index) != 0).ToList();
        }

    }
}
