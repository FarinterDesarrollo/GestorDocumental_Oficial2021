using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GestorDocumentos.Models;
using Microsoft.AspNet.Identity.Owin;

namespace GestorDocumentos.Archivos
{
    
    public class DropboxListSubniveles
    {
        public IEnumerable<SelectListItem> GetCarpetaEncabezadoId()
        {
            var data = new ApplicationDbContext();
            return data.ConfCarpetaEncabezados.OrderBy(x => x.Descripcion).Select(x => new SelectListItem
            {
                Text = x.Descripcion,
                Value = x.Id.ToString()
            }).ToList();
        }

        public IEnumerable<SelectListItem> GetCarpetaDetalleEncabezadoId(int idcarpeta)
        {
            var data = new ApplicationDbContext();
           
            return data.ConfCarpetaDetalle.Where(y => y.CarpetaEncabezadoid == idcarpeta).Select(x => new SelectListItem
            {
                Text = x.Descripcion,
                Value = x.Id.ToString()
            }).ToList();
        }

        public IEnumerable<SelectListItem> GetTipoCampo(int id)
        {
            var data = new ApplicationDbContext();
            return data.Tipo_Campo.Where(y => y.Id == id).Select(x => new SelectListItem
            {
                Text = x.Longitud.ToString(),
                Value = x.Id.ToString()
            });
        }

        public IEnumerable<SelectListItem> GetAreayEncabezadoCarpeta(int areaid )
        {
            
            var data = new ApplicationDbContext();
            return data.ConfCarpetaEncabezados.OrderBy(x => x.Descripcion).Where(y => y.AreaId == areaid).Select(x => new SelectListItem
            {
                Text = x.Descripcion.ToString(),
                Value = x.Id.ToString()
            });
        }

        public IEnumerable<SelectListItem> GetAreayEncabezadoCarpetaIdpermisos(int areaid, string rname)
        {
            var data = new ApplicationDbContext();
            List<SelectListItem> list = new List<SelectListItem>();
            var rolexareaxcarpertaxsubnivel = (from a in data.Areas
                                               join rxaxcxs in data.RoleXAreaXCarpetasXSubniveles on a.Id equals rxaxcxs.AreaId
                                               join ec in data.ConfCarpetaEncabezados on rxaxcxs.CarpetaEncabezadoid equals ec.Id
                                               where a.Id == ec.AreaId && rxaxcxs.AreaId == ec.AreaId && rxaxcxs.RoleName == rname && rxaxcxs.AreaId == areaid
                                               select new { ec.Id, ec.Descripcion }).OrderBy(x => x.Descripcion).Distinct().ToList();

            if (rolexareaxcarpertaxsubnivel.Count == 0)
            {
                var lista = (from a in data.Areas
                             join rxaxc in data.RoleXAreaXCarpetas on a.Id equals rxaxc.AreaId
                             join ec in data.ConfCarpetaEncabezados on rxaxc.CarpetaEncabezadoid equals ec.Id
                             where a.Id == ec.AreaId && rxaxc.AreaId == ec.AreaId && rxaxc.RoleName == rname && rxaxc.AreaId == areaid
                             select new { ec.Id, ec.Descripcion }).OrderBy(x => x.Descripcion).ToList();


                if (lista.Count == 0)
                {
                    var areas = (from a in data.Areas
                                 join axr in data.RoleXAreas on a.Id equals axr.AreaId
                                 where axr.RoleName == rname
                                 select new { a.Id, a.Descripcion }).ToList();
                    if (areas.Count != 0)
                    {
                        var lst = (from cc in data.ConfCarpetaEncabezados where cc.AreaId == areaid select new { cc.Id, cc.Descripcion }).OrderBy(x => x.Descripcion).ToList();

                        foreach (var item in lst)
                        {
                            list.Add(new SelectListItem() { Value = item.Id.ToString(), Text = item.Descripcion });
                        }
                    }
                }
                else
                {

                    foreach (var item in lista)
                    {
                        list.Add(new SelectListItem() { Value = item.Id.ToString(), Text = item.Descripcion });
                    }


                }
            }
            else
            {
                foreach (var item in rolexareaxcarpertaxsubnivel)
                {
                    list.Add(new SelectListItem() { Value = item.Id.ToString(), Text = item.Descripcion });
                }

            }

            return list;

            //var data = new ApplicationDbContext();

            //var lista = (from a in data.Areas
            //             join rxaxc in data.RoleXAreaXCarpetas on a.Id equals rxaxc.AreaId
            //             join ec in data.ConfCarpetaEncabezados on rxaxc.CarpetaEncabezadoid equals ec.Id
            //             where a.Id == ec.AreaId && rxaxc.AreaId == ec.AreaId && rxaxc.RoleName==rname && rxaxc.AreaId== areaid
            //             select new { ec.Id, ec.Descripcion }).ToList();

            //List<SelectListItem> list = new List<SelectListItem>();
            //if (lista.Count==0)
            //{
            //    var areas = (from a in data.Areas
            //                 join axr in data.RoleXAreas on a.Id equals axr.AreaId
            //                 where axr.RoleName == rname 
            //                 select new { a.Id, a.Descripcion }).ToList();
            //    if (areas.Count != 0)
            //    {
            //        var lst = (from cc in data.ConfCarpetaEncabezados where cc.AreaId == areaid select new { cc.Id, cc.Descripcion }).ToList();

            //        foreach (var item in lst)
            //        {
            //            list.Add(new SelectListItem() { Value = item.Id.ToString(), Text = item.Descripcion });
            //        }
            //    }
            //}
            //else
            //{
            //    foreach (var item in lista)
            //    {
            //        list.Add(new SelectListItem() { Value = item.Id.ToString(), Text = item.Descripcion });
            //    }


            //}
            //return list;

        }
        //GetAreayEncabezadoCarpetaIdpermisos

