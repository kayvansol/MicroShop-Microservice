using MicroShop.Domain.DTOs.Product;
using MicroShop.Infra.Sql.Repositories.ProductRepo;

namespace MicroShop.Application.UseCases.Product.Queries
{
    public class GetAllProductQueryHandler : IRequestHandler<GetAllProductQuery, ResultDto<List<GetAllProductDto>>>
    {
        private readonly IProductRepository productRepository;
        private readonly IMapper mapper;

        public GetAllProductQueryHandler(IProductRepository productRepository, IMapper mapper)
        {
            this.productRepository = productRepository;
            this.mapper = mapper;
        }

        public async Task<ResultDto<List<GetAllProductDto>>> Handle(GetAllProductQuery request, CancellationToken cancellationToken)
        {
            var result = await productRepository.GetAllProductsAsync();

            return ResultDto<List<GetAllProductDto>>.ReturnData(result, (int)EnumResponseStatus.OK, (int)EnumResultCode.Success, EnumResultCode.Success.GetDisplayName());
        }
    }
}
