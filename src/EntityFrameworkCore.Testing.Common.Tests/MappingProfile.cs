using AutoMapper;

namespace EntityFrameworkCore.Testing.Common.Tests
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TestEntity, TestViewModel>();

            CreateMap<TestReadOnlyEntity, TestViewModel>();
        }
    }
}