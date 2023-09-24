using GestorDocumentos.Clases;
using GestorDocumentos.Models;
using GestorDocumentos.Models.Request;
using Newtonsoft.Json.Linq;
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

        public string VerificarTipoRol(string nombreRol)
        {
            try
            {
                string qryTabla = "dbo." + '"' + "AspNetRoles" + '"';
                string qryAlias = '"' + "rol" + '"';
                string qryJoin = '"' + "rol" + '"' + "." + '"' + "Name" + '"';
                string qryColumna = '"' + "Name" + '"';

                string result = "";
                string respuesta = "";

                _globales.query = $@"select tr.tipo from {qryTabla} {qryAlias} inner join dbo.rolxtipo rxt
                                     on {qryJoin}=rxt.rolnombre inner join dbo.tipo_rol tr
                                     on rxt.idtipo=tr.id where {qryAlias}.{qryColumna}='{nombreRol}'";
                result = _osql.get(_globales.query, _conexion.Conectar());
                if(result == "A")
                {
                    respuesta = result;
                }
                else
                {
                    respuesta = "SinTipo";
                }

                return respuesta;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "Error";
            }
        }

        public List<RolxTipoRequest> LsRolxTipo()
        {
            try
            {
                string qryTabla = "dbo." + '"' + "AspNetRoles" + '"';
                string qryAlias = '"' + "rol" + '"';
                string qryJoin = '"' + "rol" + '"' + "." + '"' + "Name" + '"';
                string qryColumna = '"' + "Name" + '"';

                DataTable result = new DataTable();
                List<RolxTipoRequest> respuesta = new List<RolxTipoRequest>();

                _globales.query = $@"select {qryJoin} AS nombre,CASE WHEN idtipo > 0 THEN 1 ELSE 0 END AS idtipo, tipo  
                                     from {qryTabla} {qryAlias} left join dbo.rolxtipo rxt
                                     on {qryJoin}=rxt.rolnombre left join dbo.tipo_rol tr on rxt.idtipo=tr.id";

                result = _osql.ddt(_globales.query, _conexion.Conectar());

                foreach(DataRow item in result.Rows)
                {
                    respuesta.Add(new RolxTipoRequest { 
                        nombre=item.Field<string>("nombre"),
                        idtipo=item.Field<int>("idtipo"),
                        tipo=item.Field<string>("tipo")
                    });
                }

                return respuesta;
            }
            catch (Exception ex)
            {
                List<RolxTipoRequest> respuesta = new List<RolxTipoRequest>();
                Console.WriteLine(ex.Message);
                respuesta.Add(new RolxTipoRequest {
                    error = ex.Message
                });
                return respuesta;
            }
        }

        public string GuardarTipoRol(string roles, string tipo)
        {
            try
            {
                // Heredar valores de clase
                List<RolxTipoRequest> value = new List<RolxTipoRequest>();

                // Divide la cadena en elementos individuales
                string[] elementos = roles.Split(',');

                // Utilizar un HashSet para almacenar valores únicos
                HashSet<string> valoresUnicos = new HashSet<string>();

                // Agregar elementos al HashSet (eliminando duplicados)
                foreach (string elemento in elementos)
                {
                    value.Add(new RolxTipoRequest { 
                        nombre=elemento,
                        tipo=tipo
                    });
                }

                if (tipo == "A")
                {
                    foreach(var item in value)
                    {
                        _globales.query = $"INSERT INTO dbo.rolxtipo VALUES('{item.nombre}',1)";
                        _osql.save(_globales.query, _conexion.Conectar());
                    }
                }
                else
                {
                    foreach (var item in value)
                    {
                        _globales.query = $"DELETE FROM dbo.rolxtipo WHERE rolnombre='{item.nombre}' and idtipo=1";
                        _osql.save(_globales.query, _conexion.Conectar());
                    }
                }

                return "OK";
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "Error";
            }
        }

    }
}