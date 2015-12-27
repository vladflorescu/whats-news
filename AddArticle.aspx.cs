using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Data.SqlClient;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

public partial class AddArticle : BasePage
{
    protected void showErrorMessage(string message)
    {
      ((Literal)LVContent.FindControl("LAnswer")).Text = message;
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
      if (!Page.IsPostBack && userIsConnected())
      {
        HtmlSelect Categories = null;
        Categories = (HtmlSelect)LVContent.FindControl("CategoriesSelect");

        String query = "SELECT * FROM Categories ORDER BY Name";
        SqlConnection con = new SqlConnection(this.connectionString);
        con.Open();
        SqlCommand com = new SqlCommand(query, con);
        SqlDataReader reader = com.ExecuteReader();

        while (reader.Read())
        {
          //Debug.WriteLine(reader["Id"].ToString());
          ListItem option = new ListItem((string)reader["Name"], reader["Id"].ToString());
          if (option != null) Categories.Items.Add(option);
        }
      }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void BSubmit_Click(object sender, EventArgs e)
    {
      if (Page.IsValid)
      {
        string currentUserId   = Membership.GetUser().ProviderUserKey.ToString();
        string currentUserName = Membership.GetUser().UserName;
        bool currentUserIsAdministratorOrPublisher = this.currentUserIsAdministratorOrPublisher();

        string Title = ((TextBox)(LVContent.FindControl("TBTitle"))).Text.Trim();
        string Preview = ((TextBox)(LVContent.FindControl("TBPreview"))).Text.Trim();
        string Content = ((TextBox)(LVContent.FindControl("TBArticle"))).Text.Trim();

        if (Title == "" || Content == "") 
        {
          showErrorMessage("The title and the content can't be empty.");
          return;
        }

        List<int> OldSelectedCategoriesIds = ((HtmlSelect)LVContent.FindControl("CategoriesSelect"))
          .Items.OfType<ListItem>()
          .Where(item => item.Selected)
          .Select(el => int.Parse(el.Value))
          .ToList();

        List<string> NewCategoriesNames = ((HtmlInputControl)LVContent.FindControl("HiddenCategories"))
          .Value.Split('|')
          .Where(name => name != "")
          .ToList();

        if (OldSelectedCategoriesIds.Count == 0 && NewCategoriesNames.Count == 0)
        {
          showErrorMessage("At least one category must be chosen.");
          return;
        }

        //I can't use the inline if here
        DateTime? PublicationDate = null;
        if (currentUserIsAdministratorOrPublisher) PublicationDate = DateTime.Now;

        string PublisherId = (currentUserIsAdministratorOrPublisher) ? currentUserId 
                                                                     : null;
        string UserId = (currentUserIsAdministratorOrPublisher) ? null
                                                                : currentUserId;
        bool Accepted = (currentUserIsAdministratorOrPublisher) ? true : false;

        string queryBeginning = "INSERT INTO Articles (Title, Content, Accepted";
        string queryMiddle = " OUTPUT INSERTED.Id";
        string queryEnd = " VALUES (@title, @content, @accepted";

        if (Preview != "")
        {
          queryBeginning += ", Preview";
          queryEnd += ", @preview";
        }

        if (currentUserIsAdministratorOrPublisher)
        {
          queryBeginning += ", PublisherId, PublicationDate)";
          queryEnd += ", @publisherId, @publicationDate)";
        }
        else
        {
          queryBeginning += ", UserId)";
          queryEnd += ", @userId)";
        }

        string articleQuery = queryBeginning + queryMiddle + queryEnd;
        SqlConnection connection = null;

        try
        {
          connection = new SqlConnection(this.connectionString);
          connection.Open();

          SqlCommand articleCommand = new SqlCommand(articleQuery, connection);
          articleCommand.Parameters.AddWithValue("title", Title);
          articleCommand.Parameters.AddWithValue("content", Content);
          articleCommand.Parameters.AddWithValue("accepted", Accepted);

          if (Preview != "") articleCommand.Parameters.AddWithValue("preview", Preview);

          if (currentUserIsAdministratorOrPublisher)
          {
            articleCommand.Parameters.AddWithValue("publisherId", PublisherId);
            articleCommand.Parameters.AddWithValue("publicationDate", PublicationDate);
          }
          else
          {
            articleCommand.Parameters.AddWithValue("userId", UserId);
          }

          try 
          {
            int ArticleId = (int)articleCommand.ExecuteScalar();

            List<int> NewCategoriesIds = new List<int>();

            string categoryQuery = "INSERT INTO Categories (Name, Accepted)"
              + " OUTPUT INSERTED.Id"
              + " VALUES (@name, @accepted)";

            //Try is in foreach because if it generates an error
            //because if there is an error thrown because there is
            //already a category with the same name in the database
            //then the application won't add its id to the newIds list.
            foreach (string Name in NewCategoriesNames)
            {
              try
              {
                SqlCommand categoryCommand = new SqlCommand(categoryQuery, connection);

                categoryCommand.Parameters.AddWithValue("name", Name);
                categoryCommand.Parameters.AddWithValue("accepted", userIsConnected());

                int id = (int)categoryCommand.ExecuteScalar();
                Debug.WriteLine("categoryID " + id);
                NewCategoriesIds.Add(id);
              }
              catch (SqlException ex)
              {
                if (ex.Message.StartsWith("Violation of UNIQUE KEY constraint"))
                {
                  string getAlreadyExistentIdQuery = "SELECT TOP 1 Id FROM Categories WHERE Name = @name";
                  SqlCommand gaeiCommand = new SqlCommand(getAlreadyExistentIdQuery, connection);

                  gaeiCommand.Parameters.AddWithValue("name", Name);

                  try
                  {
                    SqlDataReader reader = gaeiCommand.ExecuteReader();
                    if (reader.Read()) OldSelectedCategoriesIds.Add((int)reader["Id"]);
                  }
                  catch
                  {
                    Debug.WriteLine("Go cry in a corner");
                  }
                }
              }
              catch (Exception ex)
              {
                showErrorMessage("Database category insert error: " + ex.Message);
              }
            }

            List<int> AllCategoriesIds = OldSelectedCategoriesIds
              .Concat(NewCategoriesIds)
              .ToList();

            string associativeQuery = "INSERT INTO ArticlesInCategories (ArticleId, CategoryId)"
              + " VALUES (@articleId, @categoryId)";

            try
            {
              foreach (int catId in AllCategoriesIds)
              {
                SqlCommand associativeCommand = new SqlCommand(associativeQuery, connection);
                associativeCommand.Parameters.AddWithValue("articleId", ArticleId);
                associativeCommand.Parameters.AddWithValue("categoryId", catId);

                associativeCommand.ExecuteNonQuery();
              }

              connection.Close();
              Response.Redirect("Index.aspx");
            }
            catch (Exception ex)
            {
              showErrorMessage("Something bad happended... " + ex.Message);
              connection.Close();
            }
          }
          catch(Exception ex)
          {
            showErrorMessage("Database article insert error: " + ex.Message);
            connection.Close();
          }
        }
        catch(Exception ex)
        {
          showErrorMessage("Can't connect to the database: " + ex.Message);
          connection.Close();
        }
      }
    }

}