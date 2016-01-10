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
  protected string OrderField = "PublicationDate DESC";
  protected bool ItemsForRequestedCategory = false;

  protected bool PageIsValid(int PageNumberArg)
  {
    return PageNumberArg != null && PageNumberArg > 0;
  }

  protected bool PageHasItems(int PageNumberArg)
  {
    return PageIsValid(PageNumberArg)
           && ArticlesNumber - (PageNumberArg - 1) * ItemsLimit > 0;
  }

  protected string SpecificPageUrl(int Page)
  {
    var QueryHash = HttpUtility.ParseQueryString(Request.QueryString.ToString());
    QueryHash.Set("Page", Page.ToString());
    return Request.Url.AbsolutePath + "?" + QueryHash.ToString();
  }

  protected int CalculateSelectedIndex(string OrderBy)
  {
    switch (OrderBy)
    {
      case "Publication Date (ASC)":
        return 1;
      case "Title (DESC)":
        return 2;
      case "Title (ASC)":
        return 3;
      default:
        return 0;
    }
  }

  protected void InitializePageNumber()
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

  protected void InitializeOrderField()
  {
    string OrderParam =  Server.UrlDecode(Request.QueryString["OrderBy"]);
    switch (OrderParam) {
      case "Publication Date (ASC)":
        OrderField = "PublicationDate ASC";
        break;
      case "Title (DESC)":
        OrderField = "Title DESC";
        break;
      case "Title (ASC)":
        OrderField = "Title ASC";
        break;
      default:
        OrderField = "PublicationDate DESC";
        break;
    }
  }

  protected void InitializeDatabaseQueryParameters()
  {
    InitializePageNumber();
    InitializeOrderField();
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

  protected void ItemBound(object sender, RepeaterItemEventArgs args)
  {
    ItemsForRequestedCategory = true;
    BoundPreviewToParagraphsRepeater(sender, args);
  }

  protected string CalculateArticlesQuery(bool AcceptedResults, bool Count = false)
  {
    string CategoryId = Request.Params["CategoryId"];

    string AfterSelectStatement = (Count == false)
      ? "*" //" a.Id, a.Title, a.Preview, a.PublisherId, a.PublicationDate, a.Content"
      : " COUNT(*)";

    string SearchValue = Request.Params["Q"];
    string LikeStatement = "";
    if (!String.IsNullOrEmpty(SearchValue))
    {
      //I'm looking in the title and in the preview
      LikeStatement += " AND ( LOWER(a.Title) LIKE '%" + SearchValue.ToLower()
        + "%' OR LOWER(a.Preview) LIKE '%" + SearchValue.ToLower() + "%')";
    }


    if (!String.IsNullOrEmpty(CategoryId))
    {
      return "SELECT" + AfterSelectStatement
        + " FROM ArticlesInCategories aic"
        + " INNER JOIN Articles a"
        + " ON aic.ArticleId = a.Id"
        + " WHERE aic.CategoryId = " + CategoryId
          + " AND a.Accepted = " + (AcceptedResults ? "1" : "0")
          + LikeStatement
        + ((Count == false) ? (" ORDER BY " + OrderField) : "");
    }
    else
    {
      return "SELECT" + AfterSelectStatement
        + " FROM Articles a"
        + " WHERE a.Accepted = " + (AcceptedResults ? "1" : "0")
          + LikeStatement
        + ((Count == false) ? (" ORDER BY " + OrderField) : "");
    }
  }

  protected void SetSelectCommandForArticlesDataSource(SqlDataSource DataSourceArg, bool AcceptedResults, bool UsePagination)
  {
    string ArticlesQuery = CalculateArticlesQuery(AcceptedResults);

    if (UsePagination)
    {
      ArticlesQuery += " OFFSET " + ((PageNumber - 1) * ItemsLimit)
        + " ROWS FETCH NEXT " + ItemsLimit + " ROWS ONLY";
    }

    DataSourceArg.ConnectionString = this.connectionString;
    DataSourceArg.SelectCommand = ArticlesQuery;
  }

  protected void SetArticlesNumber(SqlConnection Connection, bool AcceptedResults)
  {
    String Query = CalculateArticlesQuery(AcceptedResults, true);
    SqlCommand Command = new SqlCommand(Query, Connection);
    ArticlesNumber = int.Parse(Command.ExecuteScalar().ToString());
  }
}