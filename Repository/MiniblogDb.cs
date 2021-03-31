using Domain.Entities;
using Domain.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System;

namespace Repo
{
    public class MiniblogDb : DbContext
    {
        public DbSet<Role> Roles { get; set; }
        public DbSet<ExtendedRole> ExtendedRoles { get; set; }
        public DbSet<Entity> Entities { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Series> Series { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<BaseDisplayOptions> BaseArticlesOptions { get; set; }
        public DbSet<ArticleOptions> ArticleOptions { get; set; }
        public DbSet<FoundWord> FoundWords { get; set; }
        public DbSet<IndexInfo> IndexInfos { get; set; }
        public DbSet<CheckList> CheckLists { get; set; }

        public MiniblogDb(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>()
                .Property(e => e.Type)
                .HasConversion(
                v => v.ToString(),
                v => (RoleType)Enum.Parse(typeof(RoleType), v));

            modelBuilder.Entity<CheckList>()
                .Property(c => c.CheckAction)
                .HasConversion(
                v => v.ToString(),
                v => (CheckAction)Enum.Parse(typeof(CheckAction), v));

            CheckList listToCheck = new CheckList { Id = Guid.NewGuid(), CheckAction = CheckAction.Verify };
            CheckList blackList = new CheckList { Id = Guid.NewGuid(), CheckAction = CheckAction.Delete };
            modelBuilder.Entity<CheckList>().HasData(listToCheck, blackList);

            modelBuilder.Entity<Article>()
                .Property(a => a.EntryType)
                .HasConversion(
                v => v.ToString(),
                v => (EntryType)Enum.Parse(typeof(EntryType), v));

            modelBuilder.Entity<Article>()
                .HasOne(a => a.User)
                .WithMany(u => u.Articles)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Article>()
                .HasOne(a => a.Topic)
                .WithMany(t => t.Articles)
                .HasForeignKey(a => a.TopicId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Article>()
                .HasOne(a => a.Series)
                .WithMany(t => t.Articles)
                .HasForeignKey(a => a.SeriesId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<UserBookmark>()
                .HasKey(k => new { k.UserId, k.ArticleId });

            modelBuilder.Entity<UserBookmark>()
                .HasOne(u => u.User)
                .WithMany(a => a.Bookmarked)
                .HasForeignKey(ub => ub.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserBookmark>()
                .HasOne(a => a.Article)
                .WithMany(u => u.Bookmarks)
                .HasForeignKey(ub => ub.ArticleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserFavourite>()
                .HasKey(k => new { k.UserId, k.ArticleId });

            modelBuilder.Entity<UserFavourite>()
                .HasOne(a => a.User)
                .WithMany(uf => uf.Liked)
                .HasForeignKey(k => k.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserFavourite>()
                .HasOne(uf => uf.Article)
                .WithMany(a => a.Likes)
                .HasForeignKey(uf => uf.ArticleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ArticleTag>()
                .HasKey(k => new { k.ArticleId, k.TagId });

            modelBuilder.Entity<ArticleTag>()
                .HasOne(a => a.Article)
                .WithMany(at => at.ArticleTags)
                .HasForeignKey(k => k.ArticleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ArticleTag>()
                .HasOne(at => at.Tag)
                .WithMany(t => t.ArticleTags)
                .HasForeignKey(fk => fk.TagId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Author)
                .WithMany(a => a.Comments)
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<CommentLikes>()
                .HasKey(k => new { k.UserId, k.CommentId });

            modelBuilder.Entity<CommentLikes>()
                .HasOne(cl => cl.User)
                .WithMany(u => u.LikedComments)
                .HasForeignKey(cl => cl.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CommentLikes>()
                .HasOne(cl => cl.Comment)
                .WithMany(c => c.Likes)
                .HasForeignKey(cl => cl.CommentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Image>()
                .HasOne(i => i.Article)
                .WithMany(ar => ar.Images)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<IndexInfo>()
                .HasOne(i => i.FoundWord)
                .WithMany(f => f.IndexInfos)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FoundWord>()
                .HasIndex(f => f.Word)
                .IsUnique();

            modelBuilder.Entity<ArticleOptions>()
                .HasOne(u => u.Article)
                .WithOne(ar => ar.DisplayOptions)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);
        }
    }
}
