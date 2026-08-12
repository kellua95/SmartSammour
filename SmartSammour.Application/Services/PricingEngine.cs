using SmartSammour.Core.Entities;

namespace SmartSammour.Application.Services
{
    public class PricingEngine
    {
        public decimal Calculate(Service service, Plan plan, List<AddOn> selectedAddOns)
        {
            var TotalPrice = service.BasePrice + plan.ExtraFee;
            TotalPrice += selectedAddOns.Sum(addOn => addOn.ExtraPrice);
            return TotalPrice;
        }
    }
}
