﻿// <auto-generated />
using System;
using Identity.App.Models.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Identity.App.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("13980529140158_Inisialize02")]
    partial class Inisialize02
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Identity.App.Models.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("CreatedByBrowserName")
                        .HasMaxLength(1000);

                    b.Property<string>("CreatedByIp")
                        .HasMaxLength(255);

                    b.Property<int?>("CreatedByUserId");

                    b.Property<DateTimeOffset?>("CreatedDateTime");

                    b.Property<string>("Description");

                    b.Property<string>("ModifiedByBrowserName")
                        .HasMaxLength(1000);

                    b.Property<string>("ModifiedByIp")
                        .HasMaxLength(255);

                    b.Property<int?>("ModifiedByUserId");

                    b.Property<DateTimeOffset?>("ModifiedDateTime");

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("Role");
                });

            modelBuilder.Entity("Identity.App.Models.RoleClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("CreatedByBrowserName")
                        .HasMaxLength(1000);

                    b.Property<string>("CreatedByIp")
                        .HasMaxLength(255);

                    b.Property<int?>("CreatedByUserId");

                    b.Property<DateTimeOffset?>("CreatedDateTime");

                    b.Property<string>("ModifiedByBrowserName")
                        .HasMaxLength(1000);

                    b.Property<string>("ModifiedByIp")
                        .HasMaxLength(255);

                    b.Property<int?>("ModifiedByUserId");

                    b.Property<DateTimeOffset?>("ModifiedDateTime");

                    b.Property<int>("RoleId");

                    b.Property<int?>("RoleId1");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.HasIndex("RoleId1");

                    b.ToTable("RoleClaim");
                });

            modelBuilder.Entity("Identity.App.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AccessFailedCount");

                    b.Property<DateTimeOffset?>("BirthDate");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("CreatedByBrowserName")
                        .HasMaxLength(1000);

                    b.Property<string>("CreatedByIp")
                        .HasMaxLength(255);

                    b.Property<int?>("CreatedByUserId");

                    b.Property<DateTimeOffset?>("CreatedDateTime");

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FirstName")
                        .HasMaxLength(450);

                    b.Property<bool>("IsActive");

                    b.Property<bool>("IsEmailPublic");

                    b.Property<string>("LastName")
                        .HasMaxLength(450);

                    b.Property<DateTimeOffset?>("LastVisitDateTime");

                    b.Property<string>("Location");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("ModifiedByBrowserName")
                        .HasMaxLength(1000);

                    b.Property<string>("ModifiedByIp")
                        .HasMaxLength(255);

                    b.Property<int?>("ModifiedByUserId");

                    b.Property<DateTimeOffset?>("ModifiedDateTime");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("PhotoFileName")
                        .HasMaxLength(450);

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("User");
                });

            modelBuilder.Entity("Identity.App.Models.UserClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("CreatedByBrowserName")
                        .HasMaxLength(1000);

                    b.Property<string>("CreatedByIp")
                        .HasMaxLength(255);

                    b.Property<int?>("CreatedByUserId");

                    b.Property<DateTimeOffset?>("CreatedDateTime");

                    b.Property<string>("ModifiedByBrowserName")
                        .HasMaxLength(1000);

                    b.Property<string>("ModifiedByIp")
                        .HasMaxLength(255);

                    b.Property<int?>("ModifiedByUserId");

                    b.Property<DateTimeOffset?>("ModifiedDateTime");

                    b.Property<int>("UserId");

                    b.Property<int?>("UserId1");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.HasIndex("UserId1");

                    b.ToTable("UserClaim");
                });

            modelBuilder.Entity("Identity.App.Models.UserLogin", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("CreatedByBrowserName")
                        .HasMaxLength(1000);

                    b.Property<string>("CreatedByIp")
                        .HasMaxLength(255);

                    b.Property<int?>("CreatedByUserId");

                    b.Property<DateTimeOffset?>("CreatedDateTime");

                    b.Property<string>("ModifiedByBrowserName")
                        .HasMaxLength(1000);

                    b.Property<string>("ModifiedByIp")
                        .HasMaxLength(255);

                    b.Property<int?>("ModifiedByUserId");

                    b.Property<DateTimeOffset?>("ModifiedDateTime");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<int>("UserId");

                    b.Property<int?>("UserId1");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.HasIndex("UserId1");

                    b.ToTable("UserLogin");
                });

            modelBuilder.Entity("Identity.App.Models.UserRole", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("RoleId");

                    b.Property<string>("CreatedByBrowserName")
                        .HasMaxLength(1000);

                    b.Property<string>("CreatedByIp")
                        .HasMaxLength(255);

                    b.Property<int?>("CreatedByUserId");

                    b.Property<DateTimeOffset?>("CreatedDateTime");

                    b.Property<string>("ModifiedByBrowserName")
                        .HasMaxLength(1000);

                    b.Property<string>("ModifiedByIp")
                        .HasMaxLength(255);

                    b.Property<int?>("ModifiedByUserId");

                    b.Property<DateTimeOffset?>("ModifiedDateTime");

                    b.Property<int?>("RoleId1");

                    b.Property<int?>("UserId1");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.HasIndex("RoleId1");

                    b.HasIndex("UserId1");

                    b.ToTable("UserRole");
                });

            modelBuilder.Entity("Identity.App.Models.UserToken", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("CreatedByBrowserName")
                        .HasMaxLength(1000);

                    b.Property<string>("CreatedByIp")
                        .HasMaxLength(255);

                    b.Property<int?>("CreatedByUserId");

                    b.Property<DateTimeOffset?>("CreatedDateTime");

                    b.Property<string>("ModifiedByBrowserName")
                        .HasMaxLength(1000);

                    b.Property<string>("ModifiedByIp")
                        .HasMaxLength(255);

                    b.Property<int?>("ModifiedByUserId");

                    b.Property<DateTimeOffset?>("ModifiedDateTime");

                    b.Property<int?>("UserId1");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.HasIndex("UserId1");

                    b.ToTable("UserToken");
                });

            modelBuilder.Entity("Identity.App.Models.UserUsedPassword", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CreatedByBrowserName")
                        .HasMaxLength(1000);

                    b.Property<string>("CreatedByIp")
                        .HasMaxLength(255);

                    b.Property<int?>("CreatedByUserId");

                    b.Property<DateTimeOffset?>("CreatedDateTime");

                    b.Property<string>("HashedPassword");

                    b.Property<string>("ModifiedByBrowserName")
                        .HasMaxLength(1000);

                    b.Property<string>("ModifiedByIp")
                        .HasMaxLength(255);

                    b.Property<int?>("ModifiedByUserId");

                    b.Property<DateTimeOffset?>("ModifiedDateTime");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserUsedPassword");
                });

            modelBuilder.Entity("Identity.App.Models.RoleClaim", b =>
                {
                    b.HasOne("Identity.App.Models.Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Identity.App.Models.Role", "Role")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId1");
                });

            modelBuilder.Entity("Identity.App.Models.UserClaim", b =>
                {
                    b.HasOne("Identity.App.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Identity.App.Models.User", "User")
                        .WithMany("Claims")
                        .HasForeignKey("UserId1");
                });

            modelBuilder.Entity("Identity.App.Models.UserLogin", b =>
                {
                    b.HasOne("Identity.App.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Identity.App.Models.User", "User")
                        .WithMany("Logins")
                        .HasForeignKey("UserId1");
                });

            modelBuilder.Entity("Identity.App.Models.UserRole", b =>
                {
                    b.HasOne("Identity.App.Models.Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Identity.App.Models.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId1");

                    b.HasOne("Identity.App.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Identity.App.Models.User", "User")
                        .WithMany("Roles")
                        .HasForeignKey("UserId1");
                });

            modelBuilder.Entity("Identity.App.Models.UserToken", b =>
                {
                    b.HasOne("Identity.App.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Identity.App.Models.User", "User")
                        .WithMany("UserTokens")
                        .HasForeignKey("UserId1");
                });

            modelBuilder.Entity("Identity.App.Models.UserUsedPassword", b =>
                {
                    b.HasOne("Identity.App.Models.User", "User")
                        .WithMany("UserUsedPasswords")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
