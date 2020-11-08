using System.Collections.Generic;
using PM.MVC.Models.EF;

namespace PM.MVC.ViewModels
{
    public class SuitableProjectResourceViewModel
    {
        public List<SuitableProjectResourceListItem> SuitableProjectResourceList { get; set; }
    }

    public class SuitableProjectResourceListItem
    {
        public IdentityResource IdentityResource { get; set; }

        public bool IsChecked { get; set; }
    }
}