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
        public DbSet<User> Users { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Series> Series { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<BaseDisplayOptions> BaseArticlesOptions { get; set; }
        public DbSet<ArticleOptions> ArticleOptions { get; set; }
        public DbSet<FoundWord> FoundWords { get; set; }
        public DbSet<IndexInfo> IndexInfos { get; set; }

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

            modelBuilder.Entity<Article>()
                .Property(a => a.EntryType)
                .HasConversion(
                v => v.ToString(),
                v => (EntryType)Enum.Parse(typeof(EntryType), v));

            modelBuilder.Entity<Article>()
                .HasOne(a => a.User)
                .WithMany(u => u.Articles)
                .HasForeignKey(a => a.UserId);

            modelBuilder.Entity<UserBookmark>()
                .HasKey(k => new { k.UserId, k.ArticleId });

            modelBuilder.Entity<UserBookmark>()
                .HasOne(u => u.User)
                .WithMany(a => a.Bookmarked)
                .HasForeignKey(ub => ub.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserBookmark>()
                .HasOne(a => a.Article)
                .WithMany(u => u.Bookmarks)
                .HasForeignKey(ub => ub.ArticleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserFavourite>()
                .HasKey(k => new { k.UserId, k.ArticleId });

            modelBuilder.Entity<UserFavourite>()
                .HasOne(a => a.User)
                .WithMany(uf => uf.Liked)
                .HasForeignKey(k => k.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserFavourite>()
                .HasOne(uf => uf.Article)
                .WithMany(a => a.Likes)
                .HasForeignKey(uf => uf.ArticleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Author)
                .WithMany(a => a.Comments)
                .HasForeignKey(c => c.AuthorId);

            modelBuilder.Entity<CommentLikes>()
                .HasKey(k => new { k.UserId, k.CommentId });

            modelBuilder.Entity<CommentLikes>()
                .HasOne(cl => cl.User)
                .WithMany(u => u.LikedComments)
                .HasForeignKey(cl => cl.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CommentLikes>()
                .HasOne(cl => cl.Comment)
                .WithMany(c => c.Likes)
                .HasForeignKey(cl => cl.CommentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Author)
                .WithMany(us => us.Comments)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Image>()
                .HasOne(i => i.Article)
                .WithMany(ar => ar.Images)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<IndexInfo>()
                .HasOne(i => i.FoundWord)
                .WithMany(f => f.IndexInfos)
                .OnDelete(DeleteBehavior.Cascade);

            //ArticleOptions
            modelBuilder.Entity<ArticleOptions>()
                .HasOne(u => u.Article)
                .WithOne(ar => ar.DisplayOptions)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ArticleOptions>()
                .Property(e => e.ColorTheme)
                .HasConversion(
                v => v.ToString(),
                v => (ColorTheme)Enum.Parse(typeof(ColorTheme), v));

            modelBuilder.Entity<Topic>()
                .HasOne(t => t.Author)
                .WithMany(u => u.Topics)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);

            Role UserRole = new Role()
            {
                Id = Guid.NewGuid(),
                Type = RoleType.User,
                WriteArticles = true,
                WriteComments = true,
                WriteMessages = true,
                CreateTopics = true,
                CreateTags = true,
                OverrideOwnArticle = true
            };
            ExtendedRole EditorRole = new ExtendedRole()
            {
                Id = Guid.NewGuid(),
                Type = RoleType.Editor,
                WriteArticles = true,
                WriteComments = true,
                WriteMessages = true,
                CreateTopics = true,
                CreateTags = true,
                ModerateArticles = true,
                ModerateComments = true,
                ModerateTopics = true,
                ModerateTags = true,
                OverrideMenu = true,
                OverrideOwnArticle = true
            };
            ExtendedRole AdministratorRole = new ExtendedRole()
            {
                Id = Guid.NewGuid(),
                Type = RoleType.Administrator,
                WriteArticles = true,
                WriteComments = true,
                WriteMessages = true,
                CreateTopics = true,
                CreateTags = true,
                ModerateArticles = true,
                ModerateComments = true,
                ModerateTopics = true,
                ModerateTags = true,
                OverrideMenu = true,
                OverrideOwnArticle = true
            };

            modelBuilder.Entity<Role>().HasData(UserRole);
            modelBuilder.Entity<ExtendedRole>().HasData(EditorRole, AdministratorRole);

            User Administrator = new User()
            {
                Id = Guid.NewGuid(),
                Username = "Administrator",
                Email = "test@test.com",
                Hash = "�y|�\u0018�-�d�\a�]?�v#\u0004��\u0006=S,�\\^ר��O",
                RoleId = AdministratorRole.Id,
                DateOfRegistration = DateTimeOffset.UtcNow
            };
            User Editor = new User()
            {
                Id = Guid.NewGuid(),
                Username = "Editor",
                Email = "editor@test.com",
                Hash = "�y|�\u0018�-�d�\a�]?�v#\u0004��\u0006=S,�\\^ר��O",
                RoleId = EditorRole.Id,
                DateOfRegistration = DateTimeOffset.UtcNow
            };
            User User = new User()
            {
                Id = Guid.NewGuid(),
                Username = "User",
                Email = "user@test.com",
                Hash = "�y|�\u0018�-�d�\a�]?�v#\u0004��\u0006=S,�\\^ר��O",
                RoleId = UserRole.Id,
                DateOfRegistration = DateTimeOffset.UtcNow
            };

            modelBuilder.Entity<User>().HasData(Administrator, Editor, User);
        }
    }
}
