using RankingUp.Club.Application.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace RankingUp.Club.Application.Tests.ViewModels;

[TestClass]
public class ClubDetailViewModelTests
{
    [TestMethod]
    public void Valid_model_has_no_validation_errors()
    {
        var model = CreateValidModel();

        var results = Validate(model);

        Assert.HasCount(0, results);
    }

    [TestMethod]
    public void Missing_required_text_fields_returns_each_field_error()
    {
        var model = CreateValidModel();
        model.Name = null!;
        model.AddressNumber = null!;
        model.Address = null!;
        model.State = null!;
        model.Country = null!;
        model.PostalCode = null!;

        var memberNames = Validate(model)
            .SelectMany(result => result.MemberNames)
            .ToArray();

        CollectionAssert.AreEquivalent(
            new[] { "Name", "AddressNumber", "Address", "State", "Country", "PostalCode" },
            memberNames);
    }

    [TestMethod]
    [DataRow(nameof(ClubDetailViewModel.Name), 100)]
    [DataRow(nameof(ClubDetailViewModel.Description), 200)]
    [DataRow(nameof(ClubDetailViewModel.AddressNumber), 10)]
    [DataRow(nameof(ClubDetailViewModel.Address), 100)]
    [DataRow(nameof(ClubDetailViewModel.City), 50)]
    [DataRow(nameof(ClubDetailViewModel.State), 50)]
    [DataRow(nameof(ClubDetailViewModel.Country), 50)]
    [DataRow(nameof(ClubDetailViewModel.PostalCode), 30)]
    [DataRow(nameof(ClubDetailViewModel.Phone), 50)]
    [DataRow(nameof(ClubDetailViewModel.Email), 200)]
    public void Text_field_over_maximum_length_returns_validation_error(string propertyName, int maximumLength)
    {
        var model = CreateValidModel();
        typeof(ClubDetailViewModel).GetProperty(propertyName)!.SetValue(model, new string('x', maximumLength + 1));

        var results = Validate(model);

        Assert.IsTrue(results.Any(result => result.MemberNames.Contains(propertyName)));
    }

    [TestMethod]
    [DataRow(nameof(ClubDetailViewModel.Name), 100)]
    [DataRow(nameof(ClubDetailViewModel.AddressNumber), 10)]
    [DataRow(nameof(ClubDetailViewModel.Address), 100)]
    [DataRow(nameof(ClubDetailViewModel.State), 50)]
    [DataRow(nameof(ClubDetailViewModel.Country), 50)]
    [DataRow(nameof(ClubDetailViewModel.PostalCode), 30)]
    public void Required_text_field_at_maximum_length_is_valid(string propertyName, int maximumLength)
    {
        var model = CreateValidModel();
        typeof(ClubDetailViewModel).GetProperty(propertyName)!.SetValue(model, new string('x', maximumLength));

        var results = Validate(model);

        Assert.IsFalse(results.Any(result => result.MemberNames.Contains(propertyName)));
    }

    private static List<ValidationResult> Validate(ClubDetailViewModel model)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), results, true);
        return results;
    }

    private static ClubDetailViewModel CreateValidModel() => new()
    {
        Name = "RankingUp Club",
        Description = "Club used by the validation tests",
        AddressNumber = "100",
        AddressComplement = "Court A",
        AddressDistrict = "Centro",
        Address = "Rua do Esporte",
        City = "Sao Paulo",
        State = "SP",
        Country = "Brasil",
        PostalCode = "01000-000",
        Phone = "5511999990000",
        BusinessHourStart = new TimeOnly(8, 0),
        BusinessHourEnd = new TimeOnly(22, 0),
        Email = "club@rankingup.local"
    };
}
