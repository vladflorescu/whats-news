using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class Index : CollectionPage
{
    protected string CategoryName = null;
    protected bool ItemsForRequestedCategory = false;

    protected void ItemBound(object sender, RepeaterItemEventArgs args)
    {
      ItemsForRequestedCategory = true;
      BoundPreviewToParagraphsRepeater(sender, args);
    }

    protected void CalculatePageNumber()
    {
      try
      {
        PageNumber = int.Parse(Request.Params["Page"]);
        if (!PageIsValid(PageNumber)) PageNumber = 1;
      }
      catch (Exception ex)
      {
        PageNumber = 1;
      }
    }

    protected void RequestCategoryName(SqlConnection con)
    {
      string CategoryId = Request.Params["CategoryId"];

      if (String.IsNullOrEmpty(CategoryId))
      {
        this.CategoryName = "Latest";
      }
      else
      {
        string query = "SELECT Name FROM Categories WHERE Id = "
          + Request.QueryString.Get("CategoryId");

        SqlCommand com = new SqlCommand(query, con);
        this.CategoryName = com.ExecuteScalar().ToString();
      }
    }

    protected string CalculateArticlesQuery(string CategoryId, bool Count = false)
    {
      string AfterSelectStatement = (Count == false)
        ? " a.Id, a.Title, a.Preview, a.PublisherId, a.PublicationDate, a.Content"
        : " COUNT(*)";

      if (!String.IsNullOrEmpty(CategoryId))
      {
        return "SELECT" + AfterSelectStatement
          + " FROM ArticlesInCategories aic"
          + " INNER JOIN Articles a"
          + " ON aic.ArticleId = a.Id"
          + " WHERE aic.CategoryId = " + CategoryId
            + " AND a.Accepted = 1"
          + ((Count == false) ? " ORDER BY a.PublicationDate DESC" : "");
      }
      else
      {
        return "SELECT" + AfterSelectStatement
          + " FROM Articles a"
          + " WHERE a.Accepted = 1"
          + ((Count == false) ? " ORDER BY a.PublicationDate DESC" : "");
      }
    }

    protected void SetArticlesNumber(SqlConnection Connection)
    {
      String Query = CalculateArticlesQuery(Request.Params["CategoryId"], true);
      SqlCommand Command = new SqlCommand(Query, Connection);
      ArticlesNumber = int.Parse(Command.ExecuteScalar().ToString());
    }

    protected void SetAttributesForArticlesDataSource()
    {
      string CategoryId = Request.QueryString.Get("CategoryId");
      string ArticlesQuery = CalculateArticlesQuery(CategoryId);

      ArticlesQuery += " OFFSET " + ((PageNumber - 1) * ItemsLimit)
        + " ROWS FETCH NEXT " + ItemsLimit + " ROWS ONLY";

      SDSArticles.ConnectionString = this.connectionString;
      SDSArticles.SelectCommand = ArticlesQuery;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
      if (!Page.IsPostBack)
      {
        try
        {
          PageNumber = int.Parse(Request.Params["Page"]);
          if (!PageIsValid(PageNumber)) PageNumber = 1;
        }
        catch (Exception ex)
        {
          PageNumber = 1;
        }

        SqlConnection Connection = new SqlConnection(connectionString);
        try
        {
          Connection.Open();

          RequestCategoryName(Connection);
          SetArticlesNumber(Connection);
          SetAttributesForArticlesDataSource();
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


    protected void PopulateCategoriesList(SqlConnection con)
    {
      string CategoriesUrlBeginning = "~/Index.aspx";

      HtmlGenericControl LatestLi = new HtmlGenericControl("li");
      HtmlAnchor LatestAhchor = new HtmlAnchor();
      LatestAhchor.InnerText = "Latest";
      LatestAhchor.HRef = CategoriesUrlBeginning;

      LatestLi.Controls.Add(LatestAhchor);
      CategoriesList.Controls.Add(LatestLi);

      String query = "SELECT Id, Name FROM Categories WHERE Accepted = 1"
        + " ORDER BY Name";
      SqlCommand com = new SqlCommand(query, con);
      SqlDataReader reader = com.ExecuteReader();

      while (reader.Read())
      {
        HtmlGenericControl CategoryLi = new HtmlGenericControl("li");
        HtmlAnchor CategoryAnchor = new HtmlAnchor();
        CategoryAnchor.InnerText = (string)reader["Name"];
        CategoryAnchor.HRef = CategoriesUrlBeginning
          + "?CategoryId=" + reader["Id"].ToString();
        CategoryLi.Controls.Add(CategoryAnchor);
        CategoriesList.Controls.Add(CategoryLi);
      }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
      if (!Page.IsPostBack)
      {
        SqlConnection Connection = new SqlConnection(connectionString);
        try
        {
          Connection.Open();
          PopulateCategoriesList(Connection);
        }
        catch (Exception ex)
        {
          Debug.WriteLine("Error in Page_PreRender: " + ex.Message);
        }
        finally
        {
          Connection.Close();
        }
      }
    }

    protected List<int> GetArticleCategoriesIds(SqlConnection Connection, int ArticleId)
    {
      List<int> ArticleCategoriesIds = new List<int>();
      string Query = "SELECT DISTINCT CategoryId"
        + " FROM ArticlesInCategories"
        + " WHERE ArticleId = @articleId";

      SqlCommand Command = new SqlCommand(Query, Connection);
      Command.Parameters.AddWithValue("articleId", ArticleId);

      SqlDataReader Reader = Command.ExecuteReader();
      while (Reader.Read())
      {
        ArticleCategoriesIds.Add(int.Parse(Reader["CategoryId"].ToString()));
      }

      return ArticleCategoriesIds;
    }

    protected void DeleteArticle(SqlConnection connection, int ArticleId)
    {
      string query = "DELETE FROM Articles WHERE Id = @id";
      SqlCommand command = new SqlCommand(query, connection);
      command.Parameters.AddWithValue("id", ArticleId);
      command.ExecuteNonQuery();
    }

    protected int GetNumberOfArticles(SqlConnection Connection, int CategoryId)
    {
      string Query = "SELECT COUNT(*) FROM ArticlesInCategories WHERE CategoryId = @categoryId";
      SqlCommand Command = new SqlCommand(Query, Connection);
      Command.Parameters.AddWithValue("categoryId", CategoryId);

      return int.Parse(Command.ExecuteScalar().ToString());
    }

    protected void DeleteCategoriesWhichAreNowEmpty(SqlConnection Connection, List<int> CategoriesIds)
    {
      List<int> EmptyCategoriesIds = new List<int>();
      foreach (int CategoryId in CategoriesIds)
      {
        if (GetNumberOfArticles(Connection, CategoryId) == 0)
        {
          EmptyCategoriesIds.Add(CategoryId);
        }
      }

      if (EmptyCategoriesIds.Count != 0)
      {
        String Query = "DELETE FROM Categories WHERE";

        int i = 0;
        foreach (int id in EmptyCategoriesIds)
        {
          Query += (" Id = @cid" + i);
          ++i;
          if (i != EmptyCategoriesIds.Count) Query += " OR";
        }

        SqlCommand Command = new SqlCommand(Query, Connection);

        i = 0;
        foreach (int id in EmptyCategoriesIds)
        {
          Command.Parameters.AddWithValue("cid" + i, id);
          ++i;
        }

        Command.ExecuteNonQuery();
      }
    }

    protected void DeleteButton_ServerClick(object sender, EventArgs e)
    {

      try
      {
        int ArticleId = int.Parse(((HtmlButton)sender).Attributes["data-article-id"]);

        SqlConnection connection = new SqlConnection(connectionString);
        try
        {
          connection.Open();
          try
          {
            List<int> CategoriesIds = GetArticleCategoriesIds(connection, ArticleId);
            try
            {
              DeleteArticle(connection, ArticleId);
              try
              {
                DeleteCategoriesWhichAreNowEmpty(connection, CategoriesIds);
                Response.Redirect(Request.Url.AbsoluteUri);
              }
              catch (Exception ex)
              {
                LAnswer.Text = "Error while deleting empty categories: " + ex.Message;
              }
            }
            catch (Exception ex)
            {
              LAnswer.Text = "Error while deleting article: " + ex.Message;
            }
          }
          catch (Exception ex)
          {
            LAnswer.Text = "Error while getting categories ids: " + ex.Message;
          }
        }
        catch (Exception ex)
        {
          LAnswer.Text = "Error while opening connection: " + ex.Message;
        }
        finally
        {
          connection.Close();
        }
      }
      catch (Exception ex)
      {
        LAnswer.Text = "Error while parsing article id: " + ex.Message;
      }
    }
}