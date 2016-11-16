using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace MyFirstMvc.Controllers
{
    public class DarkSoulsMapController : Controller
    {
        //
        // GET: /DarkSouls/
        private static string connectStr =
            "Data Source=BRUCEZHANG-PC;Initial Catalog=Customer;Persist Security Info=True;User ID=sa;Password=1993";
        public SqlConnection connection = new SqlConnection(connectStr);
        public DataSet DarkSoulsDS = new DataSet();
        public DataTable dt = new DataTable();

        public ActionResult Map1()
        {
            SqlDataAdapter selectAdapter = new SqlDataAdapter(@"select * from MapInfo",connection);
            selectAdapter.Fill(DarkSoulsDS, "MapInfo");
            dt = DarkSoulsDS.Tables["MapInfo"];
            int i = dt.Rows.Count;
            return View(dt);
        }

        public ActionResult Map2()
        {
            return View();
        }
        public ActionResult Map3()
        {
            return View();
        }
        public ActionResult Map4()
        {
            return View();
        }
    }
}
