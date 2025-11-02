
namespace MicroShop.Core.Validators
{
    public class AddCategoryCommandValidator: AbstractValidator<AddCategoryCommand>
    {
        public AddCategoryCommandValidator()
        {
            RuleFor(x => x.AddDto.CategoryName).NotEmpty().WithMessage("مقدار نام را وارد نمایید");

            RuleFor(x => x.AddDto.CategoryName).Length(4, 10).WithMessage("طول رشته خلاصه بین 4 و 10 می باشد");

        }
    }
}
