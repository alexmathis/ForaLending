using ForaLending.API.Models;

namespace ForaLending.API.CalculationService
{
    public interface ICalculationService
    {
        decimal CalculateStandardFundableAmount(Company company);
        decimal CalculateSpecialFundableAmount(Company company);
    }
}
