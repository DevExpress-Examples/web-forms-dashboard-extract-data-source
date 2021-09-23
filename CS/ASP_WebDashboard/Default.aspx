<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ASP_WebDashboard.Default" %>

<%@ Register assembly="DevExpress.Dashboard.v21.1.Web.WebForms, Version=21.1.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.DashboardWeb" tagprefix="dx" %>

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
            }).done(function (result) {
                dashboard.ReloadData();
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
