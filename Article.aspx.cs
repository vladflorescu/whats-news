using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Article : ArticlePage
{
    protected string Title;
    protected List<string> Paragraphs;
    protected string PublisherId;
    protected string UserId;
    protected DateTime PublicationDate;

    protected string queryUserName(SqlConnection con, string Id)
    {
      string query = "SELECT UserName FROM Users WHERE UserId = @id";
      SqlCommand com = new SqlCommand(query, con);
      com.Parameters.AddWithValue("id", Id);

      try
      {
        return com.ExecuteScalar().ToString();
      }
      catch
      {
        return null;
      }
    }

    protected void Page_Load(object sender, EventArgs e)
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
            ArticleIsAccepted = (bool)reader["Accepted"];
            if (ArticleIsAccepted)
            {
              LTitle.Text = Title = reader["Title"].ToString();

              string Content = reader["Content"].ToString();
              char[] NewlineCharacters = GetNewlineCharacters(Content).ToCharArray();
              ParagraphsRepeater.DataSource = Paragraphs = Content.Split(NewlineCharacters)
                .Where(str => str != "")
                .ToList();
              ParagraphsRepeater.DataBind();

              PublisherId = reader["PublisherId"].ToString();
              string PublisherName = queryUserName(connection, PublisherId);
              LPublisherName.Text = (!String.IsNullOrEmpty(PublisherName))
                ? PublisherName
                : "|NotFound|";

              UserId = reader["UserId"].ToString();
              if (!String.IsNullOrEmpty(UserId))
              {
                string UserName = queryUserName(connection, PublisherId);
                LUserName.Text = (!String.IsNullOrEmpty(UserName))
                  ? UserName
                  : "|NotFound|";
              }

              PublicationDate = DateTime.Parse(reader["PublicationDate"].ToString());
              LPublicationDate.Text = PublicationDate.ToLongDateString();
              LPublicationHour.Text = PublicationDate.ToShortTimeString();
            }
          }
          else
          {
            GoodId = false;
          }
        }
        catch (Exception ex)
        {
          LAnswer.Text = "Database error: " + ex.Message;
        }
        finally
        {
          connection.Close();
        }
      }
    }
}