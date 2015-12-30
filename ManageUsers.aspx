<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="ManageUsers.aspx.cs" Inherits="ManageUsers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
  <script src='<%=ResolveClientUrl("~/Scripts/manage-users.js") %>'></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
  <asp:LoginView ID="LVContent" runat="server">
    <RoleGroups>
      <asp:RoleGroup Roles="administrator">
        <ContentTemplate>
          <main class="col-xs-8 col-xs-offset-2">
            <table class="table">
              <thead>
                <tr class="table-actions">
                  <td>User</td>
                  <td class="u-text-end">Editor</td>
                  <td class="u-text-end">Delete</td>
                </tr>
              </thead>
              <tbody>
                <asp:Repeater ID="RNonAdministratorUsers" runat="server">
                  <ItemTemplate>
                    <tr>
                      <td>
                        <%# DataBinder.GetDataItem(Container) %>
                      </td>
                      <td id="CheckboxTd" runat="server" class="u-text-end">
<%--                        <asp:CheckboxWithoutSpan ID="CBEditor" runat="server" CssClass="toggle"
                          OnCheckedChanged="EditorToggle_ServerChange"
                          Checked='<%# Roles.GetRolesForUser((string)DataBinder.GetDataItem(Container))
                                            .Contains("publisher") %>'></asp:CheckboxWithoutSpan>--%>
                        <input type="checkbox" id="EditorToggle" runat="server"
                          class="toggle" data-user-name='<%# DataBinder.GetDataItem(Container) %>'
                          checked='<%# Roles.GetRolesForUser((string)DataBinder.GetDataItem(Container))
                                            .Contains("publisher") %>' />
                      </td>
                      <td class="u-text-end">
                        <button id="DeleteButton" runat="server" class="btn btn-link"
                        data-user-name='<%# DataBinder.GetDataItem(Container) %>'
                        onserverclick="DeleteButton_ServerClick">
                          <i class="fa fa-times fa-2x text-danger"></i>
                        </button>
                      </td>
                    </tr>
                  </ItemTemplate>
                </asp:Repeater>
              </tbody>
            </table>
          </main>

          <asp:Button ID="BInvisibleSubmit" runat="server"
            OnClick="SubmitChange" 
            CssClass="manage-users-invisible-submit-button u-is-hidden"></asp:Button>

          <input id="HiddenUserName" runat="server" class="hidden-user-name" type="hidden" />
          <input id="HiddenRoleValue" runat="server" class="hidden-role-value" type="hidden" />
          <input id="HiddenToggleValue" runat="server" class="hidden-toggle-value" type="hidden" />

        </ContentTemplate>
      </asp:RoleGroup>
    </RoleGroups>
    <LoggedInTemplate>
      <div class="notification">
        <p class="lead">You don't have the permission to access this page.</p>
      </div>
    </LoggedInTemplate>
    <AnonymousTemplate>
      <div class="notification">
        <p class="lead">You don't have the permission to access this page.</p>
      </div>
    </AnonymousTemplate>
  </asp:LoginView>
</asp:Content>

