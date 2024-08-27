﻿using ENTITIES.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DATA.Data.Configuration;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuario");
        builder.Property(p => p.Id)
                .IsRequired();
        builder.Property(p => p.Nombres)
                .IsRequired();
              
        builder.Property(p => p.ApellidoPaterno)
                .IsRequired();
        builder.Property(p => p.Username)
                .IsRequired();
        builder.Property(p => p.Email)
                .IsRequired();

        builder
        .HasMany(p => p.Roles)
        .WithMany(p => p.Usuarios)
        .UsingEntity<UsuariosRoles>(
            j => j
                .HasOne(pt => pt.Rol)
                .WithMany(t => t.UsuariosRoles)
                .HasForeignKey(pt => pt.RolId),
            j => j
                .HasOne(pt => pt.Usuario)
                .WithMany(p => p.UsuariosRoles)
                .HasForeignKey(pt => pt.UsuarioId),
            j =>
            {
                j.HasKey(t => new { t.UsuarioId, t.RolId });
            });

        // RELACION ENTRE EL USUARIO Y LOS TOKEN
        builder.HasMany(p => p.RefreshTokens)
            .WithOne(p => p.Usuario)
            .HasForeignKey(p => p.UsuarioId);
    }
}

