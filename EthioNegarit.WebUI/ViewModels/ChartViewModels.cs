using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EthioNegarit.WebUI.ViewModels
{
    public class ChartViewModel
    {
        //Point chart points – data set one
        public string pointchart_ds1_point_J;
        public string pointchart_ds1_point_F;
        public string pointchart_ds1_point_M;
        public string pointchart_ds1_point_A;
        public string pointchart_ds1_point_MY;
        public string pointchart_ds1_point_JU;
        public string pointchart_ds1_point_JY;
        public string pointchart_ds1_point_AG;
        public string pointchart_ds1_point_S;
        public string pointchart_ds1_point_O;
        public string pointchart_ds1_point_N;
        public string pointchart_ds1_point_D;

        public int YMax { get; set; }
        public int YMin { get; set; }
        public int Year { get; set; }
        public IEnumerable<SelectListItem> Years { get; set; }
    }
}