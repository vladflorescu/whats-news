<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Article.aspx.cs" Inherits="Article" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
  <% if(GoodId) { %>
    <div class="text-danger" style="margin-left: 65px; width: calc(100% - 80px);">
      <p class="lead">
        <asp:Literal ID="LAnswer" runat="server"></asp:Literal>
      </p>
    </div>
    <% if(ArticleIsAccepted) { %>
      <div class="article-title-wrapper">
        <p class="article-title"><asp:Literal ID="LTitle" runat="server"></asp:Literal></p>
      </div>

      <div class="article-sub">
        <div>
          <span>published by </span>
          <span><i><asp:Literal ID="LPublisherName" runat="server"></asp:Literal></i></span>
          <% if(!String.IsNullOrEmpty(UserId)) { %>
            <span> as a suggestion of </span>
            <span><asp:Literal ID="LUserName" runat="server"></asp:Literal></span>
          <% } %>
        </div>
        <div>
          <span>on </span>
          <span><asp:Literal ID="LPublicationDate" runat="server"></asp:Literal></span>
          <span> at </span>
          <span><asp:Literal ID="LPublicationHour" runat="server"></asp:Literal></span>
        </div>
      </div>

      <div class="article-content-wrapper show-page">
        <asp:Repeater ID="ParagraphsRepeater" runat="server">
          <ItemTemplate>
            <p><%# DataBinder.GetDataItem(Container) %></p>
          </ItemTemplate>
        </asp:Repeater>
      </div>

      <div class="comments-wrapper">
        <p class="lead">Comments:</p>

        <asp:LoginView ID="LVComment" runat="server">
          <LoggedInTemplate>
            <asp:TextBox ID="TBComment" runat="server" TextMode="MultiLine" Rows="5"
              Placeholder="Write a comment here..." CssClass="comment-textarea"></asp:TextBox>
            <asp:Button ID="BCommentSubmit" runat="server" Text="Post" OnClick="BCommentSubmit_Click"
              CssClass="btn btn-primary btn- u-margin-Txxs pull-right u-color-gray u-bg-color-ink" /> 
          </LoggedInTemplate>
        </asp:LoginView>

        <div class="clearfix"></div>

        <div class="comments">
          <asp:Repeater ID="RComments" runat="server">
            <ItemTemplate>
              <div class="comment">
                <p class="comment-name"><%# DataBinder.Eval(Container.DataItem, "UserName") %></p>
                <p class="comment-date">
                  <%# ((DateTime)DataBinder.Eval(Container.DataItem, "Date")).ToShortDateString()
                    + ", " + ((DateTime)DataBinder.Eval(Container.DataItem, "Date")).ToShortTimeString()%>
                </p>
                <div class="comment-content"><%# DataBinder.Eval(Container.DataItem, "Content") %></div>
              </div>
            </ItemTemplate>
          </asp:Repeater>
        </div>

        <div id="NoCommentsNotification" runat="server" class="u-margin-Txl" visible="false">
          <h4 class="u-align-middle">No comments for the article.</h4>
        </div>
      </div>

    <% } else { %>
      
      <div style="margin-top: 40px; margin-left: 40px; width: calc(100% - 80px);">
        <p class="lead">The article requested was not yet approved to be shown. </p>
      </div>

    <% } %>

  <% } else { %>

    <div style="margin-top: 40px; margin-left: 40px; width: calc(100% - 80px);">
      <p class="lead">No existent article with the specified id.</p>
    </div>

  <% } %>
</asp:Content>

