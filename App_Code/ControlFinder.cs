using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

public class ControlFinder<T> where T : Control
{
  private readonly List<T> _foundControls = new List<T>();
  public IEnumerable<T> FoundControls
  {
    get { return _foundControls; }
  }

  public void FindChildControlsRecursive(Control control)
  {
    foreach (Control childControl in control.Controls)
    {
      if (childControl.GetType() == typeof(T))
      {
        _foundControls.Add((T)childControl);
      }
      else
      {
        FindChildControlsRecursive(childControl);
      }
    }
  }
}