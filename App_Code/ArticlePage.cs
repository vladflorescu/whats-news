using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ArticlePage
/// </summary>
public class ArticlePage : BasePage
{
    protected bool GoodId = true;
    protected bool ArticleIsAccepted = false;
    protected bool ShowPage = true;

    protected bool IdIsValid(string Id)
    {
      return !String.IsNullOrEmpty(Id) && Id.All(char.IsDigit);
    }
}