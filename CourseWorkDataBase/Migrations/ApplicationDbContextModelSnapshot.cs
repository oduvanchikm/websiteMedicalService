﻿// <auto-generated />
using System;
using CourseWorkDataBase.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CourseWorkDataBase.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0-rc.2.24474.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("CourseWorkDataBase.Models.Appointment", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("AppointmentSlotId")
                        .HasColumnType("bigint");

                    b.Property<DateTimeOffset>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("PatientId")
                        .HasColumnType("bigint");

                    b.Property<long>("StatusId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("AppointmentSlotId")
                        .IsUnique();

                    b.HasIndex("PatientId");

                    b.HasIndex("StatusId");

                    b.ToTable("Appointments");
                });

            modelBuilder.Entity("CourseWorkDataBase.Models.AppointmentSlot", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("DoctorId")
                        .HasColumnType("bigint");

                    b.Property<DateTimeOffset>("EndTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsBooked")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset>("StartTime")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("DoctorId");

                    b.ToTable("AppointmentSlots");
                });

            modelBuilder.Entity("CourseWorkDataBase.Models.Clinic", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Clinics");
                });

            modelBuilder.Entity("CourseWorkDataBase.Models.Doctor", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("ID"));

                    b.Property<long?>("ClinicId")
                        .HasColumnType("bigint");

                    b.Property<string>("FamilyName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("SpecialtyID")
                        .HasColumnType("bigint");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("ID");

                    b.HasIndex("ClinicId");

                    b.HasIndex("SpecialtyID");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Doctors");
                });

            modelBuilder.Entity("CourseWorkDataBase.Models.Patient", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FamilyName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Patients");
                });

            modelBuilder.Entity("CourseWorkDataBase.Models.Role", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            Id = 1L,
                            Name = "Admin"
                        },
                        new
                        {
                            Id = 2L,
                            Name = "Doctor"
                        },
                        new
                        {
                            Id = 3L,
                            Name = "Patient"
                        });
                });

            modelBuilder.Entity("CourseWorkDataBase.Models.Specialty", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("NameSpecialty")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Specialties");
                });

            modelBuilder.Entity("CourseWorkDataBase.Models.Status", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Statuses");

                    b.HasData(
                        new
                        {
                            Id = 1L,
                            Name = "Booked"
                        },
                        new
                        {
                            Id = 2L,
                            Name = "Not booked"
                        },
                        new
                        {
                            Id = 3L,
                            Name = "Canceled"
                        });
                });

            modelBuilder.Entity("CourseWorkDataBase.Models.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("RoleId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 1L,
                            CreatedAt = new DateTimeOffset(new DateTime(2023, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            Email = "admin@example.com",
                            Password = "$2a$11$o.sTnyjh8Mr9ArOWpr5Q..rsRPFHJ7EJ6pIeFUyVEfP2fe5b1riHm",
                            RoleId = 1L
                        });
                });

            modelBuilder.Entity("CourseWorkDataBase.Models.Appointment", b =>
                {
                    b.HasOne("CourseWorkDataBase.Models.AppointmentSlot", "AppointmentSlot")
                        .WithOne("Appointment")
                        .HasForeignKey("CourseWorkDataBase.Models.Appointment", "AppointmentSlotId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("CourseWorkDataBase.Models.Patient", "Patient")
                        .WithMany("Appointments")
                        .HasForeignKey("PatientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CourseWorkDataBase.Models.Status", "Status")
                        .WithMany("Appointments")
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AppointmentSlot");

                    b.Navigation("Patient");

                    b.Navigation("Status");
                });

            modelBuilder.Entity("CourseWorkDataBase.Models.AppointmentSlot", b =>
                {
                    b.HasOne("CourseWorkDataBase.Models.Doctor", "Doctor")
                        .WithMany("AppointmentSlots")
                        .HasForeignKey("DoctorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Doctor");
                });

            modelBuilder.Entity("CourseWorkDataBase.Models.Doctor", b =>
                {
                    b.HasOne("CourseWorkDataBase.Models.Clinic", "Clinic")
                        .WithMany("Doctors")
                        .HasForeignKey("ClinicId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("CourseWorkDataBase.Models.Specialty", "Specialty")
                        .WithMany("Doctors")
                        .HasForeignKey("SpecialtyID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("CourseWorkDataBase.Models.User", "User")
                        .WithOne("Doctor")
                        .HasForeignKey("CourseWorkDataBase.Models.Doctor", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Clinic");

                    b.Navigation("Specialty");

                    b.Navigation("User");
                });

            modelBuilder.Entity("CourseWorkDataBase.Models.Patient", b =>
                {
                    b.HasOne("CourseWorkDataBase.Models.User", "User")
                        .WithOne("Patient")
                        .HasForeignKey("CourseWorkDataBase.Models.Patient", "UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("CourseWorkDataBase.Models.User", b =>
                {
                    b.HasOne("CourseWorkDataBase.Models.Role", "Role")
                        .WithMany("User")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("CourseWorkDataBase.Models.AppointmentSlot", b =>
                {
                    b.Navigation("Appointment")
                        .IsRequired();
                });

            modelBuilder.Entity("CourseWorkDataBase.Models.Clinic", b =>
                {
                    b.Navigation("Doctors");
                });

            modelBuilder.Entity("CourseWorkDataBase.Models.Doctor", b =>
                {
                    b.Navigation("AppointmentSlots");
                });

            modelBuilder.Entity("CourseWorkDataBase.Models.Patient", b =>
                {
                    b.Navigation("Appointments");
                });

            modelBuilder.Entity("CourseWorkDataBase.Models.Role", b =>
                {
                    b.Navigation("User");
                });

            modelBuilder.Entity("CourseWorkDataBase.Models.Specialty", b =>
                {
                    b.Navigation("Doctors");
                });

            modelBuilder.Entity("CourseWorkDataBase.Models.Status", b =>
                {
                    b.Navigation("Appointments");
                });

            modelBuilder.Entity("CourseWorkDataBase.Models.User", b =>
                {
                    b.Navigation("Doctor");

                    b.Navigation("Patient");
                });
#pragma warning restore 612, 618
        }
    }
}
