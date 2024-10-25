
using Microsoft.EntityFrameworkCore;
using RecommendationEngine.Domain.Entities;
using System.Xml.Linq;
namespace RecommendationEngine.Infrastructure.Database
{
    public class MySqlDbContext : DbContext
    {
        public MySqlDbContext(DbContextOptions<MySqlDbContext> options)
            : base(options)
        {
        }

        // DbSet for the column_configuration table
        public DbSet<ColumnConfiguration> ColumnConfigurations { get; set; }
        public DbSet<TemplateConfiguration> TemplateConfigurations { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TemplateConfiguration>(entity =>
            {
                entity.ToTable("template_configuration");

                entity.HasKey(e => e.TemplateId); // Assuming column_id is the primary key

                entity.Property(e => e.TemplateId).HasColumnName("template_id").IsRequired();
                entity.Property(e => e.Language).HasColumnName("language");
                entity.Property(e => e.TemplateName).HasColumnName("template_name");
            });

            // Mapping entity to the existing table 'column_configuration'
            modelBuilder.Entity<ColumnConfiguration>(entity =>
            {
                entity.ToTable("column_configuration");

                entity.HasKey(e => e.ColumnId); // Assuming column_id is the primary key

                // Mapping columns
                entity.Property(e => e.ColumnId).HasColumnName("column_id").IsRequired();
                entity.Property(e => e.Index).HasColumnName("index");
                entity.Property(e => e.Name).HasColumnName("name").IsRequired();
                entity.Property(e => e.ColumnHeader).HasColumnName("column_header");
                entity.Property(e => e.RegexExp).HasColumnName("regex_exp");
                entity.Property(e => e.RegexErrorMessage).HasColumnName("regex_error_message");
                entity.Property(e => e.Mandatory).HasColumnName("mandatory").HasConversion<int>(); // Bit is treated as boolean
                entity.Property(e => e.MandatoryErrorMessage).HasColumnName("mandatory_error_message");
                entity.Property(e => e.Precision).HasColumnName("precision");
                entity.Property(e => e.TemplateId).HasColumnName("template_id");
            });

           
        }
    }
}
