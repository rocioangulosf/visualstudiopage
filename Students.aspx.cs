using System;
using System.Web.UI.WebControls;
using Escuela.Web.Services;
using System.Web.UI;   // ⬅️ agrega esto

namespace Escuela.Web
{
    public partial class Students : System.Web.UI.Page
    {
        private readonly EstudianteService _service = new EstudianteService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        private void BindGrid(string filtro = null)
        {
            gvEstudiantes.DataSource = _service.Listar(filtro);
            gvEstudiantes.DataBind();
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            BindGrid(txtFiltro.Text);
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtFiltro.Text = string.Empty;
            BindGrid();
        }

        protected void gvEstudiantes_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            // Aseguramos que el índice de página esté dentro del rango
            if (e.NewPageIndex >= 0 && e.NewPageIndex < gvEstudiantes.PageCount)
            {
                gvEstudiantes.PageIndex = e.NewPageIndex;
                BindGrid(txtFiltro.Text);
            }
        }

        protected void gvEstudiantes_RowEditing(object sender, GridViewEditEventArgs e)
        {
            // Validar índice antes de usarlo
            if (e.NewEditIndex >= 0 && e.NewEditIndex < gvEstudiantes.DataKeys.Count)
            {
                int id = (int)gvEstudiantes.DataKeys[e.NewEditIndex].Value;
                Response.Redirect($"~/StudentForm.aspx?id={id}");
            }
        }

        protected void gvEstudiantes_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            // Validar índice antes de usarlo
            if (e.RowIndex >= 0 && e.RowIndex < gvEstudiantes.DataKeys.Count)
            {
                int id = (int)gvEstudiantes.DataKeys[e.RowIndex].Value;
                try
                {
                    _service.Eliminar(id);
                    BindGrid(txtFiltro.Text);
                    lblMsg.Text = string.Empty;
                }
                catch (Exception ex)
                {
                    lblMsg.Text = "No se pudo eliminar el registro: " + ex.Message;
                }
            }
        }

        protected void gvEstudiantes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Buscar de forma segura el LinkButton de Delete en TODA la fila
                foreach (Control control in e.Row.Cells[e.Row.Cells.Count - 1].Controls)
                {
                    if (control is LinkButton lb && lb.CommandName == "Delete")
                    {
                        lb.OnClientClick = "return confirm('¿Eliminar este estudiante?');";
                    }
                }
            }
        }
    }
}
