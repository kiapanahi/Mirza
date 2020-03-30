using System.Linq;
using Mirza.Web.Models;
using Mirza.Web.Validators;
using Xunit;

namespace Mirza.Web.UnitTests.ModelTests
{
    public class UserTests
    {
        private readonly UserValidator _validator;
        public UserTests()
        {
            _validator = new UserValidator();
        }

        [Theory]
        [InlineData("", false)]
        [InlineData(null, false)]
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", false)]
        [InlineData("sample first name", true)]
        public void ValidFirstName(string firstName, bool success)
        {
            var u = new User
            {
                FirstName = firstName
            };

            var validationResult = _validator.Validate(u);
            var expectedErrorExists = validationResult.Errors.Any(e => e.PropertyName == nameof(User.FirstName));

            Assert.Equal(!success, expectedErrorExists);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData(null, false)]
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", false)]
        [InlineData("sample last name", true)]
        public void ValidLastName(string lastName, bool success)
        {
            var u = new User
            {
                LastName = lastName
            };

            var validationResult = _validator.Validate(u);
            var expectedErrorExists = validationResult.Errors.Any(e => e.PropertyName == nameof(User.LastName));

            Assert.Equal(!success, expectedErrorExists);
        }
    }
}
