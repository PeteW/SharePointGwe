<%@ Page Language="C#" AutoEventWireup="true" ValidateRequest="false" CodeBehind="GweConfig.aspx.cs"
    Inherits="Oxbow.Gwe.SharePoint.GweConfig, Oxbow.Gwe.SharePoint, Version=1.0.0.0, Culture=neutral, PublicKeyToken=00490a8ac19f86b5"
    MasterPageFile="~/_layouts/v4.master" %>

<%@ Assembly Name="Oxbow.Gwe.Core, Version=1.0.0.0, Culture=Neutral, PublicKeyToken=0cd9d8500cf32c1c" %>
<%@ Import Namespace="Oxbow.Gwe.Core.Models" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<asp:Content ID="ContentMain" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <script type="text/javascript" src="/_layouts/Oxbow.Gwe/js/jquery-1.6.2.min.js"></script>
    <script type="text/javascript" src="/_layouts/Oxbow.Gwe/js/jquery-ui-1.8.16.custom.min.js"></script>
    <script type="text/javascript" src="/_layouts/Oxbow.Gwe/js/linq.min.js"></script>
    <script type="text/javascript" src="/_layouts/Oxbow.Gwe/js/codeMirror.js"></script>
    <script type="text/javascript" src="/_layouts/Oxbow.Gwe/js/codeMirror.javascript.js"></script>
    <script type="text/javascript" src="/_layouts/Oxbow.Gwe/js/codeMirror.xml.js"></script>
    <script type="text/javascript" src="/_layouts/Oxbow.Gwe/js/codeMirror.css.js"></script>
    <script type="text/javascript" src="/_layouts/Oxbow.Gwe/js/codeMirror.html.js"></script>
    <link href="/_layouts/Oxbow.Gwe/css/jquery-ui-1.8.16.custom.css" type="text/css"
        rel="stylesheet" />
    <link href="/_layouts/Oxbow.Gwe/css/codeMirror.css" type="text/css" rel="stylesheet" />
    <script type="text/javascript">
        function SetTransitionActionOrder() {
            $(".transitionActionContainers").each(function (j, u) {
                $(this).find(".sortableItemContainer").each(function (i, v) {
                    $(v).find("input[id*=hidWorkflowTransitionActionElementOrderId]").val(i);
                });
            });
        }
        function SetTimeTriggerOrder() {
            $(".timeTriggerContainers").each(function () {
                $(this).find(".sortableItemContainer").each(function (i, v) {
                    $(v).find("input[id*=hidTriggerOrderId]").val(i);
                });
            });
        }
        $(document).ready(function () {
            $(".prompt").click(function () {
                return confirm("Are you sure?");
            });
            $(".lnkToggleWorkflowTransitionActions").click(function () {
                var transitionId = $(this).attr("rel");
                var container = $(this).closest("fieldset").find(".transitionActionContainers");
                var transitionStateField = $("#<%=hidExpandedWorkflowTransitionIds.ClientID %>");
                var expandedTransitionIds = transitionStateField.val().split(",");
                if (container.is(":visible")) {
                    container.hide("300");
                    expandedTransitionIds = Enumerable.From(expandedTransitionIds).Where(function (x) { return x != transitionId; }).ToArray();
                } else {
                    container.show("300");
                    expandedTransitionIds.push(transitionId);
                }
                transitionStateField.val(expandedTransitionIds.join(","));
                return false;
            });
            $(".lnkToggleWorkflowConfigurationVariables").click(function () {
                var container = $(".dvWorkflowConfigurationVariables");
                if (container.is(":visible")) {
                    container.hide("300");
                    $("#<%=hidIsWorkflowConfigurationVariablesSectionVisible.ClientID %>").val("false");
                } else {
                    container.show("300");
                    $("#<%=hidIsWorkflowConfigurationVariablesSectionVisible.ClientID %>").val("true");
                }
            });
            $(".lnkAddWorkflowTransitionActionMenuToggle").click(function () {
                $(this).next().toggle(300);
                return false;
            });
            $(".lnkShowHelp").click(function () {
                $(".dvHelp").show("slow");
                return false;
            });
            $(".transitionActionContainers ul").sortable({
                stop: function (event, ui) {
                    SetTransitionActionOrder();
                }
            });
            $(".timeTriggerContainers ul").sortable({
                stop: function (event, ui) {
                    SetTimeTriggerOrder();
                }
            });
            $(".transitionActionContainers").each(function () {
                var transitionStateField = $("#<%=hidExpandedWorkflowTransitionIds.ClientID %>");
                if (transitionStateField.val().indexOf($(this).attr("rel")) == -1) {
                    $(this).hide();
                }
            });
            $(".dvWorkflowConfigurationVariables").toggle($("#<%=hidIsWorkflowConfigurationVariablesSectionVisible.ClientID %>").val() == "true");
            /*
            $(".htmlEditor").each(function(i, v) {
            CodeMirror.fromTextArea(v, {
            mode: "text/html",
            theme: "night"
            });
            });*/
        });
    </script>
    <style type="text/css">
        .formTable
        {
            width: 100%;
        }
        .dvHelp
        {
            display: none;
        }
        .hidden
        {
            display: none;
        }
        .labelCell
        {
            font-weight: bold;
            width: 150px;
        }
        .formField
        {
            width: 90%;
        }
        .transitionContainer
        {
            -moz-border-radius: 4px;
            -webkit-border-radius: 4px;
            -khtml-border-radius: 4px;
            border-radius: 4px;
            border: solid 1px #000;
            padding: 5px;
            margin: 5px;
            background-color: #ddd;
        }
        .transitionActionContainers ul li
        {
            list-style-image: none;
            list-style-type: none;
        }
        .timeTriggerContainers ul li
        {
            list-style-image: none;
            list-style-type: none;
        }
        .sortableItemContainer
        {
            -moz-border-radius: 4px;
            -webkit-border-radius: 4px;
            -khtml-border-radius: 4px;
            border-radius: 4px;
            border: solid 1px #000;
            padding: 5px;
            margin: 5px 0px 0px -40px;            
            background-color: #aad;
        }
        .transitionHeader
        {
            font-size: 1.5em;
        }
        .buttonHeader
        {
            text-align: right;
        }
        .workflowTransitionActionHeader
        {
            font-size: 1.1em;
            font-style: italic;
        }
        .buttonHeader input
        {
            width: 200px;
        }
        .helpTable tr td
        {
            border: 1px solid black;
            padding: 4px;
        }
        .fullWidth
        {
            width: 100%;
        }
        body.v4master {
           overflow: visible;
           height: inherit;
           width: inherit;
        }
        body #s4-workspace {
	        overflow: visible !important;
        }
        body #s4-ribbonrow {
	        position: fixed;
	        z-index: 1000;
        }
        #s4-ribbonrow .ms-MenuUIPopupBody, #s4-ribbonrow .ms-popoutMenu, .ms-cui-menu[id ^= "Ribbon."] {
	        position: fixed !important;
        }
        .ms-dlgOverlay {
	        width: 100% !important;
        }    
