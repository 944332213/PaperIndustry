using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Nsfttz.DataAccessLayer.Client.Config;

namespace Nsfttz.DataAccessLayer.Client.SqlServer
{
    #region SQL日志

    /// <summary>
    /// SQL日志
    /// </summary>
    public class StructuredQueryLanguageLog
    {
        /// <summary>
        /// sql文本
        /// </summary>
        public string StructuredQueryLanguage { get; set; }

        /// <summary>
        /// sql参数数组
        /// </summary>
        public SqlParameter[] SqlParameterArray { get; set; }

        /// <summary>
        /// 是否存在异常
        /// </summary>
        public bool IsException { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// 发生时间
        /// </summary>
        public DateTime OccurrenceTime { get; set; }
    }

    #endregion

    /// <summary>
    /// 结构化查询语言
    /// </summary>
    public class StructuredQueryLanguage : SqlHelper, IDisposable
    {
        #region 成员变量

        /// <summary>
        /// sql文本分隔符
        /// </summary>
        protected const string SqlTextSeparator = " \n ";

        private SqlConnection _mConnection;
        private StringBuilder _mSqlBuilder;
        private List<SqlParameter> _mSqlParameterCollection;
        private CommandType? _mCommandType;
        private bool? _mKeepAlive;
        private bool? _mIsTransaction;
        private IList<StructuredQueryLanguageLog> _mExecuteLog;

        #endregion

        #region 构造方法

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="connection"></param>
        public StructuredQueryLanguage(SqlConnection connection)
        {
            SetSqlConnection(connection);
        }

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="connectionString"></param>
        public StructuredQueryLanguage(string connectionString)
            : this(new SqlConnection(connectionString))
        {
        }

        ///// <summary>
        ///// 析构函数
        ///// </summary>
        //~StructuredQueryLanguage()
        //{
        //    Dispose();
        //}

        #endregion

        #region 属性成员

        /// <summary>
        /// SQL文本缓冲
        /// </summary>
        protected StringBuilder SqlBuilder
        {
            get { return _mSqlBuilder ?? (_mSqlBuilder = new StringBuilder()); }
        }

        /// <summary>
        /// SQL参数集
        /// </summary>
        protected List<SqlParameter> SqlParameterCollection
        {
            get { return _mSqlParameterCollection ?? (_mSqlParameterCollection = new List<SqlParameter>()); }
        }

        /// <summary>
        /// SQL连接
        /// </summary>
        public SqlConnection Connection
        {
            get { return _mConnection; }
            set
            {
                if (value == null)
                {
                    throw new Exception("错误的SqlConnection链接");
                }
                _mConnection = value;
            }
        }

        public CommandType CommandType
        {
            get { return _mCommandType ?? (CommandType)(_mCommandType = CommandType.Text); }
            set { _mCommandType = value; }
        }

        /// <summary>
        /// 保持连接
        /// 若设置为true，则需使用using或在完结时调用Dispose();
        /// </summary>
        public bool KeepAlive
        {
            get { return _mKeepAlive ?? (bool)(_mKeepAlive = false); }
            set { _mKeepAlive = value; }
        }

        /// <summary>
        /// 是否开启事务
        /// </summary>
        public bool IsTransaction
        {
            get { return _mIsTransaction ?? (bool)(_mIsTransaction = false); }
            set { _mIsTransaction = value; }
        }

        /// <summary>
        /// 是否已执行
        /// 每当执行结束将此值标记为True,调用AddText或AddParameter时标记为False
        /// </summary>
        protected bool IsExecute { get; private set; }

        /// <summary>
        /// 执行日志
        /// </summary>
        private IList<StructuredQueryLanguageLog> ExecuteLog
        {
            get { return _mExecuteLog ?? (_mExecuteLog = new List<StructuredQueryLanguageLog>()); }
        }

        /// <summary>
        /// 日志[只读]
        /// </summary>
        public IReadOnlyCollection<StructuredQueryLanguageLog> Log
        {
            get { return new ReadOnlyCollection<StructuredQueryLanguageLog>(ExecuteLog); }
        }

