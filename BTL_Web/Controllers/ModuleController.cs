using MyBTL.DAO;
using MyBTL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BTL_Web.Controllers
{
    public class ModuleController : Controller
    {
        MenusDAO menusDAO = new MenusDAO();
        ///////////////////////////////////////////////////////////////////
        //GET: MainMenu
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult MainMenu()
        {
            List<Menus> list = menusDAO.getListByParentId(0);
            return View(list);
        }
    }
}