﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MonoRec.Data;

#nullable disable

namespace MonoRec.Migrations
{
    [DbContext(typeof(MonoRecDbContext))]
    [Migration("20220803170859_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.7");

            modelBuilder.Entity("DoctorPatient", b =>
                {
                    b.Property<int>("DoctorsDoctorId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PatientsPatientId")
                        .HasColumnType("INTEGER");

                    b.HasKey("DoctorsDoctorId", "PatientsPatientId");

                    b.HasIndex("PatientsPatientId");

                    b.ToTable("DoctorPatient");
                });

            modelBuilder.Entity("MonoRec.Models.Doctor", b =>
                {
                    b.Property<int>("DoctorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("DoctorAge")
                        .HasColumnType("INTEGER");

                    b.Property<string>("DoctorEmail")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("DoctorName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("DoctorSex")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Specialty")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("DoctorId");

                    b.ToTable("Doctors");
                });

            modelBuilder.Entity("MonoRec.Models.DoctorNote", b =>
                {
                    b.Property<int>("DoctorNoteId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("DoctorId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Note")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("VisitId")
                        .HasColumnType("INTEGER");

                    b.HasKey("DoctorNoteId");

                    b.HasIndex("DoctorId");

                    b.ToTable("DoctorNotes");
                });

            modelBuilder.Entity("MonoRec.Models.Patient", b =>
                {
                    b.Property<int>("PatientId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("PatientAge")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PatientEmail")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PatientName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("PatientSex")
                        .HasColumnType("INTEGER");

                    b.HasKey("PatientId");

                    b.ToTable("Patients");
                });

            modelBuilder.Entity("MonoRec.Models.Visit", b =>
                {
                    b.Property<int>("VisitId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Diagnosis")
                        .HasColumnType("TEXT");

                    b.Property<int>("DoctorId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PatientId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Symptoms")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("VisitDate")
                        .HasColumnType("TEXT");

                    b.HasKey("VisitId");

                    b.HasIndex("DoctorId");

                    b.HasIndex("PatientId");

                    b.ToTable("Visits");
                });

            modelBuilder.Entity("DoctorPatient", b =>
                {
                    b.HasOne("MonoRec.Models.Doctor", null)
                        .WithMany()
                        .HasForeignKey("DoctorsDoctorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MonoRec.Models.Patient", null)
                        .WithMany()
                        .HasForeignKey("PatientsPatientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MonoRec.Models.DoctorNote", b =>
                {
                    b.HasOne("MonoRec.Models.Doctor", null)
                        .WithMany("DoctorNotes")
                        .HasForeignKey("DoctorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MonoRec.Models.Visit", b =>
                {
                    b.HasOne("MonoRec.Models.Doctor", null)
                        .WithMany("Visits")
                        .HasForeignKey("DoctorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MonoRec.Models.Patient", null)
                        .WithMany("Visits")
                        .HasForeignKey("PatientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MonoRec.Models.Doctor", b =>
                {
                    b.Navigation("DoctorNotes");

                    b.Navigation("Visits");
                });

            modelBuilder.Entity("MonoRec.Models.Patient", b =>
                {
                    b.Navigation("Visits");
                });
#pragma warning restore 612, 618
        }
    }
}
