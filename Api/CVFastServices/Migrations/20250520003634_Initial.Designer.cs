﻿// <auto-generated />
using System;
using CVFastServices.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CVFastApi.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250520003634_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("CVFastApi.Models.AccessLog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("AccessedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("IP")
                        .IsRequired()
                        .HasMaxLength(45)
                        .HasColumnType("character varying(45)");

                    b.Property<Guid>("ShortLinkId")
                        .HasColumnType("uuid");

                    b.Property<string>("UserAgent")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.HasKey("Id");

                    b.HasIndex("ShortLinkId");

                    b.ToTable("AccessLogs");
                });

            modelBuilder.Entity("CVFastApi.Models.Address", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Complement")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Country")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<Guid>("CurriculumId")
                        .HasColumnType("uuid");

                    b.Property<string>("Neighborhood")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Number")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("Street")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.Property<string>("ZipCode")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.HasKey("Id");

                    b.HasIndex("CurriculumId");

                    b.ToTable("Addresses");
                });

            modelBuilder.Entity("CVFastApi.Models.Contact", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("CurriculumId")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsPrimary")
                        .HasColumnType("boolean");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.HasKey("Id");

                    b.HasIndex("CurriculumId");

                    b.ToTable("Contacts");
                });

            modelBuilder.Entity("CVFastApi.Models.Curriculum", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<string>("Summary")
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Curriculums");
                });

            modelBuilder.Entity("CVFastApi.Models.Education", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("CurriculumId")
                        .HasColumnType("uuid");

                    b.Property<string>("Degree")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Description")
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)");

                    b.Property<DateOnly?>("EndDate")
                        .HasColumnType("date");

                    b.Property<string>("FieldOfStudy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Institution")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateOnly>("StartDate")
                        .HasColumnType("date");

                    b.HasKey("Id");

                    b.HasIndex("CurriculumId");

                    b.ToTable("Educations");
                });

            modelBuilder.Entity("CVFastApi.Models.Experience", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<Guid>("CurriculumId")
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)");

                    b.Property<DateOnly?>("EndDate")
                        .HasColumnType("date");

                    b.Property<string>("Location")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateOnly>("StartDate")
                        .HasColumnType("date");

                    b.HasKey("Id");

                    b.HasIndex("CurriculumId");

                    b.ToTable("Experiences");
                });

            modelBuilder.Entity("CVFastApi.Models.ShortLink", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("CurriculumId")
                        .HasColumnType("uuid");

                    b.Property<string>("Hash")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<bool>("IsRevoked")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("RevokedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("CurriculumId");

                    b.HasIndex("Hash")
                        .IsUnique();

                    b.ToTable("ShortLinks");
                });

            modelBuilder.Entity("CVFastApi.Models.Skill", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("CurriculumId")
                        .HasColumnType("uuid");

                    b.Property<int>("Proficiency")
                        .HasColumnType("integer");

                    b.Property<string>("TechName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.HasKey("Id");

                    b.HasIndex("CurriculumId");

                    b.ToTable("Skills");
                });

            modelBuilder.Entity("CVFastApi.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CVFastApi.Models.AccessLog", b =>
                {
                    b.HasOne("CVFastApi.Models.ShortLink", "ShortLink")
                        .WithMany("AccessLogs")
                        .HasForeignKey("ShortLinkId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ShortLink");
                });

            modelBuilder.Entity("CVFastApi.Models.Address", b =>
                {
                    b.HasOne("CVFastApi.Models.Curriculum", "Curriculum")
                        .WithMany("Addresses")
                        .HasForeignKey("CurriculumId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Curriculum");
                });

            modelBuilder.Entity("CVFastApi.Models.Contact", b =>
                {
                    b.HasOne("CVFastApi.Models.Curriculum", "Curriculum")
                        .WithMany("Contacts")
                        .HasForeignKey("CurriculumId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Curriculum");
                });

            modelBuilder.Entity("CVFastApi.Models.Curriculum", b =>
                {
                    b.HasOne("CVFastApi.Models.User", "User")
                        .WithMany("Curriculums")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("CVFastApi.Models.Education", b =>
                {
                    b.HasOne("CVFastApi.Models.Curriculum", "Curriculum")
                        .WithMany("Educations")
                        .HasForeignKey("CurriculumId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Curriculum");
                });

            modelBuilder.Entity("CVFastApi.Models.Experience", b =>
                {
                    b.HasOne("CVFastApi.Models.Curriculum", "Curriculum")
                        .WithMany("Experiences")
                        .HasForeignKey("CurriculumId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Curriculum");
                });

            modelBuilder.Entity("CVFastApi.Models.ShortLink", b =>
                {
                    b.HasOne("CVFastApi.Models.Curriculum", "Curriculum")
                        .WithMany("ShortLinks")
                        .HasForeignKey("CurriculumId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Curriculum");
                });

            modelBuilder.Entity("CVFastApi.Models.Skill", b =>
                {
                    b.HasOne("CVFastApi.Models.Curriculum", "Curriculum")
                        .WithMany("Skills")
                        .HasForeignKey("CurriculumId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Curriculum");
                });

            modelBuilder.Entity("CVFastApi.Models.Curriculum", b =>
                {
                    b.Navigation("Addresses");

                    b.Navigation("Contacts");

                    b.Navigation("Educations");

                    b.Navigation("Experiences");

                    b.Navigation("ShortLinks");

                    b.Navigation("Skills");
                });

            modelBuilder.Entity("CVFastApi.Models.ShortLink", b =>
                {
                    b.Navigation("AccessLogs");
                });

            modelBuilder.Entity("CVFastApi.Models.User", b =>
                {
                    b.Navigation("Curriculums");
                });
#pragma warning restore 612, 618
        }
    }
}
