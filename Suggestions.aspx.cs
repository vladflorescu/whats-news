using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class Suggestions : CollectionPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
      if (!Page.IsPostBack && currentUserIsAdministratorOrPublisher())
      {
        InitializeDatabaseQueryParameters();

        HtmlSelect SortSelect = (HtmlSelect)LVContent.FindControl("SortCategory");
        if (SortSelect != null)
        {
          SortSelect.SelectedIndex = CalculateSelectedIndex(Request.QueryString["OrderBy"]);
        }

        SqlConnection Connection = new SqlConnection(connectionString);
        try
        {
          Connection.Open();
          SetArticlesNumber(Connection, false);
          SqlDataSource SDSArticles = (SqlDataSource)LVContent.FindControl("SDSArticles");
          if (SDSArticles != null)
          {
            SetSelectCommandForArticlesDataSource(SDSArticles, false, true);
          }
        }
        catch (Exception ex)
        {
          Debug.WriteLine("Error in Page_Load: " + ex.Message);
        }
        finally
        {
          Connection.Close();
        }
      }
    }

    protected void SearchButton_ServerClick(object sender, EventArgs e)
    {
      TextBox TBSearch = (TextBox)LVContent.FindControl("TBSearch");
      HtmlSelect SortCategory = (HtmlSelect)LVContent.FindControl("SortCategory");
      string SearchValue = (TBSearch != null) ? TBSearch.Text : "";
      string SortValue = (SortCategory != null) ? SortCategory.Value : "";

      var QueryHash = HttpUtility.ParseQueryString(Request.QueryString.ToString());
      QueryHash.Set("Q", SearchValue);
      QueryHash.Set("OrderBy", SortValue);
      QueryHash.Set("Page", "1");
      string Url = Request.Url.AbsolutePath;
      string UpdatedQueryString = "?" + QueryHash.ToString();
      Response.Redirect(Url + UpdatedQueryString);
    }
}