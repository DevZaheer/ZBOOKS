using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ZBooksApi.Models;

public partial class ZBooksDbContext : DbContext
{
    public ZBooksDbContext()
    {
    }

    public ZBooksDbContext(DbContextOptions<ZBooksDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Chapter> Chapters { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Purchase> Purchases { get; set; }

    public virtual DbSet<ReadingProgress> ReadingProgresses { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Wishlist> Wishlists { get; set; }

  
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.AuthorId).HasName("PK__Authors__70DAFC34ED7A17F0");

            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.Photo).HasMaxLength(500);
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.BookId).HasName("PK__Books__3DE0C207E3B7C994");

            entity.Property(e => e.CoverImage).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsAvailable).HasDefaultValue(true);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasMany(d => d.Authors).WithMany(p => p.Books)
                .UsingEntity<Dictionary<string, object>>(
                    "BookAuthor",
                    r => r.HasOne<Author>().WithMany()
                        .HasForeignKey("AuthorId")
                        .HasConstraintName("FK_BookAuthors_Authors"),
                    l => l.HasOne<Book>().WithMany()
                        .HasForeignKey("BookId")
                        .HasConstraintName("FK_BookAuthors_Books"),
                    j =>
                    {
                        j.HasKey("BookId", "AuthorId");
                        j.ToTable("BookAuthors");
                    });

            entity.HasMany(d => d.Categories).WithMany(p => p.Books)
                .UsingEntity<Dictionary<string, object>>(
                    "BookCategory",
                    r => r.HasOne<Category>().WithMany()
                        .HasForeignKey("CategoryId")
                        .HasConstraintName("FK_BookCategories_Categories"),
                    l => l.HasOne<Book>().WithMany()
                        .HasForeignKey("BookId")
                        .HasConstraintName("FK_BookCategories_Books"),
                    j =>
                    {
                        j.HasKey("BookId", "CategoryId");
                        j.ToTable("BookCategories");
                    });
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A0B2C27145B");

            entity.HasIndex(e => e.Name, "UQ__Categori__737584F67060A96A").IsUnique();

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Chapter>(entity =>
        {
            entity.HasKey(e => e.ChapterId).HasName("PK__Chapters__0893A36A3B24052A");

            entity.HasIndex(e => new { e.BookId, e.ChapterNumber }, "UQ_Chapters_Book_ChapterNumber").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Book).WithMany(p => p.Chapters)
                .HasForeignKey(d => d.BookId)
                .HasConstraintName("FK_Chapters_Books");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A380F931094");

            entity.HasIndex(e => e.PaymentReference);

            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.PaymentReference).HasMaxLength(200);
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.Purchase).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PurchaseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payments_Purchases");
        });

        modelBuilder.Entity<Purchase>(entity =>
        {
            entity.HasKey(e => e.PurchaseId).HasName("PK__Purchase__6B0A6BBEA95FC8FD");

            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PurchasedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.Book).WithMany(p => p.Purchases)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Purchases_Books");

            entity.HasOne(d => d.User).WithMany(p => p.Purchases)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Purchases_Users");
        });

        modelBuilder.Entity<ReadingProgress>(entity =>
        {
            entity.HasKey(e => e.ReadingProgressId).HasName("PK__ReadingP__D1C770A43A7A4AAB");

            entity.ToTable("ReadingProgress");

            entity.HasIndex(e => new { e.UserId, e.BookId }, "UQ_ReadingProgress_User_Book").IsUnique();

            entity.Property(e => e.CurrentChapter).HasDefaultValue(1);
            entity.Property(e => e.CurrentPage).HasDefaultValue(1);
            entity.Property(e => e.LastReadAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ProgressPercentage).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.Book).WithMany(p => p.ReadingProgresses)
                .HasForeignKey(d => d.BookId)
                .HasConstraintName("FK_ReadingProgress_Books");

            entity.HasOne(d => d.User).WithMany(p => p.ReadingProgresses)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_ReadingProgress_Users");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__Reviews__74BC79CE895E0DA5");

            entity.HasIndex(e => new { e.UserId, e.BookId }, "UQ_Reviews_User_Book").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Book).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.BookId)
                .HasConstraintName("FK_Reviews_Books");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Reviews_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C248568EC");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534A8B14F2A").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasDefaultValue("Customer");
        });

        modelBuilder.Entity<Wishlist>(entity =>
        {
            entity.HasKey(e => e.WishlistId).HasName("PK__Wishlist__233189EB9DA2B8E6");

            entity.HasIndex(e => new { e.UserId, e.BookId }, "UQ_Wishlists_User_Book").IsUnique();

            entity.Property(e => e.AddedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Book).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.BookId)
                .HasConstraintName("FK_Wishlists_Books");

            entity.HasOne(d => d.User).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Wishlists_Users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
