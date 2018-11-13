# How to use DashboardExtractDataSource in the ASPxDashboard control


<p>This example demonstrates basic approaches and code snippets that can be used to add a <a href="https://documentation.devexpress.com/Dashboard/clsDevExpressDashboardCommonDashboardExtractDataSourcetopic.aspx">DashboardExtractDataSource</a> to the <a href="https://documentation.devexpress.com/Dashboard/clsDevExpressDashboardWebASPxDashboardtopic.aspx">ASPxDashboard</a> control. The Extract data source is often used if it requires a lot of time to request data from a database using a complex query or a stored procedure. The <a href="https://documentation.devexpress.com/Dashboard/clsDevExpressDashboardCommonDashboardExtractDataSourcetopic.aspx">DashboardExtractDataSource</a> allows you to request this data once and save it in the compressed and optimized form to a file. Then it will be possible to load data from this file and prepare a new file version when data is updated. This concept is described in the <a href="https://community.devexpress.com/blogs/news/archive/2016/08/16/faster-dashboards-with-the-data-extract-source.aspx">Faster Dashboards with the “Data Extract” Source</a> blog.</p>
<p><br>The following code snippet is used to create a DashboardExtractDataSource and connect it to a <a href="https://documentation.devexpress.com/Dashboard/clsDevExpressDashboardCommonDashboardSqlDataSourcetopic.aspx">DashboardSqlDataSource</a>. Note that I use the <a href="https://documentation.devexpress.com/CoreLibraries/DevExpressDataAccessSqlConnectionOptions_CommandTimeouttopic.aspx">CommandTimeout</a> property to increase the query timeout:</p>


```cs
DashboardSqlDataSource nwindDataSource = new DashboardSqlDataSource("Northwind Invoices", "nwindConnection");
SelectQuery invoicesQuery = SelectQueryFluentBuilder
	.AddTable("Invoices")
	.SelectColumns("City", "Country", "Salesperson", "OrderDate", "Shippers.CompanyName", "ProductName", "UnitPrice", "Quantity", "Discount", "ExtendedPrice", "Freight")
	.Build("Invoices");
nwindDataSource.Queries.Add(invoicesQuery);
nwindDataSource.ConnectionOptions.CommandTimeout = 600;

DashboardExtractDataSource extractDataSource = new DashboardExtractDataSource("Invoices Extract Data Source");
extractDataSource.ExtractSourceOptions.DataSource = nwindDataSource;
extractDataSource.ExtractSourceOptions.DataMember = "Invoices";

```


<p> </p>
<p>To add the data source to the ASPxDashboard control use the solution described in the <a href="https://documentation.devexpress.com/Dashboard/CustomDocument116300.aspx">Register Default Data Sources</a> help topic:</p>


```cs
DataSourceInMemoryStorage dataSourceStorage = new DataSourceInMemoryStorage();
dataSourceStorage.RegisterDataSource("extractDataSource", GetExtractDataSource().SaveToXml());
ASPxDashboard1.SetDataSourceStorage(dataSourceStorage);
```


<p> </p>
<p>The AddExtractDataSource method is used to extract data from a database. Note that it is impossible to update a used file, thus a new file is always created:</p>


```cs
var ds = GetExtractDataSource(path);
string fileName = DateTime.Now.ToString(extractFileName);
ds.FileName = path + "Temp\\" + fileName;
ds.UpdateExtractFile();
```


<p>In this example, data is extracted on a button click. However, in a real-life application, this solution can be insufficient (e.g. the site may be deployed to the web farm server). We recommend creating a separate windows service that should update data automatically every hour or every day.<br><br>At last, the <a href="https://documentation.devexpress.com/Dashboard/DevExpressDashboardWebASPxDashboardViewer_ConfigureDataConnectiontopic.aspx">ConfigureDataConnection</a> event is used to connect the data source to the latest file version:</p>


```cs
using (var ds = CreateExtractDataSource()) {
	ds.FileName = tempPath + fileName;
	ds.UpdateExtractFile();
}
```


<p> </p>

<br/>


