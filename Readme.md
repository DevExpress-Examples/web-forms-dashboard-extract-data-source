<!-- default file list -->
*Files to look at*:

* [Default.aspx](./CS/ASP_WebDashboard/Default.aspx) (VB: [Default.aspx](./VB/ASP_WebDashboard/Default.aspx))
* [Default.aspx.cs](./CS/ASP_WebDashboard/Default.aspx.cs) (VB: [Default.aspx.vb](./VB/ASP_WebDashboard/Default.aspx.vb))
<!-- default file list end -->
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
extractDataSource.FileName = extractFileName;
```

<p> </p>
<br>To set the correct extract file name for loaded dashboards, handle the  <a href="https://documentation.devexpress.com/Dashboard/DevExpress.DashboardWeb.ASPxDashboard.ConfigureDataConnection.event">ConfigureDataConnection</a> event:</p>

```cs
protected void ASPxDashboard1_ConfigureDataConnection(object sender, ConfigureDataConnectionWebEventArgs e) {
	ExtractDataSourceConnectionParameters extractCP = e.ConnectionParameters as ExtractDataSourceConnectionParameters;
	if (extractCP != null) {
		extractCP.FileName = extractFileName;
	}
}
```

<p> </p>
<p>To add the data source to the ASPxDashboard control use the solution described in the <a href="https://documentation.devexpress.com/Dashboard/CustomDocument116300.aspx">Register Default Data Sources</a> help topic:</p>


```cs
DataSourceInMemoryStorage dataSourceStorage = new DataSourceInMemoryStorage();
dataSourceStorage.RegisterDataSource("extractDataSource", GetExtractDataSource().SaveToXml());
ASPxDashboard1.SetDataSourceStorage(dataSourceStorage);
```

<p> </p>
<p>To create a data extract file when the dashboard is loaded for the first time, use the following code :</p>

```cs
if (!File.Exists(extractFileName)) {
     using (var ds = CreateExtractDataSource()) {
         ds.UpdateExtractFile();
     }
}
```

<p>To update the data extract file and load the updated data in ASPxDashboard, send an AJAX request to the server and call the DashboardExtractDataSource.UpdateFile method there. Once a new file is created on the server, you can return the callback back to the client and call the ASPxClientDashboard.Refresh method to reload the control with new data:

```js
function UpdateExtractDataSource() {
    $.ajax({
        url: "Default.aspx/UpdateExtractDataSource",
        type: "POST",
        data: {},
        contentType: "application/json; charset=utf-8"
    }).done(function (result) {
        dashboard.ReloadData();
    });
}
```

```cs
[WebMethod]
public static void UpdateExtractDataSource() {
     DashboardExtractDataSource ds = CreateExtractDataSource();
     ManualResetEvent mre = new ManualResetEvent(false);
     DashboardExtractDataSource.UpdateFile(ds,
          (a, b) => { mre.Set(); },
          (a, b) => { });
          // Wait until data is refreshed in Extract Data Source
     mre.WaitOne();
}
```
<p> </p>

<p>In this example, data is extracted on a button click. However, in a real-life application, this solution can be insufficient (e.g. the site may be deployed to the web farm server). We recommend creating a separate windows service that should update data automatically every hour or every day.<br>


