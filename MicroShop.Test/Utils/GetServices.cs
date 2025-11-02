using MicroShop.OrderApi.Rest.Mapper;

namespace MicroShop.Test.Utils
{
    public class GetServices
    {
        public static IMapper GetMapper()
        {

            var mapperConfig = new MapperConfiguration(c =>
            {
                c.AddProfile(new MapperProfile());
            });

            return mapperConfig.CreateMapper();

        }

        public static MicroShopContext GetStoreContext()
        {
            return new MicroShopContext();
        }

    }
}
