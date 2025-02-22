using BLAExercise.Data.Models;
using Bogus;

namespace BLAExercise.Tests.Helpers;

public static class CustomFaker
{
    public static User User => Users.Generate();

    public static Faker<User> Users => new Faker<User>()
        .RuleFor(u => u.Id, f => f.Random.Int(1, int.MaxValue))
        .RuleFor(u => u.Password, f => f.Random.AlphaNumeric(12))
        .RuleFor(u => u.CreationDate, f => f.Date.Past().ToUniversalTime())
        .RuleFor(u => u.Email, f => f.Internet.Email());

    public static Faker<Sneaker> Sneakers => new Faker<Sneaker>()
        .RuleFor(s => s.Id, f => f.Random.Int(1, int.MaxValue))
        .RuleFor(s => s.UserId, f => User.Id)
        .RuleFor(s => s.Brand, f => f.Random.Words())
        .RuleFor(s => s.Name, f => f.Random.Words())
        .RuleFor(s => s.Year, f => f.Random.Int(1, 9999))
        .RuleFor(u => u.CreationDate, f => f.Date.Past().ToUniversalTime())
        .RuleFor(u => u.Rate, f => f.Random.Int(1, 5))
        .RuleFor(u => u.SizeUS, f => f.Random.Float())
        .RuleFor(u => u.Price, f => f.Random.Decimal());
}
