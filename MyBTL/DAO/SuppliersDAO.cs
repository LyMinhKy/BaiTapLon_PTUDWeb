using MyBTL.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBTL.DAO
{
    public class SuppliersDAO
    {
        private MyDBContext db = new MyDBContext();

        //SELECT * FROM...
        public List<Suppliers> getlist()
        {
            return db.Suppliers.ToList();
        }
        //Index trả về các status 1,2 và 0 <=> thùng rác

        //SELECT * FROM... chỉ với status 1,2
        public List<Suppliers> getlist(string status = "ALL")//status 0,1,2
        {
            List<Suppliers> list = null;
            switch (status)
            {
                case "Index"://1,2
                    {
                        list = db.Suppliers.Where(m => m.Status != 0).ToList();
                        break;
                    }
                case "Trash"://0
                    {
                        list = db.Suppliers.Where(m => m.Status == 0).ToList();
                        break;
                    }
                default:
                    {
                        list = db.Suppliers.ToList();
                        break;
                    }
            }
            return list;
        }
        //Detail
        public Suppliers getRow(int? id)
        {
            if (id == null)
                return null;
            else
                return db.Suppliers.Find(id);
        }

        //Create
        //Chèn dòng dữ liệu mới 
        public int Insert(Suppliers row)
        {
            db.Suppliers.Add(row);
            return db.SaveChanges();
        }


        //Update
        //Hàm này cập nhật Database
        public int Update(Suppliers row)
        {
            db.Entry(row).State = EntityState.Modified;
            return db.SaveChanges();
        }

        //Delete
        public int Delete(Suppliers row)
        {
            db.Suppliers.Remove(row);
            return db.SaveChanges();
        }
    }
}
