using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using KellermanSoftware.CompareNetObjects;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Common.Tests
{
    public class AutoMapperTests : BaseForTests
    {
        private class DataEntity
        {
            public Guid Id { get; set; }

            public string Code { get; set; }
        }

        private class BusinessEntity
        {
            public Guid id { get; set; }

            public string code { get; set; }
        }

        private class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<DataEntity, BusinessEntity>().ForMember(d => d.id, o => o.MapFrom(s => s.Id)).ForMember(d => d.code, o => o.MapFrom(s => s.Code)).ReverseMap();
            }
        }

        [Test]
        public async Task ProjectToThenToListAsync_DataEntities_ReturnsExpectedResult()
        {
            var dataEntites = new AsyncEnumerable<DataEntity>(Fixture.CreateMany<DataEntity>());
            var expectedResult = new AsyncEnumerable<BusinessEntity>(((IEnumerable<DataEntity>) dataEntites).Select(x => new BusinessEntity { id = x.Id, code = x.Code }));

            var mapper = new Mapper(new MapperConfiguration(x => x.AddProfile(new MappingProfile())));

            var actualResult = await mapper.ProjectTo<BusinessEntity>(dataEntites, null).ToListAsync();

            var compareLogic = new CompareLogic { Config = { IgnoreObjectTypes = true, IgnoreCollectionOrder = true } };
            var comparisonResult = compareLogic.Compare(expectedResult, actualResult);
            Console.WriteLine(comparisonResult.Differences.ToList());
        }
    }
}