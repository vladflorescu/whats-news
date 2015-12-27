using System;
using System.Collections.Generic;
using System.Data;
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
}