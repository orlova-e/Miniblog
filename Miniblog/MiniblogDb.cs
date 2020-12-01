using Microsoft.EntityFrameworkCore;
using Miniblog.Models.Entities;
using Miniblog.Models.Entities.Enums;
using System;

namespace Miniblog
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
        public DbSet<BaseArticlesOptions> BaseArticlesOptions { get; set; }
        public DbSet<ArticleOptions> ArticleOptions { get; set; }

        public MiniblogDb(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
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

            modelBuilder.Entity<Article>()
                .Property(a => a.EntryType)
                .HasConversion(
                v => v.ToString(),
                v => (EntryType)Enum.Parse(typeof(EntryType), v));

            modelBuilder.Entity<Article>()
                .HasOne(a => a.User)
                .WithMany(u => u.Articles)
                .HasForeignKey(a => a.UserId);

            //modelBuilder.Entity<User>()
            //    .HasMany(u => u.Articles)
            //    .WithOne(a => a.User);

            //bookmarks
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

            //likes
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

            //likes
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

            modelBuilder.Entity<WebsiteOptions>()
                .Property(e => e.WebsiteLanguage)
                .HasConversion(
                v => v.ToString(),
                v => (Languages)Enum.Parse(typeof(Languages), v));
            modelBuilder.Entity<WebsiteOptions>()
                .Property(e => e.WebsiteVisibility)
                .HasConversion(
                v => v.ToString(),
                v => (Visibility)Enum.Parse(typeof(Visibility), v));
            modelBuilder.Entity<WebsiteOptions>()
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

            //UserArticlesDisplayOptions
            modelBuilder.Entity<ArticleOptions>()
                .HasOne(u => u.Article)
                .WithOne(ar => ar.DisplayOptions)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ArticleOptions>()
                .Property(e => e.ColorTheme)
                .HasConversion(
                v => v.ToString(),
                v => (ColorTheme)Enum.Parse(typeof(ColorTheme), v));

            modelBuilder.Entity<ListDisplayOptions>()
                .Property(e => e.ListDisplayDefaultType)
                .HasConversion(
                v => v.ToString(),
                v => (DisplayType)Enum.Parse(typeof(DisplayType), v));
            modelBuilder.Entity<ListDisplayOptions>()
                .Property(e => e.LayoutDefaultType)
                .HasConversion(
                v => v.ToString(),
                v => (ListLayoutType)Enum.Parse(typeof(ListLayoutType), v));
            modelBuilder.Entity<ListDisplayOptions>()
                .Property(e => e.ListSortingDefaultType)
                .HasConversion(
                v => v.ToString(),
                v => (ListSorting)Enum.Parse(typeof(ListSorting), v));

            //CommentsDisplayOptions
            modelBuilder.Entity<CommentsOptions>()
                .Property(e => e.SortingCommentsDefaultType)
                .HasConversion(
                v => v.ToString(),
                v => (SortingComments)Enum.Parse(typeof(SortingComments), v));

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
                //ReadComments = true,
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
                //ReadComments = true,
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
                //ReadComments = true,
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
                //Role = AdministratorRole,
                DateOfRegistration = DateTimeOffset.UtcNow
            };
            User Editor = new User()
            {
                Id = Guid.NewGuid(),
                Username = "Editor",
                Email = "editor@test.com",
                Hash = "�y|�\u0018�-�d�\a�]?�v#\u0004��\u0006=S,�\\^ר��O",
                RoleId = EditorRole.Id,
                //Role = EditorRole,
                DateOfRegistration = DateTimeOffset.UtcNow
            };
            User User = new User()
            {
                Id = Guid.NewGuid(),
                Username = "User",
                Email = "user@test.com",
                Hash = "�y|�\u0018�-�d�\a�]?�v#\u0004��\u0006=S,�\\^ר��O",
                RoleId = UserRole.Id,
                //Role = UserRole,
                DateOfRegistration = DateTimeOffset.UtcNow
            };

            modelBuilder.Entity<User>().HasData(Administrator, Editor, User);

            WebsiteOptions websiteDisplayOptions = new WebsiteOptions()
            {
                Id = Guid.NewGuid(),
                Name = "Miniblog",
                Subtitle = string.Empty,
                ShowListOfPopularAndRecent = true,
                ShowAuthors = true,
                ShowSearchOption = true,
                ShowTopics = true,
                ColorTheme = ColorTheme.Blue,
                WebsiteLanguage = Languages.English,
                WebsiteVisibility = Visibility.Public,
                WebsiteDateFormat = "dd.MM.yyyy"
            };

            modelBuilder.Entity<WebsiteOptions>().HasData(websiteDisplayOptions);

            ListDisplayOptions listDisplayOptions = new ListDisplayOptions()
            {
                Id = Guid.NewGuid(),
                Username = true,
                DateAndTime = true,
                Tags = true,
                Topic = true,
                Series = true,
                Likes = true,
                Bookmarks = true,
                Comments = true,
                OverrideForUserArticle = false,
                ArticlesPerPage = 10,
                WordsPerPreview = 50,
                ListDisplayDefaultType = DisplayType.Preview,
                LayoutDefaultType = ListLayoutType.Row,
                ListSortingDefaultType = ListSorting.NewFirst
            };

            modelBuilder.Entity<ListDisplayOptions>().HasData(listDisplayOptions);

            CommentsOptions commentsOptions = new CommentsOptions()
            {
                Id = Guid.NewGuid(),
                AllowNesting = true,
                Depth = 3,
                SortingCommentsDefaultType = SortingComments.NewFirst
            };

            modelBuilder.Entity<CommentsOptions>().HasData(commentsOptions);
        }
    }
}
