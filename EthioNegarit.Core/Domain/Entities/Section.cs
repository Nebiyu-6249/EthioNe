using System;
using System.Collections.Generic;
using EthioNegarit.Utilities;

namespace EthioNegarit.Core.Domain.Entities
{

    /// <summary>
    ///     Section class is just a way to segment the categories in the main home page
    /// </summary>
    public partial class Section: Entity
    {

       public Section()
        {
            Id = GuidComb.GenerateComb();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int SortOrder { get; set; }
        public DateTime DateCreated { get; set; }

        public virtual IList<Category> Categories { get; set; }
        
    }
}
