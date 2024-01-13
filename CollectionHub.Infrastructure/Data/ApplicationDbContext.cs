using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CollectionHub.DataManagement
{
    public partial class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext() { }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public virtual DbSet<CategoryDb> Categories { get; set; }

        public virtual DbSet<CollectionDb> Collections { get; set; }

        public virtual DbSet<CommentDb> Comments { get; set; }

        public virtual DbSet<ItemDb> Items { get; set; }

        public virtual DbSet<LikeDb> Likes { get; set; }

        public virtual DbSet<TagDb> Tags { get; set; }

        public virtual DbSet<UserDb> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CategoryDb>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("category_id_primary");

                entity.ToTable("Category");

                entity.Property(e => e.Name).HasMaxLength(255);
            });

            modelBuilder.Entity<CollectionDb>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("collection_id_primary");

                entity.ToTable("Collection");

                entity.Property(e => e.Id)
                    .HasColumnName("id");
                entity.Property(e => e.Bool1Name).HasMaxLength(255);
                entity.Property(e => e.Bool2Name).HasMaxLength(255);
                entity.Property(e => e.Bool3Name).HasMaxLength(255);
                entity.Property(e => e.CreationDate).HasColumnType("datetimeoffset");
                entity.Property(e => e.Date1Name).HasMaxLength(255);
                entity.Property(e => e.Date2Name).HasMaxLength(255);
                entity.Property(e => e.Date3Name).HasMaxLength(255);
                entity.Property(e => e.ImageUrl).HasMaxLength(255);
                entity.Property(e => e.Int1Name).HasMaxLength(255);
                entity.Property(e => e.Int2Name).HasMaxLength(255);
                entity.Property(e => e.Int3Name).HasMaxLength(255);
                entity.Property(e => e.Name).HasMaxLength(255);
                entity.Property(e => e.String1Name).HasMaxLength(255);
                entity.Property(e => e.String2Name).HasMaxLength(255);
                entity.Property(e => e.String3Name).HasMaxLength(255);
                entity.Property(e => e.Text1Name).HasMaxLength(255);
                entity.Property(e => e.Text2Name).HasMaxLength(255);
                entity.Property(e => e.Text3Name).HasMaxLength(255);
                entity.Property(e => e.UserId).HasMaxLength(255);

                entity.HasOne(d => d.Category).WithMany(p => p.Collections)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("collection_categoryid_foreign");

                entity.HasOne(d => d.User).WithMany(p => p.Collections)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("collection_userid_foreign");
            });

            modelBuilder.Entity<CommentDb>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("comment_id_primary");

                entity.ToTable("Comment");

                entity.Property(e => e.Id);
                entity.Property(e => e.CreationDate).HasColumnType("datetimeoffset");
                entity.Property(e => e.UserId).HasMaxLength(255);

                entity.HasOne(d => d.Item).WithMany(p => p.Comments)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("comment_itemid_foreign");

                entity.HasOne(d => d.User).WithMany(p => p.Comments)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("comment_userid_foreign");
            });

            modelBuilder.Entity<ItemDb>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("item_id_primary");

                entity.ToTable("Item");

                entity.Property(e => e.Id);
                entity.Property(e => e.Date1Value).HasColumnType("datetimeoffset");
                entity.Property(e => e.Date2Value).HasColumnType("datetimeoffset");
                entity.Property(e => e.Date3Value).HasColumnType("datetimeoffset");
                entity.Property(e => e.Name).HasMaxLength(255);
                entity.Property(e => e.String1Value).HasMaxLength(255);
                entity.Property(e => e.String2Value).HasMaxLength(255);
                entity.Property(e => e.String3Value).HasMaxLength(255);

                entity.HasOne(d => d.Collection).WithMany(p => p.Items)
                    .HasForeignKey(d => d.CollectionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("item_collectionid_foreign");
            });

            modelBuilder.Entity<LikeDb>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("like_id_primary");

                entity.ToTable("Like");

                entity.Property(e => e.Id)                    
                    .HasColumnName("id");
                entity.Property(e => e.UserId).HasMaxLength(255);

                entity.HasOne(d => d.Item).WithMany(p => p.Likes)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("like_itemid_foreign");

                entity.HasOne(d => d.User).WithMany(p => p.Likes)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("like_userid_foreign");
            });

            modelBuilder.Entity<TagDb>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("tag_id_primary");

                entity.ToTable("Tag");

                entity.HasIndex(e => e.Name, "tag_name_unique").IsUnique(false);

                entity.Property(e => e.Id)                    
                    .HasColumnName("id");
                entity.Property(e => e.Name).HasMaxLength(255);

                entity.HasOne(d => d.Item).WithMany(p => p.Tags)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tag_itemid_foreign");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
