using EthioNegarit.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthioNegarit.Core.Events
{
   public class FavouriteEventArgs : EthioNegaritEventArgs
    {
        public Favourite Favourite { get; set; }
    }
}
