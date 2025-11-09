using System;
using System.Collections.Generic;
using System.Linq;
using Escuela.Web.Data;
using Escuela.Web.Models;

namespace Escuela.Web.Services
{
    public class EstudianteService
    {
        public IEnumerable<Estudiante> Listar(string filtro = null)
        {
            using (var ctx = new EscuelaContext())
            {
                var q = ctx.Estudiantes.AsQueryable();
                if (!string.IsNullOrWhiteSpace(filtro))
                {
                    filtro = filtro.Trim();
                    q = q.Where(e => e.Nombre.Contains(filtro) ||
                                     (e.Email != null && e.Email.Contains(filtro)));
                }
                return q.OrderBy(e => e.IdEstudiante).ToList();
            }
        }

        public Estudiante Obtener(int id)
        {
            using (var ctx = new EscuelaContext())
            {
                return ctx.Estudiantes.Find(id);
            }
        }

        public void Crear(Estudiante e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));
            Validar(e);
            using (var ctx = new EscuelaContext())
            {
                ValidarEmailUnico(ctx, e.Email);
                ctx.Estudiantes.Add(e);
                ctx.SaveChanges();
            }
        }

        public void Actualizar(Estudiante cambios)
        {
            if (cambios == null) throw new ArgumentNullException(nameof(cambios));
            Validar(cambios);
            using (var ctx = new EscuelaContext())
            {
                var existente = ctx.Estudiantes.Find(cambios.IdEstudiante);
                if (existente == null) throw new KeyNotFoundException("El estudiante no existe.");
                if (!string.Equals(existente.Email ?? "", cambios.Email ?? "", StringComparison.Ordinal))
                    ValidarEmailUnico(ctx, cambios.Email, excluirId: cambios.IdEstudiante);

                existente.Nombre = cambios.Nombre;
                existente.Email = cambios.Email;
                existente.Edad = cambios.Edad;
                ctx.SaveChanges();
            }
        }

        public void Eliminar(int id)
        {
            using (var ctx = new EscuelaContext())
            {
                var e = ctx.Estudiantes.Find(id);
                if (e == null) return;
                ctx.Estudiantes.Remove(e);
                ctx.SaveChanges();
            }
        }

        private static void Validar(Estudiante e)
        {
            e.Nombre = (e.Nombre ?? "").Trim();
            if (string.IsNullOrWhiteSpace(e.Nombre))
                throw new InvalidOperationException("El nombre es obligatorio.");

            e.Email = string.IsNullOrWhiteSpace(e.Email) ? null : e.Email.Trim();
            if (e.Edad < 18 || e.Edad > 40)
                throw new InvalidOperationException("La edad debe estar entre 18 y 40.");
        }

        private static void ValidarEmailUnico(EscuelaContext ctx, string email, int? excluirId = null)
        {
            if (string.IsNullOrEmpty(email)) return;
            bool existe = ctx.Estudiantes.Any(x => x.Email == email && (!excluirId.HasValue || x.IdEstudiante != excluirId.Value));
            if (existe) throw new InvalidOperationException("El email ya está registrado.");
        }
    }
}
