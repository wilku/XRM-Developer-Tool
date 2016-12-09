﻿using JosephM.Core.Service;

namespace JosephM.CustomisationImporter.Service
{
    public class CustomisationImportResponseItem : ServiceResponseItem
    {
        public int? ExcelRow { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public bool Updated { get; set; }
    }
}