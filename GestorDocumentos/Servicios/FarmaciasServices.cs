using GestorDocumentos.Clases;
using GestorDocumentos.Models;
using GestorDocumentos.Models.Request;
using GestorDocumentos.Models.Response;
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
        Sqlpg _oSqlpg = new Sqlpg();
        Sql _oSql = new Sql();
        
        public IQueryable<MantenimientoSubniveles> MS_Farmacias_Permisos(int id, int numero, int opt)
        {
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


            _globales.query = "select abreviacion from dbo.farmacias_subnivel_detalle";
            DataTable dtAbreaviacion = new DataTable();
            dtAbreaviacion = _oSqlpg.ddt(_globales.query, Conexion.GD);

            for (int x = 0; x < dtAbreaviacion.Rows.Count; x++)
            {
                if (x == 0)
                {
                    campos = qryColumna + $" NOT LIKE E'{dtAbreaviacion.Rows[x]["abreviacion"]}%' AND ";
                }
                else
                {
                    contador += 1;
                    elementos = qryColumna + $" NOT LIKE E'{dtAbreaviacion.Rows[x]["abreviacion"]}%' AND ";
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
            
            dtSubniveles = _oSqlpg.ddt(qry, _conexion.Conectar());

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


            _globales.query = "select abreviacion from dbo.farmacias_subnivel_detalle";
            DataTable dtAbreaviacion = new DataTable();
            dtAbreaviacion = _oSqlpg.ddt(_globales.query, Conexion.GD);

            for (int x = 0; x < dtAbreaviacion.Rows.Count; x++)
            {
                if (x == 0)
                {
                    campos = qryColumna + $" NOT LIKE E'{dtAbreaviacion.Rows[x]["abreviacion"]}%' AND ";
                }
                else
                {
                    contador += 1;
                    elementos = qryColumna + $" NOT LIKE E'{dtAbreaviacion.Rows[x]["abreviacion"]}%' AND ";
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

            dtSubniveles = _oSqlpg.ddt(qry, _conexion.Conectar());

            List<MantenimientoSubniveles> ms = new List<MantenimientoSubniveles>();

            foreach (DataRow item in dtSubniveles.Rows)
            {
                ms.Add(new MantenimientoSubniveles
                {
                    Id = item.Field<int>("Id"),
                    AreaId = item.Field<int>("AreaId"),
                    CarpetaEncabezadoid = item.Field<int>("CarpetaEncabezadoid"),
                    ConfCarpetaDetalleId = item.Field<int>("ConfCarpetaDetalleId"),
                    Descripcion = item.Field<string>("Descripcion")
                });
            }

            return ms;
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
                result = _oSqlpg.get(_globales.query, _conexion.Conectar());
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

                result = _oSqlpg.ddt(_globales.query, _conexion.Conectar());

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
                        _oSqlpg.save(_globales.query, _conexion.Conectar());
                    }
                }
                else
                {
                    foreach (var item in value)
                    {
                        _globales.query = $"DELETE FROM dbo.rolxtipo WHERE rolnombre='{item.nombre}' and idtipo=1";
                        _oSqlpg.save(_globales.query, _conexion.Conectar());
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

        public List<DabaBaseLdcom> BasedeDatosConec()
        {
            try
            {
                _globales.query = "SELECT database_id, name FROM sys.databases WHERE name like 'LDCOM%'";
                DataTable dtDataBase = new DataTable();
                dtDataBase = _oSql.ddt(_globales.query, Conexion.siteplus);

                List<DabaBaseLdcom> modelDataBaseLd = new List<DabaBaseLdcom>();

                foreach (DataRow item in dtDataBase.Rows)
                {
                    modelDataBaseLd.Add(new DabaBaseLdcom
                    {
                        id = item.Field<int>("database_id"),
                        name = item.Field<string>("name")
                    }); 
                }

                return modelDataBaseLd;
            }
            catch(Exception ex)
            {
                List<DabaBaseLdcom> modelDataBaseLd = new List<DabaBaseLdcom>();
                Console.WriteLine(ex.Message);
                modelDataBaseLd.Add(new DabaBaseLdcom { 
                    error = ex.Message
                });
                return modelDataBaseLd;
            }
        }

        public List<SucursalesLdcom> SucursalesLsConec(string database)
        {
            try
            {
                _globales.query = $"SELECT suc_id, suc_nombre FROM {database}.dbo.sucursal";
                DataTable dtSucursal = new DataTable();
                dtSucursal = _oSql.ddt(_globales.query, Conexion.siteplus);

                List<SucursalesLdcom> modelSucursalLd = new List<SucursalesLdcom>();

                foreach (DataRow item in dtSucursal.Rows)
                {
                    modelSucursalLd.Add(new SucursalesLdcom
                    {
                        suc_id = item.Field<short>("suc_id"),
                        suc_nombre = item.Field<string>("suc_nombre"),
                    });
                }

                return modelSucursalLd;
            }
            catch (Exception ex)
            {
                List<SucursalesLdcom> modelSucursalLd = new List<SucursalesLdcom>();
                Console.WriteLine(ex.Message);
                modelSucursalLd.Add(new SucursalesLdcom { 
                    error=ex.Message
                });
                return modelSucursalLd;
            }
        }

        public List<SucursalesAbreviacionLdcom> SucursalesAbreviacionConec(string database, string letra)
        {
            try
            {
                _globales.query = $@"SELECT suc_id,suc_nombre, case when suc_id < 10 then '{letra}00' + cast(Suc_Id as varchar) when suc_id >= 10 and suc_id < 100 then '{letra}0' + cast(Suc_Id as varchar)
                                     when suc_id >= 100 then '{letra}' + cast(Suc_Id as varchar)end  AS 'Abreviacion' FROM {database}.dbo.sucursal";
                DataTable dtSucursal = new DataTable();
                dtSucursal = _oSql.ddt(_globales.query, Conexion.siteplus);

                List<SucursalesAbreviacionLdcom> modelSucursaAbreviacionlLd = new List<SucursalesAbreviacionLdcom>();

                foreach (DataRow item in dtSucursal.Rows)
                {
                    modelSucursaAbreviacionlLd.Add(new SucursalesAbreviacionLdcom
                    {
                        suc_id = item.Field<short>("suc_id"),
                        suc_nombre = item.Field<string>("suc_nombre"),
                        abreviacion = item.Field<string>("abreviacion")
                    });
                }

                return modelSucursaAbreviacionlLd;
            }
            catch (Exception ex)
            {
                List<SucursalesAbreviacionLdcom> SucursalesAbreviacionLdcom = new List<SucursalesAbreviacionLdcom>();
                Console.WriteLine(ex.Message);
                SucursalesAbreviacionLdcom.Add(new SucursalesAbreviacionLdcom { 
                    error=ex.Message
                });
                return SucursalesAbreviacionLdcom;
            }
        }

        public List<SucursalesAbreviacionLdcom> SucursalesAbreviacionRecarga(string database, string letra, DataTable dtDetalles)
        {
            try
            {
                _globales.query = $@"SELECT suc_id,suc_nombre, case when suc_id < 10 then '{letra}00' + cast(Suc_Id as varchar) when suc_id >= 10 and suc_id < 100 then '{letra}0' + cast(Suc_Id as varchar)
                                     when suc_id >= 100 then '{letra}' + cast(Suc_Id as varchar)end  AS 'Abreviacion' FROM {database}.dbo.sucursal";
                DataTable dtSucursal = new DataTable();
                dtSucursal = _oSql.ddt(_globales.query, Conexion.siteplus);

                List<SucursalesAbreviacionLdcom> modelSucursaAbreviacionlLd = new List<SucursalesAbreviacionLdcom>();

                // Comparar los DataTables y encontrar registros nuevos en dataTableB
                var newRecordsInB = dtDetalles.AsEnumerable()
                    .Where(rowB => !dtSucursal.AsEnumerable()
                        .Any(rowA => rowA.Field<short>("suc_id") == rowB.Field<short>("suc_id")))
                    .ToList();

                // Mostrar los registros nuevos en dataTableB
                foreach (var newRow in newRecordsInB)
                {
                    modelSucursaAbreviacionlLd.Add(new SucursalesAbreviacionLdcom
                    {
                        suc_id = newRow.Field<short>("suc_id"),
                        suc_nombre = newRow.Field<string>("suc_nombre"),
                        abreviacion = newRow.Field<string>("abreviacion")
                    });
                }

                if (modelSucursaAbreviacionlLd.Count == 0)
                {
                    modelSucursaAbreviacionlLd.Add(new SucursalesAbreviacionLdcom
                    {
                        error = "sin cambios"
                    });
                }

                return modelSucursaAbreviacionlLd;
            }
            catch (Exception ex)
            {
                List<SucursalesAbreviacionLdcom> SucursalesAbreviacionLdcom = new List<SucursalesAbreviacionLdcom>();
                Console.WriteLine(ex.Message);
                SucursalesAbreviacionLdcom.Add(new SucursalesAbreviacionLdcom
                {
                    error = ex.Message
                });
                return SucursalesAbreviacionLdcom;
            }
        }

        public List<EmpresaLdcom> EmpresaLsConec(string database)
        {
            try
            {
                _globales.query = $"SELECT Emp_id, Emp_nombre FROM {database}.dbo.Empresa";
                DataTable dtEmpresa = new DataTable();
                dtEmpresa = _oSql.ddt(_globales.query, Conexion.siteplus);

                List<EmpresaLdcom> modelEmpresaLd = new List<EmpresaLdcom>();

                foreach (DataRow item in dtEmpresa.Rows)
                {
                    modelEmpresaLd.Add(new EmpresaLdcom
                    {
                        emp_id = item.Field<short>("Emp_id"),
                        emp_nombre = item.Field<string>("Emp_nombre"),
                    });
                }

                return modelEmpresaLd;
            }
            catch (Exception ex)
            {
                List<EmpresaLdcom> modelEmpresaLd = new List<EmpresaLdcom>();
                Console.WriteLine(ex.Message);
                modelEmpresaLd.Add(new EmpresaLdcom { 
                    error=ex.Message
                });
                return modelEmpresaLd;
            }
        }

        public List<CadenaLdcom> CadenaLsConec(string database)
        {
            try
            {
                _globales.query = $"SELECT top 1 Cadena_id, Cadena_Nombre FROM {database}.dbo.Cadena order by cadena_id asc";
                DataTable dtCadena = new DataTable();
                dtCadena = _oSql.ddt(_globales.query, Conexion.siteplus);

                List<CadenaLdcom> modelCadenaLd = new List<CadenaLdcom>();

                foreach (DataRow item in dtCadena.Rows)
                {
                    modelCadenaLd.Add(new CadenaLdcom
                    {
                        cadena_id = item.Field<short>("Cadena_id"),
                        cadena_nombre = item.Field<string>("Cadena_Nombre"),
                    });
                }

                return modelCadenaLd;
            }
            catch (Exception ex)
            {
                List<CadenaLdcom> modelCadenaLd = new List<CadenaLdcom>();
                Console.WriteLine(ex.Message);
                modelCadenaLd.Add(new CadenaLdcom { 
                    error=ex.Message
                });
                return modelCadenaLd;
            }
        }

        public string GuardarInformacionFarmacias(FarmaciasEncabezadoRequest model)
        {
            try
            {

                _globales.query = $"SELECT COUNT(*) FROM dbo.farmacias_subnivel_encabezado WHERE emp_id={model.empId}";
                int existeEmpresa = _oSqlpg.getn(_globales.query, Conexion.GD);
                if(existeEmpresa > 0)
                {
                    return "EMP EXISTS";
                }

                _globales.query = $@"INSERT INTO dbo.farmacias_subnivel_encabezado(emp_id,emp_nombre,cadena_nombre,alias_cadena,notacion)
                                     VALUES({model.empId},'{model.empNombre.Trim()}','{model.cadenaNombre.Trim()}','{model.aliasCadena.Trim()}','{model.abreviacion.Trim()}')";
                _oSqlpg.save(_globales.query, Conexion.GD);


                _globales.query = $"SELECT id FROM dbo.farmacias_subnivel_encabezado WHERE emp_id={model.empId}";
                int feId = _oSqlpg.getn(_globales.query, Conexion.GD);
                if (feId == 0)
                {
                    return "FE NOT FOUND";
                }

                var sucursalesLdcom = SucursalesAbreviacionConec(model.baseDatos.Trim(), model.abreviacion.Trim());
                if (sucursalesLdcom[0].error != "" && sucursalesLdcom[0].error != null)
                {
                    return "ERROR";
                }

                foreach (var item in sucursalesLdcom)
                {
                    _globales.query = $@"INSERT INTO dbo.farmacias_subnivel_detalle(fe_id,suc_id,suc_nombre,abreviacion)
                                         VALUES({feId},{item.suc_id},'{item.suc_nombre}','{item.abreviacion}')";
                    _oSqlpg.save(_globales.query, Conexion.GD);
                }

                return "OK";
            }
            catch(Exception ex)
            {
                return "ERROR";
            }
        }

        public string RecargarInformacionFarmacias(FarmaciasEncabezadoRequest model)
        {
            try
            {
                _globales.query = $"SELECT * FROM dbo.farmacias_subnivel_encabezado WHERE emp_id={model.empId}";
                DataTable dtEncabezado = new DataTable();
                dtEncabezado = _oSqlpg.ddt(_globales.query, Conexion.GD);
                if (dtEncabezado.Rows.Count == 0)
                {
                    return "EMP NOT EXISTS";
                }

                int feId = Convert.ToInt32(dtEncabezado.Rows[0]["id"]);

                DataTable dtDetalle = new DataTable();
                _globales.query = $"select * from dbo.farmacias_subnivel_detalle where fe_id={feId}";
                dtDetalle = _oSqlpg.ddt(_globales.query, Conexion.GD);


                var sucursalesLdcom = SucursalesAbreviacionRecarga(model.baseDatos, model.abreviacion, dtDetalle);
                if(sucursalesLdcom[0].error != "" && sucursalesLdcom[0].error != null)
                {
                    if(sucursalesLdcom[0].error == "sin cambios")
                    {
                        return "SIN CAMBIOS";
                    }
                    else
                    {
                        return "ERROR";
                    }
                }
                
                foreach (var item in sucursalesLdcom)
                {
                    _globales.query = $@"INSERT INTO dbo.farmacias_subnivel_detalle(fe_id,suc_id,suc_nombre,abreviacion)
                                         VALUES({feId},{item.suc_id},'{item.suc_nombre}','{item.abreviacion}')";
                    _oSqlpg.save(_globales.query, Conexion.GD);
                }

                return "OK";
            }
            catch(Exception ex)
            {
                return "ERROR";
            }
        }

        public List<FarmaciasEncabezadoResponse> GetFarmaciasEncabezadoResponses(int limit, int offset)
        {
            try
            {
                if(limit == 0)
                {
                    limit = 15;
                }

                if(offset == 0)
                {
                    offset = 0;
                }

                List<FarmaciasEncabezadoResponse> farmaciasEncabezadoResponses = new List<FarmaciasEncabezadoResponse>();

                _globales.query = $"select * from dbo.farmacias_subnivel_encabezado";
                DataTable dtFarmaciasEncabezado = new DataTable();
                dtFarmaciasEncabezado = _oSqlpg.ddt(_globales.query, Conexion.GD);
                foreach(DataRow item in dtFarmaciasEncabezado.Rows)
                {
                    farmaciasEncabezadoResponses.Add(new FarmaciasEncabezadoResponse { 
                        id=item.Field<int>("id"),
                        empId=item.Field<short>("emp_id"),
                        empNombre=item.Field<string>("emp_nombre"),
                        cadenaNombre=item.Field<string>("cadena_nombre"),
                        aliasCadena= item.Field<string>("alias_cadena")
                    });
                }

                farmaciasEncabezadoResponses.Skip(offset).Take(limit).ToList();

                return farmaciasEncabezadoResponses;
            }
            catch(Exception ex)
            {
                List<FarmaciasEncabezadoResponse> farmaciasEncabezadoResponsesError = new List<FarmaciasEncabezadoResponse>();
                farmaciasEncabezadoResponsesError.Add(new FarmaciasEncabezadoResponse { 
                    error=ex.Message
                });

                return farmaciasEncabezadoResponsesError;
            }
        }

        public List<SucursalesAbreviacionLdcom> SucursalesAbreviacionLdcom(short empId)
        {
            try
            {
                _globales.query = $"select * from dbo.farmacias_subnivel_detalle where emp_id{empId}";
                DataTable dtSucursal = new DataTable();
                dtSucursal = _oSql.ddt(_globales.query, Conexion.siteplus);

                List<SucursalesAbreviacionLdcom> modelSucursaAbreviacionlLd = new List<SucursalesAbreviacionLdcom>();

                foreach (DataRow item in dtSucursal.Rows)
                {
                    modelSucursaAbreviacionlLd.Add(new SucursalesAbreviacionLdcom
                    {
                        suc_id = item.Field<short>("suc_id"),
                        suc_nombre = item.Field<string>("suc_nombre"),
                        abreviacion = item.Field<string>("abreviacion")
                    });
                }

                return modelSucursaAbreviacionlLd;
            }
            catch(Exception ex)
            {
                List<SucursalesAbreviacionLdcom> SucursalesAbreviacionLdcom = new List<SucursalesAbreviacionLdcom>();
                Console.WriteLine(ex.Message);
                SucursalesAbreviacionLdcom.Add(new SucursalesAbreviacionLdcom
                {
                    error = ex.Message
                });
                return SucursalesAbreviacionLdcom;
            }
        }
    }
}