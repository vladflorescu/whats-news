using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

/// <summary>
/// Summary description for BasePage
/// </summary>
public class BasePage : System.Web.UI.Page
{
    protected String connectionString = @"Data Source=(localdb)\MSSQLLocalDB; AttachDbFilename=|DataDirectory|\Database.mdf; Integrated Security=True; MultipleActiveResultSets=True";

    protected bool userIsConnected()
    {
      return HttpContext.Current.User.Identity.IsAuthenticated;
    }

    protected bool currentUserIsAdministrator()
    {
      if (!userIsConnected()) return false;

      foreach (string role in Roles.GetRolesForUser())
      {
        if (role == "administrator") return true;
      }

      return false;
    }

    protected bool currentUserIsPublisher()
    {
      if (!userIsConnected()) return false;

      foreach (string role in Roles.GetRolesForUser())
      {
        if (role == "publisher") return true;
      }

      return false;
    }

    protected bool currentUserIsAdministratorOrPublisher()
    {
      return currentUserIsAdministrator() || currentUserIsPublisher();
    }

    protected string GetNewlineCharacters(string Sample)
    {
      if (Sample.Contains("\r\n"))
      {
        return "\r\n";
      }
      else if (Sample.Contains("\r"))
      {
        return "\r";
      }

      return "\n";
    }
}