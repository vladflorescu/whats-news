using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class ManageUsers : BasePage
{
  protected List<string> GetUsersNamesWithRoleDifferentThan(string Role)
  {
    List<string> ArgumentRoleUserNames = Roles.GetUsersInRole(Role).ToList();
    List<string> NonArgumentRoleUserNames = new List<string>();

    MembershipUserCollection Users = Membership.GetAllUsers();
    foreach (MembershipUser User in Users)
    {
      if (!ArgumentRoleUserNames.Contains(User.UserName))
      {
        NonArgumentRoleUserNames.Add(User.UserName);
      }
    }

    return NonArgumentRoleUserNames;
  }

  protected void BindUsersToNonAdministratorUsersRepeater()
  {
    ControlFinder<Repeater> RepeaterControlFinder = new ControlFinder<Repeater>();
    RepeaterControlFinder.FindChildControlsRecursive(LVContent);

    //Repeater NonAdministratorUsersRepeater = RepeaterControlFinder.FoundControls
    //  .ToList()
    //  .ElementAt(0);

    //Pare ca merge si pe mai multe nivele...
    Repeater NonAdministratorUsersRepeater = (Repeater)LVContent.FindControl("RNonAdministratorUsers");

    NonAdministratorUsersRepeater.DataSource = GetUsersNamesWithRoleDifferentThan("administrator");
    NonAdministratorUsersRepeater.DataBind();
  }

  protected void Page_Load(object sender, EventArgs e)
  {
    if (!Page.IsPostBack && currentUserIsAdministrator())
    {
      BindUsersToNonAdministratorUsersRepeater();
    }
  }

  protected bool InvisibleInputValuesAreValid()
  {
    string HiddenUserName = ((HtmlInputHidden)LVContent.FindControl("HiddenUserName")).Value;
    string HiddenRoleValue = ((HtmlInputHidden)LVContent.FindControl("HiddenRoleValue")).Value;
    string HiddenToggleValue = ((HtmlInputHidden)LVContent.FindControl("HiddenToggleValue")).Value;

    bool UserNameIsValid = GetUsersNamesWithRoleDifferentThan("administrator")
      .Contains(HiddenUserName);

    bool RoleIsValid = HiddenRoleValue != "administrator"
                         && Roles.GetAllRoles().ToList().Contains(HiddenRoleValue);

    bool ToggleValueIsValid = HiddenToggleValue == "true" || HiddenToggleValue == "false";

    return UserNameIsValid && RoleIsValid && ToggleValueIsValid;
  }

  protected void SubmitChange(object sender, EventArgs e)
  {
    if (InvisibleInputValuesAreValid())
    {
      string HiddenUserName = ((HtmlInputHidden)LVContent.FindControl("HiddenUserName")).Value;
      string HiddenRoleValue = ((HtmlInputHidden)LVContent.FindControl("HiddenRoleValue")).Value;
      string HiddenToggleValue = ((HtmlInputHidden)LVContent.FindControl("HiddenToggleValue")).Value;

      if (HiddenToggleValue == "true")
      {
        Roles.AddUserToRole(HiddenUserName, HiddenRoleValue);
      }
      else
      {
        Roles.RemoveUserFromRole(HiddenUserName, HiddenRoleValue);
      }
    }

    Response.Redirect(Request.Url.AbsolutePath);
  }

  protected void DeleteButton_ServerClick(object sender, EventArgs e)
  {
    string UserName = ((HtmlButton)sender).Attributes["data-user-name"];
    if (GetUsersNamesWithRoleDifferentThan("administrator").Contains(UserName))
    {
      Membership.DeleteUser(UserName);
    }

    Response.Redirect(Request.Url.AbsolutePath);
  }
}