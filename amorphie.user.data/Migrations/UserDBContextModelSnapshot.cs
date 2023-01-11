﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using amorphie.user.data;

#nullable disable

namespace amorphie.user.data.Migrations
{
    [DbContext(typeof(UserDBContext))]
    partial class UserDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("amorphie.user.data.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("LastLoginDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .HasColumnType("text");

                    b.Property<int?>("State")
                        .HasColumnType("integer");

                    b.Property<string>("Surname")
                        .HasColumnType("text");

                    b.Property<string>("TcNo")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = new Guid("d6359d7a-ac9a-47c9-ac6d-80c057743448"),
                            Name = "Damla",
                            Password = "12345",
                            Surname = "Erhan",
                            TcNo = "12345"
                        });
                });

            modelBuilder.Entity("amorphie.user.data.UserSecurityQuestion", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("SecurityQuestion")
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserSecurityQuestions");

                    b.HasData(
                        new
                        {
                            Id = new Guid("0b02f9e1-20ad-47b3-bf11-ff51d86b50cb"),
                            SecurityQuestion = "en sevdiğiniz araba",
                            UserId = new Guid("d6359d7a-ac9a-47c9-ac6d-80c057743448")
                        });
                });

            modelBuilder.Entity("amorphie.user.data.UserTag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserTags");

                    b.HasData(
                        new
                        {
                            Id = new Guid("f9f88ec4-fd3f-4042-bf40-0c672d972a3a"),
                            Name = "user-list-get",
                            UserId = new Guid("d6359d7a-ac9a-47c9-ac6d-80c057743448")
                        });
                });

            modelBuilder.Entity("amorphie.user.data.UserSecurityQuestion", b =>
                {
                    b.HasOne("amorphie.user.data.User", "User")
                        .WithMany("UserSecurityQuestions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("amorphie.user.data.UserTag", b =>
                {
                    b.HasOne("amorphie.user.data.User", "User")
                        .WithMany("UserTags")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("amorphie.user.data.User", b =>
                {
                    b.Navigation("UserSecurityQuestions");

                    b.Navigation("UserTags");
                });
#pragma warning restore 612, 618
        }
    }
}
