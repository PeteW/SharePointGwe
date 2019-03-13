using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oxbow.GenericDashboard.Core;
using Oxbow.GenericDashboard.Core.Models;

namespace Oxbow.GenericDashboard.Web._layouts.Oxbow.GenericDashboard
{
    public partial class ExcelExport : System.Web.UI.Page
    {
        private GenericDashboardConfiguration GenericDashboardConfiguration
        {
            get
            {
                var urlParam = Request.Params["q"];
                if (string.IsNullOrEmpty(urlParam))
                    throw new ArgumentNullException("q");
                return GenericDashboardConfiguration.Hydrate(urlParam);
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            var workBook = new ExcelRenderer().Render(GenericDashboardConfiguration);
            var stream = new MemoryStream();
            workBook.writeXLSX(stream);
            stream.Seek(0, SeekOrigin.Begin);
            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.openxmlformats";
            Response.AddHeader("Content-Disposition", "attachment; filename=DashboardExport.xlsx");
            Response.BinaryWrite(stream.ToArray());
            Response.Flush();
            Response.End();
        }
    }
}
