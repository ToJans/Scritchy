<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<Example.Domain.Readmodel.StockDictionary>" %>

<%@ Import Namespace="Example.Web.Helpers" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Scritchy - CQRS without the plumbing</title>
    <style type="text/css">
        h1
        {
            display: block;
            padding-left: 90px;
            background-image: url("/content/scritchy-title.png");
            background-repeat: no-repeat;
            background-position: 10px 10px;
            background-color: White;
            font-size: 80px;
            text-indent: 3000px;
            border: 2px black solid;
            height: 120px;
            padding-top: 0px;
            border-radius: 5px;
            -moz-border-radius: 5px;
            -webkit-border-radius: 5px;
        }
        
        .rounded
        {
            border-radius: 5px;
            -moz-border-radius: 5px;
            -webkit-border-radius: 5px;
        }
        
        body
        {
            margin: 0;
            padding: 10px;
            padding-top: 0px;
            font-family: Segoe UI;
        }
        
        h1, h2, h3, h4, h5, h6
        {
            font-family: Segoe Script;
        }
        
        h2, h3, h4, h5, h6
        {
            color: White;
            background-color: #333D88;
            border-radius: 5px;
            -moz-border-radius: 5px;
            -webkit-border-radius: 5px;
        }
        
        .middle
        {
            color: Black;
            background: #8088C3;
            margin: 0;
            border-left: #333D88 260px solid;
            border-right: #333D88 260px solid;
            padding: 10px;
            min-height: 1000px;
        }
        
        .box
        {
            background: #6873c3;
            border: black 1px solid;
            position: fixed;
            top: 30px;
            width: 220px;
            padding: 10px;
            border-radius: 5px;
            -moz-border-radius: 5px;
            -webkit-border-radius: 5px;
        }
        
        .left
        {
            left: 20px;
        }
        
        .right
        {
            right: 20px;
        }
        
        label
        {
            display: inline-block;
            width: 80px;
        }
        
        form, .left ul
        {
            background: white;
            padding: 5px;
            margin-top: 0px;
            border-radius: 5px;
            -moz-border-radius: 5px;
            -webkit-border-radius: 5px;
        }
        
        .left ul
        {
            padding-left: 1.5em;
        }
        
        
        
        input
        {
            display: inline-block;
            width: 120px;
        }
        .success
        {
            color: Green;
        }
        
        .failure
        {
            color: Red;
        }
        
        .stocklist
        {
            background-color: White;
            width: 70%;
            display: inline-block;
            border: 1px black solid;
            padding: 10px;
            border-radius: 5px;
            -moz-border-radius: 5px;
            -webkit-border-radius: 5px;
        }
        
        .stocklist > h3
        {
            text-decoration: underline;
        }
        h4 a
        {
            color: White;
        }
        em
        {
            font-style: normal;
            background-color: Gray;
        }
        strong
        {
            background-color: Yellow;
        }
    </style>
