using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for CollectionPage
/// </summary>
public class CollectionPage : BasePage
{
  protected const int ItemsLimit = 5;
  protected int ArticlesNumber;
  protected int PageNumber = 1;

  protected bool PageIsValid(int PageNumberArg)
  {
    return PageNumberArg != null && PageNumberArg > 0;
  }

  protected bool PageHasItems(int PageNumberArg)
  {
    return PageIsValid(PageNumberArg)
           && ArticlesNumber - (PageNumberArg - 1) * ItemsLimit > 0;
  }


  protected int CalculatePageNumber()
  {
    int pn;

    try
    {
      pn = int.Parse(Request.Params["Page"]);
      if (!PageIsValid(pn)) pn = 1;
    }
    catch (Exception ex)
    {
      pn = 1;
    }

    return pn;
  }

  protected void BoundPreviewToParagraphsRepeater(object sender, RepeaterItemEventArgs args)
  {
    if (args.Item.ItemType == ListItemType.Item || args.Item.ItemType == ListItemType.AlternatingItem)
    {
      string Preview = ((DataRowView)args.Item.DataItem).Row["Preview"].ToString();
      string NewLineCharacters = GetNewlineCharacters(Preview);

      List<string> Paragraphs = Preview.Split(NewLineCharacters.ToCharArray())
        .Where(str => str != "")
        .ToList();

      if (Paragraphs.Count == 0) Paragraphs.Add("No preview available");

      Repeater ParagraphsRepeater = (Repeater)args.Item.FindControl("ParagraphsRepeater");
      ParagraphsRepeater.DataSource = Paragraphs;
      ParagraphsRepeater.DataBind();
    }
  }


  protected string CalculateArticlesQuery(string CategoryId, bool AcceptedResults, bool Count = false)
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
          + " AND a.Accepted = " + (AcceptedResults ? "1" : "0")
        + ((Count == false) ? " ORDER BY a.PublicationDate DESC" : "");
    }
    else
    {
      return "SELECT" + AfterSelectStatement
        + " FROM Articles a"
        + " WHERE a.Accepted = " + (AcceptedResults ? "1" : "0")
        + ((Count == false) ? " ORDER BY a.PublicationDate DESC" : "");
    }
  }

  protected void SetSelectCommandForArticlesDataSource(SqlDataSource DataSourceArg, String CategoryId, bool AcceptedResults, bool UsePagination)
  {
    string ArticlesQuery = CalculateArticlesQuery(CategoryId, AcceptedResults);

    if (UsePagination)
    {
      ArticlesQuery += " OFFSET " + ((PageNumber - 1) * ItemsLimit)
        + " ROWS FETCH NEXT " + ItemsLimit + " ROWS ONLY";
    }

    DataSourceArg.ConnectionString = this.connectionString;
    DataSourceArg.SelectCommand = ArticlesQuery;
  }

  protected void SetArticlesNumber(SqlConnection Connection, String CategoryId, bool AcceptedResults)
  {
    String Query = CalculateArticlesQuery(CategoryId, AcceptedResults, true);
    SqlCommand Command = new SqlCommand(Query, Connection);
    ArticlesNumber = int.Parse(Command.ExecuteScalar().ToString());
  }
}