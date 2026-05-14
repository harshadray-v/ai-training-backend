using CustomerApi.Core.DTOs;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;

namespace CustomerApi.Tests;

public class DtoValidationTests
{
    private static List<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model);
        Validator.TryValidateObject(model, context, results, validateAllProperties: true);
        return results;
    }

    // ----- CreateCustomerDto -----

    [Fact]
    public void CreateCustomerDto_ValidData_NoErrors()
    {
        var dto = new CreateCustomerDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Phone = "(555) 123-4567",
            Company = "ACME",
            Status = "active"
        };

        var errors = ValidateModel(dto);
        errors.Should().BeEmpty();
    }

    [Fact]
    public void CreateCustomerDto_MissingFirstName_HasError()
    {
        var dto = new CreateCustomerDto
        {
            FirstName = "",
            LastName = "Doe",
            Email = "john@example.com",
            Phone = "(555) 123-4567",
            Company = "ACME",
            Status = "active"
        };

        var errors = ValidateModel(dto);
        errors.Should().Contain(e => e.MemberNames.Contains("FirstName"));
    }

    [Fact]
    public void CreateCustomerDto_FirstNameTooShort_HasError()
    {
        var dto = new CreateCustomerDto
        {
            FirstName = "A",
            LastName = "Doe",
            Email = "john@example.com",
            Phone = "(555) 123-4567",
            Company = "ACME",
            Status = "active"
        };

        var errors = ValidateModel(dto);
        errors.Should().Contain(e => e.MemberNames.Contains("FirstName"));
    }

    [Fact]
    public void CreateCustomerDto_InvalidEmail_HasError()
    {
        var dto = new CreateCustomerDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "not-an-email",
            Phone = "(555) 123-4567",
            Company = "ACME",
            Status = "active"
        };

        var errors = ValidateModel(dto);
        errors.Should().Contain(e => e.MemberNames.Contains("Email"));
    }

    [Fact]
    public void CreateCustomerDto_InvalidStatus_HasError()
    {
        var dto = new CreateCustomerDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Phone = "(555) 123-4567",
            Company = "ACME",
            Status = "unknown"
        };

        var errors = ValidateModel(dto);
        errors.Should().Contain(e => e.MemberNames.Contains("Status"));
    }

    [Fact]
    public void CreateCustomerDto_MissingAllRequired_HasMultipleErrors()
    {
        var dto = new CreateCustomerDto();

        var errors = ValidateModel(dto);
        errors.Should().HaveCountGreaterThanOrEqualTo(2);
    }

    // ----- UpdateCustomerDto -----

    [Fact]
    public void UpdateCustomerDto_AllNull_NoErrors()
    {
        var dto = new UpdateCustomerDto();

        var errors = ValidateModel(dto);
        errors.Should().BeEmpty();
    }

    [Fact]
    public void UpdateCustomerDto_ValidPartialUpdate_NoErrors()
    {
        var dto = new UpdateCustomerDto
        {
            FirstName = "Updated",
            Status = "inactive"
        };

        var errors = ValidateModel(dto);
        errors.Should().BeEmpty();
    }

    [Fact]
    public void UpdateCustomerDto_InvalidEmail_HasError()
    {
        var dto = new UpdateCustomerDto
        {
            Email = "bad-email"
        };

        var errors = ValidateModel(dto);
        errors.Should().Contain(e => e.MemberNames.Contains("Email"));
    }

    [Fact]
    public void UpdateCustomerDto_InvalidStatus_HasError()
    {
        var dto = new UpdateCustomerDto
        {
            Status = "banned"
        };

        var errors = ValidateModel(dto);
        errors.Should().Contain(e => e.MemberNames.Contains("Status"));
    }

    [Fact]
    public void UpdateCustomerDto_FirstNameTooShort_HasError()
    {
        var dto = new UpdateCustomerDto
        {
            FirstName = "A"
        };

        var errors = ValidateModel(dto);
        errors.Should().Contain(e => e.MemberNames.Contains("FirstName"));
    }
}
