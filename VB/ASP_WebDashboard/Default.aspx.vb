Imports DevExpress.DashboardCommon
Imports DevExpress.DashboardWeb
Imports DevExpress.DataAccess.Sql
Imports System
Imports System.IO
Imports System.Threading
Imports System.Web.Hosting
Imports System.Web.Services

Namespace ASP_WebDashboard
	Partial Public Class [Default]
		Inherits System.Web.UI.Page

		Private Shared extractFileName As String = HostingEnvironment.MapPath("~/App_Data/ExtractDataSource/") & "ExtractDS.dat"
		Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
			Dim dataSourceStorage As New DataSourceInMemoryStorage()
			dataSourceStorage.RegisterDataSource("extractDataSource", CreateExtractDataSource().SaveToXml())
			ASPxDashboard1.SetDataSourceStorage(dataSourceStorage)

			If Not File.Exists(extractFileName) Then
				Using ds = CreateExtractDataSource()
					ds.UpdateExtractFile()
				End Using
			End If
		End Sub

		Protected Sub ASPxDashboard1_ConfigureDataConnection(ByVal sender As Object, ByVal e As ConfigureDataConnectionWebEventArgs)
			Dim extractCP As ExtractDataSourceConnectionParameters = TryCast(e.ConnectionParameters, ExtractDataSourceConnectionParameters)
			If extractCP IsNot Nothing Then
				extractCP.FileName = extractFileName
			End If
		End Sub

		Private Shared Function CreateExtractDataSource() As DashboardExtractDataSource
			Dim nwindDataSource As New DashboardSqlDataSource("Northwind Invoices", "nwindConnection")
			Dim invoicesQuery As SelectQuery = SelectQueryFluentBuilder.AddTable("Invoices").SelectColumns("City", "Country", "Salesperson", "OrderDate", "Shippers.CompanyName", "ProductName", "UnitPrice", "Quantity", "Discount", "ExtendedPrice", "Freight").Build("Invoices")
			nwindDataSource.Queries.Add(invoicesQuery)
			nwindDataSource.ConnectionOptions.DbCommandTimeout = 600

			Dim extractDataSource As New DashboardExtractDataSource("Invoices Extract Data Source")
			extractDataSource.ExtractSourceOptions.DataSource = nwindDataSource
			extractDataSource.ExtractSourceOptions.DataMember = "Invoices"
			extractDataSource.FileName = extractFileName

			Return extractDataSource
		End Function

		<WebMethod>
		Public Shared Sub UpdateExtractDataSource()
			Dim ds As DashboardExtractDataSource = CreateExtractDataSource()
			Dim mre As New ManualResetEvent(False)
			DashboardExtractDataSource.UpdateFile(ds, Sub(a, b)
				mre.Set()
			End Sub, Sub(a, b)
End Sub)
			' Wait until the data is refreshed in the Extract Data Source.
			mre.WaitOne()
		End Sub
	End Class
End Namespace