        /// <summary>
        /// SQL文本
        /// </summary>
        public string SqlText
        {
            get { return IsExecute ? Log.Last().StructuredQueryLanguage : SqlBuilder.ToString(); }
            set { ClearText().AddText(value); }
        }

        /// <summary>
        /// SQL参数数组
        /// </summary>
        public SqlParameter[] SqlParameterArray
        {
            get { return IsExecute ? Log.Last().SqlParameterArray : SqlParameterCollection.ToArray(); }
            set { ClearParameter().AddParameter(value); }
        }

        #endregion

        #region 设置属性

        /// <summary>
        /// 设置连接
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public StructuredQueryLanguage SetSqlConnection(SqlConnection connection)
        {
            Connection = connection;
            return this;
        }

        /// <summary>
        /// 设置连接
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public StructuredQueryLanguage SetSqlConnection(string connectionString)
        {
            return SetSqlConnection(new SqlConnection(connectionString));
        }

        #endregion

        #region 设置参数

        /// <summary>
        /// 添加SQL文本
        /// </summary>
        /// <param name="sqlTextArray">SQL文本数组</param>
        /// <returns></returns>
        public StructuredQueryLanguage AddText(params string[] sqlTextArray)
        {
            if (IsExecute)
            {
                IsExecute = false;
            }
            if (sqlTextArray == null || sqlTextArray.All(string.IsNullOrWhiteSpace))
            {
                return this;
            }
            if (SqlBuilder.Length > 0)
            {
                SqlBuilder.Append(SqlTextSeparator);
            }
            SqlBuilder.Append(string.Join(SqlTextSeparator, sqlTextArray));
            return this;
        }

        /// <summary>
        /// 替换SQL文本
        /// </summary>
        /// <param name="sqlText">SQL文本</param>
        /// <returns></returns>
        public StructuredQueryLanguage ReplaceText(string sqlText)
        {
            SqlBuilder.Clear();
            AddText(sqlText);
            return this;
        }

        /// <summary>
        /// 添加SQL参数
        /// </summary>
        /// <param name="sqlParameterArray">SQL参数数组</param>
        /// <returns></returns>
        public StructuredQueryLanguage AddParameter(params SqlParameter[] @sqlParameterArray)
        {
            if (IsExecute)
            {
                IsExecute = false;
            }
            if (sqlParameterArray == null)
            {
                return this;
            }
            foreach (var sqlParameter in sqlParameterArray)
            {
                var index =
                    SqlParameterCollection.FindIndex(
                        item =>
                            string.Equals(item.ParameterName, sqlParameter.ParameterName,
                                StringComparison.CurrentCultureIgnoreCase));
                if (index >= 0)
                {
                    SqlParameterCollection[index] = sqlParameter;
                }
                else
                {
                    SqlParameterCollection.Add(sqlParameter);
                }
            }
            return this;
        }

        #endregion

        #region 使用事务

        /// <summary>
        /// 使用事务
        /// sql模式 对当前sql添加事务
        /// </summary>
        /// <param name="errorArray"></param>
        /// <returns></returns>
        public virtual StructuredQueryLanguage Transaction(params string[] errorArray)
        {
            if (SqlBuilder.Length <= 0)
            {
                return this;
            }
            SqlText =
                string.Format(
                    @"BEGIN TRANSACTION BEGIN TRY {0} COMMIT TRANSACTION END TRY BEGIN CATCH {1} ROLLBACK TRANSACTION END CATCH",
                    SqlText, string.Join(SqlTextSeparator, errorArray));
            return this;
        }

        #endregion

        #region 语句执行

        protected T Execute<T>(Func<T> action)
        {
            if (SqlBuilder.Length <= 0)
            {
                return default(T);
            }
            T result;
            try
            {
                result = action();
                if (!KeepAlive)
                {
                    Connection.Close();
                }
                AddExecuteLog();
            }
            catch (Exception ex)
            {
                if (Connection != null && Connection.State != ConnectionState.Closed)
                {
                    Connection.Close();
                }
                AddExecuteLog(ex);
                throw;
            }
            IsExecute = true;
            Clear();
            return result;
        }

