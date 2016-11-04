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
                            <asp:TextBox ID="txtName" runat="server" class="form-control" required></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="txtMobile" class="col-lg-3 control-label">Mobile No.:</label>
                        <div class="col-lg-9">
                            <asp:TextBox ID="txtMobile" runat="server" class="form-control" type="number" MaxLength="8" required></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="txtPostal" class="col-lg-3 control-label">Postal Code:</label>
                        <div class="col-lg-9">
                            <asp:TextBox ID="txtPostal" runat="server" class="form-control" type="number" MaxLength="6" required></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="txtUnitNo" class="col-lg-3 control-label">Unit No.:</label>
                        <div class="col-lg-9">
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
                            <asp:Button ID="btnSubmit" class="btn btn-primary" runat="server" Text="Submit" OnClick="btnSubmit_Click" />
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