        public IEnumerable<SelectListItem> GetControlarNivelesMax(int id1, int id2)
        {
            var data = new ApplicationDbContext();

            List<SelectListItem> lista = (from t in data.ConfCarpetaDetalle
                                          where t.AreaId == id1 && t.CarpetaEncabezadoid == id2
                                          orderby t.NivelId descending
                                          select new SelectListItem()
                                          {
                                              Value = t.NivelId.ToString()
                                          }).Take(1).ToList();
            int valor = lista.Count;

            if (valor == 0)
            {
                return data.ConfCarpetaDetalle.Select(x => new SelectListItem
                {
                    Text = "1",
                    Value = "1"
                });
            }
            else
            {
                //Agregado para validar la cantidad de subniveles con la carp.encabezado 22/04/2020
                var vmax = (from ced in data.ConfCarpetaDetalle
                            where ced.AreaId == id1 && ced.CarpetaEncabezadoid == id2
                            select ced).Max(x => x.NivelId);
                int v1 = vmax;
                var subniveles = (from ce in data.ConfCarpetaEncabezados
                                  where ce.AreaId == id1 && ce.Id == id2
                                  select ce).Max(x => x.Subniveles);
                int v2 = subniveles;

                if (v1 >= v2)
                {
                    return data.ConfCarpetaEncabezados.Where(y => y.AreaId == id1 && y.Id == id2).Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Subniveles.ToString()
                    });
                }
                else {
                   int nivelid = v1 + 1;
                    return data.ConfCarpetaDetalle.Where(y => y.AreaId == id1 && y.CarpetaEncabezadoid == id2).OrderByDescending(z => z.NivelId).Take(1).Select(x => new SelectListItem
                    {
                        Text = nivelid.ToString(),
                        Value = x.Id.ToString()
                    });
                }

                //********************************************************************************************************************

                //return data.ConfCarpetaDetalle.Where(y => y.AreaId == id1 && y.CarpetaEncabezadoid == id2).OrderByDescending(z => z.NivelId).Take(1).Select(x => new SelectListItem
                //{
                //    Text = x.NivelId.ToString(),
                //    Value = x.Id.ToString()
                //});
            }
        }

        public IEnumerable<SelectListItem> GetControlarTipodeCampo(int id1, int id2, int id3)
        {
            var data = new ApplicationDbContext();

            return data.ConfCarpetaDetalle.Where(y => y.AreaId == id1 && y.CarpetaEncabezadoid == id2 && y.Id == id3).Select(x => new SelectListItem
            {
                Text = x.Longitud.ToString(),
                Value = x.Id.ToString()
            });
        }

        public string  rolename (string usuario)
        {
            var data = new ApplicationDbContext();
            
            var usersWithRoles = (from user in data.Users
                                  where user.UserName == usuario
                                  select new
                                  {
                                      UserId = user.Id,
                                      Username = user.UserName,
                                      Email = user.Email,
                                      RoleNames = (from userRole in user.Roles
                                                   join role in data.Roles on userRole.RoleId
                                                   equals role.Id
                                                   select role.Name).ToList()
                                  }).ToList().Select(p => new

                                  {
                                      UserId = p.UserId,
                                      Username = p.Username,
                                      Email = p.Email,
                                      Role = string.Join(",", p.RoleNames)
                                  });
            string rollname = "";
            foreach (var item in usersWithRoles)
            {
                rollname = item.Role;
            }
            return (rollname);
        }

        public IEnumerable<SelectListItem> GetSubnivelesId(int idcarpetadetalle)
        {
            var data = new ApplicationDbContext();

            return data.MantenimientoSubniveles.OrderBy(x => x.Descripcion).Where(y => y.ConfCarpetaDetalleId == idcarpetadetalle).Select(x => new SelectListItem
            {
                Text = x.Descripcion,
                Value = x.Id.ToString()
            }).ToList();
        }


        public IEnumerable<SelectListItem> GetAreayEncabezadoCarpetaId(int areaid )
        {
            var data = new ApplicationDbContext();
            List<SelectListItem> list = new List<SelectListItem>();

            //var encabezado_carpeta= (from a in data.Areas
            //                         join ec in data.ConfCarpetaEncabezados on a.Id equals ec.AreaId
            //                         select new { ec.Id, ec.Descripcion }
            //                        ).ToList();
            var lst = (from cc in data.ConfCarpetaEncabezados where cc.AreaId == areaid select new { cc.Id, cc.Descripcion }).ToList();

            foreach (var item in lst)
            {
                list.Add(new SelectListItem() { Value = item.Id.ToString(), Text = item.Descripcion });
            }
 
            return list;

           
        }

    }
}