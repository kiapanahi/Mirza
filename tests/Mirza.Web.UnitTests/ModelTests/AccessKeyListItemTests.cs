using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Mirza.Web.Dto;
using Mirza.Web.Models;
using Xunit;

namespace Mirza.Web.UnitTests.ModelTests
{
    public class AccessKeyListItemTests
    {
        [Fact]
        public void Converts_Domain_Model_To_Dto()
        {
            var accessKey = new AccessKey("01234567890123456789012345678901")
            {
                Id = 1,
                OwnerId = 5,
                State = AccessKeyState.Active,
                Expiration = DateTime.Parse("2020-04-10")
            };

            var dto = new AccessKeyListItem(accessKey);

            Assert.NotNull(dto);
            Assert.Equal("1399/01/22", dto.ExpirationPersian);
            Assert.Equal("012***************************01", dto.Key);
            Assert.Equal(1, dto.Id);
            Assert.Equal(AccessKeyState.Active, dto.State);
        }
    }
}
