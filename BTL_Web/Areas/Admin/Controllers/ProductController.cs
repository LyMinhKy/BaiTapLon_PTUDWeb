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
    public class ProductController : Controller
    {
        ProductsDAO productsDAO = new ProductsDAO();
        CategoriesDAO categoriesDAO = new CategoriesDAO();
        SuppliersDAO suppliersDAO = new SuppliersDAO();

        /////////////////////////////////////////////////////////
        // GET: Admin/Product/Index
        public ActionResult Index()
        {
            //
            return View(productsDAO.getlist("Index"));
        }

        /////////////////////////////////////////////////////////
        // GET: Admin/Product/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                //Hiện thông báo
                TempData["message"] = new XMessage("danger", "Không tìm thấy");
                return RedirectToAction("Index");
            }
            //Tìm mẫu tin ứng với id=id
            Products products = productsDAO.getRow(id);
            if (products == null)
            {
                //Hiện thông báo
                TempData["message"] = new XMessage("danger", "Không tìm thấy");
                return RedirectToAction("Index");
            }
            return View(products);
        }

        /////////////////////////////////////////////////////////////////////////////////
        // GET: Admin/Product/Create
        public ActionResult Create()
        {
            ViewBag.ListCatID = new SelectList(categoriesDAO.getlist("Index"), "Id", "Name");//sai CatId - truy van tu bang Categories
            ViewBag.ListSupID = new SelectList(suppliersDAO.getlist("Index"), "Id", "Name");//sai SupplierID - truy van bang Suppliers
            //dùng để lựa chon từ danh sách droplist như bảng C: Parent Id và S: Parent Id
            return View();
        }

        // POST: Admin/Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Products products)
        {
            if (ModelState.IsValid)
            {
                //Xử lý một số trường tự động
                //Create At
                products.CreateAt = DateTime.Now;
                //Update At
                products.UpdateAt = DateTime.Now;
                //CreateBy
                products.CreateBy = Convert.ToInt32(Session["UserID"]);
                //UpdateBy
                products.UpdateBy = Convert.ToInt32(Session["UserID"]);
                //Slug
                products.Slug = XString.Str_Slug(products.Name);
                //xu ly cho phan upload hình ảnh
                var img = Request.Files["img"];//lay thong tin file
                if (img.ContentLength != 0)
                {
                    string[] FileExtentions = new string[] { ".jpg", ".jpeg", ".png", ".gif" };
                    //kiem tra tap tin co hay khong
                    if (FileExtentions.Contains(img.FileName.Substring(img.FileName.LastIndexOf("."))))//lay phan mo rong cua tap tin
                    {
                        string slug = products.Slug;
                        //ten file = Slug + phan mo rong cua tap tin
                        string imgName = slug + img.FileName.Substring(img.FileName.LastIndexOf("."));
                        products.Img = imgName;
                        //upload hinh
                        string PathDir = "~/Public/img/product";
                        string PathFile = Path.Combine(Server.MapPath(PathDir), imgName);
                        img.SaveAs(PathFile);
                    }
                }//ket thuc phan upload hinh anh
                //Lưu vào DB
                productsDAO.Insert(products);
                //Hiện thông báo
                TempData["message"] = new XMessage("success", "Thêm mới sản phâm thành công");
                return RedirectToAction("Index");
            }
            ViewBag.ListCatID = new SelectList(categoriesDAO.getlist("Index"), "Id", "Name");//sai CatId - truy van tu bang Categories
            ViewBag.ListSupID = new SelectList(suppliersDAO.getlist("Index"), "Id", "Name");//sai SupplierID - truy van bang Suppliers
            return View(products);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        // GET: Admin/Product/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                //Hiện thông báo
                TempData["message"] = new XMessage("danger", "Không tìm thấy");
                return RedirectToAction("Index");
            }
            Products products = productsDAO.getRow(id);
            if (products == null)
            {
                //Hiện thông báo
                TempData["message"] = new XMessage("danger", "Không tìm thấy");
                return RedirectToAction("Index");
            }
            ViewBag.ListCatID = new SelectList(categoriesDAO.getlist("Index"), "Id", "Name");//sai CatId - truy van tu bang Categories
            ViewBag.ListSupID = new SelectList(suppliersDAO.getlist("Index"), "Id", "Name");//sai SupplierID - truy van bang Suppliers
            return View(products);
        }

        // POST: Admin/Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Products products)
        {
            if (ModelState.IsValid)
            {
                //Xử lý một số trường tự động
                //Update At
                products.UpdateAt = DateTime.Now;
                //Slug
                products.Slug = XString.Str_Slug(products.Name);

                //Trước khi cập nhật thì xóa ảnh cũ
                var img = Request.Files["img"];//lay thong tin file
                string PathDir = "~/Public/img/product";
                if (img.ContentLength != 0 && products.Img != null)//Tồn tại một logo trước đó
                {
                    //Xóa ảnh cũ
                    string DelPath = Path.Combine(Server.MapPath(PathDir), products.Img);
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
                        string slug = products.Slug;
                        //ten file = Slug + phan mo rong cua tap tin
                        string imgName = slug + img.FileName.Substring(img.FileName.LastIndexOf("."));
                        products.Img = imgName;
                        //upload hinh
                        string PathFile = Path.Combine(Server.MapPath(PathDir), imgName);
                        img.SaveAs(PathFile);
                    }
                }//ket thuc phan upload hinh anh
                //Lưu vào DB
                productsDAO.Update(products);
                //Hiện thông báo
                TempData["message"] = new XMessage("success", "Cập nhật sản phẩm thành công");
                return RedirectToAction("Index");

            }
            return View(products);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        // GET: Admin/Product/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                //thong bao that bai
                TempData["message"] = new XMessage("danger", "Không tồn tại sản phẩm");
                return RedirectToAction("Index");
            }
            Products products = productsDAO.getRow(id);
            if (products == null)
            {
                //thong bao that bai
                TempData["message"] = new XMessage("danger", "Không tồn tại sản phẩm");
                return RedirectToAction("Index");
            }
            return View(products);
        }

        // POST: Admin/Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Products products = productsDAO.getRow(id);
            productsDAO.Delete(products);
            //thong bao xoa mau tin thanh cong
            TempData["message"] = new XMessage("success", "Xóa sản phẩm thành công");
            return RedirectToAction("Trash");
        }

        //////////////////////////////////////////////////////////////////////////
        // GET: Admin/Products/Status/5
        public ActionResult Status(int? id)
        {
            if (id == null)
            {
                //hien thi thong bao
                TempData["message"] = new XMessage("danger", "Cập nhật trạng thái thất bại");
                return RedirectToAction("Index");
            }
            Products products = productsDAO.getRow(id);
            if (products == null)
            {
                //hien thi thong bao
                TempData["message"] = new XMessage("danger", "Cập nhật trạng thái thất bại");
                return RedirectToAction("Index");
            }
            //cap nhat trang thai
            products.Status = (products.Status == 1) ? 2 : 1;
            //cap nhạt Update At
            products.UpdateAt = DateTime.Now;

            //Update DB
            productsDAO.Update(products);
            //hien thi thong bao
            TempData["message"] = new XMessage("success", "Cập nhật trạng thái thành công");
            //tro ve trang Index
            return RedirectToAction("Index");
        }

        ////////////////////////////////////////////////////////////////////
        /// DelTrash
        // GET: Admin/Product/DelTrash/5
        //Chuyển tin từ status 1,2 sang 0: Ko hiển thị ở index
        public ActionResult DelTrash(int? id)
        {

            if (id == null)
            {
                //Thông báo thất bại
                TempData["message"] = ("danger", "Không tìm thấy thông tin");
                return RedirectToAction("Index");

            }
            //Truy vấn id
            Products products = productsDAO.getRow(id);
            if (products == null)
            {
                //hien thi thong bao
                TempData["message"] = new XMessage("danger", "Không tìm thấy thông tin");
                return RedirectToAction("Index");
            }

            //Chuyển đổi trạng thái status từ 1,2 => 0: không hiển thị ở index
            products.Status = 0;

            //Cập nhật giá trị updateAt
            products.UpdateAt = DateTime.Now;

            // Cập nhật lại Database
            productsDAO.Update(products);

            //Thông báo trạng thái thành công
            TempData["message"] = new XMessage("success", "Xóa thông tin thành công");
            return RedirectToAction("Index");
        }

        /////////////////////////////////////////////////////////////////////////////////////
        //TRASH: Luc thung rac
        // GET: Admin/Products/Trash
        public ActionResult Trash()
        {
            return View(productsDAO.getlist("Trash"));
        }

        /////////////////////////////////////////////////////////////////////////////////////////
        /// Recover
        // GET: Admin/Product/Recover/5
        public ActionResult Recover(int? id)
        {

            if (id == null)
            {
                //Thông báo thất bại
                TempData["message"] = ("danger", "Không phục hồi được");
                return RedirectToAction("Index");

            }
            //Truy vấn id
            Products products = productsDAO.getRow(id);
            if (products == null)
            {
                //hien thi thong bao
                TempData["message"] = new XMessage("danger", "Không phục hồi được");
                return RedirectToAction("Index");
            }

            //Chuyển đổi trạng thái status từ 0->2:Không xuất bản
            products.Status = 2;

            //Cập nhật giá trị updateAt
            products.UpdateAt = DateTime.Now;

            // Cập nhật lại Database
            productsDAO.Update(products);

            //Thông báo phục hồi thành công
            TempData["message"] = new XMessage("success", "Phục hồi thông tin thành công");
            return RedirectToAction("Trash");
        }
    }
}
