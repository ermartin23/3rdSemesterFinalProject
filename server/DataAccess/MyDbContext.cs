﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using dataaccess.Entities;

namespace Infrastructure.Postgres.Scaffolding;

public partial class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Board> Boards { get; set; }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<Repeatingboard> Repeatingboards { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Board>(entity =>
        {
            entity.HasKey(e => e.Boardid).HasName("board_pkey");

            entity.ToTable("board", "deadpigeonsdb");

            entity.Property(e => e.Boardid)
                .ValueGeneratedNever()
                .HasColumnName("boardid");
            entity.Property(e => e.Chosennumbers).HasColumnName("chosennumbers");
            entity.Property(e => e.Deletedat).HasColumnName("deletedat");
            entity.Property(e => e.Gameid).HasColumnName("gameid");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValue(false)
                .HasColumnName("isdeleted");
            entity.Property(e => e.Iswinningboard).HasColumnName("iswinningboard");
            entity.Property(e => e.Playerid).HasColumnName("playerid");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
            entity.Property(e => e.Repeatingboardid).HasColumnName("repeatingboardid");

            entity.HasOne(d => d.Game).WithMany(p => p.Boards)
                .HasForeignKey(d => d.Gameid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("board_gameid_fkey");

            entity.HasOne(d => d.Player).WithMany(p => p.Boards)
                .HasForeignKey(d => d.Playerid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("board_playerid_fkey");

            entity.HasOne(d => d.Repeatingboard).WithMany(p => p.Boards)
                .HasForeignKey(d => d.Repeatingboardid)
                .HasConstraintName("board_repeatingboardid_fkey");
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.Gameid).HasName("game_pkey");

            entity.ToTable("game", "deadpigeonsdb");

            entity.Property(e => e.Gameid)
                .ValueGeneratedNever()
                .HasColumnName("gameid");
            entity.Property(e => e.Createdat).HasColumnName("createdat");
            entity.Property(e => e.Cutofftime).HasColumnName("cutofftime");
            entity.Property(e => e.Deletedat).HasColumnName("deletedat");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValue(false)
                .HasColumnName("isdeleted");
            entity.Property(e => e.Weekidentity).HasColumnName("weekidentity");
            entity.Property(e => e.Winningnumbers).HasColumnName("winningnumbers");
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.Playerid).HasName("player_pkey");

            entity.ToTable("player", "deadpigeonsdb");

            entity.Property(e => e.Playerid)
                .ValueGeneratedNever()
                .HasColumnName("playerid");
            entity.Property(e => e.Active).HasColumnName("active");
            entity.Property(e => e.Createdat).HasColumnName("createdat");
            entity.Property(e => e.Deletedat).HasColumnName("deletedat");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValue(false)
                .HasColumnName("isdeleted");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Phone).HasColumnName("phone");
            entity.Property(e => e.Updatedat).HasColumnName("updatedat");
        });

        modelBuilder.Entity<Repeatingboard>(entity =>
        {
            entity.HasKey(e => e.Repeatingboardid).HasName("repeatingboard_pkey");

            entity.ToTable("repeatingboard", "deadpigeonsdb");

            entity.Property(e => e.Repeatingboardid)
                .ValueGeneratedNever()
                .HasColumnName("repeatingboardid");
            entity.Property(e => e.Deletedat).HasColumnName("deletedat");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValue(false)
                .HasColumnName("isdeleted");
            entity.Property(e => e.Isrepeating)
                .HasDefaultValue(false)
                .HasColumnName("isrepeating");
            entity.Property(e => e.Playerid).HasColumnName("playerid");

            entity.HasOne(d => d.Player).WithMany(p => p.Repeatingboards)
                .HasForeignKey(d => d.Playerid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("repeatingboard_playerid_fkey");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Transactionid).HasName("transaction_pkey");

            entity.ToTable("transaction", "deadpigeonsdb");

            entity.Property(e => e.Transactionid)
                .ValueGeneratedNever()
                .HasColumnName("transactionid");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("now()")
                .HasColumnName("createdat");
            entity.Property(e => e.Deletedat).HasColumnName("deletedat");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValue(false)
                .HasColumnName("isdeleted");
            entity.Property(e => e.Mobilepaytransactionnumber).HasColumnName("mobilepaytransactionnumber");
            entity.Property(e => e.Playerid).HasColumnName("playerid");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'pending'::text")
                .HasColumnName("status");

            entity.HasOne(d => d.Player).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.Playerid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("transaction_playerid_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}