        #region ExecuteNonQuery

        /// <summary>
        /// 对连接执行 Transact-SQL 语句并返回受影响的行数。
        /// </summary>
        /// <returns></returns>
        public int ExecuteNonQuery()
        {
            return
                Execute(
                    () =>
                        IsTransaction
                            ? ExecuteNonQuery(Connection.BeginTransaction(), CommandType, SqlText, SqlParameterArray)
                            : ExecuteNonQuery(Connection, CommandType, SqlText, SqlParameterArray));
        }

        #endregion

        #region ExecuteScalar

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行。
        /// </summary>
        /// <returns></returns>
        public object ExecuteScalar()
        {
            return Execute(() => ExecuteScalar(Connection, CommandType, SqlText, SqlParameterArray));
        }

        #endregion

        #region ExecuteTable

        /// <summary>
        /// 在 System.Data.DataSet 中添加或刷新行以匹配使用 System.Data.DataSet 和 System.Data.DataTable 名称的数据源中的行。
        /// </summary>
        /// <returns></returns>
        public DataTable ExecuteTable()
        {
            return Execute(() => ExecuteTable(Connection, CommandType, SqlText, SqlParameterArray));
        }

        #endregion

        #region ExecuteDataSet

        /// <summary>
        /// 在 System.Data.DataSet 中添加或刷新行。
        /// </summary>
        /// <returns></returns>
        public DataSet ExecuteDataSet()
        {
            return Execute(() => ExecuteDataSet(Connection, CommandType, SqlText, SqlParameterArray));
        }

        #endregion

        #endregion

        #region 日志相关

        /// <summary>
        /// 添加SQL执行日志
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        private void AddExecuteLog(Exception exception = null)
        {
            var log = new StructuredQueryLanguageLog
            {
                Exception = exception,
                IsException = exception != null,
                OccurrenceTime = DateTime.Now,
                StructuredQueryLanguage = SqlText,
                SqlParameterArray = SqlParameterArray,
            };
            ExecuteLog.Add(log);
            AddLog(log);
        }

        /// <summary>
        /// 添加SQL日志 
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public StructuredQueryLanguage AddLog(Exception exception)
        {
            AddLog(new StructuredQueryLanguageLog
            {
                Exception = exception,
                IsException = exception != null,
                OccurrenceTime = DateTime.Now,
                StructuredQueryLanguage = SqlText,
                SqlParameterArray = SqlParameterArray,
            });
            return this;
        }

        /// <summary>
        /// 添加SQL日志
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        protected virtual void AddLog(StructuredQueryLanguageLog log)
        {
            if (DataConvert.TryParse<int>(ConfigManager.GetAppSetting("IsLogSql")) == 1 && log.Exception != null)
            {
            }
        }

        #endregion

        #region 资源方法

        /// <summary>
        /// 清除SQL文本
        /// </summary>
        /// <returns></returns>
        public StructuredQueryLanguage ClearText()
        {
            SqlBuilder.Clear();
            return this;
        }

        /// <summary>
        /// 清除SQL参数
        /// </summary>
        /// <returns></returns>
        public StructuredQueryLanguage ClearParameter()
        {
            SqlParameterCollection.Clear();
            return this;
        }

        /// <summary>
        /// 清除SQL文本及参数
        /// </summary>
        /// <returns></returns>
        public StructuredQueryLanguage Clear()
        {
            return ClearText().ClearParameter();
        }

        /// <summary>
        /// 释放对象资源
        /// </summary>
        public void Dispose()
        {
            if (Connection != null)
            {
                Connection.Dispose();
                GC.SuppressFinalize(this);
            }
            _mSqlBuilder = null;
            _mSqlParameterCollection = null;
            _mExecuteLog = null;
        }

        #endregion

    }
}