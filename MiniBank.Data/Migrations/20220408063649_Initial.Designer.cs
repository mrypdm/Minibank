﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MiniBank.Data.Context;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MiniBank.Data.Migrations
{
    [DbContext(typeof(MiniBankContext))]
    [Migration("20220408063649_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityByDefaultColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.15");

            modelBuilder.Entity("MiniBank.Data.BankAccounts.BankAccountDbModel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<double>("Amount")
                        .HasColumnType("double precision")
                        .HasColumnName("amount");

                    b.Property<DateTime?>("ClosingDate")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("closing_date");

                    b.Property<string>("CurrencyCode")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("currency_code");

                    b.Property<bool>("IsClosed")
                        .HasColumnType("boolean")
                        .HasColumnName("is_closed");

                    b.Property<DateTime>("OpeningDate")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("opening_date");

                    b.Property<string>("UserId")
                        .HasColumnType("text")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_bank_account_id");

                    b.ToTable("bank_account");
                });

            modelBuilder.Entity("MiniBank.Data.Transfers.TransferDbModel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<double>("Amount")
                        .HasColumnType("double precision")
                        .HasColumnName("amount");

                    b.Property<string>("CurrencyCode")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("currency_code");

                    b.Property<string>("FromAccountId")
                        .HasColumnType("text")
                        .HasColumnName("from_account_id");

                    b.Property<string>("ToAccountId")
                        .HasColumnType("text")
                        .HasColumnName("to_account_id");

                    b.Property<DateTime>("TransferDateTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("transfer_date_time");

                    b.HasKey("Id")
                        .HasName("pk_transfer_id");

                    b.ToTable("transfer");
                });

            modelBuilder.Entity("MiniBank.Data.Users.UserDbModel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<string>("Email")
                        .HasColumnType("text")
                        .HasColumnName("email");

                    b.Property<string>("Login")
                        .HasColumnType("text")
                        .HasColumnName("login");

                    b.HasKey("Id")
                        .HasName("pk_user_id");

                    b.ToTable("user");
                });
#pragma warning restore 612, 618
        }
    }
}
