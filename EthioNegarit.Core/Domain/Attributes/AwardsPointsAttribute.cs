using System;


namespace EthioNegarit.Core.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AwardsPointsAttribute : Attribute
    {
        public int Points { get; set; }

        public AwardsPointsAttribute(int points)
        {
            Points = points;
        }
    }
}

