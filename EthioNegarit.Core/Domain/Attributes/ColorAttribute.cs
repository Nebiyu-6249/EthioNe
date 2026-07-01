using System;
namespace EthioNegarit.Core.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
   public class ColorAttribute : Attribute
    {
        public string Color { get; set; }

        public ColorAttribute(string color)
        {
            Color = color;
        }
    }
}
