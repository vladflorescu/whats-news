<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Suggestions.aspx.cs" Inherits="Suggestions" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
  <main class="col-xs-offset-3 col-xs-9">
    <h2 class="suggestions-title">Articles Suggested by Users</h2>

    <%-- ConnectionString and Select Command in code behind --%>
    <asp:SqlDataSource ID="SDSArticles" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>"
      SelectCommand="SELECT a.Id, a.Title, a.Preview, a.PublisherId, a.PublicationDate, a.Content FROM Articles a WHERE a.Accepted = 0"></asp:SqlDataSource>

    <asp:Repeater ID="RArticles" runat="server" DataSourceID="SDSArticles" OnItemDataBound="BoundPreviewToParagraphsRepeater">
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
  </main>
</asp:Content>

