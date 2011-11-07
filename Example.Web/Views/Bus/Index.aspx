<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<Example.Domain.Readmodel.StockDictionary>" %>
<%@ import Namespace="Example.Web.Helpers" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Scritchy - CQRS without the plumbing</title>
    <style type="text/css">
        h1
        {
            display:block;
            padding-left:90px;
            background-image: url("/content/scritchy-title.png");
            background-repeat:no-repeat;
            background-position:10px 10px;
            background-color:White;
            font-size:80px;
            text-indent: 3000px;
            border: 2px black solid;
            height:120px;
            padding-top:0px;
             border-radius:5px;
            -moz-border-radius:5px;
            -webkit-border-radius:5px;
        }
        
        .rounded
        {
             border-radius:5px;
            -moz-border-radius:5px;
            -webkit-border-radius:5px;
        }
        
        body
        {
            margin: 0;
            padding: 10px;
            padding-top:0px;
            font-family: Segoe UI;
        }
        
        h1,h2,h3,h4,h5,h6
        {
            font-family:Segoe Script;
        }
        
        h2,h3,h4,h5,h6
        {
            color:White;
            background-color: #333D88;
             border-radius:5px;
            -moz-border-radius:5px;
            -webkit-border-radius:5px;
            
        }
        
        .middle 
        {
            color:Black;
            background:#8088C3;
            margin:0;
            border-left:#333D88 260px solid;
            border-right:#333D88 260px solid;
            padding:10px;
            min-height:1000px;
        }
        
        .box
        {
            background:#6873c3;
            border:black 1px solid;
            position:fixed;
            top:30px;
            width:220px;
            padding:10px;
             border-radius:5px;
            -moz-border-radius:5px;
            -webkit-border-radius:5px;
        }
        
        .left
        {
           left:20px;
        }
        
        .right
        {
            right:20px;
        }
        
        label
        {
            display:inline-block;
            width:80px;
        }
        
        form, .left ul 
        {
            background:white;
            padding:5px;
            margin-top:0px;
             border-radius:5px;
            -moz-border-radius:5px;
            -webkit-border-radius:5px;
        }
        
        .left ul 
        {
            padding-left:1.5em;
        }
        
        
        
        input 
        {
            display:inline-block;
            width:120px;
        }
        .success
        {
            color:Green;
        }
        
        .failure
        {
            color:Red;
        }
        
        .stocklist
        {
            background-color:White;
            width:70%;
            display:inline-block;
            border:1px black solid;
            padding:10px;
             border-radius:5px;
            -moz-border-radius:5px;
            -webkit-border-radius:5px;
        }
        
        .stocklist > h3
        {
            text-decoration:underline;
        }
    </style>
</head>
<body>
    <div class="middle">
    <h1>Scritchy</h1> 
    <h2>CQRS without the plumbing - Example app</h2>
    <p>
        This example app shows how you can implement a CQRS app without to much effort using the Scritchy libraries.
    </p>
    <p>
        More info and full source over at <a href="http://github.com/ToJans/Scritchy" target="_blank">github</a>.
    </p>
    <p>You find the example code in the following locations: 
    <ul>
    <li>
        <i>Domain logic</i> in the <a href="https://github.com/ToJans/Scritchy/blob/master/Example/Domain/StockItem.cs" target="_blank">Example.Domain.StockItem class</a>.
    </li>
    <li>
        <i>Commands</i> in the <a href="https://github.com/ToJans/Scritchy/tree/master/Example/Domain/Commands" target="_blank">Example.Domain.Commands folder</a>.
    </li>
    <li>
        <i>Events</i> in the <a href="https://github.com/ToJans/Scritchy/tree/master/Example/Domain/Events" target="_blank">Example.Domain.Events folder</a>.
    </li>
    <li>
        <i>View builder</i> in the <a href="https://github.com/ToJans/Scritchy/blob/master/Example/Domain/Readmodel/StockDictionary.cs" target="_blank">Example.Domain.Readmodel.StockDictionaryHandler class</a>.
    </li>
    </ul>
    <p>
        No infrastructure setup is required; Scritchy finds out by itself 
        what the commands/events/ar's and eventhandlers are.
    </p>
    <p>
        Eventstorage is also auto-wired; Scrichy looks for a connectionstring named "eventstore" 
        in the app configuration, and if it can not find it, it uses the InMemoryEventStore.<br />
        Currently only tested with SQLite, but other implementations should be trivial.

    </p>    
    <p>
        Enjoy!
    </p>
    <p><a href="http://twitter.com/#/ToJans" target="_blank">ToJans@Twitter</a></p>
    <div class="stocklist">
    <h3>This is the example of a stocklist</h3>
    </p>
    <% if (Model.Count == 0)
       {%>
       <b>Currently there are no items allowed in your stocklist</b>
    <% }
       else
       { %>
    <ul>
    <% foreach (var k in Model.Keys)
       { %>
            <li>
            <%= k %> : <%= Model[k].Name %> = <%= Model[k].Amount %>
            </li>
    <% } %>
    </ul>
    <% } %>
    </div>
    </div>
    <div class="box left">
    <h3>Recent commands</h3>
    <ul>
        <% foreach (var e in (ViewData["RecentCommands"] as dynamic).Items as IEnumerable<dynamic>)
           { 
               string msgclass=e.Message=="OK"?"success":"failure";%>
            <li class="<%= msgclass %>" > <%=(e.Command as object).GetType().Name%>: <b><%= e.Message %></b>
                <%= Html.Dump(e.Command as object) %>
            </li>           
            <% } %>
    </ul>
    </div>
    <div class="box right">
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
