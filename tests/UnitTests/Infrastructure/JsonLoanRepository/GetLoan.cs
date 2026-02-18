using System.Collections.Generic;
using System.IO;
using Library.ApplicationCore;
using Library.ApplicationCore.Entities;
using Library.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace Library.UnitTests.Infrastructure;

public class GetLoanTest
{
	private readonly ILoanRepository _mockLoanRepository;
	private readonly JsonLoanRepository _jsonLoanRepository;
	private readonly IConfiguration _configuration;
	private readonly JsonData _jsonData;

	public GetLoanTest()
	{
		_mockLoanRepository = Substitute.For<ILoanRepository>();

		var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
		var jsonRoot = Path.Combine(repoRoot, "src", "Library.Console", "Json");

		_configuration = new ConfigurationBuilder()
			.AddInMemoryCollection(new Dictionary<string, string?>
			{
				["JsonPaths:Authors"] = Path.Combine(jsonRoot, "Authors.json"),
				["JsonPaths:Books"] = Path.Combine(jsonRoot, "Books.json"),
				["JsonPaths:BookItems"] = Path.Combine(jsonRoot, "BookItems.json"),
				["JsonPaths:Patrons"] = Path.Combine(jsonRoot, "Patrons.json"),
				["JsonPaths:Loans"] = Path.Combine(jsonRoot, "Loans.json"),
			})
			.Build();
		_jsonData = new JsonData(_configuration);
		_jsonLoanRepository = new JsonLoanRepository(_jsonData);
	}

	[Fact(DisplayName = "JsonLoanRepository.GetLoan: Returns loan when ID is found")]
	public async Task GetLoan_ReturnsLoanWhenFound()
	{
		// Arrange
		const int loanId = 1;
		var expectedLoan = new Loan { Id = loanId };
		_mockLoanRepository.GetLoan(loanId).Returns(expectedLoan);

		// Act
		var expected = await _mockLoanRepository.GetLoan(loanId);
		var actual = await _jsonLoanRepository.GetLoan(loanId);

		// Assert
		Assert.NotNull(actual);
		Assert.Equal(expected!.Id, actual!.Id);
	}	
	
     [Fact(DisplayName = "JsonLoanRepository.GetLoan: Returns null when ID is not found")]
     public async Task GetLoan_ReturnsNullWhenIdIsNotFound()
     {
         // Arrange
         var loanId = 999; // Loan ID that does not exist in Loans.json

         _mockLoanRepository.GetLoan(loanId).Returns((Loan?)null);

         // Act
         var actualLoan = await _jsonLoanRepository.GetLoan(loanId);

         // Assert
         Assert.Null(actualLoan);
     }

}
