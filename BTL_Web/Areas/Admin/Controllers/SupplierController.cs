using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BTL_Web.Library;
using MyBTL.DAO;
using MyBTL.Model;

namespace BTL_Web.Areas.Admin.Controllers
{
    public class SupplierController : Controller
    {
        SuppliersDAO suppliersDAO = new SuppliersDAO();

        //////////////////////////////////////////////////////////////////////
        // GET: Admin/Supplier/Index
        public ActionResult Index()
        {
            //
            return View(suppliersDAO.getlist("Index"));
        }

        //////////////////////////////////////////////////////////////////////
        // GET: Admin/Supplier/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                //Hiện thông báo
                TempData["message"] = new XMessage("danger", "Không tìm thấy");
                return RedirectToAction("Index");
            }
            //Tìm mẫu tin ứng với id=id
            Suppliers suppliers = suppliersDAO.getRow(id);
            if (suppliers == null)
            {
                //Hiện thông báo
                TempData["message"] = new XMessage("danger", "Không tìm thấy");
                return RedirectToAction("Index");
            }
            return View(suppliers);
        }

        //////////////////////////////////////////////////////////////////////
        // GET: Admin/Supplier/Create
        public ActionResult Create()
        {
            ViewBag.OrderList = new SelectList(suppliersDAO.getlist("Index"), "Order", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Suppliers suppliers)
        {
            if (ModelState.IsValid)
            {
                //Xử lý một số trường tự động
                //Create At
                suppliers.CreateAt = DateTime.Now;
                //Update At
                suppliers.UpdateAt = DateTime.Now;
                //CreateBy
                suppliers.CreateBy = Convert.ToInt32(Session["UserID"]);
                //UpdateBy
                suppliers.UpdateBy = Convert.ToInt32(Session["UserID"]);

                //Order
                if (suppliers.Order == null)
                {
                    suppliers.Order = 1;
                }
                else
                {
                    suppliers.Order += 1;
                }
                //Slug
                suppliers.Slug = XString.Str_Slug(suppliers.Name);
                //xu ly cho phan upload hình ảnh
                var img = Request.Files["img"];//lay thong tin file
                if (img.ContentLength != 0)
                {
                    string[] FileExtentions = new string[] { ".jpg", ".jpeg", ".png", ".gif" };
                    //kiem tra tap tin co hay khong
                    if (FileExtentions.Contains(img.FileName.Substring(img.FileName.LastIndexOf("."))))//lay phan mo rong cua tap tin
                    {
                        string slug = suppliers.Slug;
                        //ten file = Slug + phan mo rong cua tap tin
                        string imgName = slug + img.FileName.Substring(img.FileName.LastIndexOf("."));
                        suppliers.Image = imgName;
                        //upload hinh
                        string PathDir = "~/Public/img/supplier";
                        string PathFile = Path.Combine(Server.MapPath(PathDir), imgName);
                        img.SaveAs(PathFile);
                    }
                }//ket thuc phan upload hinh anh
                //Lưu vào DB
                suppliersDAO.Insert(suppliers);
                //Hiện thông báo
                TempData["message"] = new XMessage("success", "Thêm mới nhà cung cấp thành công");
                return RedirectToAction("Index");
            }
            ViewBag.OrderList = new SelectList(suppliersDAO.getlist("Index"), "Order", "Name");
            return View(suppliers);
        }


        //////////////////////////////////////////////////////////////////////
        // GET: Admin/Supplier/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                //Hiện thông báo
                TempData["message"] = new XMessage("danger", "Không tìm thấy");
                return RedirectToAction("Index");
            }
            Suppliers suppliers = suppliersDAO.getRow(id);
            if (suppliers == null)
            {
                //Hiện thông báo
                TempData["message"] = new XMessage("danger", "Không tìm thấy");
                return RedirectToAction("Index");
            }
            ViewBag.OrderList = new SelectList(suppliersDAO.getlist("Index"), "Order", "Name");
            return View(suppliers);
        }

        // POST: Admin/Supplier/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Suppliers suppliers)
        {
            if (ModelState.IsValid)
            {
                //Xử lý một số trường tự động
                //Update At
                suppliers.UpdateAt = DateTime.Now;
                //CreateBy
                suppliers.CreateBy = Convert.ToInt32(Session["UserID"]);
                //UpdateBy
                suppliers.UpdateBy = Convert.ToInt32(Session["UserID"]);
                //Slug
                suppliers.Slug = XString.Str_Slug(suppliers.Name);
                //Order
                if (suppliers.Order == null)
                {
                    suppliers.Order = 1;
                }
                else
                {
                    suppliers.Order += 1;
                }

                //Trước khi cập nhật thì xóa ảnh cũ
                var img = Request.Files["img"];//lay thong tin file
                string PathDir = "~/Public/img/supplier";
                if (img.ContentLength != 0 && suppliers.Image != null)//Tồn tại một logo trước đó
                {
                    //Xóa ảnh cũ
                    string DelPath = Path.Combine(Server.MapPath(PathDir), suppliers.Image);
                    System.IO.File.Delete(DelPath);
                }
                //Up hình ảnh mới
                //xu ly cho phan upload hình ảnh
                if (img.ContentLength != 0)
                {
                    string[] FileExtentions = new string[] { ".jpg", ".jpeg", ".png", ".gif" };
                    //kiem tra tap tin co hay khong
                    if (FileExtentions.Contains(img.FileName.Substring(img.FileName.LastIndexOf("."))))//lay phan mo rong cua tap tin
                    {
                        string slug = suppliers.Slug;
                        //ten file = Slug + phan mo rong cua tap tin
                        string imgName = slug + img.FileName.Substring(img.FileName.LastIndexOf("."));
                        suppliers.Image = imgName;
                        //upload hinh
                        string PathFile = Path.Combine(Server.MapPath(PathDir), imgName);
                        img.SaveAs(PathFile);
                    }
                }//ket thuc phan upload hinh anh
                //Lưu vào DB
                suppliersDAO.Update(suppliers);
                //Hiện thông báo
                TempData["message"] = new XMessage("success", "Cập nhật nhà cung cấp thành công");
                return RedirectToAction("Index");

            }
            ViewBag.OrderList = new SelectList(suppliersDAO.getlist("Index"), "Order", "Name");
            return View(suppliers);
        }

        //////////////////////////////////////////////////////////////////////
        // GET: Admin/Supplier/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                //Hiện thông báo
                TempData["message"] = new XMessage("danger", "Không tìm thấy");
                return RedirectToAction("Index");
            }
            Suppliers suppliers = suppliersDAO.getRow(id);
            if (suppliers == null)
            {
                //Hiện thông báo
                TempData["message"] = new XMessage("danger", "Không tìm thấy");
                return RedirectToAction("Index");
            }
            return View(suppliers);
        }
        // POST: Admin/Supplier/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Suppliers suppliers = suppliersDAO.getRow(id);
            //tim va xoa anh cua NCC
            if (suppliersDAO.Delete(suppliers) == 1)
            {
                string PathDir = "~/Public/img/supplier";
                if (suppliers.Image != null)//ton tai mot logo cua NCC tu truoc
                {
                    //xoa anh cu
                    string DelPath = Path.Combine(Server.MapPath(PathDir), suppliers.Image);
                    System.IO.File.Delete(DelPath);
                }
            }
            //hien thi thong bao thanh cong
            TempData["message"] = new XMessage("success", "Xóa nhà cung cấp thành công");
            return RedirectToAction("Trash");
        }

        ////////////////////////////////////////////////////////////////////
        // GET: Admin/Supplier/Status/5
        public ActionResult Status(int? id)
        {
            if (id == null)
            {
                //hien thi thong bao
                TempData["message"] = new XMessage("danger", "Cập nhật tình trạng nhà cung cấp thất bại");
                return RedirectToAction("Index");
            }
            Suppliers suppliers = suppliersDAO.getRow(id);
            if (suppliers == null)
            {
                //hien thi thong bao
                TempData["message"] = new XMessage("danger", "Cập nhật tình trạng nhà cung cấp thất bại");
                return RedirectToAction("Index");
            }
            //cap nhat trang thai
            suppliers.Status = (suppliers.Status == 1) ? 2 : 1;
            //cap nhạt Update At
            suppliers.UpdateAt = DateTime.Now;
            //cap nhat Update By
            suppliers.UpdateBy = Convert.ToInt32(Session["UserID"]);
            //Update DB
            suppliersDAO.Update(suppliers);
            //hien thi thong bao
            TempData["message"] = new XMessage("success", "Cập nhật tình trạng nhà cung cấp thành công");
            //tro ve trang Index
            return RedirectToAction("Index");
        }

        //////////////////////////////////////////////////////////////////////
        /// DelTrash
        // GET: Admin/Supplier/DelTrash/5
        //Chuyển tin từ status 1,2 sang 0: Ko hiển thị ở index
        public ActionResult DelTrash(int? id)
        {

            if (id == null)
            {
                //Thông báo thất bại
                TempData["message"] = ("danger", "Không tìm thấy nhà cung cấp");
                return RedirectToAction("Index");

            }
            //Truy vấn id
            Suppliers suppliers = suppliersDAO.getRow(id);
            if (suppliers == null)
            {
                //hien thi thong bao
                TempData["message"] = new XMessage("danger", "Không tìm thấy nhà cung cấp");
                return RedirectToAction("Index");
            }

            //Chuyển đổi trạng thái status từ 1,2 => 0: không hiển thị ở index
            suppliers.Status = 0;

            //Cập nhật giá trị updateBy
            suppliers.UpdateBy = Convert.ToInt32(Session["UserID"]);

            //Cập nhật giá trị updateAt
            suppliers.UpdateAt = DateTime.Now;

            // Cập nhật lại Database
            suppliersDAO.Update(suppliers);

            //Thông báo trạng thái thành công
            TempData["message"] = new XMessage("success", "Xóa nhà cung cấp thành công");
            return RedirectToAction("Index");
        }

        ///////////////////////////////////////////////////////////////////////
        ///TRASH: Thùng rác
        // GET: Admin/Trash
        public ActionResult Trash()
        {
            return View(suppliersDAO.getlist("Trash"));
        }

        /////////////////////////////////////////////////////////////////////////
        /// Recover
        // GET: Admin/Supplier/Recover/5
        public ActionResult Recover(int? id)
        {

            if (id == null)
            {
                //Thông báo thất bại
                TempData["message"] = ("danger", "Không phục hồi được");
                return RedirectToAction("Index");

            }
            //Truy vấn id
            Suppliers suppliers = suppliersDAO.getRow(id);
            if (suppliers == null)
            {
                //hien thi thong bao
                TempData["message"] = new XMessage("danger", "Không phục hồi được");
                return RedirectToAction("Index");
            }

            //Chuyển đổi trạng thái status từ 0->2:Không xuất bản
            suppliers.Status = 2;

            //Cập nhật giá trị updateBy
            suppliers.UpdateBy = Convert.ToInt32(Session["UserID"]);

            //Cập nhật giá trị updateAt
            suppliers.UpdateAt = DateTime.Now;

            // Cập nhật lại Database
            suppliersDAO.Update(suppliers);

            //Thông báo phục hồi thành công
            TempData["message"] = new XMessage("success", "Phục hồi nhà cung cấp thành công");
            return RedirectToAction("Trash");
        }
    }
}
