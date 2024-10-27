using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BatDongSan.Models
{
    public class DataModel
    {
        private string connetionString = "workstation id=doanphanmembatdongsan.mssql.somee.com;packet size=4096;user id=vinhbopco_SQLLogin_1;pwd=kn2xcfrz8c;data source=doanphanmembatdongsan.mssql.somee.com;persist security info=False;initial catalog=doanphanmembatdongsan;TrustServerCertificate=True";
        public ArrayList get(string sql)
        {
            ArrayList dataList = new ArrayList();

            try
            {
                // Sử dụng "using" để đảm bảo tài nguyên được giải phóng đúng cách
                using (SqlConnection connection = new SqlConnection(connetionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ArrayList row = new ArrayList();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    row.Add(reader.GetValue(i).ToString());
                                }
                                dataList.Add(row);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                // Xử lý ngoại lệ nếu có lỗi xảy ra
                Console.WriteLine("SQL Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ chung
                Console.WriteLine("Error: " + ex.Message);
            }

            return dataList;
        }
    }
}