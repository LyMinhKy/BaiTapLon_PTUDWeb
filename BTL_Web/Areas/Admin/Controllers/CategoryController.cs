using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BTL_Web.Library;
using MyBTL.DAO;
using MyBTL.Model;

namespace BTL_Web.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        CategoriesDAO categoriesDAO = new CategoriesDAO();
        //private MyDBContext db = new MyDBContext();
        ///////////////////////////////////////////////////////////////////////////////////////////
        ///INDEX
        // GET: Admin/Category
        public ActionResult Index()
        {
            return View(categoriesDAO.getlist("Index"));
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        /// DETAIL
        // GET: Admin/Category/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                TempData["message"] = new XMessage("danger", "Không tìm thấy sản phẩm");
                return RedirectToAction("Index");
            }
            Categories categories = categoriesDAO.getRow(id);
            if (categories == null)
            {
                TempData["message"] = new XMessage("danger", "Không tìm thấy sản phẩm");
                return RedirectToAction("Index"); 
            }
            return View(categories);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        /// CREATE
        // GET: Admin/Category/Create
        public ActionResult Create()
        {
            ViewBag.CatList = new SelectList(categoriesDAO.getlist("Index"), "Id", "Name");
            ViewBag.OrderList = new SelectList(categoriesDAO.getlist("Index"), "Order", "Name");
            return View();
        }
        
        // POST: Admin/Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Categories categories)
        {
            if (ModelState.IsValid)
            {
                //Xử lý một số trường tự động
                //Create At
                categories.CreateAt = DateTime.Now;
                //Update At
                categories.UpdateAt = DateTime.Now;
                //CreateBy
                categories.CreateBy = Convert.ToInt32(Session["UserID"]);
                //UpdateBy
                categories.UpdateBy = Convert.ToInt32(Session["UserID"]);
                //ParentId
                if (categories.ParentId == null)
                {
                    categories.ParentId = 0;
                }
                //Order
                if (categories.Order == null)
                {
                    categories.Order = 1;
                }
                else
                {
                    categories.Order += 1;
                }
                //Slug
                categories.Slug = XString.Str_Slug(categories.Name);
                //Thêm mới dòng dữ liệu
                categoriesDAO.Insert(categories);
                //Thông báo dữ liệu thành công
                TempData["message"] = new XMessage("success", "Tạo mới sản phẩm thành công");
                return RedirectToAction("Index");
            }
            ViewBag.CatList = new SelectList(categoriesDAO.getlist("Index"), "Id", "Name");
            ViewBag.OrderList = new SelectList(categoriesDAO.getlist("Index"), "Order", "Name");
            return View(categories);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        /// EDIT
        // GET: Admin/Category/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                if (id == null)
                {
                    //Thông báo thất bại
                    TempData["message"] = ("danger", "Cập nhật sản phẩm thất bại");
                    return RedirectToAction("Index");

                }
            }
            Categories categories = categoriesDAO.getRow(id);
            if (categories == null)
            {
                //Thông báo thất bại
                TempData["message"] = ("danger", "Cập nhật sản phẩm thất bại");
                return RedirectToAction("Index");
            }
            ViewBag.CatList = new SelectList(categoriesDAO.getlist("Index"), "Id", "Name");
            ViewBag.OrderList = new SelectList(categoriesDAO.getlist("Index"), "Order", "Name");
            return View(categories);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Categories categories)
        {
            if (ModelState.IsValid)
            {
                //Cập nhật một số trường thông tin
                //ParentId
                if (categories.ParentId == null)
                {
                    categories.ParentId = 0;
                }
                //Order
                if (categories.Order == null)
                {
                    categories.Order = 1;
                }
                else
                {
                    categories.Order += 1;
                }
                //Slug
                categories.Slug = XString.Str_Slug(categories.Name);
                //Update At
                categories.UpdateAt = DateTime.Now;

                // Cập nhật lại Database
                categoriesDAO.Update(categories);
                categories.UpdateBy = Convert.ToInt32(Session["UserID"]);
                //Thông báo thất bại
                TempData["message"] = new XMessage("success", "Cập nhật sản phẩm thành công");
                return RedirectToAction("Index");
            }
            ViewBag.CatList = new SelectList(categoriesDAO.getlist("Index"), "Id", "Name");
            ViewBag.OrderList = new SelectList(categoriesDAO.getlist("Index"), "Order", "Name");
            return View(categories);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        /// DELETE
        // GET: Admin/Category/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                //Thông báo thất bại
                TempData["message"] = new XMessage("danger", "Xóa sản phẩm thất bại");
                return RedirectToAction("Index");
            }
            Categories categories = categoriesDAO.getRow(id);
            if (categories == null)
            {
                //Thông báo thất bại
                TempData["message"] = new XMessage("danger", "Xóa sản phẩm thất bại");
                return RedirectToAction("Index");
            }
            return View(categories);
        }

        // POST: Admin/Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //delete 1 dòng dữ liệu
            Categories categories = categoriesDAO.getRow(id);
            categoriesDAO.Delete(categories);

            //Thông báo thành công
            TempData["message"] = new XMessage("success", "Xóa sản phẩm thành công");
            return RedirectToAction("Trash");
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        /// status
        // GET: Admin/Category/Status/5
        public ActionResult Status(int? id)
        {

            if (id == null)
            {
                //Thông báo thất bại
                TempData["message"] = ("danger", "Thay đổi tình trạng sản phẩm thất bại");
                return RedirectToAction("Index");

            }
            //Truy vấn id
            Categories categories = categoriesDAO.getRow(id);
            if (categories == null)
            {
                //hien thi thong bao
                TempData["message"] = new XMessage("danger", "Thay đổi tình trạng sản phẩm thất bại");
                return RedirectToAction("Index");
            }

            //Chuyển đổi trạng thái status từ 1<->2
            categories.Status = (categories.Status == 1) ? 2 : 1;

            //Cập nhật giá trị updateBy
            categories.UpdateBy = Convert.ToInt32(Session["UserID"]);

            //Cập nhật giá trị updateAt
            categories.UpdateAt = DateTime.Now;

            // Cập nhật lại Database
            categoriesDAO.Update(categories);

            //Thông báo trạng thái thành công
            TempData["message"] = new XMessage("success", "Thay đổi tình trạng sản phẩm thành công");
            return RedirectToAction("Index");
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        /// DelTrash
        // GET: Admin/Category/DelTrash/5
        //Chuyển tin từ status 1,2 sang 0: Ko hiển thị ở index
        public ActionResult DelTrash(int? id)
        {

            if (id == null)
            {
                //Thông báo thất bại
                TempData["message"] = ("danger", "Không tìm thấy sản phẩm");
                return RedirectToAction("Index");

            }
            //Truy vấn id
            Categories categories = categoriesDAO.getRow(id);
            if (categories == null)
            {
                //hien thi thong bao
                TempData["message"] = new XMessage("danger", "Không tìm thấy sản phẩm");
                return RedirectToAction("Index");
            }

            //Chuyển đổi trạng thái status từ 1,2 => 0: không hiển thị ở index
            categories.Status = 0;

            //Cập nhật giá trị updateBy
            categories.UpdateBy = Convert.ToInt32(Session["UserID"]);

            //Cập nhật giá trị updateAt
            categories.UpdateAt = DateTime.Now;

            // Cập nhật lại Database
            categoriesDAO.Update(categories);

            //Thông báo trạng thái thành công
            TempData["message"] = new XMessage("success", "Xóa sản phẩm thành công");
            return RedirectToAction("Index");
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        ///TRASH
        // GET: Admin/Trash
        public ActionResult Trash()
        {
            return View(categoriesDAO.getlist("Trash"));
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        /// Recover
        // GET: Admin/Category/Delete/5
        public ActionResult Recover(int? id)
        {

            if (id == null)
            {
                //Thông báo thất bại
                TempData["message"] = ("danger", "Không phục hồi được");
                return RedirectToAction("Index");

            }
            //Truy vấn id
            Categories categories = categoriesDAO.getRow(id);
            if (categories == null)
            {
                //hien thi thong bao
                TempData["message"] = new XMessage("danger", "Không phục hồi được");
                return RedirectToAction("Index");
            }

            //Chuyển đổi trạng thái status từ 0->2:Không xuất bản
            categories.Status = 2;

            //Cập nhật giá trị updateBy
            categories.UpdateBy = Convert.ToInt32(Session["UserID"]);

            //Cập nhật giá trị updateAt
            categories.UpdateAt = DateTime.Now;

            // Cập nhật lại Database
            categoriesDAO.Update(categories);

            //Thông báo phục hồi thành công
            TempData["message"] = new XMessage("success", "Phục hồi sản phẩm thành công");
            return RedirectToAction("Index");
        }
    }
}
