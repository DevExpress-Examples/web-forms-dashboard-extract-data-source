<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/128580409/19.2.3%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/T506198)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->
<!-- default file list -->
*Files to look at*:

* [Default.aspx](./CS/ASP_WebDashboard/Default.aspx) (VB: [Default.aspx](./VB/ASP_WebDashboard/Default.aspx))
* [Default.aspx.cs](./CS/ASP_WebDashboard/Default.aspx.cs) (VB: [Default.aspx.vb](./VB/ASP_WebDashboard/Default.aspx.vb))
<!-- default file list end -->

# Dashboard for Web Forms - How to Use DashboardExtractDataSource to provide data

This example demonstrates basic approaches and code snippets that can be used to bind the [ASPxDashboard](https://docs.devexpress.com/Dashboard/DevExpress.DashboardWeb.ASPxDashboard) control to the [DashboardExtractDataSource](https://docs.devexpress.com/Dashboard/DevExpress.DashboardCommon.DashboardExtractDataSource). 

The [Extract Data Source](https://docs.devexpress.com/Dashboard/115900) improves performance when a complex query or a stored procedure takes a significant time to get data from a database. 

The [DashboardExtractDataSource](https://docs.devexpress.com/Dashboard/DevExpress.DashboardCommon.DashboardExtractDataSource) class implements the Extract Data Source concept and allows you to request the data once and save it in the compressed and optimized form to a file. Subsequently tan application can retrieve data from that file or create a new file when data is updated.

This concept is described in the <a href="https://community.devexpress.com/blogs/news/archive/2016/08/16/faster-dashboards-with-the-data-extract-source.aspx">Faster Dashboards with the “Data Extract” Source</a> blog.

The code snippet below creates the **DashboardExtractDataSource** and connects it to the [DashboardSqlDataSource](https://docs.devexpress.com/Dashboard/DevExpress.DashboardCommon.DashboardSqlDataSource) instance. The [DbCommandTimeout](https://docs.devexpress.com/CoreLibraries/DevExpress.DataAccess.Sql.ConnectionOptions.DbCommandTimeout) property is set to 600 to increase the query timeout.


```cs
DashboardSqlDataSource nwindDataSource = new DashboardSqlDataSource("Northwind Invoices", "nwindConnection");
SelectQuery invoicesQuery = SelectQueryFluentBuilder
	.AddTable("Invoices")
	.SelectColumns("City", "Country", "Salesperson", "OrderDate", "Shippers.CompanyName", "ProductName", "UnitPrice", "Quantity", "Discount", "ExtendedPrice", "Freight")
	.Build("Invoices");
nwindDataSource.Queries.Add(invoicesQuery);
nwindDataSource.ConnectionOptions.DbCommandTimeout = 600;

DashboardExtractDataSource extractDataSource = new DashboardExtractDataSource("Invoices Extract Data Source");
extractDataSource.ExtractSourceOptions.DataSource = nwindDataSource;
extractDataSource.ExtractSourceOptions.DataMember = "Invoices";
extractDataSource.FileName = extractFileName;
```

To set the correct extract file name for loaded dashboards, handle the [ConfigureDataConnection](https://docs.devexpress.com/Dashboard/DevExpress.DashboardWeb.ASPxDashboard.ConfigureDataConnection) event:

```cs
protected void ASPxDashboard1_ConfigureDataConnection(object sender, ConfigureDataConnectionWebEventArgs e) {
	ExtractDataSourceConnectionParameters extractCP = e.ConnectionParameters as ExtractDataSourceConnectionParameters;
	if (extractCP != null) {
		extractCP.FileName = extractFileName;
	}
}
```

To add the data source to the ASPxDashboard control use the solution described in the [Register Default Data Sources](https://docs.devexpress.com/Dashboard/116300) help topic:


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

To update the data extract file and load the updated data in ASPxDashboard, send an AJAX request to the server and call the [DashboardExtractDataSource.UpdateFile](https://docs.devexpress.com/Dashboard/DevExpress.DashboardCommon.DashboardExtractDataSource.UpdateFile(DashboardExtractDataSource--Action-String--ExtractUpdateResult---Action-String--ExtractUpdateResult-)) method there. Once a new file is created on the server, you can return the callback back to the client and call the [ASPxClientDashboard.Refresh](https://docs.devexpress.com/Dashboard/js-ASPxClientDashboard?#js_ASPxClientDashboard_Refresh) method to reload the control with new data:

```js
function UpdateExtractDataSource() {
    $.ajax({
        url: "Default.aspx/UpdateExtractDataSource",
        type: "POST",
        data: {},
        contentType: "application/json; charset=utf-8"
    }).then(function (result) {
        dashboard.ReloadData();
        DevExpress.ui.notify(result.d, "success", 5000);
    }, function () {
        DevExpress.ui.notify("We could not update extract data source.", "error", 2000)
    });
}
```

```cs
[WebMethod]
public static string UpdateExtractDataSource() {
    DashboardExtractDataSource ds = CreateExtractDataSource();
    StringBuilder sb = new StringBuilder("We updated your extract data source. ");
    var task = DashboardExtractDataSource.UpdateFile(ds,
        (fileName, result) => {
            sb.AppendLine($"{DateTime.Now.ToString("T")} - Data Updated - {result} - {Path.GetFileName(fileName)}. ");
        },
        (fileName, result) => {
            sb.AppendLine($"{DateTime.Now.ToString("T")} - File Updated - {result} - {Path.GetFileName(fileName)}. ");
        });
    // Wait until the data is refreshed in the Extract Data Source.
    task.Wait();
    return sb.ToString();
}
```

In this example, data is extracted on a button click. However, in a real-life application, this solution can be insufficient (e.g. the site may be deployed to the web farm server). We recommend creating a separate windows service that should update data automatically every hour or every day.

## Documentation

- [DashboardExtractDataSource Class](https://docs.devexpress.com/Dashboard/DevExpress.DashboardCommon.DashboardExtractDataSource)
- [Extract Data Source](https://docs.devexpress.com/Dashboard/115900/winforms-dashboard/winforms-designer/create-dashboards-in-the-winforms-designer/providing-data/extract-data-source)

## More Examples

- [Dashboard for Web Forms - How to Register Data Sources](https://github.com/DevExpress-Examples/asp-net-web-forms-dashboard-register-data-sources)
