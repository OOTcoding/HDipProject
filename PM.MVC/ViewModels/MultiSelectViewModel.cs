using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PM.MVC.ViewModels
{
    public class MultiSelectViewModel
    {
        public int[] Ids { get; set; }

        public List<SelectListItem> Elements { set; get; }
    }
}