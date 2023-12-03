using MyBTL.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBTL.DAO
{
    public class CategoriesDAO
    {
        private MyDBContext db = new MyDBContext();

        //SELECT * FROM...
        public List<Categories> getlist()
        {
            return db.Categories.ToList();
        }
        //Index trả về các status 1,2 và 0 <=> thùng rác

        //SELECT * FROM... chỉ với status 1,2
        public List<Categories> getlist(string status = "ALL")//status 0,1,2
        {
            List<Categories> list = null;
            switch (status)
            {
                case "Index"://1,2
                    {
                        list = db.Categories.Where(m => m.Status != 0).ToList();
                        break;
                    }
                case "Trash"://0
                    {
                        list = db.Categories.Where(m => m.Status == 0).ToList();
                        break;
                    }
                default:
                    {
                        list = db.Categories.ToList();
                        break;
                    }
            }
            return list;
        }
        //Detail
        public Categories getRow(int? id)
        {
            if (id == null)
                return null;
            else
                return db.Categories.Find(id);
        }

        //Create
        //Chèn dòng dữ liệu mới 
        public int Insert(Categories row)
        {
            db.Categories.Add(row);
            return db.SaveChanges();
        }

        //Edit
        //Hàm này cập nhật Database
        public int Update(Categories row)
        {
            db.Entry(row).State = EntityState.Modified;
            return db.SaveChanges();
        }

        //Delete
        public int Delete(Categories row)
        {
            db.Categories.Remove(row);
            return db.SaveChanges();
        }
    }
}
