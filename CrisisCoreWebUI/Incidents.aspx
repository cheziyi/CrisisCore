<%@ Page Title="Incidents" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Incidents.aspx.cs" Inherits="CrisisCoreWebUI.Incidents" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <%-- Display of success or error message --%>
    <asp:Panel ID="successMsg" class="alert alert-success fade in" runat="server" Visible="false">
        <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
        <strong>
            <asp:Literal ID="litStatus" runat="server"></asp:Literal></strong>
        <asp:Literal ID="litMessage" runat="server"></asp:Literal>
    </asp:Panel>
    <div class="row">
        <div class="well col-lg-6">
            <%-- Form display for user to input new incident information --%>
            <div class="form-horizontal">
                <fieldset>
                    <legend>Add New Incident</legend>
                    <div class="form-group">
                        <label for="ddlIncidenType" class="col-lg-3 control-label">Incident Type:</label>
                        <div class="col-lg-9">
                            <asp:DropDownList ID="ddlIncidenType" runat="server" class="form-control"></asp:DropDownList>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="txtName" class="col-lg-3 control-label">Name:</label>
                        <div class="col-lg-9">
                            <%-- Textbox for reporter's name with required validation --%>
                            <asp:RequiredFieldValidator ID="requiredName" runat="server" ErrorMessage="Please enter the reporter's name" ControlToValidate="txtName" Display="Dynamic" ValidationGroup="AddIncident"></asp:RequiredFieldValidator>
                            <asp:TextBox ID="txtName" runat="server" class="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="txtMobile" class="col-lg-3 control-label">Mobile No.:</label>
                        <div class="col-lg-9">
                            <%-- Textbox for reporter's mobile number with required validation and regex validation (8 numeric characters starting with 8 or 9) --%>
                            <asp:RequiredFieldValidator ID="requiredMobile" runat="server" ErrorMessage="Please enter a mobile number" ControlToValidate="txtMobile" Display="Dynamic" ValidationGroup="AddIncident"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="regexMobile" runat="server" ErrorMessage="Please enter a valid mobile number" ControlToValidate="txtMobile" Display="Dynamic" ValidationGroup="AddIncident" ValidationExpression="[89][0-9]{7}"></asp:RegularExpressionValidator>
                            <asp:TextBox ID="txtMobile" runat="server" class="form-control" type="number"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="txtPostal" class="col-lg-3 control-label">Postal Code:</label>
                        <div class="col-lg-9">
                            <%-- Textbox for reporter's postal code with required validation and regex validation (6 numeric characters) --%>
                            <asp:RequiredFieldValidator ID="requiredPostal" runat="server" ErrorMessage="Please enter a postal code" ControlToValidate="txtPostal" Display="Dynamic" ValidationGroup="AddIncident"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="regexPostal" runat="server" ErrorMessage="Please enter a valid postal code" ControlToValidate="txtPostal" Display="Dynamic" ValidationGroup="AddIncident" ValidationExpression="[0-9]{6}"></asp:RegularExpressionValidator>
                            <asp:TextBox ID="txtPostal" runat="server" class="form-control" type="number"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="txtUnitNo" class="col-lg-3 control-label">Unit No.:</label>
                        <div class="col-lg-9">
                            <%-- Textbox for reporter's unit number with required validation and regex validation (in the format #12-34 up to #12-3456) --%>
                            <asp:RegularExpressionValidator ID="regexUnitNo" runat="server" ErrorMessage="Please enter a valid unit number" ControlToValidate="txtUnitNo" Display="Dynamic" ValidationGroup="AddIncident" ValidationExpression="#[0-9]{2}-[0-9]{2,4}"></asp:RegularExpressionValidator>
                            <asp:TextBox ID="txtUnitNo" runat="server" class="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="txtAddInfo" class="col-lg-3 control-label">Additional Info:</label>
                        <div class="col-lg-9">
                            <asp:TextBox ID="txtAddInfo" runat="server" class="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-lg-9 col-lg-offset-3">
                            <%-- Submit button with validation on the above textboxes in the same validation group --%>
                            <asp:Button ID="btnSubmit" class="btn btn-primary" runat="server" Text="Submit" OnClick="btnSubmit_Click" ValidationGroup="AddIncident" />
                        </div>
                    </div>
                </fieldset>
            </div>
        </div>
    </div>
    <%-- Table display for the information on unresolved incidents --%>
    <div class="row">
        <div class="col-lg-12">
            <fieldset>
                <legend>All Unresolved Incidents</legend>
                <table class="table table-striped table-hover ">
                    <thead>
                        <tr>
                            <th>Incident Type</th>
                            <th>Name</th>
                            <th>Mobile No.</th>
                            <th>Postal Code</th>
                            <th>Unit No.</th>
                            <th>Reported Time</th>
                            <th>Resolve Incident?</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:PlaceHolder ID="plcTable" runat="server"></asp:PlaceHolder>
                    </tbody>
                </table>
            </fieldset>
        </div>
    </div>
    <%-- Embedded JavaScript --%>
    <script>
        // Prompt for resolve confirmation.
        function ResolveIncidentConfirmation() {
            return confirm("Are you sure you want to resolve this incident?");
        }
    </script>
</asp:Content>
