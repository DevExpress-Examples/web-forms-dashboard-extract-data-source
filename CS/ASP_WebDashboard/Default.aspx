<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ASP_WebDashboard.Default" %>

<%@ Register assembly="DevExpress.Dashboard.v21.1.Web.WebForms, Version=21.1.11.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.DashboardWeb" tagprefix="dx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
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
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <input type="button" value="Update Extract Data Source" onclick="UpdateExtractDataSource();" />
        <dx:ASPxDashboard ID="ASPxDashboard1" runat="server" ClientInstanceName="dashboard" DashboardStorageFolder="~/App_Data/Dashboards/" 
            OnConfigureDataConnection="ASPxDashboard1_ConfigureDataConnection" >
        </dx:ASPxDashboard>    
    </div>
    </form>
</body>
</html>
