using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Sql;
using System.Data.Sql.Table;
using System.Data.SqlClient;
using System.Linq;
using Nsfttz.DataAccessLayer.Client.SqlServer;

namespace Nsfttz.DataAccessLayer.DataTable.SqlServer.Base.Base
{
    /// <summary> 
    /// 基本数据访问
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseRepository<T> where T : class, new()
    {
        private DataBaseTypeData<T> _mDataBaseTypeData;
        private StructuredQueryLanguage _mSql;
        private SqlParameter[] _mIdSqlParameters;

        /// <summary>
        /// 链接字符串
        /// </summary>
        protected abstract string ConnectionString { get; }

        /// <summary>
        /// 数据表名
        /// </summary>
        public abstract string TableName { get; }

        /// <summary>
        /// 数据库类型数据
        /// </summary>
        protected internal DataBaseTypeData<T> DataBaseTypeData
        {
            get { return _mDataBaseTypeData ?? (_mDataBaseTypeData = new DataBaseTypeData<T>()); }
        }

        protected internal StructuredQueryLanguage Sql
        {
            get { return _mSql ?? (_mSql = new StructuredQueryLanguage(ConnectionString)); }
        }

        protected SqlParameter[] IdSqlParameters
        {
            get
            {
                if (_mIdSqlParameters == null)
                {
                    _mIdSqlParameters = DataBaseTypeData.GetSqlParameters(null, TableFieldLable.PrimaryKey);
                    if (_mIdSqlParameters == null || _mIdSqlParameters.Length <= 0)
                    {
                        _mIdSqlParameters = new SqlParameter[0];
                        throw new Exception("未找到模型主键");
                    }
                }
                if (_mIdSqlParameters.Length <= 0)
                {
                    throw new Exception("未找到模型主键");
                }
                return _mIdSqlParameters;
            }
        }

        /// <summary>
        /// 筛选所有
        /// </summary>
        /// <returns></returns>
        public System.Data.DataTable SelectAll()
        {
            DataBaseTypeData.Reset();
            Sql.AddText("SELECT");
            Sql.AddText(string.Join(",",
                DataBaseTypeData.GetSqlParameters().Select(item => string.Format("[{0}]", item.ParameterName))));
            Sql.AddText("FROM");
            Sql.AddText(TableName);
            Sql.AddText("WITH(NOLOCK)");
            return Sql.ExecuteTable();
        }

        /// <summary>
        /// 筛选列表
        /// 所有
        /// </summary>
        /// <returns></returns>
        public List<T> SelectListAll()
        {
            using (var dataTable = SelectAll())
            {
                return dataTable.ToEntityList<T>();
            }
        }

        /// <summary>
        /// 筛选一条
        /// 通过主键
        /// 采用顺序赋值
        /// </summary>
        /// <param name="id"></param>
        /// <param name="otherIds"></param>
        /// <returns></returns>
        public virtual T SelectOneById(dynamic id, params dynamic[] otherIds)
        {
            DataBaseTypeData.Reset();
            otherIds = otherIds == null || otherIds.Length <= 0 ? new[] { id } : new[] { id }.Concat(otherIds).ToArray();
            if (otherIds.Length != IdSqlParameters.Length)
            {
                throw new Exception("参数数量与主键数不一致");
            }
            for (int i = 0; i < IdSqlParameters.Length; i++)
            {
                IdSqlParameters[i].Value = otherIds[i];
            }
            Sql.AddText("SELECT");
            Sql.AddText(string.Join(",", DataBaseTypeData.GetSqlParameters().Select(item => string.Format("[{0}]", item.ParameterName))));
            Sql.AddText("FROM");
            Sql.AddText(TableName);
            Sql.AddText("WITH(NOLOCK)");
            Sql.AddText("WHERE");
            Sql.AddText(string.Join(" AND ",
                IdSqlParameters.Select(idSqlParameter => string.Format("[{0}] = @{0}", idSqlParameter.ParameterName))));
            Sql.AddParameter(IdSqlParameters);
            return Sql.ExecuteTableToEntity<T>();
        }

        /// <summary>
        /// 插入一条
        /// </summary>
        /// <param name="model">数据</param>
        /// <param name="setPermitImplement">需限制更改的字段，默认不限制，主键始终限制</param>
        /// <returns></returns>
        public virtual bool InsertOne(T model, PermitImplement<string> setPermitImplement = null)
        {
            DataBaseTypeData.Reset().Model = model;
            Sql.AddText("INSERT INTO");
            Sql.AddText(TableName);
            var valueSqlParameters = DataBaseTypeData.GetSqlParameters(setPermitImplement, tfl => (tfl & TableFieldLable.Identity) == 0);
            Sql.AddText("(");
            Sql.AddText(string.Join(",", valueSqlParameters.Select(item => string.Format("[{0}]", item.ParameterName))));
            Sql.AddText(")VALUES(");
            Sql.AddText(string.Join(",", valueSqlParameters.Select(item => string.Format("@{0}", item.ParameterName))));
            Sql.AddText(")");
            Sql.AddParameter(valueSqlParameters);
            return Sql.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// 修改一条
        /// 通过主键
        /// </summary>
        /// <param name="model">数据</param>
        /// <param name="setPermitImplement">需限制更改的字段，默认不限制，主键与标识列始终限制</param>
        /// <param name="additionSet">附加更改，其键将覆盖对应字段的默认更改</param>
        /// <returns></returns>
        public virtual bool UpdateOneById(T model, PermitImplement<string> setPermitImplement = null, Dictionary<string, string> additionSet = null)
        {
            DataBaseTypeData.Reset().Model = model;
            if (setPermitImplement == null)
            {
                setPermitImplement = new StringPermitImplement().SetComparer(StringComparison.CurrentCultureIgnoreCase);
            }
            setPermitImplement.SetPermit(false, IdSqlParameters.Select(item => item.ParameterName));
            setPermitImplement.SetPermit(false,
                DataBaseTypeData.GetSqlParameters(TableFieldLable.Identity).Select(item => item.ParameterName));
            Sql.AddText("UPDATE");
            Sql.AddText(TableName);
            Sql.AddText("SET");
            var sets = new List<string>();
            if (additionSet != null)
            {
                sets.AddRange(additionSet.Select(item => string.Format("[{0}] = {1}", item.Key, item.Value)));
                setPermitImplement.SetPermit(false, additionSet.Keys);
            }
            var setSqlParameters = DataBaseTypeData.GetSqlParameters(setPermitImplement);
            Sql.AddText(string.Join(",",
                sets.Concat(setSqlParameters.Select(item => string.Format("[{0}] = @{0}", item.ParameterName)))));
            Sql.AddParameter(setSqlParameters);
            Sql.AddText("WHERE");
            Sql.AddText(string.Join(" AND ",
                IdSqlParameters.Select(item => string.Format("[{0}] = @{0}", item.ParameterName))));
            Sql.AddParameter(IdSqlParameters);
            return Sql.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// 修改列表
        /// 通过相等条件
        /// </summary>
        /// <param name="model">数据</param>
        /// <param name="equestWherePermitImplement"></param>
        /// <param name="setPermitImplement">需限制更改的字段，默认不限制，主键与标识列始终限制</param>
        /// <param name="additionSet">附加更改，其键将覆盖对应字段的默认更改</param>
        /// <returns></returns>
        public virtual bool UpdateListByEquestWhere(T model, PermitImplement<string> equestWherePermitImplement, PermitImplement<string> setPermitImplement = null, Dictionary<string, string> additionSet = null)
        {
            DataBaseTypeData.Reset().Model = model;
            if (setPermitImplement == null)
            {
                setPermitImplement = new StringPermitImplement().SetComparer(StringComparison.CurrentCultureIgnoreCase);
            }
            setPermitImplement.SetPermit(false, IdSqlParameters.Select(item => item.ParameterName));
            setPermitImplement.SetPermit(false,
                DataBaseTypeData.GetSqlParameters(TableFieldLable.Identity).Select(item => item.ParameterName));
            Sql.AddText("UPDATE");
            Sql.AddText(TableName);
            Sql.AddText("SET");
            var sets = new List<string>();
            if (additionSet != null)
            {
                sets.AddRange(additionSet.Select(item => string.Format("[{0}] = {1}", item.Key, item.Value)));
                setPermitImplement.SetPermit(false, additionSet.Keys);
            }
            var setSqlParameters = DataBaseTypeData.GetSqlParameters(setPermitImplement);
            Sql.AddText(string.Join(",",
                sets.Concat(setSqlParameters.Select(item => string.Format("[{0}] = @{0}", item.ParameterName)))));
            Sql.AddText("WHERE");
            var whereSqlParameters = DataBaseTypeData.GetSqlParameters(equestWherePermitImplement);
            if (whereSqlParameters == null || whereSqlParameters.Length <= 0)
            {
                throw new Exception("未找到条件");
            }
            Sql.AddText(string.Join(" AND ",
                whereSqlParameters.Select(item => string.Format("[{0}] = @{0}", item.ParameterName))));
            Sql.AddParameter(whereSqlParameters.Concat(setSqlParameters).ToArray());
            return Sql.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// 添加或修改一条
        /// 通过主键
        /// </summary>
        /// <param name="model">数据</param>
        /// <param name="setPermitImplement">需限制更改的字段，默认不限制，主键与标识列始终限制</param>
        /// <param name="additionSet">附加更改，其键将覆盖对应字段的默认更改；插入时，此项不生效</param>
        /// <returns></returns>
        public virtual bool InsertOrUpdateOneById(T model, PermitImplement<string> setPermitImplement = null, Dictionary<string, string> additionSet = null)
        {
            DataBaseTypeData.Reset().Model = model;
            Sql.AddText(string.Format("IF(EXISTS("));
            Sql.AddText("SELECT 0 FROM");
            Sql.AddText(TableName);
            Sql.AddText("WHERE");
            Sql.AddText(string.Join(" AND ",
                IdSqlParameters.Select(item => string.Format("[{0}] = @{0}", item.ParameterName))));
            Sql.AddText("))BEGIN");
            if (setPermitImplement == null)
            {
                setPermitImplement = new StringPermitImplement().SetComparer(StringComparison.CurrentCultureIgnoreCase);
            }
            setPermitImplement.SetPermit(false,
                DataBaseTypeData.GetSqlParameters(TableFieldLable.Identity).Select(item => item.ParameterName));
            var valueSqlParameters = DataBaseTypeData.GetSqlParameters(setPermitImplement, tfl => (tfl & TableFieldLable.Identity) == 0);
            Sql.AddText("UPDATE");
            Sql.AddText(TableName);
            Sql.AddText("SET");
            var sets = new List<string>();
            setPermitImplement.SetPermit(false, IdSqlParameters.Select(item => item.ParameterName));
            setPermitImplement.SetPermit(false,
                DataBaseTypeData.GetSqlParameters(TableFieldLable.Identity).Select(item => item.ParameterName));
            if (additionSet != null)
            {
                sets.AddRange(additionSet.Select(item => string.Format("[{0}] = {1}", item.Key, item.Value)));
                setPermitImplement.SetPermit(false, additionSet.Keys);
            }
            var setSqlParameters = DataBaseTypeData.GetSqlParameters(setPermitImplement);
            Sql.AddText(string.Join(",",
                sets.Concat(setSqlParameters.Select(item => string.Format("[{0}] = @{0}", item.ParameterName)))));
            Sql.AddText("WHERE");
            Sql.AddText(string.Join(" AND ",
                IdSqlParameters.Select(item => string.Format("[{0}] = @{0}", item.ParameterName))));
            Sql.AddText("END ELSE BEGIN");
            Sql.AddText("INSERT INTO");
            Sql.AddText(TableName);
            Sql.AddText("(");
            Sql.AddText(string.Join(",", valueSqlParameters.Select(item => string.Format("[{0}]", item.ParameterName))));
            Sql.AddText(")VALUES(");
            Sql.AddText(string.Join(",", valueSqlParameters.Select(item => string.Format("@{0}", item.ParameterName))));
            Sql.AddText(")");
            Sql.AddText("END");
            Sql.AddParameter(
                IdSqlParameters.Concat(setSqlParameters)
                    .Concat(valueSqlParameters)
                    .Distinct(item => item.ParameterName)
                    .ToArray());
            return Sql.ExecuteNonQuery() > 0;
        }

    }

    /// <summary>
    /// 基本标识列数据访问
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseIdentityRepository<T> : BaseRepository<T> where T : class, new()
    {
        private SqlParameter[] _mIdentitySqlParameter;

        protected SqlParameter IdentitySqlParameter
        {
            get
            {
                if (_mIdentitySqlParameter == null)
                {
                    _mIdentitySqlParameter = DataBaseTypeData.GetSqlParameters(null, TableFieldLable.Identity);
                    if (_mIdentitySqlParameter == null || _mIdentitySqlParameter.Length != 1)
                    {
                        _mIdentitySqlParameter = new SqlParameter[0];
                        throw new Exception("未找到模型标识列或标识列过多");
                    }
                }
                if (_mIdentitySqlParameter.Length <= 0)
                {
                    throw new Exception("未找到模型标识列或标识列过多");
                }
                return _mIdentitySqlParameter.First();
            }
        }

        /// <summary>
        /// 筛选一条
        /// 通过标识列
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public T SelectOneByIdentity(long identity)
        {
            DataBaseTypeData.Reset();
            IdentitySqlParameter.Value = identity;
            Sql.AddText("SELECT");
            Sql.AddText(string.Join(",", DataBaseTypeData.GetSqlParameters().Select(item => string.Format("[{0}]", item.ParameterName))));
            Sql.AddText("FROM");
            Sql.AddText(TableName);
            Sql.AddText("WITH(NOLOCK)");
            Sql.AddText("WHERE");
            Sql.AddText(string.Format("[{0}] = @{0}", IdentitySqlParameter.ParameterName));
            Sql.AddParameter(IdentitySqlParameter);
            return Sql.ExecuteTableToEntity<T>();
        }

        /// <summary>
        /// 筛选一条
        /// 通过标识列
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public T SelectOneByIdentity(int identity)
        {
            return SelectOneByIdentity((long)identity);
        }

        /// <summary>
        /// 插入一条
        /// </summary>
        /// <param name="model">数据</param>
        /// <param name="setPermitImplement">需限制更改的字段，默认不限制，主键始终限制</param>
        /// <returns></returns>
        public new int InsertOne(T model, PermitImplement<string> setPermitImplement = null)
        {
            DataBaseTypeData.Reset().Model = model;
            Sql.AddText("INSERT INTO");
            Sql.AddText(TableName);
            var valueSqlParameters = DataBaseTypeData.GetSqlParameters(setPermitImplement, tfl => (tfl & TableFieldLable.Identity) == 0);
            Sql.AddText("(");
            Sql.AddText(string.Join(",", valueSqlParameters.Select(item => string.Format("[{0}]", item.ParameterName))));
            Sql.AddText(")VALUES(");
            Sql.AddText(string.Join(",", valueSqlParameters.Select(item => string.Format("@{0}", item.ParameterName))));
            Sql.AddText(")");
            Sql.AddParameter(valueSqlParameters);
            Sql.AddText("SELECT SCOPE_IDENTITY()");
            return Sql.ExecuteScalarTryParse<int>() ?? 0;
        }

        /// <summary>
        /// 修改一条
        /// 通过标识列
        /// </summary>
        /// <param name="model">数据</param>
        /// <param name="setPermitImplement">需限制更改的字段，默认不限制，标识列始终限制</param>
        /// <param name="additionSet">附加更改，其键将覆盖对应字段的默认更改</param>
        /// <returns></returns>
        public bool UpdateOneByIdentity(T model, PermitImplement<string> setPermitImplement = null, Dictionary<string, string> additionSet = null)
        {
            DataBaseTypeData.Reset().Model = model;
            if (setPermitImplement == null)
            {
                setPermitImplement = new StringPermitImplement().SetComparer(StringComparison.CurrentCultureIgnoreCase);
            }
            setPermitImplement.SetPermit(false, IdentitySqlParameter.ParameterName);
            Sql.AddText("UPDATE");
            Sql.AddText(TableName);
            Sql.AddText("SET");
            var sets = new List<string>();
            if (additionSet != null)
            {
                sets.AddRange(additionSet.Select(item => string.Format("[{0}] = {1}", item.Key, item.Value)));
                setPermitImplement.SetPermit(false, additionSet.Keys);
            }
            var setSqlParameters = DataBaseTypeData.GetSqlParameters(setPermitImplement);
            Sql.AddText(string.Join(",",
                sets.Concat(setSqlParameters.Select(item => string.Format("[{0}] = @{0}", item.ParameterName)))));
            Sql.AddParameter(setSqlParameters);
            Sql.AddText("WHERE");
            Sql.AddText(string.Format("[{0}] = @{0}", IdentitySqlParameter.ParameterName));
            Sql.AddParameter(IdentitySqlParameter);
            return Sql.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// 添加或修改一条
        /// 通过标识列
        /// </summary>
        /// <param name="model">数据</param>
        /// <param name="setPermitImplement">需限制更改的字段，默认不限制，标识列始终限制</param>
        /// <param name="additionSet">附加更改，其键将覆盖对应字段的默认更改；插入时，此项不生效</param>
        /// <returns>为0时更改失败，大于0时为自动增长编号，为1时为修改或添加成功</returns>
        public int InsertOrUpdateOneByIdentity(T model, PermitImplement<string> setPermitImplement = null, Dictionary<string, string> additionSet = null)
        {
            DataBaseTypeData.Reset().Model = model;
            Sql.AddText(string.Format("IF(EXISTS("));
            Sql.AddText("SELECT 0 FROM");
            Sql.AddText(TableName);
            Sql.AddText("WHERE");
            Sql.AddText(string.Format("[{0}] = @{0}", IdentitySqlParameter.ParameterName));
            Sql.AddText("))BEGIN");
            if (setPermitImplement == null)
            {
                setPermitImplement = new StringPermitImplement().SetComparer(StringComparison.CurrentCultureIgnoreCase);
            }
            setPermitImplement.SetPermit(false,
                DataBaseTypeData.GetSqlParameters(TableFieldLable.Identity).Select(item => item.ParameterName));
            var valueSqlParameters = DataBaseTypeData.GetSqlParameters(setPermitImplement, tfl => (tfl & TableFieldLable.Identity) == 0);
            Sql.AddText("UPDATE");
            Sql.AddText(TableName);
            Sql.AddText("SET");
            var sets = new List<string>();
            setPermitImplement.SetPermit(false, IdentitySqlParameter.ParameterName);
            if (additionSet != null)
            {
                sets.AddRange(additionSet.Select(item => string.Format("[{0}] = {1}", item.Key, item.Value)));
                setPermitImplement.SetPermit(false, additionSet.Keys);
            }
            var setSqlParameters = DataBaseTypeData.GetSqlParameters(setPermitImplement);
            Sql.AddText(string.Join(",",
                sets.Concat(setSqlParameters.Select(item => string.Format("[{0}] = @{0}", item.ParameterName)))));
            Sql.AddText("WHERE");
            Sql.AddText(string.Format("[{0}] = @{0}", IdentitySqlParameter.ParameterName));
            Sql.AddText("SELECT 1");
            Sql.AddText("END ELSE BEGIN");
            Sql.AddText("INSERT INTO");
            Sql.AddText(TableName);
            Sql.AddText("(");
            Sql.AddText(string.Join(",", valueSqlParameters.Select(item => string.Format("[{0}]", item.ParameterName))));
            Sql.AddText(")VALUES(");
            Sql.AddText(string.Join(",", valueSqlParameters.Select(item => string.Format("@{0}", item.ParameterName))));
            Sql.AddText(")");
            Sql.AddText("SELECT SCOPE_IDENTITY()");
            Sql.AddText("END");
            Sql.AddParameter(
                new[] { IdentitySqlParameter }.Concat(setSqlParameters)
                    .Concat(valueSqlParameters)
                    .Distinct(item => item.ParameterName)
                    .ToArray());
            return Sql.ExecuteScalarTryParse<int>() ?? 0;
        }

    }

}
