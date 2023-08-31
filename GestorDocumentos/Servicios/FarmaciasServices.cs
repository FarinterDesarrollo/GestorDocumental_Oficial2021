using GestorDocumentos.Clases;
using GestorDocumentos.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GestorDocumentos.Servicios
{
    public class FarmaciasServices
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        Globales _globales = new Globales();
        Conexion _conexion = new Conexion();
        Sqlpg _osql = new Sqlpg();

        public IQueryable<MantenimientoSubniveles> MS_Farmacias_Permisos(int id, int numero, int opt)
        {
            string abecedario = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            List<char> letras = new List<char>();
            List<string> lista = new List<string>();
            string campos = "";
            string elementos = "";
            string extent1 = "Extent1";
            string descripcion = "Descripcion";
            string qryColumna = '"' + descripcion + '"';
            string qryTabla = "dbo." + '"' + "MantenimientoSubniveles" + '"';
            string qryCarpetaEncabezadoid = '"' + "CarpetaEncabezadoid" + '"';
            string qryId = '"' + "Id" + '"';
            string subQuery = "";
            int contador = 0;

            foreach (char letra in abecedario)
            {
                letras.Add(letra);
            }

            int n = 7;
            foreach (char letra in letras)
            {
                for(int i = 0; i < n; i++)
                {
                    lista.Add(letra + Convert.ToString(i));
                }
            }

            for (int x = 0; x < lista.Count; x++)
            {
                if (x == 0)
                {
                    campos = qryColumna + $" NOT LIKE E'{lista[x]}%' AND ";
                }
                else
                {
                    contador += 1;
                    elementos = qryColumna + $" NOT LIKE E'{lista[contador]}%' AND ";
                    campos = campos + elementos;
                }
            }

            int indice = campos.LastIndexOf("AND");
            subQuery = campos.Substring(0, indice);

            string qry = "";

            if (opt == 1)
            {
                qry = $@"SELECT * FROM {qryTabla} 
                            WHERE {qryCarpetaEncabezadoid}={id} 
                            AND {subQuery}
                            union all 
                            SELECT * FROM {qryTabla} 
                            WHERE {qryCarpetaEncabezadoid}={id}";
            }
            else
            {
                qry = $@"SELECT * FROM {qryTabla} 
                            WHERE {qryCarpetaEncabezadoid}={id} 
                            AND {subQuery}
                            union all 
                            SELECT * FROM {qryTabla} 
                            WHERE {qryCarpetaEncabezadoid}={id} AND {qryId}={numero}";
            }

            DataTable dtSubniveles = new DataTable();
            dtSubniveles = _osql.ddt(qry, _conexion.Conectar());

            List<MantenimientoSubniveles> _ms = new List<MantenimientoSubniveles>();

            foreach (DataRow item in dtSubniveles.Rows)
            {
                _ms.Add(new MantenimientoSubniveles { 
                    Id=item.Field<int>("Id"),
                    AreaId=item.Field<int>("AreaId"),
                    CarpetaEncabezadoid= item.Field<int>("CarpetaEncabezadoid"),
                    ConfCarpetaDetalleId= item.Field<int>("ConfCarpetaDetalleId"),
                    Descripcion= item.Field<string>("Descripcion")
                });
            }

            IQueryable<MantenimientoSubniveles> ms = _ms.AsQueryable();

            return ms;
        }

        public List<MantenimientoSubniveles> MS_Farmacias_PermisosLista(int id, int numero, int opt)
        {
            string abecedario = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            List<char> letras = new List<char>();
            List<string> lista = new List<string>();
            string campos = "";
            string elementos = "";
            string extent1 = "Extent1";
            string descripcion = "Descripcion";
            string qryColumna = '"' + descripcion + '"';
            string qryTabla = "dbo." + '"' + "MantenimientoSubniveles" + '"';
            string qryCarpetaEncabezadoid = '"' + "CarpetaEncabezadoid" + '"';
            string qryId = '"' + "Id" + '"';
            string subQuery = "";
            int contador = 0;

            foreach (char letra in abecedario)
            {
                letras.Add(letra);
            }

            int n = 7;
            foreach (char letra in letras)
            {
                for (int i = 0; i < n; i++)
                {
                    lista.Add(letra + Convert.ToString(i));
                }
            }

            for (int x = 0; x < lista.Count; x++)
            {
                if (x == 0)
                {
                    campos = qryColumna + $" NOT LIKE E'{lista[x]}%' AND ";
                }
                else
                {
                    contador += 1;
                    elementos = qryColumna + $" NOT LIKE E'{lista[contador]}%' AND ";
                    campos = campos + elementos;
                }
            }

            int indice = campos.LastIndexOf("AND");
            subQuery = campos.Substring(0, indice);
            string qry = "";

            if (opt == 1)
            {
                qry = $@"SELECT * FROM {qryTabla} 
                            WHERE {qryCarpetaEncabezadoid}={id} 
                            AND {subQuery}
                            union all 
                            SELECT * FROM {qryTabla} 
                            WHERE {qryCarpetaEncabezadoid}={id}";
            }
            else
            {
                qry = $@"SELECT * FROM {qryTabla} 
                            WHERE {qryCarpetaEncabezadoid}={id} 
                            AND {subQuery}
                            union all 
                            SELECT * FROM {qryTabla} 
                            WHERE {qryCarpetaEncabezadoid}={id} AND {qryId}={numero}";
            }

            DataTable dtSubniveles = new DataTable();
            dtSubniveles = _osql.ddt(qry, _conexion.Conectar());

            List<MantenimientoSubniveles> _ms = new List<MantenimientoSubniveles>();

            foreach (DataRow item in dtSubniveles.Rows)
            {
                _ms.Add(new MantenimientoSubniveles
                {
                    Id = item.Field<int>("Id"),
                    AreaId = item.Field<int>("AreaId"),
                    CarpetaEncabezadoid = item.Field<int>("CarpetaEncabezadoid"),
                    ConfCarpetaDetalleId = item.Field<int>("ConfCarpetaDetalleId"),
                    Descripcion = item.Field<string>("Descripcion")
                });
            }

            return _ms;
        }

    }
}