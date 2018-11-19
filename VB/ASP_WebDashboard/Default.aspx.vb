Imports DevExpress.DashboardCommon
Imports DevExpress.DashboardWeb
Imports DevExpress.DataAccess.Sql
Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Web
Imports System.Web.Hosting
Imports System.Web.Services
Imports System.Web.UI
Imports System.Web.UI.WebControls

Namespace ASP_WebDashboard
    Partial Public Class [Default]
        Inherits System.Web.UI.Page

        Private Const extractFileName As String = """ExtractDS_""yyyyMMddHHmmssfff"".dat"""
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
            Dim dataSourceStorage As New DataSourceInMemoryStorage()
            dataSourceStorage.RegisterDataSource("extractDataSource", CreateExtractDataSource().SaveToXml())
            ASPxDashboard1.SetDataSourceStorage(dataSourceStorage)
        End Sub

        Protected Sub ASPxDashboard1_ConfigureDataConnection(ByVal sender As Object, ByVal e As ConfigureDataConnectionWebEventArgs)
            Dim extractCP As ExtractDataSourceConnectionParameters = TryCast(e.ConnectionParameters, ExtractDataSourceConnectionParameters)
            If extractCP IsNot Nothing Then
                extractCP.FileName = GetExtractFileName()
            End If
        End Sub
        Protected Sub ASPxDashboard1_CustomParameters(ByVal sender As Object, ByVal e As CustomParametersWebEventArgs)
            e.Parameters.Add(New DashboardParameter("ExtractFileName", GetType(String), GetExtractFileName()))
        End Sub

        Private Function GetExtractFileName() As String
            Dim path = Server.MapPath("~/App_Data/ExtractDataSource/")
            Dim file = Directory.GetFiles(path).Select(Function(fn) New FileInfo(fn)).OrderByDescending(Function(f) f.CreationTime).FirstOrDefault()
            If file IsNot Nothing Then
                Return file.FullName
            Else
                Return AddExtractDataSource()
            End If
        End Function

        Private Shared Function CreateExtractDataSource() As DashboardExtractDataSource
            Dim nwindDataSource As New DashboardSqlDataSource("Northwind Invoices", "nwindConnection")
            Dim invoicesQuery As SelectQuery = SelectQueryFluentBuilder.AddTable("Invoices").SelectColumns("City", "Country", "Salesperson", "OrderDate", "Shippers.CompanyName", "ProductName", "UnitPrice", "Quantity", "Discount", "ExtendedPrice", "Freight").Build("Invoices")
            nwindDataSource.Queries.Add(invoicesQuery)
            nwindDataSource.ConnectionOptions.CommandTimeout = 600

            Dim extractDataSource As New DashboardExtractDataSource("Invoices Extract Data Source")

            extractDataSource.ExtractSourceOptions.DataSource = nwindDataSource
            extractDataSource.ExtractSourceOptions.DataMember = "Invoices"
            Return extractDataSource
        End Function

        <WebMethod> _
        Public Shared Function AddExtractDataSource() As String
            Dim fileName As String = Date.Now.ToString(extractFileName)
            Dim path As String = HostingEnvironment.MapPath("~/App_Data/ExtractDataSource/")
            Dim tempPath As String = path & "Temp\"
            Directory.CreateDirectory(tempPath)
            Using ds = CreateExtractDataSource()
                ds.FileName = tempPath & fileName
                ds.UpdateExtractFile()
            End Using
            File.Move(tempPath & fileName, path & fileName)
            If Not Directory.EnumerateFiles(tempPath).Any() Then
                Directory.Delete(tempPath)
            End If
            Return path & fileName
        End Function


    End Class
End Namespace