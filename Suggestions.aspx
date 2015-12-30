<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Suggestions.aspx.cs" Inherits="Suggestions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script src='<%=ResolveClientUrl("~/Scripts/filtering-options.js") %>'></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
  <asp:LoginView ID="LVContent" runat="server">
    <RoleGroups>
      <asp:RoleGroup Roles="administrator, publisher">
        <ContentTemplate>
          <div class="filtering-options">
            <div class="form-inline">
              <div class="form-group sort-group u-margin-Rxxs">
                <label>Sort by:</label>
                <select id="SortCategory" runat="server"
                class="form-control input-sm">
                  <option value="Publication Date (DESC)">Publication Date (DESC)</option>
                  <option value="Publication Date (ASC)">Publication Date (ASC)</option>
                  <option value="Title (DESC)">Title (DESC)</option>
                  <option value="Title (ASC)">Title (ASC)</option>
                </select>
              </div>

              <div class="search-group">
                <asp:TextBox ID="TBSearch" runat="server" placeholder="Search for article..."
                  CssClass="form-control input-sm search-input"></asp:TextBox>
                <button ID="SearchButton" runat="server" onserverclick="SearchButton_ServerClick"
                class="btn btn-default btn-sm search-button">
                  <i class="fa fa-search"></i>
                </button>   
              </div>
            </div>
          </div>

          <main class="col-xs-offset-3 col-xs-9">
            <h2 class="suggestions-title">Articles Suggested by Users</h2>

            <% if (ItemsForRequestedCategory == false) { %>
              <div class="notification u-margin-Ll">
                <% if (!PageHasItems(PageNumber) && PageNumber > 1) { %>
                  <p class="lead">The page number is too high.</p>
                <% } else { %>
                  <p class="lead">No available articles for the requested category.</p>
                <% } %>
              </div>
            <% } %>

            <%-- ConnectionString and Select Command in code behind --%>
            <asp:SqlDataSource ID="SDSArticles" runat="server"></asp:SqlDataSource>

            <asp:Repeater ID="RArticles" runat="server" DataSourceID="SDSArticles" OnItemDataBound="ItemBound">
              <ItemTemplate>
                <div class="article-wrapper">
                  <h3 class="u-margin-Bm u-wrap-true"><%# DataBinder.Eval(Container.DataItem, "Title") %></h3>
                  <div class="content-wrapper">
                    <asp:Repeater ID="ParagraphsRepeater" runat="server">
                      <ItemTemplate>
                        <p><%# DataBinder.GetDataItem(Container) %></p>
                      </ItemTemplate>
                    </asp:Repeater>
                  </div>
                  <asp:HyperLink ID="HLEditArticle" NavigateUrl='<%# "~/EditArticle.aspx?Id=" + DataBinder.Eval(Container.DataItem, "Id") %>' runat="server">Go to edit page.</asp:HyperLink>
                </div>
              </ItemTemplate>
            </asp:Repeater>

            <div class="pagination-component">
              <ul class="pager">
                <% if (PageHasItems(PageNumber - 1)) { %>
                  <li>
                    <a href="<%= SpecificPageUrl(PageNumber - 1) %>" class="pagination-link">Previous Page</a>
                  </li>
                <% } %>

                <% if (PageHasItems(PageNumber + 1)) { %>
                  <li>
                    <a href="<%= SpecificPageUrl(PageNumber + 1) %>" class="pagination-link">Next Page</a>
                  </li>
                <% } %>
              </ul>
            </div>
          </main>
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

