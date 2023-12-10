using Bogus;
using Comic.Api.Code.Enums;
using Comic.API.Domain;

namespace Comic.API.Data;

public class FakeDataGenerator
{
    public static List<Author> GenerateAuthors(int count)
    {
        var faker = new Faker<Author>()
            .RuleFor(a => a.Name, f => f.Name.FullName())
            .RuleFor(a => a.CreatedOn, f => f.Date.Past())
            .RuleFor(a => a.ChangedOn, f => f.Date.Recent());

        return faker.Generate(count);
    }

    public static List<Category> GenerateCategories(int count)
    {
        var faker = new Faker<Category>()
            .RuleFor(c => c.Name, f => f.Commerce.Department())
            .RuleFor(c => c.CreatedOn, f => f.Date.Past())
            .RuleFor(c => c.ChangedOn, f => f.Date.Recent());

        return faker.Generate(count);
    }

    public static List<Chapter> GenerateChapters(int count, List<Domain.Comic> comics)
    {
        var faker = new Faker<Chapter>()
            .RuleFor(ch => ch.Name, f => $"Chapter {f.Random.Number(1, 1000)}")
            .RuleFor(ch => ch.ComicId, f => f.PickRandom(comics).Id)
            .RuleFor(ch => ch.CreatedOn, f => f.Date.Past())
            .RuleFor(ch => ch.ChangedOn, f => f.Date.Recent(30));

        return faker.Generate(count);
    }

    public static List<Domain.Comic> GenerateComics(int count, List<Author> authors, List<Category> categories)
    {
        var faker = new Faker<Domain.Comic>()
            .RuleFor(c => c.Name, f => f.Lorem.Word())
            .RuleFor(c => c.AliasName, f => f.Lorem.Word())
            .RuleFor(c => c.CoverUrl, f => f.Internet.Url())
            .RuleFor(c => c.Description, f => f.Lorem.Paragraph())
            .RuleFor(c => c.Status, f => f.PickRandom<ComicStatus>())
            //.RuleFor(c => c.TotalHearts, f => f.Random.Number(1, 1000))
            .RuleFor(c => c.Rating, f => f.Random.Number(1, 5))
            .RuleFor(c => c.TotalRating, f => f.Random.Number(1, 100))
            //.RuleFor(c => c.Categories, f => f.PickRandom(categories, f.Random.Number(1, 3)).ToList())
            //.RuleFor(c => c.Authors, f => f.PickRandom(authors, f.Random.Number(1, 3)).ToList())
            .RuleFor(c => c.CreatedOn, f => f.Date.Past())
            .RuleFor(c => c.ChangedOn, f => f.Date.Recent(30));

        return faker.Generate(count);
    }

    public static List<Comment> GenerateComments(int count, List<AppUser> users, List<Chapter> chapters)
    {
        var faker = new Faker<Comment>()
            .RuleFor(com => com.Content, f => f.Lorem.Paragraph())
            .RuleFor(com => com.UserId, f => f.PickRandom(users).Id)
            .RuleFor(com => com.ChapterId, f => f.PickRandom(chapters).Id)
            .RuleFor(com => com.CreatedOn, f => f.Date.Past())
            .RuleFor(com => com.ChangedOn, f => f.Date.Recent());

        return faker.Generate(count);
    }

    public static List<DailyComicView> GenerateDailyComicViews(int count, List<Chapter> chapters)
    {
        var faker = new Faker<DailyComicView>()
            .RuleFor(view => view.ViewDate, f => f.Date.Recent(30))
            .RuleFor(view => view.ViewCount, f => f.Random.Number(1, 1000))
            .RuleFor(view => view.ChapterId, f => f.PickRandom(chapters).Id);

        return faker.Generate(count);
    }

    public static List<Image> GenerateImages(int count, List<Chapter> chapters)
    {
        var faker = new Faker<Image>()
            .RuleFor(img => img.Name, f => f.Lorem.Word())
            .RuleFor(img => img.Url, f => f.Image.PicsumUrl())
            .RuleFor(img => img.ChapterId, f => f.PickRandom(chapters).Id)
            .RuleFor(img => img.CreatedOn, f => f.Date.Past())
            .RuleFor(img => img.ChangedOn, f => f.Date.Recent());

        return faker.Generate(count);
    }

    public static List<AppUser> GenerateAppUsers(int count)
    {
        var faker = new Faker<AppUser>()
            .RuleFor(u => u.UserName, f => f.Internet.UserName())
            .RuleFor(u => u.DisplayName, f => f.Name.FullName())
            .RuleFor(u => u.Email, f => f.Internet.Email());

        return faker.Generate(count);
    }

    public static List<UserComicFollower> GenerateUserComicFollowers(int count, List<AppUser> users, List<Domain.Comic> comics)
    {
        var faker = new Faker<UserComicFollower>()
            .RuleFor(f => f.UserId, f => f.PickRandom(users).Id)
            .RuleFor(f => f.ComicId, f => f.PickRandom(comics).Id)
            .RuleFor(f => f.CreatedOn, f => f.Date.Past())
            .RuleFor(f => f.ChangedOn, f => f.Date.Recent());

        return faker.Generate(count);
    }

    public static List<UserRating> GenerateUserRatings(int count, List<AppUser> users, List<Domain.Comic> comics)
    {
        var faker = new Faker<UserRating>()
            .RuleFor(r => r.UserId, f => f.PickRandom(users).Id)
            .RuleFor(r => r.ComicId, f => f.PickRandom(comics).Id)
            .RuleFor(r => r.Rating, f => f.Random.Number(1, 5))
            .RuleFor(r => r.CreatedOn, f => f.Date.Past())
            .RuleFor(r => r.ChangedOn, f => f.Date.Recent());

        return faker.Generate(count);
    }

    public static List<AuthorComic> GenerateAuthorComics(int count, List<Author> authors, List<Domain.Comic> comics)
    {
        var faker = new Faker<AuthorComic>()
            .RuleFor(f => f.AuthorId, f => f.PickRandom(authors).Id)
            .RuleFor(f => f.ComicId, f => f.PickRandom(comics).Id)
            .RuleFor(f => f.CreatedOn, f => f.Date.Past())
            .RuleFor(f => f.ChangedOn, f => f.Date.Recent());

        return faker.Generate(count);
    }

    public static List<CategoryComic> GenerateCategoryComics(int count, List<Category> categories, List<Domain.Comic> comics)
    {
        var faker = new Faker<CategoryComic>()
            .RuleFor(f => f.CategoryId, f => f.PickRandom(categories).Id)
            .RuleFor(f => f.ComicId, f => f.PickRandom(comics).Id)
            .RuleFor(f => f.CreatedOn, f => f.Date.Past())
            .RuleFor(f => f.ChangedOn, f => f.Date.Recent());

        return faker.Generate(count);
    }
}
