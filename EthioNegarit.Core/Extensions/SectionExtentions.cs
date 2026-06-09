using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Web.Mvc;
using EthioNegarit.Core.Domain.Entities;

namespace EthioNegarit.Core.Extensions
{
    public static class SectionExtentions
    {
        public static List<SelectListItem> ToSelectList(this List<Section> sections, bool addEmptyFirstItem = true)
        {
            var selectList = sections.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            }).ToList();

            if (addEmptyFirstItem)
            {
                selectList.Insert(0, new SelectListItem { Value = "Select", Text = "Select" });
            }

            return selectList;
        }
    }
}
