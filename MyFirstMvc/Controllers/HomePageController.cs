using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyFirstMvc.Models;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.Security;

namespace MyFirstMvc.Controllers
{
    public class HomePageController : Controller
    {
        private bool islogin = false;
        private static string KEY = ":mlbjqys@111";
        //连接用户信息数据库
        private static string connectStr = 
            "Data Source=BRUCEZHANG-PC;Initial Catalog=Customer;Persist Security Info=True;User ID=sa;Password=1993";
        public SqlConnection connection = new SqlConnection(connectStr);
        public DataSet CustomerDS = new DataSet();
        public DataTable dt = new DataTable();

        public ActionResult Index()
        {
            if (Request.Cookies["account"] != null && Request.Cookies["ssid"] != null)
            {
                string account = Request.Cookies["account"].Value;
                string ssid = Request.Cookies["ssid"].Value;
                islogin = ssid.Equals(Encrypt(account+KEY));
            }
            ViewBag.i = islogin;
            SqlDataAdapter selectAdapter = 
                new SqlDataAdapter(@"select UserName,UserId from CustomerInfo",connection);
            selectAdapter.Fill(CustomerDS, "CustomerInfo");
            dt = CustomerDS.Tables["CustomerInfo"];
            int i = dt.Rows.Count; 
            return View(dt);
        }
        
        public ActionResult Register()
        {
            
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterModel rm)
        {
            //var rm = new RegisterModel { UserName = username,UserId = userid, UserPassWord = userpassword };

            if (rm.UserName == null)
            {
                ModelState.AddModelError("userName", "用户名不能为空。");
            }
            if (rm.UserId.Length != 3)
            {
                ModelState.AddModelError("userId", "ID必须为三位数字。");
            }
            if (rm.UserPassWord.Length != 3)
            {
                ModelState.AddModelError("userPassword", "密码必须为三位。");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    string sqlcom =
                        @"insert into CustomerInfo values('" + rm.UserId + "','" + rm.UserName + "','" + rm.UserPassWord + "')";
                    SqlDataAdapter insertAdapter = new SqlDataAdapter(sqlcom, connection);
                    insertAdapter.Fill(CustomerDS,"CustomerRegist");
                    return Redirect("Index");
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError("UserId",ex.Message);
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        public ActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogIn(RegisterModel rm)
        {
            SqlDataAdapter longinAdapter =
                new SqlDataAdapter(@"select UserId,UserPassword from CustomerInfo", connection);
            longinAdapter.Fill(CustomerDS, "LogCustomerInfo");
            dt = CustomerDS.Tables["LogCustomerInfo"];
            foreach (System.Data.DataRow dr in dt.Rows)
            {
                if ( ((string)dr["UserId"]).Trim() == rm.UserId )
                {
                    if (((string)dr["UserPassWord"]).Trim() == rm.UserPassWord)
                    {
                        string account = rm.UserId;
                        string password = rm.UserPassWord;
                        string ssid = Encrypt(account + KEY);
                        HttpCookie accountCookie = new HttpCookie("account", account);
                        accountCookie.Expires = DateTime.Now.AddDays(1);
                        accountCookie.Path = "/";
                        HttpCookie ssidCookie = new HttpCookie("ssid", ssid);
                        ssidCookie.Expires = DateTime.Now.AddDays(1);
                        ssidCookie.Path = "/";
                        Response.Cookies.Add(accountCookie);
                        Response.Cookies.Add(ssidCookie);

                        return Redirect("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("userPassword", "密码错误，请重新输入.");
                        return View();
                    }
                }

            }
            ModelState.AddModelError("userId", "账户不存在，请重新输入.");
            return View();
        }

        public ActionResult LogOut()
        {
            Response.Cookies["account"].Expires = DateTime.Now.AddDays(0);
            Response.Cookies["ssid"].Expires = DateTime.Now.AddDays(0);
            return Redirect("Index");
        }

        public ActionResult UpLoad()
        {
            string del = @"delete from MapInfo where enemyName=' ' ";
            SqlDataAdapter infodel = new SqlDataAdapter(del, connection);
            infodel.Fill(CustomerDS, "delete");
            if (Request.Cookies["account"] != null && Request.Cookies["ssid"] != null)
            {
                string account = Request.Cookies["account"].Value;
                string ssid = Request.Cookies["ssid"].Value;
                islogin = ssid.Equals(Encrypt(account + KEY));
            }
            ViewBag.i = islogin;
            return View();
        }
        [HttpPost]
        public ActionResult UpLoad(HttpPostedFileBase Uploadfile,EnemyInfoModel em)
        {
            if (Uploadfile == null)
            {
                ModelState.AddModelError("picUpload", "文件未选中，请重新选择。");
                return View();
            }
            double size = Uploadfile.ContentLength;
            string mime = Uploadfile.ContentType;
            string fileName = Uploadfile.FileName;
            //正则表达式判断后缀名
            Regex reg = new Regex(@"\.(jpg|jpeg|bmp|gif|png)$");
            if (!reg.IsMatch(fileName.ToLower()))
            {
                ModelState.AddModelError("picUpload", "文件格式不正确，请选择后缀名为jpg,jpeg,bmp,gif,png的图片。");
                return View();
            }
            if (em.EnemyName == null || em.EnemyName == "")
            {
                ModelState.AddModelError("controller", "请输入敌人姓名。");
                return View();
            }
            //string newFileLocation = @"D:/Files/";
            //判断路径存在与否
            string savepath = Server.MapPath("~/Images/enemy/");
            if (!Directory.Exists(savepath))
                {
                    Directory.CreateDirectory(savepath);
                }
            //操作数据库
            try
            {
                string suffix = reg.Match(fileName).ToString();
                em.EnemyPic = System.Guid.NewGuid() + suffix;
                string sqlcom =
                    @"insert into MapInfo values('" + em.EnemyPic + "','" + em.EnemyName + "','"
                    + em.EnemyBlood + "','" + em.EnemySouls + "','" + em.EnemyItem + "')";
                SqlDataAdapter insertAdapter = new SqlDataAdapter(sqlcom, connection);
                insertAdapter.Fill(CustomerDS, "UploadMapInfo");
            }
            catch (Exception ex){
                ModelState.AddModelError("controller", ex.Message);
                return View();
            }
            //保存图片
            Uploadfile.SaveAs(savepath + em.EnemyPic);

            return Content("上传成功");
            /*使用uploadify控件上传文件
             * if (Uploadfile != null)
            {
                try
                {
                    string filePath = Server.MapPath("~/Images/enemy/");
                    //判断路径是否存在
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    string fileName = Path.GetFileName(Uploadfile.FileName); //原始文件名称
                    string fileExtension = Path.GetExtension(fileName);     //文件扩展名
                    string saveName = Guid.NewGuid().ToString() + fileExtension;
                    Uploadfile.SaveAs(filePath + saveName);
                    
                    return Json(new { Success = true, FileName = fileName, SaveName = saveName });
                }
                catch (Exception ex)
                {
                    return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Success = false, Message = "请选择要上传的文件!" }, JsonRequestBehavior.AllowGet);
            }*/
        }
        //加密
        public static string Encrypt(string str)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5");
        }
        //解密
        public static string Decrypt(string str)
        {
            return FormsAuthentication.Decrypt(str).Name.ToString();
        }
    }
}
