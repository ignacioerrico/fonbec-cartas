﻿// <auto-generated />
using System;
using Fonbec.Cartas.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Fonbec.Cartas.DataAccess.Migrations.Application
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20230531004326_AddPadrino")]
    partial class AddPadrino
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("model")
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Fonbec.Cartas.DataAccess.Entities.Actors.Coordinador", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AspNetUserId")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("nchar(36)")
                        .IsFixedLength();

                    b.Property<DateTimeOffset>("CreatedOnUtc")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("FilialId")
                        .HasColumnType("int");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<byte>("Gender")
                        .HasColumnType("tinyint");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<DateTimeOffset?>("LastUpdatedOnUtc")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NickName")
                        .HasMaxLength(16)
                        .HasColumnType("nvarchar(16)");

                    b.Property<string>("Phone")
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<DateTimeOffset?>("SoftDeletedOnUtc")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("Id");

                    b.HasIndex("FilialId");

                    b.ToTable("Coordinadores", "model");
                });

            modelBuilder.Entity("Fonbec.Cartas.DataAccess.Entities.Actors.Mediador", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AspNetUserId")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("nchar(36)")
                        .IsFixedLength();

                    b.Property<DateTimeOffset>("CreatedOnUtc")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("FilialId")
                        .HasColumnType("int");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<byte>("Gender")
                        .HasColumnType("tinyint");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<DateTimeOffset?>("LastUpdatedOnUtc")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NickName")
                        .HasMaxLength(16)
                        .HasColumnType("nvarchar(16)");

                    b.Property<string>("Phone")
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<DateTimeOffset?>("SoftDeletedOnUtc")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("Id");

                    b.HasIndex("FilialId");

                    b.ToTable("Mediadores", "model");
                });

            modelBuilder.Entity("Fonbec.Cartas.DataAccess.Entities.Actors.Padrino", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CreatedByCoordinadorId")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset>("CreatedOnUtc")
                        .HasColumnType("datetimeoffset");

                    b.Property<int?>("DeletedByCoordinadorId")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("FilialId")
                        .HasColumnType("int");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<byte>("Gender")
                        .HasColumnType("tinyint");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<DateTimeOffset?>("LastUpdatedOnUtc")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NickName")
                        .HasMaxLength(16)
                        .HasColumnType("nvarchar(16)");

                    b.Property<string>("Phone")
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<DateTimeOffset?>("SoftDeletedOnUtc")
                        .HasColumnType("datetimeoffset");

                    b.Property<int?>("UpdatedByCoordinadorId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CreatedByCoordinadorId");

                    b.HasIndex("DeletedByCoordinadorId");

                    b.HasIndex("FilialId");

                    b.HasIndex("UpdatedByCoordinadorId");

                    b.ToTable("Padrinos", "model");
                });

            modelBuilder.Entity("Fonbec.Cartas.DataAccess.Entities.Actors.Revisor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AspNetUserId")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("nchar(36)")
                        .IsFixedLength();

                    b.Property<DateTimeOffset>("CreatedOnUtc")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("FilialId")
                        .HasColumnType("int");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<byte>("Gender")
                        .HasColumnType("tinyint");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<DateTimeOffset?>("LastUpdatedOnUtc")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NickName")
                        .HasMaxLength(16)
                        .HasColumnType("nvarchar(16)");

                    b.Property<string>("Phone")
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<DateTimeOffset?>("SoftDeletedOnUtc")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("Id");

                    b.HasIndex("FilialId");

                    b.ToTable("Revisores", "model");
                });

            modelBuilder.Entity("Fonbec.Cartas.DataAccess.Entities.Filial", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTimeOffset>("CreatedOnUtc")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset?>("LastUpdatedOnUtc")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<DateTimeOffset?>("SoftDeletedOnUtc")
                        .HasColumnType("datetimeoffset");

                    b.HasKey("Id");

                    b.ToTable("Filiales", "model");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CreatedOnUtc = new DateTimeOffset(new DateTime(2023, 5, 31, 0, 43, 25, 794, DateTimeKind.Unspecified).AddTicks(3590), new TimeSpan(0, 0, 0, 0, 0)),
                            Name = "Default"
                        });
                });

            modelBuilder.Entity("Fonbec.Cartas.DataAccess.Entities.SendAlsoTo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("PadrinoId")
                        .HasColumnType("int");

                    b.Property<string>("RecipientEmail")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("RecipientFullName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("SendAsBcc")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("PadrinoId");

                    b.ToTable("SendAlsoTo", "model");
                });

            modelBuilder.Entity("Fonbec.Cartas.DataAccess.Entities.Actors.Coordinador", b =>
                {
                    b.HasOne("Fonbec.Cartas.DataAccess.Entities.Filial", "Filial")
                        .WithMany("Coordinadores")
                        .HasForeignKey("FilialId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Filial");
                });

            modelBuilder.Entity("Fonbec.Cartas.DataAccess.Entities.Actors.Mediador", b =>
                {
                    b.HasOne("Fonbec.Cartas.DataAccess.Entities.Filial", "Filial")
                        .WithMany("Mediadores")
                        .HasForeignKey("FilialId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Filial");
                });

            modelBuilder.Entity("Fonbec.Cartas.DataAccess.Entities.Actors.Padrino", b =>
                {
                    b.HasOne("Fonbec.Cartas.DataAccess.Entities.Actors.Coordinador", "CreatedByCoordinador")
                        .WithMany()
                        .HasForeignKey("CreatedByCoordinadorId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Fonbec.Cartas.DataAccess.Entities.Actors.Coordinador", "DeletedByCoordinador")
                        .WithMany()
                        .HasForeignKey("DeletedByCoordinadorId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("Fonbec.Cartas.DataAccess.Entities.Filial", "Filial")
                        .WithMany("Padrinos")
                        .HasForeignKey("FilialId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Fonbec.Cartas.DataAccess.Entities.Actors.Coordinador", "UpdatedByCoordinador")
                        .WithMany()
                        .HasForeignKey("UpdatedByCoordinadorId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("CreatedByCoordinador");

                    b.Navigation("DeletedByCoordinador");

                    b.Navigation("Filial");

                    b.Navigation("UpdatedByCoordinador");
                });

            modelBuilder.Entity("Fonbec.Cartas.DataAccess.Entities.Actors.Revisor", b =>
                {
                    b.HasOne("Fonbec.Cartas.DataAccess.Entities.Filial", "Filial")
                        .WithMany("Revisores")
                        .HasForeignKey("FilialId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Filial");
                });

            modelBuilder.Entity("Fonbec.Cartas.DataAccess.Entities.SendAlsoTo", b =>
                {
                    b.HasOne("Fonbec.Cartas.DataAccess.Entities.Actors.Padrino", null)
                        .WithMany("SendAlsoTo")
                        .HasForeignKey("PadrinoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Fonbec.Cartas.DataAccess.Entities.Actors.Padrino", b =>
                {
                    b.Navigation("SendAlsoTo");
                });

            modelBuilder.Entity("Fonbec.Cartas.DataAccess.Entities.Filial", b =>
                {
                    b.Navigation("Coordinadores");

                    b.Navigation("Mediadores");

                    b.Navigation("Padrinos");

                    b.Navigation("Revisores");
                });
#pragma warning restore 612, 618
        }
    }
}
