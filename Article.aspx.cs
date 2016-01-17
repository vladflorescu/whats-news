using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Article : ArticlePage
{
    protected int ArticleId;
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
              if (!(bool)reader["Remote"])
              {
                ArticleId = int.Parse(reader["Id"].ToString());
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
              else
              {
                Response.Redirect(reader["content"].ToString());
              }
            }
          }
          else
          {
            GoodId = false;
          }
        }
        catch (Exception ex)
        {
          LAnswer.Text = "Article database error: " + ex.Message;
        }
        finally
        {
          connection.Close();
        }
      }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
      SqlConnection Connection = new SqlConnection(this.connectionString);

      try
      {
        Connection.Open();
        string query = "SELECT u.UserName, c.Content, c.Date FROM Comments c"
          + " INNER JOIN Users u ON c.UserId = u.UserId"
          + " WHERE c.ArticleId=@id"
          + " ORDER BY c.Date DESC";
        SqlDataAdapter CommentsCommand = new SqlDataAdapter(query, Connection);
        CommentsCommand.SelectCommand.Parameters.AddWithValue("id", ArticleId);

        DataSet CommentsDataSet = new DataSet();
        CommentsCommand.Fill(CommentsDataSet);

        RComments.DataSource = CommentsDataSet;
        RComments.DataBind();

        if (CommentsDataSet.Tables[0].Rows.Count == 0) NoCommentsNotification.Visible = true;
      }
      catch (Exception ex)
      {
        LAnswer.Text = "Comments database error: " + ex.Message;
      }
      finally
      {
        Connection.Close();
        TextBox CommentTextBox = (TextBox)LVComment.FindControl("TBComment");
        if (CommentTextBox != null) CommentTextBox.Text = "";
      }
    }

    protected void BCommentSubmit_Click(object sender, EventArgs e)
    {
      String Content = ((TextBox)LVComment.FindControl("TBComment")).Text;
      String UserId = Membership.GetUser().ProviderUserKey.ToString();

      if (String.IsNullOrEmpty(Content))
      {
        LAnswer.Text = "Comments can't be empty";
      }

      if (Content != null) 
      {
        SqlConnection Connection = new SqlConnection(connectionString);

        try
        {
          Connection.Open();

          string query = "INSERT INTO Comments(Content, UserId, ArticleId, Date)"
                      + " VALUES (@content, @user_id, @article_id, @date)";

          SqlCommand Com = new SqlCommand(query, Connection);
          Com.Parameters.AddWithValue("content", Content);
          Com.Parameters.AddWithValue("user_id", UserId);
          Com.Parameters.AddWithValue("article_id", ArticleId);
          Com.Parameters.AddWithValue("date", DateTime.Now);

          Com.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
          LAnswer.Text = "Database error: " + ex.Message;
        }
        finally
        {
          Connection.Close();
        }
      }
    }
}