using MyBTL.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBTL.DAO
{
    public class ProductsDAO
    {
        private MyDBContext db = new MyDBContext();

        //SELECT * FROM...
        public List<Products> getlist()
        {
            return db.Products.ToList();
        }
        //Index trả về các status 1,2 và 0 <=> thùng rác

        //SELECT * FROM... chỉ với status 1,2
        public List<Products> getlist(string status = "ALL")//status 0,1,2
        {
            List<Products> list = null;
            switch (status)
            {
                case "Index"://1,2
                    {
                        list = db.Products.Where(m => m.Status != 0).ToList();
                        break;
                    }
                case "Trash"://0
                    {
                        list = db.Products.Where(m => m.Status == 0).ToList();
                        break;
                    }
                default:
                    {
                        list = db.Products.ToList();
                        break;
                    }
            }
            return list;
        }
        //Detail
        public Products getRow(int? id)
        {
            if (id == null)
                return null;
            else
                return db.Products.Find(id);
        }

        //Create
        //Chèn dòng dữ liệu mới 
        public int Insert(Products row)
        {
            db.Products.Add(row);
            return db.SaveChanges();
        }


        //Update
        //Hàm này cập nhật Database
        public int Update(Products row)
        {
            db.Entry(row).State = EntityState.Modified;
            return db.SaveChanges();
        }

        //Delete
        public int Delete(Products row)
        {
            db.Products.Remove(row);
            return db.SaveChanges();
        }
    }
}
