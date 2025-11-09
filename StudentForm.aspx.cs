using System;
using Escuela.Web.Models;
using Escuela.Web.Services;

namespace Escuela.Web
{
    public partial class StudentForm : System.Web.UI.Page
    {
        private readonly EstudianteService _service = new EstudianteService();
        private int? Id => int.TryParse(Request.QueryString["id"], out var x) ? x : (int?)null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                litTitulo.Text = Id.HasValue ? "Editar Estudiante" : "Nuevo Estudiante";
                if (Id.HasValue) Cargar();
            }
        }

        private void Cargar()
        {
            var e = _service.Obtener(Id.Value);
            if (e == null)
            {
                Response.Redirect("~/Students.aspx");
                return;
            }

            txtNombre.Text = e.Nombre;
            txtEmail.Text = e.Email;
            txtEdad.Text = e.Edad.ToString();
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            try
            {
                if (!int.TryParse(txtEdad.Text, out var edad))
                {
                    lblMsg.Text = "La edad debe ser un número válido.";
                    return;
                }

                if (Id.HasValue)
                {
                    var cambios = new Estudiante
                    {
                        IdEstudiante = Id.Value,
                        Nombre = txtNombre.Text,
                        Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text,
                        Edad = edad
                    };
                    _service.Actualizar(cambios);
                }
                else
                {
                    var nuevo = new Estudiante
                    {
                        Nombre = txtNombre.Text,
                        Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text,
                        Edad = edad
                        // 'Creado' lo asigna la BD por DEFAULT
                    };
                    _service.Crear(nuevo);
                }

                Response.Redirect("~/Students.aspx");
            }
            catch (InvalidOperationException ex)
            {
                // Reglas de negocio (nombre vacío, edad fuera de rango, email duplicado)
                lblMsg.Text = ex.Message;
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Error al guardar: " + ex.Message;
            }
        }
    }
}