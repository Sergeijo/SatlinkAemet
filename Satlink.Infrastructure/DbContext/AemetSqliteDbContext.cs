using System;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using Newtonsoft.Json;

using Satlink.Domain.Models;

namespace Satlink.Infrastructure.DbContxt;

internal sealed class AemetSqliteDbContext : DbContext
{
    public AemetSqliteDbContext(DbContextOptions<AemetSqliteDbContext> options) : base(options)
    {
    }

    public DbSet<PersistedRequest> AemetDownloads { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ValueConverter<DateOnly, string> dateOnlyConverter = new(
            d => d.ToString("yyyy-MM-dd"),
            s => DateOnly.Parse(s));

        ValueConverter<List<Zona>, string> zonaListConverter = new(
            v => JsonConvert.SerializeObject(v),
            v => JsonConvert.DeserializeObject<List<Zona>>(v) ?? new List<Zona>());

        modelBuilder.Entity<PersistedRequest>(entity =>
        {
            entity.ToTable("AemetDownloads");
            entity.HasKey(e => e.id);

            entity.Property(e => e.FechaDescarga)
                .HasConversion(dateOnlyConverter)
                .HasColumnName("fecha_descarga");

            // Composite unique constraint: one record per zone per day.
            entity.HasIndex(e => new { e.id, e.FechaDescarga }).IsUnique();

            entity.OwnsOne(e => e.origen, o =>
            {
                o.Property(x => x.productor).HasColumnName("origen_productor");
                o.Property(x => x.web).HasColumnName("origen_web");
                o.Property(x => x.language).HasColumnName("origen_language");
                o.Property(x => x.copyright).HasColumnName("origen_copyright");
                o.Property(x => x.notaLegal).HasColumnName("origen_notaLegal");
                o.Property(x => x.elaborado).HasColumnName("origen_elaborado");
                o.Property(x => x.inicio).HasColumnName("origen_inicio");
                o.Property(x => x.fin).HasColumnName("origen_fin");
            });

            entity.OwnsOne(e => e.situacion, s =>
            {
                s.Property(x => x.inicio).HasColumnName("situacion_inicio");
                s.Property(x => x.fin).HasColumnName("situacion_fin");
                s.Property(x => x.texto).HasColumnName("situacion_texto");
                s.Property(x => x.id).HasColumnName("situacion_id");
                s.Property(x => x.nombre).HasColumnName("situacion_nombre");
            });

            entity.OwnsOne(e => e.prediccion, p =>
            {
                p.Property(x => x.inicio).HasColumnName("prediccion_inicio");
                p.Property(x => x.fin).HasColumnName("prediccion_fin");

                // Store zones as a JSON string — avoids a separate SQLite table.
                p.Property(x => x.zona)
                    .HasColumnName("prediccion_zona")
                    .HasConversion(zonaListConverter);
            });
        });
    }
}