</style>
    <asp:HiddenField runat="server" ID="hidIsWorkflowConfigurationVariablesSectionVisible" />
    <asp:HiddenField ID="hidExpandedWorkflowTransitionIds" runat="server" />
    <asp:HyperLink ID="lnkBack" runat="server" />
    <asp:Panel ID="pnlErrors" runat="server" Visible="false">
        <div class="ui-state-error ui-corner-all" style="padding: 0pt 0.7em;">
            <p>
                <span class="ui-icon ui-icon-alert" style="float: left; margin-right: 0.3em;"></span>
                <pre>
<asp:Literal ID="ltrErrorMessage" runat="server" /></pre>
            </p>
        </div>
    </asp:Panel>
    <asp:Panel ID="pnlHighlights" runat="server" Visible="false">
        <div class="ui-state-highlight ui-corner-all" style="padding: 0pt 0.7em;">
            <p>
                <span class="ui-icon ui-icon-alert" style="float: left; margin-right: 0.3em;"></span>
                <asp:Literal ID="ltrHighlightMessage" runat="server" />
            </p>
        </div>
    </asp:Panel>
    <asp:Panel ID="pnlTimerResponse" runat="server" Visible="false">
        <div class="ui-state-highlight ui-corner-all" style="padding: 0pt 0.7em;">
            <asp:Repeater ID="rptTimerLogs" runat="server">
                <ItemTemplate>
                    <p>
                        <span class="ui-icon <%# GetIconClass(((AgentLogItem)Container.DataItem).Severity) %>"
                            style="float: left; margin-right: 0.3em;"></span>
                        <%# ((AgentLogItem)Container.DataItem).Time.ToString("g") %>
                        <%# ((AgentLogItem)Container.DataItem).Message %>
                    </p>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </asp:Panel>
    <div class="buttonHeader">
        <asp:Button ID="btnSave" Text="Save" runat="server" />
        <asp:Button ID="btnRunTimeTrigger" runat="server" Text="Run Timer" />
    </div>
    <fieldset>
        <legend>Workflow Configuration</legend>
        <table class="formTable">
            <tr>
                <td class="labelCell">
                    "Next Action"
                </td>
                <td>
                    <asp:TextBox ID="txtNextActionXpath" runat="server" CssClass="formField" />
                </td>
            </tr>
            <tr>
                <td class="labelCell">
                    Administrator to email
                </td>
                <td>
                    <asp:TextBox ID="txtAdminToEmail" runat="server" CssClass="formField" />
                </td>
            </tr>
            <tr>
                <td class="labelCell">
                    Administrator from email
                </td>
                <td>
                    <asp:TextBox ID="txtAdminFromEmail" runat="server" CssClass="formField" />
                </td>
            </tr>
            <tr>
                <td class="labelCell">
                    Administrator cc email
                </td>
                <td>
                    <asp:TextBox ID="txtAdminCcEmail" runat="server" CssClass="formField" />
                </td>
            </tr>
            <tr>
                <td class="labelCell">
                    Gwe Activated
                </td>
                <td>
                    <asp:CheckBox ID="chkRegisterEventReceiver" runat="server" AutoPostBack="true" />
                </td>
            </tr>
        </table>
        <a href="#" class="lnkShowHelp">Help</a>
        <div class="dvHelp">
            <table class="formTable helpTable">
                <tr>
                    <th>
                        Function
                    </th>
                    <th>
                        Description
                    </th>
                </tr>
                <tr>
                    <td>
                        !getmanagerforuser([username],[domain])
                    </td>
                    <td>
                        returns the login name of the manager of the supplied identity. Requires User Prfolie
                        features, will not work on SharePoint foundation.
                    </td>
                </tr>
                <tr>
                    <td>
                        !resolveuserfullname([username],[domain])
                    </td>
                    <td>
                        returns the full name of the of the supplied identity.
                    </td>
                </tr>
                <tr>
                    <td>
                        !resolveuseremail([username],[domain])
                    </td>
                    <td>
                        returns the email address of the of the supplied identity.
                    </td>
                </tr>
                <tr>
                    <td>
                        !xpath([expr])
                    </td>
                    <td>
                        parses the form and evaluates the xpath expression
                    </td>
                </tr>
                <tr>
                    <td>
                        !evalxpath([expr])
                    </td>
                    <td>
                        evaluates the xpath expression against the form
                    </td>
                </tr>
                <tr>
                    <td>
                        !htmlencode([a])
                    </td>
                    <td>
                        html encodes the expression
                    </td>
                </tr>
                <tr>
                    <td>
                        !htmldecode([a])
                    </td>
                    <td>
                        html decodes the expression
                    </td>
                </tr>
                <tr>
                    <td>
                        !getclientformurl()
                    </td>
                    <td>
                        returns the direct url of the list item
                    </td>
                </tr>
                <tr>
                    <td>
                        !getbrowserformurl([url])
                    </td>
                    <td>
                        returns the url to view the list item in the web browser. The url parameter specified
                        where to redirect after the form is closed. If the url parameter ="" then it will
                        leave a blank screen.
                    </td>
                </tr>
                <tr>
                    <td>
                        !if([x1],[x2],[x3])
                    </td>
                    <td>
                        if x1=true or 1 then return x2. if x1=false or 0 then return x3. if neither are
                        true this is an error.
                    </td>
                </tr>
                <tr>
                    <td>
                        !and([x1]...[xn])
                    </td>
                    <td>
                        Logical AND. returns true if all operands evaluate to true and/or 1. Requires at
                        least one operand. Employs lazy evaluation.
                    </td>
                </tr>
                <tr>
                    <td>
                        !or([x1]...[xn])
                    </td>
                    <td>
                        Logical OR. returns true if any operand evaluate to true and/or 1. Requires at least
                        one operand. Employs lazy evaluation.
                    </td>
                </tr>
                <tr>
                    <td>
                        !listitemfield([fieldnameexpr])
                    </td>
                    <td>
                        returns the value from the list item given an expression that evaluates a valid
                        field name.
                    </td>
                </tr>
                <tr>
                    <td>
                        !iscontenttype([contenttypename])
                    </td>
                    <td>
                        returns TRUE if the list item is associated with a content type matching the supplied
                        name.
                    </td>
                </tr>
                <tr>
                    <td>
                        !not([a])
                    </td>
                    <td>
                        negates the expression of [a]
                    </td>
                </tr>
                <tr>
                    <td>
                        !equals([a],[b])
                    </td>
                    <td>
                        returns TRUE if the value of expression a equals value of expression b. (trimmed
                        and case insensitive comparison)
                    </td>
                </tr>
                <tr>
                    <td>
                        !notequals([a],[b])
                    </td>
                    <td>
                        returns FALSE if the value of expression a equals value of expression b. (trimmed
                        and case insensitive comparison)
                    </td>
                </tr>
                <tr>
                    <td>
                        !contains([a],[b])
                    </td>
                    <td>
                        returns TRUE if the value of expression a contains value of expression b. (trimmed
                        and case insensitive comparison)
                    </td>
                </tr>
                <tr>
                    <td>
                        !concatenate([x1]...[xn])
                    </td>
                    <td>
                        concatenates the expressions together
                    </td>
                </tr>
                <tr>
                    <td>
                        !currentsiteurl()
                    </td>
                    <td>
                        returns the site url containing the item (with the slash at the end eg http://mysite/)
                    </td>
                </tr>
                <tr>
                    <td>
                        !currentweburl()
                    </td>
                    <td>
                        returns the web url containing the item (with the slash at the end eg http://mysite/myweb/)
                    </td>
                </tr>
                <tr>
                    <td>
                        !add([x1], [x2])
                    </td>
                    <td>
                        returns the result of adding x1 and x2 together. note that both operands must be
                        numeric types
                    </td>
                </tr>
                <tr>
                    <td>
                        !addDate([dateval], [amount], [interval])
                    </td>
                    <td>
                        performs date manipulation. the dateval is a date, the amount is a number (positive
                        or negative) and the interval is a value from the following set: [hours, minutes,
                        seconds, days]
                    </td>
                </tr>
                <tr>
                    <td>
                        !now()
                    </td>
                    <td>
                        returns the date now in the native infopath date time format yyyy-MM-ddTHH:mm:ss
                    </td>
                </tr>
                <tr>
                    <td>
                        !today()
                    </td>
                    <td>
                        returns the date now in the native infopath date format yyyy-MM-dd
                    </td>
                </tr>
                <tr>
                    <td>
                        !var(x)
                    </td>
                    <td>
                        Performs variable substitution. Define a variable in the variables section, give
                        it a name and a value. You can then reference this variable anywhere by using its
                        name within the !var function.
                    </td>
                </tr>
            </table>
        </div>
        <fieldset>
            <legend><a href="#" class="lnkToggleWorkflowConfigurationVariables">Variables</a></legend>
            <div class="dvWorkflowConfigurationVariables">
                <div class="buttonHeader">
                    <asp:Button runat="server" ID="btnAddWorkflowConfigurationVariable" Text="+ Add Variable..." />
                </div>
                <table class="formTable fullWidth">
                    <thead>
                        <tr>
                            <th class="labelCell">
                                Name
                            </th>
                            <th>
                                Value
                            </th>
                            <th width="50">
                                &nbsp;
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater runat="server" ID="rptWorkflowConfigurationVariables">
                            <ItemTemplate>
                                <tr>
                                    <td class="labelCell">
                                        <asp:HiddenField runat="server" ID="hidWorkflowConfigurationVariableId" Value='<%#Eval("Id") %>' />
                                        <asp:TextBox CssClass="fullWidth" runat="server" ID="txtWorkflowConfigurationVariableName" Text='<%#Eval("Name") %>' />
                                    </td>
                                    <td>
                                        <asp:TextBox CssClass="fullWidth" runat="server" ID="txtWorkflowConfigurationVariableValue" Text='<%#Eval("Value") %>' />
                                    </td>
                                    <td width="50">
                                        <asp:Button ID="btnDeleteWorkflowConfigurationVariable" runat="server" CssClass="prompt"
                                            Text="x" CommandName="Delete" CommandArgument='<%#Eval("Id") %>' />
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
            </div>
        </fieldset>
        <fieldset>
            <legend>Time triggers</legend>
            <div class="buttonHeader">
                <asp:Button ID="btnAddTimeTrigger" runat="server" Text="+ Add Time Trigger..." />
            </div>
            <div class="timeTriggerContainers">
                <ul>
                    <asp:Repeater ID="rptTimeTriggers" runat="server">
                        <ItemTemplate>
                            <li class="sortableItemContainer" style="margin-left: -35px;">
                                <table class="formTable fullWidth">
                                    <tr>
                                        <td>
                                            Trigger Alias
                                        </td>
                                        <td>
                                            View Name
                                        </td>
                                        <td>
                                            Transition To Execute
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:HiddenField ID="hidTriggerOrderId" runat="server" Value='<%#Eval("OrderId") %>' />
                                            <asp:HiddenField ID="hidTriggerId" runat="server" Value='<%#Eval("Id") %>' />
                                            <asp:TextBox ID="txtTriggerName" CssClass="fullWidth" runat="server" Text='<%#Eval("Name") %>' />
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" CssClass="fullWidth" ID="txtViewName" Text='<%#Eval("ViewName") %>' />
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtTransitionToExecute" CssClass="fullWidth" runat="server" Text='<%#Eval("TransitionToExecute") %>' />
                                        </td>
                                        <td>
                                            <asp:Button ID="btnDeleteTimeTrigger" runat="server" CssClass="prompt" Text="x" CommandName="Delete"
                                                CommandArgument='<%#Eval("Id") %>' />
                                        </td>
                                    </tr>
                                </table>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
            </div>
        </fieldset>
        <fieldset>
            <legend>Transitions</legend>
            <div class="buttonHeader">
                <asp:Button ID="btnAddTransition" Text="+ New Transition..." runat="server" />
            </div>
            <asp:Repeater ID="rptTransitions" runat="server">
                <ItemTemplate>
                    <div class="transitionContainer">
                        <asp:HiddenField ID="hidTransitionId" runat="server" Value='<%#Eval("Id") %>' />
                        <asp:HiddenField ID="hidTransitionName" runat="server" Value='<%#Eval("Name") %>' />
                        <table class="formTable" style="width: 100%">
                            <tr>
                                <td>
                                    <asp:TextBox ID="txtTransitionName" CssClass="formField transitionHeader" runat="server"
                                        Text='<%#Eval("Name") %>' />
                                </td>
                                <td align="right" style="text-align: right">
                                    <div style="float: right">
                                        <asp:Button ID="btnRemoveTransition" runat="server" Text="X" CommandArgument='<%#Eval("Id")%>'
                                            CommandName="Delete" CssClass="prompt" />
                                    </div>
                                </td>
                            </tr>
                        </table>
                        <fieldset>
                            <legend><a href="#" class="lnkToggleWorkflowTransitionActions" rel="<%#Eval("Id") %>">
                                Actions</a> </legend>
                            <div class="transitionActionContainers" rel="<%#Eval("Id") %>">
                                <div class="buttonHeader">
                                    <a href="#" class="lnkAddWorkflowTransitionActionMenuToggle">Add Action... </a>
                                    <div class="hidden">
                                        <asp:DropDownList ID="ddWorkflowTransitionActionTypes" runat="server" />
                                        <asp:Button ID="btnAddWorkflowTransitionAction" runat="server" Text="Add" CommandName="AddAction"
                                            CommandArgument='<%#Eval("Id") %>' />
                                    </div>
                                </div>
                                <ul>
                                    <asp:Repeater ID="rptActions" runat="server" OnItemDataBound="rptActions_ItemDataBound"
                                        OnItemCommand="rptActions_ItemCommand">
                                        <ItemTemplate>
                                            <li class="sortableItemContainer">
                                                <asp:HiddenField ID="hidWorkflowTransitionActionElementId" runat="server" />
                                                <asp:HiddenField ID="hidWorkflowTransitionActionElementOrderId" runat="server" />
                                                <asp:HiddenField ID="hidWorkflowTransitionActionElementName" runat="server" />
                                                <div>
                                                    <span class="workflowTransitionActionHeader">
                                                        <asp:Literal ID="ltrWorkflowTransitionActionElementHeader" runat="server" />
                                                    </span>
                                                    <div style="float: right" class="buttonHeader">
                                                        <asp:Button ID="btnDelete" runat="server" CommandArgument='<%#Eval("Id") %>' CommandName="Delete"
                                                            Text="X" CssClass="prompt" />
                                                    </div>
                                                    <table class="formTable">
                                                        <tr>
                                                            <td class="labelCell">
                                                                Alias
                                                            </td>
                                                            <td>
                                                                <asp:TextBox CssClass="formField" ID="txtWorkflowTransitionActionElementName" runat="server"
                                                                    Text='<%#Eval("Name") %>' />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="labelCell">
                                                                Condition
                                                            </td>
                                                            <td>
                                                                <asp:TextBox CssClass="formField" ID="txtWorkflowTransitionActionElementExecuteConditionExpression"
                                                                    runat="server" Text='<%#Eval("ExecuteConditionExpression") %>' />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="labelCell">
                                                                Halt on failure?
                                                            </td>
                                                            <td>
                                                                <asp:CheckBox ID="chkWorkflowTransitionActionHaltOnFailure" runat="server" Checked='<%#Eval("HaltOnFailure") %>' />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                                <asp:Panel ID="pnlWorkflowTransitionActionContainer" runat="server" />
                                            </li>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </ul>
                            </div>
                        </fieldset>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </fieldset>
    </fieldset>
    <fieldset>
        <legend>Xml</legend>
        <asp:TextBox Width="100%" TextMode="MultiLine" Rows="20" ID="ltrXml" runat="server" />
        <asp:Button runat="server" ID="btnImportXml" Text="Import" />
    </fieldset>
</asp:Content>