</head>
<body>
    <div class="middle">
        <h1>
            Scritchy</h1>
        <h2>
            CQRS without the plumbing - Example app</h2>
        <a href="#quickstart"><strong>!!! Click here to go to the quick start !!! </strong>
        </a>
        <p>
            This example app shows how you can implement a CQRS app without to much effort using
            the Scritchy libraries.
        </p>
        <p>
            More info and full source over at <a href="http://github.com/ToJans/Scritchy" target="_blank">
                github</a>.
        </p>
        <p>
            You find the example code in the following locations:
        </p>
        <ul>
            <li><i>Domain logic</i> in the <a href="https://github.com/ToJans/Scritchy/blob/master/Example/Domain/StockItem.cs"
                target="_blank">Example.Domain.StockItem class</a>. </li>
            <li><i>Commands</i> in the <a href="https://github.com/ToJans/Scritchy/tree/master/Example/Domain/Commands"
                target="_blank">Example.Domain.Commands folder</a>. </li>
            <li><i>Events</i> in the <a href="https://github.com/ToJans/Scritchy/tree/master/Example/Domain/Events"
                target="_blank">Example.Domain.Events folder</a>. </li>
            <li><i>View builder</i> in the <a href="https://github.com/ToJans/Scritchy/blob/master/Example/Domain/Readmodel/StockDictionary.cs"
                target="_blank">Example.Domain.Readmodel.StockDictionaryHandler class</a>. </li>
        </ul>
        <p>
            No infrastructure setup is required; Scritchy finds out by itself what the commands/events/ar's
            and eventhandlers are.
        </p>
        <p>
            Eventstorage is also auto-wired; Scrichy looks for a connectionstring named "eventstore"
            in the app configuration, and if it can not find it, it uses the InMemoryEventStore.<br />
            Currently works with SQLite and MsSql(CE), but other implementations should be trivial.
        </p>
        <p>
            Enjoy!
        </p>
        <p>
            <a href="http://twitter.com/#/ToJans" target="_blank">ToJans@Twitter</a>
        </p>
        <div class="stocklist">
            <h3>
                This is the example of a stocklist</h3>
            <% if (Model.Count == 0)
               {%>
            <p>
                <b>Currently there are no items allowed in your stocklist</b>
            </p>
            <% }
               else
               { %>
            <ul>
                <% foreach (var k in Model.Keys)
                   { %>
                <li>
                    <%= k %>
                    :
                    <%= Model[k].Name %>
                    =
                    <%= Model[k].Amount %>
                </li>
                <% } %>
            </ul>
            <% } %>
        </div>
        <hr />
        <p>
        </p>
        <a name="quickstart">Quick start</a>
        <h3>
            Develop a working CQRS app on your own in a few minutes</h3>
        <ol>
            <li>Start a new Project.</li>
            <li>Add <em>Scritchy</em> to the references using Nuget.</li>
            <li>Implement your domain using the following conventions:
                <ul>
                    <li>All aggregate roots must inherit from <em>Scritchy.Domain.AR</em>.</li>
                    <li>Commands and events are routed to an AR which contains a method <em>void [Command.GetType().Name]</em>
                        and where the Command contains a property named <em>[AR.GetType().Name+"Id"]</em>.</li>
                    <li>Events are routed to classes that contain a method <em>void ["On"+Event.GetType().Name]</em>.</li>
                    <li>Commands and event members are passed into the caller method by matching their property
                        name.</li>
                    <li>Remark: all event handlers must be stateless; sagas can be implemented as event handlers.</li>
                </ul>
            </li>
            <li>Configure the eventstore of your choice:
                <ul>
                    <li>In memory persistence: do nothing.</li>
                    <li>Database persistence: add a connectionstring named <em>"eventstore"</em> to your
                        web/app.config, and make sure the database is available and the required DLL's are referenced as well (i.e. <em>System.Data.SQLite</em>
                        or <em>System.Data.SqlServerCe</em> for example).</li>
                    <li>Remark: installing Scritchy using NuGet automatically adds an example SQLite and
                        MsSqlCe database to your App_data folder. you can use these files as your database.
                    </li>
                </ul>
            </li>
            <li>Create your command bus:
                <ol>
                    <li><em>var bus = new ScritchyBus();</em><br />
                        This will create a new commandbus and will auto-wire all AR's, EventHandlers, Commands.</li>
                    <li>Remark: Event handlers are instantiated trough DI, which can be set in the ScritchyBus
                        constructor.</li>
                </ol>
            </li>
            <li>You can now send commands to the bus:
                <ul>
                    <li><em>bus.RunCommand(new somecommand);</em></li>
                </ul>
            </li>
        </ol>
    </div>
    <div class="box left">
        <h3>
            Recent commands</h3>
        <ul>
            <% foreach (var e in (ViewData["RecentCommands"] as dynamic).Items as IEnumerable<dynamic>)
               {
                   string msgclass = e.Message == "OK" ? "success" : "failure";%>
            <li class="<%= msgclass %>">
                <%=(e.Command as object).GetType().Name%>: <b>
                    <%= e.Message %></b>
                <%= Html.Dump(e.Command as object) %>
            </li>
            <% } %>
        </ul>
    </div>
    <div class="box right">
        <h3>
            Commands</h3>
        <h4>
            Allow Item</h4>
        <% using (Html.BeginForm("Command", "Bus"))
           { %>
        <input type="hidden" name="commandtype" value="<%=typeof(Example.Domain.Commands.AllowItem).FullName %>" />
        <label for="StockItemId">
            Id</label>
        <input type="text" name="StockItemId" value="item/1" />
        <label for="Name">
            Name</label>
        <input type="text" name="Name" value="Item 1" />
        <button>
            Apply</button>
        <% }%>
        <h4>
            Ban Item</h4>
        <% using (Html.BeginForm("Command", "Bus"))
           { %>
        <input type="hidden" name="commandtype" value="<%=typeof(Example.Domain.Commands.BanItem).FullName %>" />
        <label for="StockItemId">
            Id</label>
        <input type="text" name="StockItemId" value="item/1" />
        <button>
            Apply</button>
        <% }%>
        <h4>
            Add Items</h4>
        <% using (Html.BeginForm("Command", "Bus"))
           { %>
        <input type="hidden" name="commandtype" value="<%=typeof(Example.Domain.Commands.AddItems).FullName %>" />
        <label for="StockItemId">
            Id</label>
        <input type="text" name="StockItemId" value="item/1" />
        <label for="Amount">
            Amount</label>
        <input type="text" name="Amount" value="3" />
        <button>
            Apply</button>
        <% }%>
        <h4>
            Remove Items</h4>
        <% using (Html.BeginForm("Command", "Bus"))
           { %>
        <input type="hidden" name="commandtype" value="<%=typeof(Example.Domain.Commands.RemoveItems).FullName %>" />
        <label for="StockItemId">
            Id</label>
        <input type="text" name="StockItemId" value="item/1" />
        <label for="Amount">
            Amount</label>
        <input type="text" name="Amount" value="3" />
        <button>
            Apply</button>
        <% }%>
    </div>
</body>
</html>
