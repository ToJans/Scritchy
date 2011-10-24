<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<Example.Domain.Readmodel.StockDictionary>" %>
<%@ import Namespace="Example.Web.Helpers" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Index</title>
</head>
<body>
    <div>
    <h3>Stock</h3>
    <ul>
    <% foreach (var k in Model.Keys)
       { %>
            <li>
            <%= k %> : <%= Model[k] %>
            </li>
    <%} %>
    </ul>
    </div>
    <div>
    <h3>Published events</h3>
    <pre>
        <% foreach (var e in ViewData["PublishedEvents"] as IEnumerable<object>)
           { %>
<%= Html.Dump(e) %>
        <% } %>}
    </pre>
    </div>
    <div>
    <h3>Commands</h3>
    <h4>Add Items</h4>
        <% using (Html.BeginForm("Command", "Bus")){ %>
                <input type="hidden" name="commandtype" value="<%=typeof(Example.Domain.Commands.AddItems).FullName %>" />
                <label for="StockItemId">Id</label>
                <input type="text" name="StockItemId" value="item/1" />
                <label for="Count">Count</label>
                <input type="text" name="Count" value="3" />
                <button>Apply</button>
         <% }%>
    <h4>Remove Items</h4>
        <% using (Html.BeginForm("Command", "Bus")){ %>
                <input type="hidden" name="commandtype" value="<%=typeof(Example.Domain.Commands.RemoveItems).FullName %>" />
                <label for="StockItemId">Id</label>
                <input type="text" name="StockItemId" value="item/1" />
                <label for="Count">Count</label>
                <input type="text" name="Count" value="3" />
                <button>Apply</button>
         <% }%>
    </div>
</body>
</html>
