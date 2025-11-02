using MicroShop.Domain.DTOs.Category;

namespace MicroShop.Test.Data.Category
{
    public class CategoryTestData
    {

        public static IEnumerable<object[]> addCategoryCommandDto()
        {
            List<AddCategoryCommandDto[]> data = new List<AddCategoryCommandDto[]>();

            AddCategoryCommandDto[] adds = new AddCategoryCommandDto[1];

            adds[0] = new AddCategoryCommandDto()
            {
                CategoryName = "BLU"
            };

            data.Add(adds);

            return data;
                        
        }
        
    }
}
