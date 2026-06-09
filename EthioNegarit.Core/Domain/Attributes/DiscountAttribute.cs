using System;

namespace EthioNegarit.Core.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DiscountAttribute : Attribute
    {
        public int Discount { get; set; }

        public DiscountAttribute(int discount)
        {
            Discount = discount;
        }
    }
}
