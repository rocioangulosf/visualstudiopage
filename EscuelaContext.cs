using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Escuela.Web.Models;

namespace Escuela.Web.Data
{
    public class EscuelaContext : DbContext
    {
        public EscuelaContext() : base("name=EscuelaConnection") { }
        public DbSet<Estudiante> Estudiantes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Entity<Estudiante>().ToTable("estudiantes", schemaName: "escolar");
            Database.SetInitializer<EscuelaContext>(null);
            base.OnModelCreating(modelBuilder);
        }
    }
}
