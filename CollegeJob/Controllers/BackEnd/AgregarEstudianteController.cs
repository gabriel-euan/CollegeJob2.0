﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business_Object;
using Data_Access_Object;
using System.Web.UI;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using Gma.QrCodeNet.Encoding.Windows.Render;
using CrystalDecisions.CrystalReports.Engine;
using Gma.QrCodeNet.Encoding;

namespace CollegeJob.Controllers.BackEnd
{
    public class AgregarEstudianteController : Controller
    {
        UsuariosDAO ObjUsuario = new UsuariosDAO();

        // GET: AgregarEstudiante
        public ActionResult Index()
        {
            return View("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AgregarUsuario(string Nombre, string Apellidos, string Cumpleanios, string Email, string Contraseña, string Tipo, string Direccion, string Telefono, HttpPostedFileBase Imagen, string CURP, string INE, string Matricula, string Universidad)
        {
            UsuarioBO BO = new UsuarioBO();
            //datos BO de Alumno
            BO.Nombre = Nombre;
            BO.Apellidos = Apellidos;
            BO.Direccion = Direccion;
            BO.FechaNac = Convert.ToDateTime(Cumpleanios);
            BO.Email = Email;
            BO.Contraseña = Contraseña;
            BO.TipoUsuario = int.Parse(Tipo);
            BO.Telefono = long.Parse(Telefono);
            BO.Estatus = "Activo";
            BO.INE = INE;
            BO.CURP = CURP;
            BO.Matricula = Matricula;
            BO.Universidad = Universidad;
            if (Imagen != null)
            {
                BO.Imagen = new byte[Imagen.ContentLength];
                Imagen.InputStream.Read(BO.Imagen, 0, Imagen.ContentLength);
            }
            else
            {

            }
            int CodAgregar = 0;

            switch (Tipo)
            {
                case "1":
                    CodAgregar = ObjUsuario.AgregarAdmin(BO);
                    break;

                case "2":
                    CodAgregar = ObjUsuario.AgregarEmpleador(BO);
                    break;

                case "3":
                    CodAgregar = ObjUsuario.AgregarEstudiante(BO);
                    break;

                default: break;
            }

            
            Session["Agregar"] = CodAgregar;
            ViewBag.Agregar = Session["Agregar"];

            return View("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DatosTabla(string Buscar)
        {
            Session["Email"] = Buscar;
            Session["IdEditar"] = ObjUsuario.BuscarId(Buscar);
            return View("Index");
        }

        public ActionResult EditarDatos()
        {
            return View(ObjUsuario.TablaUsuarios3(int.Parse(Session["IdEditar"].ToString())));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Actualizar(string ID, string Tipo2, string Tipo, string Nombre, string Apellidos, string Correo, string Contraseña, string FechaNac, string Telefono, string dirreccion, byte[] img, HttpPostedFileBase Imagen)
        {
            UsuarioBO bo = new UsuarioBO();
            if (Imagen != null)
            {
                bo.Imagen = new byte[Imagen.ContentLength];
                Imagen.InputStream.Read(bo.Imagen, 0, Imagen.ContentLength);
            }
            else
            {
                bo.Imagen = img;
            }

            if (Tipo != null)
            {
                bo.TipoUsuario = int.Parse(Tipo);
            }
            else
            {
                bo.TipoUsuario = int.Parse(Tipo2);
            }
            bo.Codigo = int.Parse(ID);
            bo.Nombre = Nombre;
            bo.Direccion = dirreccion;
            bo.Apellidos = Apellidos;
            bo.Email = Correo;
            bo.Contraseña = Contraseña;
            bo.FechaNac = Convert.ToDateTime(FechaNac);
            bo.Telefono = long.Parse(Telefono);
            int CodAct = ObjUsuario.ActualizarUsuario2(bo);
            Session["Actualizar"] = CodAct;
            ViewBag.Actualizar = CodAct;
            Index();
            return View("Index");
        }

        public ActionResult Eliminar(string Codigo)
        {
            UsuarioBO BO = new UsuarioBO();
            BO.Codigo = int.Parse(Codigo);
            ObjUsuario.EliminarUsuario(BO);

            return View("Index");
        }

        public ActionResult BuscarView(string Nombre)
        {
            if(Nombre == null)
            {
                ViewBag.Busqueda = Session["Busqueda"];
            }
            else
            {
                UsuariosDAO ObjBusqueda = new UsuariosDAO();
                ViewData["TablaBusqueda"] = ObjBusqueda.BuscquedaUsuario(Nombre);
                if(ObjBusqueda.BuscquedaUsuario(Nombre).Rows.Count > 0)
                {
                    Session["Busqueda"] = "Correcto";
                    ViewBag.Busqueda = Session["Busqueda"];
                }
                else
                {
                    Session["Busqueda"] = "No encontrado";
                    ViewBag.Busqueda = Session["Busqueda"];
                }
                Session["Busqueda"] = null;
            }
            return View("BuscarView");
        }

        public ActionResult EditarView(string Codigo)
        {
            UsuariosDAO usuariosDAO = new UsuariosDAO();
            int CodUsuario = int.Parse(Codigo);
            return View(usuariosDAO.BuscarUsuario(CodUsuario));
        }
    }
}