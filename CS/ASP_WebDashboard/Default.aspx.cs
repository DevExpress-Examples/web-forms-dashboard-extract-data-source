using DevExpress.DashboardCommon;
using DevExpress.DashboardWeb;
using DevExpress.DataAccess.Sql;
using System;
using System.IO;
using System.Threading;
using System.Web.Hosting;
using System.Web.Services;

namespace ASP_WebDashboard
{
    public partial class Default : System.Web.UI.Page {

        static string extractFileName = HostingEnvironment.MapPath("~/App_Data/ExtractDataSource/") + "ExtractDS.dat";
        protected void Page_Load(object sender, EventArgs e) {
            DataSourceInMemoryStorage dataSourceStorage = new DataSourceInMemoryStorage();
            dataSourceStorage.RegisterDataSource("extractDataSource", CreateExtractDataSource().SaveToXml());
            ASPxDashboard1.SetDataSourceStorage(dataSourceStorage);

            if (!File.Exists(extractFileName)) {
                using (var ds = CreateExtractDataSource()) {
                    ds.UpdateExtractFile();
                }
            }
        }

        protected void ASPxDashboard1_ConfigureDataConnection(object sender, ConfigureDataConnectionWebEventArgs e) {
            ExtractDataSourceConnectionParameters extractCP = e.ConnectionParameters as ExtractDataSourceConnectionParameters;
            if (extractCP != null) {
                extractCP.FileName = extractFileName;
            }
        }

        private static DashboardExtractDataSource CreateExtractDataSource() {
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

            return extractDataSource;
        }

        [WebMethod]
        public static void UpdateExtractDataSource() {
            DashboardExtractDataSource ds = CreateExtractDataSource();
            ManualResetEvent mre = new ManualResetEvent(false);
            DashboardExtractDataSource.UpdateFile(ds,
                (a, b) => { mre.Set(); },
                (a, b) => { });
            // Wait until the data is refreshed in the Extract Data Source.
            mre.WaitOne();
        }
    }
}