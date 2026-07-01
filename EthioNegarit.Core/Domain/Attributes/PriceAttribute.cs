using System;
namespace EthioNegarit.Core.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
  public  class PriceAttribute: Attribute
    {
        public double PriceRate { get; set; }

        public PriceAttribute(double priceRate)
        {
            PriceRate = priceRate;
        }
    }
}
