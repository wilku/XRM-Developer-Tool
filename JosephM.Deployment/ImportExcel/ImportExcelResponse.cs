﻿using JosephM.Core.Attributes;
using JosephM.Core.Service;
using JosephM.Deployment.DataImport;
using JosephM.Deployment.SpreadsheetImport;
using System.Collections.Generic;
using System.Linq;

namespace JosephM.Deployment.ImportExcel
{
    [Group(Sections.Summary, false, 0)]
    public class ImportExcelResponse : ServiceResponseBase<ImportExcelResponseItem>
    {
        private List<ImportedRecords> _importedRecords = new List<ImportedRecords>();

        public void LoadSpreadsheetImport(SpreadsheetImportResponse dataImportResponse)
        {
            AddResponseItems(dataImportResponse.ResponseItems.Select(r => new ImportExcelResponseItem(r)));
            _importedRecords.AddRange(dataImportResponse.GetImportSummary());
        }

        [Hidden]
        public bool IsImportSummary
        {
            get { return ImportSummary != null && ImportSummary.Any(); }
        }

        [Group(Sections.Summary)]
        [PropertyInContextByPropertyValue(nameof(IsImportSummary), true)]
        public IEnumerable<ImportedRecords> ImportSummary
        {
            get
            {
                return _importedRecords;
            }
        }

        private static class Sections
        {
            public const string Summary = "Summary";
        }
    }
}