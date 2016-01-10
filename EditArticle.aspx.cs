using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class EditArticle : ArticlePage
{
    protected string Title;
    protected string Preview;
    protected string Content;
    protected string PublisherName = null;
    protected List<Tuple<int, string, bool>> CategoriesList;

    protected bool CurrentUserHasThePermissionToUpdateArticle = true;
    
    protected void Page_Load(object sender, EventArgs e)
    {
      if (!Page.IsPostBack)
      {
        string Id = Request.Params["Id"];

        if (!IdIsValid(Id))
        {
          GoodId = false;
        }
        else
        {
          string query = "SELECT * FROM Articles a WHERE a.Id = @id";
          SqlConnection connection = new SqlConnection(connectionString);

          try
          {
            connection.Open();
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("id", Id);

            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
              try
              {
                ArticleIsAccepted = bool.Parse(reader["Accepted"].ToString());
              }
              catch (Exception ex)
              {
                LAnswer.Text = "Error while parsing \"Accepted\" field :" + ex.Message;
                ShowPage = false;
                connection.Close();
                return;
              }

              if (ArticleIsAccepted)
              {
                if (!(currentUserIsAdministrator()
                     || (currentUserIsPublisher()
                       && reader["PublisherId"].ToString()
                         == Membership.GetUser().ProviderUserKey.ToString())))
                {
                  CurrentUserHasThePermissionToUpdateArticle = false;
                  return;
                }
              }
              else
              {
                if (!currentUserIsAdministratorOrPublisher())
                {
                  CurrentUserHasThePermissionToUpdateArticle = false;
                  return;
                }
              }

              TBTitle.Text = Title = reader["Title"].ToString();
              TBPreview.Text = Preview = reader["Preview"].ToString();

              if (bool.Parse(reader["Remote"].ToString()) == false)
              {
                TBArticle.Text = Content = reader["Content"].ToString();
              }
              else
              {
                TBSource.Text = Content = reader["Content"].ToString();
                CBRemote.Checked = true;
              }
            }
            else
            {
              GoodId = false;
            }

            string categoriesQuery = "SELECT c.Id, c.Name, c.Accepted"
            + " FROM ArticlesInCategories aic"
            + " INNER JOIN Categories c"
            + " ON aic.CategoryId = c.Id"
            + " WHERE aic.ArticleId = " + Id;

            SqlCommand categoriesCommand = new SqlCommand(categoriesQuery, connection);
            SqlDataReader categoriesReader = categoriesCommand.ExecuteReader();

            CategoriesList = new List<Tuple<int, string, bool>>();

            while (categoriesReader.Read())
            {
              int CategoryId;
              bool CategoryIsAccepted;

              try
              {
                CategoryId = int.Parse(categoriesReader["Id"].ToString());
                CategoryIsAccepted = bool.Parse(categoriesReader["Accepted"].ToString());
              }
              catch (Exception ex)
              {
                LAnswer.Text = "Error while parsing categories attributes :" + ex.Message;
                ShowPage = false;
                connection.Close();
                return;
              }

              string CategoryName = categoriesReader["Name"].ToString();

              CategoriesList.Add(Tuple.Create(CategoryId, CategoryName, CategoryIsAccepted));
            }

            RCategories.DataSource = CategoriesList;
            RCategories.DataBind();

          }
          catch (Exception ex)
          {
            LAnswer.Text = "Database error: " + ex.Message;
            ShowPage = false;
          }
          finally
          {
            connection.Close();
          }
        }
      }
    }

    protected void UpdateArticle(SqlConnection connection)
    {
      string Title = TBTitle.Text.Trim();
      string Preview = TBPreview.Text.Trim();
      bool Remote = CBRemote.Checked;
      string Content = Remote ? TBSource.Text.Trim() : TBArticle.Text.Trim();

      if (Title == "" || Content == "")
      {
        throw new Exception("The title and the content can't be empty.");
      }

      string queryBeginning = "UPDATE Articles"
                            + " SET Title = @title, Preview = @preview, Remote = @remote, Content = @content";
      string queryEnd = " WHERE Id = @id";

      if (!ArticleIsAccepted)
      {
        queryBeginning += ", PublisherId = @publisherId, PublicationDate = @publicationDate, Accepted = @accepted";
      }

      string query = queryBeginning + queryEnd;

      SqlCommand command = new SqlCommand(query, connection);
      command.Parameters.AddWithValue("id", Request.Params["Id"]);
      command.Parameters.AddWithValue("title", Title);
      command.Parameters.AddWithValue("preview", Preview);
      command.Parameters.AddWithValue("remote", Remote);
      command.Parameters.AddWithValue("content", Content);
      if (!ArticleIsAccepted)
      {
        command.Parameters.AddWithValue("publisherId", Membership.GetUser().ProviderUserKey);
        command.Parameters.AddWithValue("publicationDate", DateTime.Now);
        command.Parameters.AddWithValue("accepted", true);
      }

      command.ExecuteNonQuery();
    }

    protected void UpdateNewCategories(SqlConnection connection)
    {
      List<int> NewCategoriesIds = new List<int>();

      foreach (RepeaterItem ri in RCategories.Items)
      {
        HtmlGenericControl span = (HtmlGenericControl)ri.FindControl("category");
        if (bool.Parse(span.Attributes["data-accepted"]) == false)
        {
          NewCategoriesIds.Add(int.Parse(span.Attributes["data-id"]));
        }
      }

      string query = "UPDATE Categories SET Accepted = @accepted WHERE Id = @id";

      foreach (int CategoryId in NewCategoriesIds) 
      {
        SqlCommand command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("accepted", true);
        command.Parameters.AddWithValue("id", CategoryId);
        command.ExecuteNonQuery();
      }
    }

    protected void BSubmit_Click(object sender, EventArgs e)
    {
      if (Page.IsValid)
      {
        SqlConnection connection = new SqlConnection(this.connectionString);

        try
        {
          connection.Open();
          try
          {
            UpdateArticle(connection);
            try
            {
              UpdateNewCategories(connection);
              Response.Redirect(CBRemote.Checked ? "~/Index.aspx"
                                                 : "~/Article.aspx?Id=" + Request.Params["Id"]);
            }
            catch (Exception ex)
            {
              LAnswer.Text = "Categories update exception: " + ex.Message;
            }
          }
          catch (Exception ex)
          {
            LAnswer.Text = "Article update exception: " + ex.Message;
          }
        }
        catch (Exception ex)
        {
          LAnswer.Text = "Couldn't open database connection: " + ex.Message;
        }
        finally
        {
          connection.Close();
        }
      }
    }
}