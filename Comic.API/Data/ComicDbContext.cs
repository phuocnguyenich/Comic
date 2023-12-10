using Comic.API.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Comic.API.Data;

public class ComicDbContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
{
    public ComicDbContext(DbContextOptions<ComicDbContext> options)
        : base(options)
    {
    }

    public DbSet<Author> Authors { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Chapter> Chapters { get; set; }
    public DbSet<Domain.Comic> Comics { get; set; }
    public DbSet<DailyComicView> DailyComicViews { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<AppUser> AppUsers { get; set; } // Add this DbSet
    public DbSet<UserRating> Ratings { get; set; }
    public DbSet<UserComicFollower> Followers { get; set; }
    public DbSet<AuthorComic> AuthorComics { get; set; }
    public DbSet<CategoryComic> CategoryComics { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure identity-related tables and properties here

        builder.Entity<AppUser>(b =>
        {
            b.ToTable("AspNetUsers");
        });

        builder.Entity<IdentityRole<int>>(b =>
        {
            b.ToTable("AspNetRoles");
        });

        builder.Entity<IdentityUserRole<int>>(b =>
        {
            b.ToTable("AspNetUserRoles");
        });

        builder.Entity<IdentityUserClaim<int>>(b =>
        {
            b.ToTable("AspNetUserClaims");
        });

        builder.Entity<IdentityRoleClaim<int>>(b =>
        {
            b.ToTable("AspNetRoleClaims");
        });

        builder.Entity<IdentityUserLogin<int>>(b =>
        {
            b.ToTable("AspNetUserLogins");
        });

        builder.Entity<IdentityUserToken<int>>(b =>
        {
            b.ToTable("AspNetUserTokens");
        });
    }

    public void GenerateFakeData()
    {
        //var generator = new FakeDataGenerator();

        //var authors = generator.GenerateAuthors(5);
        //var categories = generator.GenerateCategories(10);
        //var comics = generator.GenerateComics(20, authors, categories);
        //var appUsers = generator.GenerateAppUsers(50);
        //var comments = generator.GenerateComments(100, appUsers, comics, comics.SelectMany(c => c.Chapters).ToList());
        //var dailyComicViews = generator.GenerateDailyComicViews(50, comics);

        var authors = FakeDataGenerator.GenerateAuthors(5);
        var categories = FakeDataGenerator.GenerateCategories(3);
        var appUsers = FakeDataGenerator.GenerateAppUsers(10);
        var comics = FakeDataGenerator.GenerateComics(10, authors, categories);


        // Add generated data to the DbContext
        AppUsers.AddRange(appUsers);
        Authors.AddRange(authors);
        Categories.AddRange(categories);
        Comics.AddRange(comics);
        SaveChanges();

        var chapters = FakeDataGenerator.GenerateChapters(20, comics);
        Chapters.AddRange(chapters);

        SaveChanges();

        var comments = FakeDataGenerator.GenerateComments(30, appUsers, chapters);
        var dailyComicViews = FakeDataGenerator.GenerateDailyComicViews(50, chapters);
        var images = FakeDataGenerator.GenerateImages(100, chapters);
        var userComicFollowers = FakeDataGenerator.GenerateUserComicFollowers(20, appUsers, comics);
        var authorComics = FakeDataGenerator.GenerateAuthorComics(20, authors, comics);
        var categoryComics = FakeDataGenerator.GenerateCategoryComics(20, categories, comics);
        var userRatings = FakeDataGenerator.GenerateUserRatings(50, appUsers, comics);

        DailyComicViews.AddRange(dailyComicViews);
        Comments.AddRange(comments);
        Images.AddRange(images);
        Followers.AddRange(userComicFollowers);
        Ratings.AddRange(userRatings);
        AuthorComics.AddRange(authorComics);
        CategoryComics.AddRange(categoryComics);

        SaveChanges();
    }

    public void ClearAllData()
    {
        // Remove all data from each DbSet
        AppUsers.RemoveRange(AppUsers);
        Authors.RemoveRange(Authors);
        Categories.RemoveRange(Categories);
        Comics.RemoveRange(Comics);
        Chapters.RemoveRange(Chapters);
        DailyComicViews.RemoveRange(DailyComicViews);
        Comments.RemoveRange(Comments);
        Images.RemoveRange(Images);
        Followers.RemoveRange(Followers);
        Ratings.RemoveRange(Ratings);
        AuthorComics.RemoveRange(AuthorComics);
        CategoryComics.RemoveRange(CategoryComics);

        // Save changes to apply the deletions
        SaveChanges();
    }
}