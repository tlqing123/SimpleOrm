=====================================================<br/>
# SimpleOrm使用方法
支持两种资源库ExpressionRepository、QueryObjectRepository，一般使用前者，但后者只是证明表达式不是唯一的方法而已，使用分两步，第一步是创建连接，第二步创建资源库，然后就可以使用了，代码如下<br/>
```C# string connectionString = ConfigHelper.GetConnectionString("SqlConnection");
   ProviderFactory.RegisterProviderFactory(connectionString, DateProvider.SqlServer);//注册工厂建议放在全局处
   ExpressionRepository<tb_User>; repository = new ExpressionRepository<tb_User>(connectionString);//注册资源库
   var result=repository.Query<tb_User>(sql, new { }).FirstOrDefault();//查询数据
```
# SimpleOrm系统事务支持
系统事务的支持相当简单，只需要显式标志开始和结束即可，也可以根据需要设置事务隔离级别，默认为ReadCommitted读已提交（不可重复度），可以换成其他级别防止不可重复读、幻读；
``` C#     
        repository.BeginTransaction();//事务开始
        repository.Insert(new tb_User() { mobile = "15989027256", name = "bosco", password = "123456", sex = 1 });
        repository.Update(new tb_User { mobile = "15989027255" }, e => e.name == "cxb");
        repository.CommitTransaction();//事务结束
```
# SimpleOrm支持分页
这里的分页返回类型为PageList，里面包含PageIndex页面、PageSize页大小、rowCount数据总数、Itemes数据列表（Item是List&lt;TResult）,满足一般的需要；
int pageindex = 1;
int pagesize = 10;
var pagelist = repository.GetPageList(e =&gt; true, pageindex, pagesize);
#SimpleOrm支持动态SQL查询操作
写入sql语句后，在参数处加上匿名参数，此处支持object类型，实际上可以自己定义引用类型，返回的是List类型，可以根据自己的需要返回，如果想返回单条数据，可以使用QueryFirst方法，将返回类型T，使用QueryFirst方法，就算结果集有多条也只返回一条。</p>
List&lt;tb_user&gt; users=repository.Query&lt;tb_User&gt;("select * from tb_User where name=@name", new { name="cxb" });
tb_User User=repository.QueryFirst&lt;tb_User&gt;("select * from tb_User where name=@name", new { name="cxb" });
# SimpleOrm性能测试
Simple是高性能的，对此框架做了三组获取100条数据的试验，试验结果如下，项目代码中有试验代码： Dapper、SimpleOrm、.net反射结果如下
实验代码如下
``` C#        string sqlRecord = "select * from tb_User";
        ProviderFactory.RegisterProviderFactory(connectionString, DateProvider.SqlServer);
        ExpressionRepository<tb_User> repository = new ExpressionRepository<tb_User>(connectionString);
        #region SimpleOrm性能测试
        var watcher1 = new Stopwatch();
        watcher1.Start();
        for (int i = 0; i &lt; 30; i++)
        {
            repository.QueryReflect<tb_User>(sqlRecord, new { }).FirstOrDefault();
        }
        watcher1.Stop();
        var duration1 = watcher1.Elapsed.TotalMilliseconds;
        Console.WriteLine(".net反射的效率：" + duration1);
        #endregion
        #region SimpleOrm性能测试
        var watcher = new Stopwatch();
        watcher.Start();

        for (int i = 0; i &lt; 30; i++)
        {
            repository.Query<tb_User>(sqlRecord, new { }).FirstOrDefault();
        }
        watcher.Stop();
        var duration = watcher.Elapsed.TotalMilliseconds;
        Console.WriteLine("SimpleOrm的效率：" + duration);
        #endregion
                    var watcher0 = new Stopwatch();
        watcher0.Start();


        IDbConnection conn = new SqlConnection(connectionString);
        for (int i = 0; i &lt; 30; i++)
        {
            conn.Query&lt;tb_User&gt;(sqlRecord, new { }).ToList().FirstOrDefault();
        }

        watcher0.Stop();
        var duration0 = watcher0.Elapsed.TotalMilliseconds;
        Console.WriteLine("Dapper的效率：" + duration0);
```

