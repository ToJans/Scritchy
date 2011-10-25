<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<Example.Domain.Readmodel.StockDictionary>" %>
<%@ import Namespace="Example.Web.Helpers" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Index</title>
</head>
<body>
    <div style="float:left;border:1px solid black">
    <h3>Stock</h3>
    <ul>
    <% foreach (var k in Model.Keys)
       { %>
            <li>
            <%= k %> : <%= Model[k].Name %> = <%= Model[k].Amount %>
            </li>
    <% } %>
    </ul>
    </div>
    <div style="float:left;border:1px solid black">
    <h3>Published events</h3>
        <ul>
            <% foreach (var e in ViewData["PublishedEvents"] as IEnumerable<object>) { %>
                <li><%=e.GetType().Name %>
                  <%= Html.Dump(e) %>
                </li>
            <% } %>
        </ul>
    </div>
    <div style="float:left;border:1px solid black">
    <h3>Failed commands</h3>
    <ul>
        <% foreach (var e in ViewData["FailedCommands"] as IEnumerable<dynamic>)
           { %>
            <li><%=(e.Command as object).GetType().Name%>: <%= e.Message %>
                <%= Html.Dump(e.Command as object) %>
            </li>           
            <% } %>
    </ul>
    </div>
    <div style="float:left;border:1px solid black">
    <h3>Commands</h3>
    <h4>Allow Item</h4>
        <% using (Html.BeginForm("Command", "Bus")){ %>
                <input type="hidden" name="commandtype" value="<%=typeof(Example.Domain.Commands.AllowItem).FullName %>" />
                <label for="StockItemId">Id</label>
                <input type="text" name="StockItemId" value="item/1" />
                <label for="Name">Name</label>
                <input type="text" name="Name" value="Item 1" />
                <button>Apply</button>
         <% }%>
    <h4>Ban Item</h4>
        <% using (Html.BeginForm("Command", "Bus")){ %>
                <input type="hidden" name="commandtype" value="<%=typeof(Example.Domain.Commands.BanItem).FullName %>" />
                <label for="StockItemId">Id</label>
                <input type="text" name="StockItemId" value="item/1" />
                <button>Apply</button>
         <% }%>
    <h4>Add Items</h4>
        <% using (Html.BeginForm("Command", "Bus")){ %>
                <input type="hidden" name="commandtype" value="<%=typeof(Example.Domain.Commands.AddItems).FullName %>" />
                <label for="StockItemId">Id</label>
                <input type="text" name="StockItemId" value="item/1" />
                <label for="Amount">Amount</label>
                <input type="text" name="Amount" value="3" />
                <button>Apply</button>
         <% }%>
    <h4>Remove Items</h4>
        <% using (Html.BeginForm("Command", "Bus")){ %>
                <input type="hidden" name="commandtype" value="<%=typeof(Example.Domain.Commands.RemoveItems).FullName %>" />
                <label for="StockItemId">Id</label>
                <input type="text" name="StockItemId" value="item/1" />
                <label for="Amount">Amount</label>
                <input type="text" name="Amount" value="3" />
                <button>Apply</button>
         <% }%>
    </div>
</body>
</html>
