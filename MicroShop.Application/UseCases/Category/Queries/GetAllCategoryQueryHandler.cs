using MicroShop.Domain.DTOs.Category;
using MicroShop.Infra.Sql.Repositories.CategoryRepo;

namespace MicroShop.Application.UseCases.Category.Queries
{
    public class GetAllCategoryQueryHandler : IRequestHandler<GetAllCategoryQuery, ResultDto<List<GetAllCategoryDto>>>
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly IMapper mapper;

        public GetAllCategoryQueryHandler(ICategoryRepository categoryRepository,IMapper mapper)
        {
            this.categoryRepository = categoryRepository;
            this.mapper = mapper;
        }

        public async Task<ResultDto<List<GetAllCategoryDto>>> Handle(GetAllCategoryQuery request, CancellationToken cancellationToken)
        {
            var result = await categoryRepository.GetAllCategoriesAsync();

            return ResultDto<List<GetAllCategoryDto>>.ReturnData(result, (int)EnumResponseStatus.OK, (int)EnumResultCode.Success, EnumResultCode.Success.GetDisplayName());
        }
    }
}
