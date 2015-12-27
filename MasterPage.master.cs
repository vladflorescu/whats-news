using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class MasterPage : System.Web.UI.MasterPage
{
  protected void Page_Load(object sender, EventArgs e)
  {
    if (!Page.IsPostBack)
    {
      bool userIsAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;

      String currentPage = this.Page.Request.FilePath;

      if (currentPage.EndsWith("Index.aspx"))
      {
        HLHomepage.CssClass += " selected";
      }
      else if (currentPage.EndsWith("AddArticle.aspx"))
      {
        if (userIsAuthenticated)
        {
          HyperLink AddArticleMenuItem = (HyperLink)(LVAddArticleMenuItem.FindControl("HLAddArticle"));
          if (AddArticleMenuItem != null) AddArticleMenuItem.CssClass += " selected";
        }
      }
      else if (currentPage.EndsWith("Suggestions.aspx"))
      {
        foreach (string role in Roles.GetRolesForUser())
        {
          if (role == "publisher" || role == "administrator")
          {
            HyperLink SuggestionsMenuItem = (HyperLink)(LVSuggestions.FindControl("HLSuggestions"));
            if (SuggestionsMenuItem != null) SuggestionsMenuItem.CssClass += " selected";
          }
        }
      }
    }
  }
}
