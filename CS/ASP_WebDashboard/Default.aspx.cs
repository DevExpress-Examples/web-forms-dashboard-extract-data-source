using DevExpress.DashboardCommon;
using DevExpress.DashboardWeb;
using DevExpress.DataAccess.Sql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ASP_WebDashboard {
    public partial class Default : System.Web.UI.Page {

        const string extractFileName = "\"ExtractDS_\"yyyyMMddHHmmssfff\".dat\"";
        protected void Page_Load(object sender, EventArgs e) {
            string path =Server.MapPath("~/App_Data/ExtractDataSource/");
            hiPath.Value = path;           

            DataSourceInMemoryStorage dataSourceStorage = new DataSourceInMemoryStorage();
            dataSourceStorage.RegisterDataSource("extractDataSource", GetExtractDataSource( path ).SaveToXml());
            ASPxDashboard1.SetDataSourceStorage(dataSourceStorage);
        }

        protected void ASPxDashboard1_ConfigureDataConnection(object sender, ConfigureDataConnectionWebEventArgs e) {
            ExtractDataSourceConnectionParameters extractCP = e.ConnectionParameters as ExtractDataSourceConnectionParameters;
            if (extractCP != null) {
                extractCP.FileName = GetExtractFileName(Server.MapPath("~/App_Data/ExtractDataSource/"));
            }
        }

        private static string GetExtractFileName( string path) {
            
            var file = Directory.GetFiles(path).Select(fn => new FileInfo(fn)).OrderByDescending(f => f.CreationTime).FirstOrDefault();
            if (file != null)
                return file.FullName;
            return "";
        }

        private static DashboardExtractDataSource GetExtractDataSource(string path) {
            DashboardSqlDataSource nwindDataSource = new DashboardSqlDataSource("Northwind Invoices", "nwindConnection");
            SelectQuery invoicesQuery = SelectQueryFluentBuilder
                .AddTable("Invoices")
                .SelectColumns("Customers.CompanyName", "Address", "City", "Region", "PostalCode", "Country", "Salesperson", "OrderDate", "Shippers.CompanyName", "ProductName", "UnitPrice", "Quantity", "Discount", "ExtendedPrice", "Freight")
                .Build("Invoices");
            nwindDataSource.Queries.Add(invoicesQuery);
            nwindDataSource.ConnectionOptions.CommandTimeout = 600;

            DashboardExtractDataSource extractDataSource = new DashboardExtractDataSource("Invoices Extract Data Source");
            extractDataSource.FileName = GetExtractFileName(path);            
            extractDataSource.ExtractSourceOptions.DataSource = nwindDataSource;
            extractDataSource.ExtractSourceOptions.DataMember = "Invoices";
            return extractDataSource;
        }

        [WebMethod]
        public static object UpdateExtractDataSource(string path) {
            var ds = GetExtractDataSource(path);
            string fileName = DateTime.Now.ToString(extractFileName);
            ds.FileName = path + "Temp\\" + fileName;
            try {
                ds.UpdateExtractFile();
            }
            catch (Exception ex) {
                return ex.Message;
            }
            finally {
                ds.Dispose();
            }
            File.Move(path + "Temp\\" + fileName, path + fileName);
            return "Data is updated";
        }
    }
}