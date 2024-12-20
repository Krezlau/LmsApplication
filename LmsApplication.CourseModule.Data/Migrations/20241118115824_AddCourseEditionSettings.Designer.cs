﻿// <auto-generated />
using System;
using LmsApplication.CourseModule.Data.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace LmsApplication.CourseModule.Data.Migrations
{
    [DbContext(typeof(CourseDbContext))]
    [Migration("20241118115824_AddCourseEditionSettings")]
    partial class AddCourseEditionSettings
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CourseCourseCategory", b =>
                {
                    b.Property<Guid>("CategoriesId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CoursesId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("CategoriesId", "CoursesId");

                    b.HasIndex("CoursesId");

                    b.ToTable("CourseCourseCategory");
                });

            modelBuilder.Entity("LmsApplication.CourseModule.Data.Entities.Course", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Duration")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedAtUtc")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Courses");
                });

            modelBuilder.Entity("LmsApplication.CourseModule.Data.Entities.CourseCategory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedAtUtc")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("CourseCategories");
                });

            modelBuilder.Entity("LmsApplication.CourseModule.Data.Entities.CourseEdition", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CourseId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<int>("Duration")
                        .HasColumnType("int");

                    b.Property<DateTime>("EndDateUtc")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("RegistrationEndDateUtc")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("RegistrationStartDateUtc")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("StartDateUtc")
                        .HasColumnType("datetime2");

                    b.Property<int>("StudentLimit")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedAtUtc")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.ToTable("CourseEditions");
                });

            modelBuilder.Entity("LmsApplication.CourseModule.Data.Entities.CourseEditionParticipant", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CourseEditionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("ParticipantId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ParticipantRole")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedAtUtc")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CourseEditionId");

                    b.ToTable("CourseEditionParticipants");
                });

            modelBuilder.Entity("LmsApplication.CourseModule.Data.Entities.CourseEditionSettings", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("AllowAllToPost")
                        .HasColumnType("bit");

                    b.Property<Guid>("CourseEditionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("UpdatedAtUtc")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CourseEditionId")
                        .IsUnique();

                    b.ToTable("CourseEditionSettings");
                });

            modelBuilder.Entity("CourseCourseCategory", b =>
                {
                    b.HasOne("LmsApplication.CourseModule.Data.Entities.CourseCategory", null)
                        .WithMany()
                        .HasForeignKey("CategoriesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LmsApplication.CourseModule.Data.Entities.Course", null)
                        .WithMany()
                        .HasForeignKey("CoursesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("LmsApplication.CourseModule.Data.Entities.CourseEdition", b =>
                {
                    b.HasOne("LmsApplication.CourseModule.Data.Entities.Course", "Course")
                        .WithMany("Editions")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Course");
                });

            modelBuilder.Entity("LmsApplication.CourseModule.Data.Entities.CourseEditionParticipant", b =>
                {
                    b.HasOne("LmsApplication.CourseModule.Data.Entities.CourseEdition", null)
                        .WithMany("Participants")
                        .HasForeignKey("CourseEditionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("LmsApplication.CourseModule.Data.Entities.CourseEditionSettings", b =>
                {
                    b.HasOne("LmsApplication.CourseModule.Data.Entities.CourseEdition", "CourseEdition")
                        .WithOne("Settings")
                        .HasForeignKey("LmsApplication.CourseModule.Data.Entities.CourseEditionSettings", "CourseEditionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CourseEdition");
                });

            modelBuilder.Entity("LmsApplication.CourseModule.Data.Entities.Course", b =>
                {
                    b.Navigation("Editions");
                });

            modelBuilder.Entity("LmsApplication.CourseModule.Data.Entities.CourseEdition", b =>
                {
                    b.Navigation("Participants");

                    b.Navigation("Settings")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
