﻿// <auto-generated />
using System;
using BookingWorkplace.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BookingWorkplace.DataBase.Migrations
{
    [DbContext(typeof(BookingWorkplaceDbContext))]
    partial class BookingWorkplaceDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BookingWorkplace.DataBase.Entities.Equipment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.ToTable("Equipments");
                });

            modelBuilder.Entity("BookingWorkplace.DataBase.Entities.EquipmentForWorkplace", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<Guid>("EquipmentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("WorkplaceId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("WorkplaceId");

                    b.HasIndex("EquipmentId", "WorkplaceId")
                        .IsUnique();

                    b.ToTable("EquipmentForWorkplaces");
                });

            modelBuilder.Entity("BookingWorkplace.DataBase.Entities.Reservation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("TimeFrom")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("TimeTo")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("WorkplaceId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("WorkplaceId");

                    b.HasIndex("UserId", "WorkplaceId", "TimeFrom", "TimeTo")
                        .IsUnique();

                    b.ToTable("Reservations");
                });

            modelBuilder.Entity("BookingWorkplace.DataBase.Entities.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("BookingWorkplace.DataBase.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("FullName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("RoleId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("BookingWorkplace.DataBase.Entities.Workplace", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("DeskNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Floor")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Room")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("Room", "Floor", "DeskNumber")
                        .IsUnique();

                    b.ToTable("Workplaces");
                });

            modelBuilder.Entity("BookingWorkplace.DataBase.Entities.EquipmentForWorkplace", b =>
                {
                    b.HasOne("BookingWorkplace.DataBase.Entities.Equipment", "Equipment")
                        .WithMany("EquipmentForWorkplaces")
                        .HasForeignKey("EquipmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BookingWorkplace.DataBase.Entities.Workplace", "Workplace")
                        .WithMany("EquipmentForWorkplaces")
                        .HasForeignKey("WorkplaceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Equipment");

                    b.Navigation("Workplace");
                });

            modelBuilder.Entity("BookingWorkplace.DataBase.Entities.Reservation", b =>
                {
                    b.HasOne("BookingWorkplace.DataBase.Entities.User", "User")
                        .WithMany("Reservations")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BookingWorkplace.DataBase.Entities.Workplace", "Workplace")
                        .WithMany("Reservations")
                        .HasForeignKey("WorkplaceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");

                    b.Navigation("Workplace");
                });

            modelBuilder.Entity("BookingWorkplace.DataBase.Entities.User", b =>
                {
                    b.HasOne("BookingWorkplace.DataBase.Entities.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("BookingWorkplace.DataBase.Entities.Equipment", b =>
                {
                    b.Navigation("EquipmentForWorkplaces");
                });

            modelBuilder.Entity("BookingWorkplace.DataBase.Entities.Role", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("BookingWorkplace.DataBase.Entities.User", b =>
                {
                    b.Navigation("Reservations");
                });

            modelBuilder.Entity("BookingWorkplace.DataBase.Entities.Workplace", b =>
                {
                    b.Navigation("EquipmentForWorkplaces");

                    b.Navigation("Reservations");
                });
#pragma warning restore 612, 618
        }
    }
}
