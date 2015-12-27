<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Index.aspx.cs" Inherits="Index" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
  <aside class="col-xs-3">
    <ul id="CategoriesList" runat="server" class="col-xs-10"></ul>
    <div class="col-xs-2"> </div>
  </aside>

  <main class="col-xs-9">
    <div class="u-margin-Ll">
      <p class="lead text-danger">
        <asp:Literal ID="LAnswer" runat="server"></asp:Literal>
      </p>
    </div>

    <h2 class="category-title"><%= this.CategoryName %></h2>

    <%-- ConnectionString and Select Command in code behind --%>
    <asp:SqlDataSource ID="SDSArticles" runat="server"></asp:SqlDataSource>

    <asp:Repeater ID="RArticles" runat="server" DataSourceID="SDSArticles" OnItemDataBound="ItemBound">
      <ItemTemplate>
        <div class="article-wrapper">
          <div runat="server" class="edit-panel" 
          visible='<%# currentUserIsAdministrator()
            || ( currentUserIsPublisher() 
               && Membership.GetUser().ProviderUserKey.ToString()
                 == DataBinder.Eval(Container.DataItem, "PublisherId").ToString()) %>'>
            <asp:HyperLink ID="HLEdit" runat="server" NavigateUrl='<%# "~/EditArticle.aspx?Id=" + DataBinder.Eval(Container.DataItem, "Id").ToString() %>'>
              <i class="fa fa-edit fa-lg"></i>
            </asp:HyperLink>
            <button id="DeleteButton" runat="server"
              data-article-id='<%# DataBinder.Eval(Container.DataItem, "Id")  %>'
              onserverclick="DeleteButton_ServerClick"
              class="btn btn-link del-article-button u-margin-Lxxs u-padding-An">
              <i class="fa fa-times fa-lg text-danger"></i>
            </button>
          </div>

          <h3 class="u-margin-Bm u-wrap-true"><%# DataBinder.Eval(Container.DataItem, "Title") %></h3>
          <div class="content-wrapper">

            <asp:Repeater ID="ParagraphsRepeater" runat="server">
              <ItemTemplate>
                <p><%# DataBinder.GetDataItem(Container) %></p>
              </ItemTemplate>
            </asp:Repeater>
          </div>
          <asp:HyperLink ID="HLReadMore" NavigateUrl='<%# "~/Article.aspx?Id=" + DataBinder.Eval(Container.DataItem, "Id") %>' 
            runat="server">Read more.</asp:HyperLink>
        </div>
      </ItemTemplate>
    </asp:Repeater>

    <div class="pagination-component">
      <ul class="pager">
        <% if (PageHasItems(PageNumber - 1)) { %>
          <li>
            <a href="<%= ResolveUrl("~/Index.aspx?Page=" + (PageNumber - 1) + "&CategoryId=" + Request.Params["CategoryId"]) %>"
              class="pagination-link">Previous Page</a>
          </li>
        <% } %>

        <% if (PageHasItems(PageNumber + 1)) { %>
          <li>
            <a href="<%= ResolveUrl("~/Index.aspx?Page=" + (PageNumber + 1) + "&CategoryId=" + Request.Params["CategoryId"]) %>"
              class="pagination-link">Next Page</a>
          </li>
        <% } %>
      </ul>
    </div>

    <% if (ItemsForRequestedCategory == false) { %>
      <div class="notification u-margin-Ll">
        <% if (String.IsNullOrEmpty(CategoryName)) { %>
          <p class="lead">The requested category doesn't exist.</p>
        <% } else if (!PageHasItems(PageNumber)) { %>
          <p class="lead">The page number is too high.</p>
        <% } else { %>
          <p class="lead">No available articles for the requested category.</p>
        <% } %>
      </div>
    <% } %>
  </main>
</asp:Content>

