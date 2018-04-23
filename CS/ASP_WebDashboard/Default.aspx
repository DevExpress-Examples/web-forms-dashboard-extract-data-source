<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ASP_WebDashboard.Default" %>

<%@ Register assembly="DevExpress.Dashboard.v16.2.Web, Version=16.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.DashboardWeb" tagprefix="dx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function UpdateExtractDataSource(s, e) {            
            e.processOnServer = false;
            $.ajax({
                url: "Default.aspx/UpdateExtractDataSource",                
                type: "POST",
                data: JSON.stringify({path: $('#hiPath').val()}),
                contentType: "application/json; charset=utf-8"
            }).done(function (result) {
                alert(result.d);
            });

        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <dx:ASPxButton ID="bUpdateDS" runat="server" Text="Update Extract Data Source" UseSubmitBehavior="false" ClientSideEvents-Click="UpdateExtractDataSource"></dx:ASPxButton>  

        <input runat="server" id="hiPath" type="hidden" enableviewstate="true" />
    
        <dx:ASPxDashboard ID="ASPxDashboard1" runat="server" DashboardStorageFolder="~/App_Data/Dashboards/" OnConfigureDataConnection="ASPxDashboard1_ConfigureDataConnection">
        </dx:ASPxDashboard>
    
    </div>
    </form>
</body>
</html>
