<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="EditArticle.aspx.cs" Inherits="EditArticle" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
  <script src='<%=ResolveClientUrl("~/Scripts/validate-article-content.js")%>'></script>
  <script src='<%=ResolveClientUrl("~/Scripts/edit-article.js")%>'></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
  <% if (GoodId) { %>
    <% if (CurrentUserHasThePermissionToUpdateArticle) { %>
      <div class="text-danger" style="margin-left: 40px; width: calc(100% - 80px);">
        <p class="lead">
          <asp:Literal ID="LAnswer" runat="server"></asp:Literal>
        </p>
      </div>

      <% if (ShowPage) { %>
        <div class="article-title-wrapper">
          <asp:RequiredFieldValidator ID="RFVTitle" runat="server" ErrorMessage="The title can't be empty." ControlToValidate="TBTitle" CssClass="text-danger"></asp:RequiredFieldValidator>
          <asp:TextBox ID="TBTitle" runat="server" 
            placeholder="My Title" CssClass="article-title-input">
          </asp:TextBox>
        </div>

        <div id="CategoriesDiv" runat="server" class="categories-read-only">
          <label>Categories: </label>
          <asp:Repeater ID="RCategories" runat="server">
            <ItemTemplate>
              <span id="category" runat="server"
              data-id='<%# ((Tuple<int,string,bool>)DataBinder.GetDataItem(Container)).Item1 %>'
              data-accepted='<%# ((Tuple<int,string,bool>)DataBinder.GetDataItem(Container)).Item3 %>'>
                <%# ((Tuple<int,string,bool>)DataBinder.GetDataItem(Container)).Item2 %>
              </span>
            </ItemTemplate>
            <SeparatorTemplate>
              <span>,</span>
            </SeparatorTemplate>
          </asp:Repeater>
        </div>

      <div class="article-content-wrapper">
        <asp:TextBox ID="TBPreview" TextMode="MultiLine"  Columns="80" Rows="2" 
          runat="server" placeholder="Here goes the preview text for my article. (Will appear only on the index page)" 
          CssClass="preview-text-area" ></asp:TextBox>
      </div>

      <div class="form remote-source-group content-and-source-validator-wrapper">
        <div class="remote-cbox">
          <label><asp:CheckBox ID="CBRemote" runat="server"/> Use an external source</label>
        </div>
        <span class="content-and-source-validator text-danger u-inline-block"></span>
        <asp:TextBox ID="TBSource" runat="server" CssClass="form-control source-input u-is-hidden"
          placeholder="Insert the url for aricle..."></asp:TextBox>
      </div>

      <div class="article-content-wrapper content">
        <asp:TextBox ID="TBArticle" TextMode="MultiLine"  Columns="80" Rows="8" 
          runat="server" placeholder="Here goes my text." CssClass="article-text-area" ></asp:TextBox>
      </div>

      <div class="u-text-center  u-margin-Tl">
        <asp:Button ID="BSubmit" runat="server" Text="Submit Article" 
          CssClass="btn btn-primary u-bg-color-ink u-color-gray"
          OnClientClick="return validatePage()" OnClick="BSubmit_Click" />
      </div>

      <% } %>
    <% } else { %>
    <div class="notification">
      <p class="lead">You don't have the permission to modify the requested article.</p>
    </div>
    
    <% } %>
  <% } else { %>
    <div class="notification">
      <p class="lead">No existent article with the specified id.</p>
    </div>

  <% } %>
</asp:Content>

