﻿using System.Collections.Generic;
using PM.MVC.Models.EF;

namespace PM.MVC.ViewModels
{
    public class ChooseQualificationsViewModel
    {
        public ChooseQualificationsViewModel()
        {
            SuitableQualifications = new List<ChooseQualificationsListItem>();
        }

        public List<ChooseQualificationsListItem> SuitableQualifications { get; set; }
    }

    public class ChooseQualificationsListItem
    {
        public Qualification Qualification { get; set; }

        public bool IsChecked { get; set; }
    }
}