using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthioNegarit.Core.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
   public class FilesAttribute : Attribute
    {
        public int Files { get; set; }

        public FilesAttribute(int files)
        {
            Files = files;
        }
    }
}
