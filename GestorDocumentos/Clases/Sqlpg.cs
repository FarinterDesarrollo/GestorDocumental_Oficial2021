using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace GestorDocumentos.Clases
{
    public class Sqlpg
    {
        public void save(string query, string cxn)
        {
            NpgsqlConnection cox = new NpgsqlConnection(cxn);
            cox.Open();
            NpgsqlDataAdapter Regda = new NpgsqlDataAdapter(query, cox);
            Regda.SelectCommand.CommandTimeout = 180;
            Regda.SelectCommand.ExecuteNonQuery();
            cox.Close();
        }

        public void save2(string query, byte[] documento, string cxn)
        {
            NpgsqlConnection cox = new NpgsqlConnection(cxn);
            cox.Open();
            NpgsqlCommand insertCmd = new NpgsqlCommand(query, cox);
            NpgsqlParameter param = new NpgsqlParameter("dataParam", NpgsqlDbType.Bytea);
            param.Value = documento;
            insertCmd.Parameters.Add(param);
            insertCmd.ExecuteNonQuery();
            cox.Close();
        }

        public byte[] ver_imagen(string query, string cxn)
        {
            NpgsqlConnection cox = new NpgsqlConnection(cxn);
            cox.Open();
            NpgsqlCommand command = new NpgsqlCommand(query, cox);
            byte[] ImageByte = null;
            NpgsqlDataReader rdr = command.ExecuteReader();
            if (rdr.Read())
            {
                ImageByte = (byte[])rdr[0];
            }
            cox.Close();
            return ImageByte;
        }

        public DataTable ddt(string query, string cxn)
        {
            NpgsqlConnection cox = new NpgsqlConnection(cxn);
            DataTable dt = new DataTable();
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(query, cox);
            da.Fill(dt);
            return dt;
        }

        public string get(string query, string cxn)
        {
            string resp;
            NpgsqlConnection cox = new NpgsqlConnection(cxn);
            DataTable dt = new DataTable();
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(query, cox);
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
            NpgsqlConnection cox = new NpgsqlConnection(cxn);
            DataTable dt = new DataTable();
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(query, cox);
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0][0] == System.DBNull.Value)
                {
                    resp = 0;
                }
                else
                {
                    resp = Convert.ToInt16(dt.Rows[0][0]);
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
            NpgsqlConnection cox = new NpgsqlConnection(cxn);
            DataTable dt = new DataTable();
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(query, cox);
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