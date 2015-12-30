<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="AddArticle.aspx.cs" Inherits="AddArticle" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
  <script src='<%=ResolveClientUrl("~/Scripts/add-article.js")%>'></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
  <asp:LoginView ID="LVContent" runat="server">
    <LoggedInTemplate>
      <div class="text-danger" style="margin-left: 65px; width: calc(100% - 100px);">
        <p class="lead">
          <asp:Literal ID="LAnswer" runat="server"></asp:Literal>
        </p>
      </div>

      <div class="article-title-wrapper">
        <asp:RequiredFieldValidator ID="RFVTitle" runat="server" ErrorMessage="The title can't be empty." ControlToValidate="TBTitle" CssClass="text-danger"></asp:RequiredFieldValidator>
        <asp:TextBox ID="TBTitle" runat="server" 
          placeholder="My Title" CssClass="article-title-input">
        </asp:TextBox>
      </div>

      <div class="category-select-wrapper form-inline">
        <label class="u-margin-Rxs">Pick one or more categories: </label>
        <select id="CategoriesSelect" runat="server" class="form-control" multiple></select>
        <asp:TextBox ID="TBAddCategory" runat="server" placeholder="Add a new category"
          CssClass="form-control add-category-input">
        </asp:TextBox>

        <input id="HiddenCategories" runat="server" type="hidden" class="hidden-categories-input" />
      </div>

      <div class="article-content-wrapper">
        <asp:TextBox ID="TBPreview" TextMode="MultiLine"  Columns="80" Rows="2" 
          runat="server" placeholder="Here goes the preview text for my article. (Will appear only on the index page)" 
          CssClass="preview-text-area" ></asp:TextBox>
      </div>

      <div class="article-content-wrapper">
        <asp:RequiredFieldValidator ID="RFVContent" runat="server" ErrorMessage="The article can't be empty." ControlToValidate="TBArticle" CssClass="text-danger"></asp:RequiredFieldValidator>
        <asp:TextBox ID="TBArticle" TextMode="MultiLine"  Columns="80" Rows="8" 
          runat="server" placeholder="Here goes my text." CssClass="article-text-area" ></asp:TextBox>
      </div>

      <div class="u-text-center  u-margin-Tl">
        <asp:LoginView ID="LVPublishArticle" runat="server">
          <RoleGroups>
            <asp:RoleGroup Roles="administrator, publisher">
              <ContentTemplate>

                <asp:Button ID="BSubmit" runat="server" Text="Publish Article" 
                  CssClass="btn btn-primary u-bg-color-ink u-color-gray"
                  OnClick="BSubmit_Click" UseSubmitBehavior="False" />

              </ContentTemplate>
            </asp:RoleGroup>
          </RoleGroups>
          <LoggedInTemplate>

            <asp:Button ID="BSubmit" runat="server" Text="Suggest Article" 
              CssClass="btn btn-primary u-bg-color-ink u-color-gray" 
              OnClick="BSubmit_Click" UseSubmitBehavior="False" />

          </LoggedInTemplate>
        </asp:LoginView>
      </div>
    </LoggedInTemplate>
    <AnonymousTemplate>
      <div style="margin-top: 20px; margin-left: 40px">
        <h4>You are not authorized to access this page.</h4>
      </div>
    </AnonymousTemplate>
  </asp:LoginView> 
</asp:Content>

