using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Suggestions : CollectionPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
      PageNumber = CalculatePageNumber();

      SqlConnection Connection = new SqlConnection(connectionString);
      try
      {
        Connection.Open();
        SetArticlesNumber(Connection, null, false);
        SetSelectCommandForArticlesDataSource(SDSArticles, null, false, true);
      }
      catch (Exception ex)
      {
        Debug.WriteLine("Error in Page_Load: " + ex.Message);
      }
      finally
      {
        Connection.Close();
      }
    }
}