using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Miniblog.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Miniblog.Models.Entities.Enums;

namespace Miniblog
{
    public class MiniblogDb : DbContext
    {
        public DbSet<Role> Roles { get; set; }
        public DbSet<ExtendedRole> ExtendedRoles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        //public DbSet<Opportunities> Opportunities { get; set; }
        //public DbSet<JobOpportunities> JobOpportunities { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Image> Images { get; set; }
        //public DbSet<Series> Series { get; set; }
        //public DbSet<Tag> Tags { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<WebsiteDisplayOptions> WebsiteDisplayOptions { get; set; }
        public DbSet<ArticlesDisplayOptions> ArticlesDisplayOptions { get; set; }
        public DbSet<UserArticleDisplayOptions> UserArticleDisplayOptions { get; set; }
        public DbSet<ArticlesListDisplayOptions> ArticlesListDisplayOptions { get; set; }
        public DbSet<CommentsDisplayOptions> CommentsDisplayOptions { get; set; }

        public MiniblogDb(DbContextOptions options) : base(options)
        {
            //Database.EnsureCreated();
        }
        //public MiniblogDb()
        //{
        //    //Database.EnsureCreated();
        //}
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb; Database=Miniblogdb; Trusted_Connection=True;");
        //}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>()
                .Property(e => e.Type)
                .HasConversion(
                v => v.ToString(),
                v => (RoleType)Enum.Parse(typeof(RoleType), v));

            modelBuilder.Entity<WebsiteDisplayOptions>()
                .Property(e => e.WebsiteLanguage)
                .HasConversion(
                v => v.ToString(),
                v => (Languages)Enum.Parse(typeof(Languages), v));
            modelBuilder.Entity<WebsiteDisplayOptions>()
                .Property(e => e.WebsiteVisibility)
                .HasConversion(
                v => v.ToString(),
                v => (Visibility)Enum.Parse(typeof(Visibility), v));
            modelBuilder.Entity<WebsiteDisplayOptions>()
                .Property(e => e.ColorTheme)
                .HasConversion(
                v => v.ToString(),
                v => (ColorTheme)Enum.Parse(typeof(ColorTheme), v));

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Author)
                .WithMany(us => us.Comments)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Image>()
                .HasOne(i => i.Article)
                .WithMany(ar => ar.Images)
                .OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<ArticleTag>()
            //    .HasKey(t => new { t.ArticleId, t.TagId });
            //modelBuilder.Entity<ArticleTag>()
            //    .HasOne(at => at.Article)
            //    .WithMany(a => a.ArticleTags)
            //    .HasForeignKey(at => at.ArticleId)
            //    .OnDelete(DeleteBehavior.Restrict);
            //modelBuilder.Entity<ArticleTag>()
            //    .HasOne(at => at.Tag)
            //    .WithMany(t => t.ArticleTags)
            //    .HasForeignKey(at => at.TagId)
            //    .OnDelete(DeleteBehavior.Restrict);


            //UserArticlesDisplayOptions
            modelBuilder.Entity<UserArticleDisplayOptions>()
                .HasOne(u => u.Article)
                .WithOne(ar => ar.UserArticleDisplayOptions)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserArticleDisplayOptions>()
                .Property(e => e.ColorTheme)
                .HasConversion(
                v => v.ToString(),
                v => (ColorTheme)Enum.Parse(typeof(ColorTheme), v));

            //ArticlesListDisplayOptions
            //modelBuilder.Entity<ArticlesListDisplayOptions>()
            //    .HasOne(a => a.WebsiteDisplayOptions)
            //    .WithOne(w => w.ArticlesListDisplayOptions)
            //    //.HasForeignKey(f=>f.)
            //    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ArticlesListDisplayOptions>()
                .Property(e => e.ListDisplayDefaultType)
                .HasConversion(
                v => v.ToString(),
                v => (DisplayType)Enum.Parse(typeof(DisplayType), v));
            modelBuilder.Entity<ArticlesListDisplayOptions>()
                .Property(e => e.LayoutDefaultType)
                .HasConversion(
                v => v.ToString(),
                v => (ListLayoutType)Enum.Parse(typeof(ListLayoutType), v));
            modelBuilder.Entity<ArticlesListDisplayOptions>()
                .Property(e => e.ArticlesListSortingDefaultType)
                .HasConversion(
                v => v.ToString(),
                v => (ListSortingType)Enum.Parse(typeof(ListSortingType), v));

            //CommentsDisplayOptions
            modelBuilder.Entity<CommentsDisplayOptions>()
                .Property(e => e.SortingCommentsDefaultType)
                .HasConversion(
                v => v.ToString(),
                v => (SortingComments)Enum.Parse(typeof(SortingComments), v));

            modelBuilder.Entity<Topic>()
                .HasOne(t => t.Author)
                .WithMany(u => u.Topics)
                .OnDelete(DeleteBehavior.SetNull);

            Role User = new Role()
            {
                Id = Guid.NewGuid(),
                Type = RoleType.User,
                WriteArticles = true,
                WriteComments = true,
                WriteMessages = true,
                ReadComments = true,
                CreateTopics = true,
                CreateTags = true
            };
            ExtendedRole Editor = new ExtendedRole()
            {
                Id = Guid.NewGuid(),
                Type = RoleType.Editor,
                WriteArticles = true,
                WriteComments = true,
                WriteMessages = true,
                ReadComments = true,
                CreateTopics = true,
                CreateTags = true,
                ModerateArticles = true,
                ModerateComments = true,
                ModerateTopics = true,
                ModerateTags = true
            };
            ExtendedRole Administrator = new ExtendedRole()
            {
                Id = Guid.NewGuid(),
                Type = RoleType.Administrator,
                WriteArticles = true,
                WriteComments = true,
                WriteMessages = true,
                ReadComments = true,
                CreateTopics = true,
                CreateTags = true,
                ModerateArticles = true,
                ModerateComments = true,
                ModerateTopics = true,
                ModerateTags = true
            };

            modelBuilder.Entity<Role>().HasData(User);
            modelBuilder.Entity<ExtendedRole>().HasData(Editor, Administrator);
        }
    }
}
