using SimpleMapper.Providers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMapper
{
    /// <summary>
    /// DB共享上下文
    /// </summary>
    public class DbContext:IDisposable
    {
        public DbConnection conn;//连接对象
        public DbTransaction tran;//事务对象
        private BaseProvider dbprovider;//数据提供者
        private string connectionString;

        public DbContext(string connectionString) {
            this.connectionString = connectionString;
            dbprovider = ProviderFactory.GetProviderFatory(connectionString);
        }

        #region 创建资源库
        //有类型
        public Repository<T> CreateRepository<T>()where T:new() {
            return new Repository<T>(this.connectionString,this);
        }
        //无类型
        public NonRepository CreateNonRepository()
        {
            return new NonRepository(this.connectionString);
        }
        #endregion

        #region 连接池管理
        //创建事务
        public void CreateTransaction(IsolationLevel level = IsolationLevel.Unspecified)
        {
            this.CreateConnection();
            tran = conn.BeginTransaction(level);
        }
        //提交事务
        public void CommitTransaction()
        {
            if(!tran.IsNullOrSpace()) tran.Commit();
            this.Close();
        }
        //回滚事务
        public void Rollback()
        {
            if (!tran.IsNullOrSpace())
            {
                tran.Rollback();
                this.Close();
            }
        }
        private void CreateConnection()
        {
            if (conn.IsNullOrSpace()) conn = dbprovider.CreateDbConnection();

            if (conn.State != ConnectionState.Open)
            {
                conn.ConnectionString = this.connectionString;
                conn.Open();
            }
        }
        public void Close()
        {
            this.conn.Close();
            this.tran.Dispose();
        }

        public void Dispose()
        {
            this.conn.Dispose();
            this.tran.Dispose();
        }
        #endregion
    }
}
