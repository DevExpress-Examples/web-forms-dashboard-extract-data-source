Imports DevExpress.DashboardCommon
Imports DevExpress.DashboardWeb
Imports DevExpress.DataAccess.Sql
Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Web
Imports System.Web.Services
Imports System.Web.UI
Imports System.Web.UI.WebControls

Namespace ASP_WebDashboard
    Partial Public Class [Default]
        Inherits System.Web.UI.Page

        Private Const extractFileName As String = """ExtractDS_""yyyyMMddHHmmssfff"".dat"""
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
            Dim path As String =Server.MapPath("~/App_Data/ExtractDataSource/")
            hiPath.Value = path

            Dim dataSourceStorage As New DataSourceInMemoryStorage()
            dataSourceStorage.RegisterDataSource("extractDataSource", GetExtractDataSource(path).SaveToXml())
            ASPxDashboard1.SetDataSourceStorage(dataSourceStorage)
        End Sub

        Protected Sub ASPxDashboard1_ConfigureDataConnection(ByVal sender As Object, ByVal e As ConfigureDataConnectionWebEventArgs)
            Dim extractCP As ExtractDataSourceConnectionParameters = TryCast(e.ConnectionParameters, ExtractDataSourceConnectionParameters)
            If extractCP IsNot Nothing Then
                extractCP.FileName = GetExtractFileName(Server.MapPath("~/App_Data/ExtractDataSource/"))
            End If
        End Sub

        Private Shared Function GetExtractFileName(ByVal path As String) As String

            Dim file = Directory.GetFiles(path).Select(Function(fn) New FileInfo(fn)).OrderByDescending(Function(f) f.CreationTime).FirstOrDefault()
            If file IsNot Nothing Then
                Return file.FullName
            End If
            Return ""
        End Function

        Private Shared Function GetExtractDataSource(ByVal path As String) As DashboardExtractDataSource
            Dim nwindDataSource As New DashboardSqlDataSource("Northwind Invoices", "nwindConnection")
            Dim invoicesQuery As SelectQuery = SelectQueryFluentBuilder.AddTable("Invoices").SelectColumns("Customers.CompanyName", "Address", "City", "Region", "PostalCode", "Country", "Salesperson", "OrderDate", "Shippers.CompanyName", "ProductName", "UnitPrice", "Quantity", "Discount", "ExtendedPrice", "Freight").Build("Invoices")
            nwindDataSource.Queries.Add(invoicesQuery)
            nwindDataSource.ConnectionOptions.CommandTimeout = 600

            Dim extractDataSource As New DashboardExtractDataSource("Invoices Extract Data Source")
            extractDataSource.FileName = GetExtractFileName(path)
            extractDataSource.ExtractSourceOptions.DataSource = nwindDataSource
            extractDataSource.ExtractSourceOptions.DataMember = "Invoices"
            Return extractDataSource
        End Function

        <WebMethod> _
        Public Shared Function UpdateExtractDataSource(ByVal path As String) As Object
            Dim ds = GetExtractDataSource(path)
            Dim fileName As String = Date.Now.ToString(extractFileName)
            ds.FileName = path & "Temp\" & fileName
            Try
                ds.UpdateExtractFile()
            Catch ex As Exception
                Return ex.Message
            Finally
                ds.Dispose()
            End Try
            File.Move(path & "Temp\" & fileName, path & fileName)
            Return "Data is updated"
        End Function
    End Class
End Namespace