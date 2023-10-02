using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace GestorDocumentos.Clases
{
    public class Sql
    {
        public void save(string query, string cxn)
        {
            SqlConnection cox = new SqlConnection(cxn);
            cox.Open();
            SqlDataAdapter Regda = new SqlDataAdapter(query, cox);
            Regda.SelectCommand.CommandTimeout = 180;
            Regda.SelectCommand.ExecuteNonQuery();
            cox.Close();
        }

        public DataTable ddt(string query, string cxn)
        {
            SqlConnection cox = new SqlConnection(cxn);
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(query, cox);
            da.Fill(dt);
            return dt;
        }

        public string get(string query, string cxn)
        {
            string resp;
            SqlConnection cox = new SqlConnection(cxn);
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(query, cox);
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0][0] == System.DBNull.Value)
                {
                    resp = "";
                }
                else
                {
                    resp = Convert.ToString(dt.Rows[0][0]);
                }
            }
            else
            {
                resp = "";
            }
            return resp;
        }

        public int getn(string query, string cxn)
        {
            int resp;
            SqlConnection cox = new SqlConnection(cxn);
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(query, cox);
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0][0] == System.DBNull.Value)
                {
                    resp = 0;
                }
                else
                {
                    resp = Convert.ToInt32(dt.Rows[0][0]);
                }
            }
            else
            {
                resp = 0;
            }
            return resp;
        }

        public double getd(string query, string cxn)
        {
            double resp;
            SqlConnection cox = new SqlConnection(cxn);
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(query, cox);
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0][0] == System.DBNull.Value)
                {
                    resp = 0;
                }
                else
                {
                    resp = Convert.ToDouble(dt.Rows[0][0]);
                }
            }
            else
            {
                resp = 0;
            }
            return resp;
        }
    }
}