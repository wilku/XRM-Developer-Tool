﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using JosephM.Application.ViewModel.RecordEntry;
using JosephM.Application.ViewModel.RecordEntry.Form;
using JosephM.Prism.XrmModule.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using JosephM.Core.FieldType;
using JosephM.Core.Utility;
using JosephM.Record.Xrm.Test;
using JosephM.Record.Xrm.XrmRecord;
using JosephM.Xrm.ImportExporter.Service;

namespace JosephM.Xrm.ImporterExporter.Test
{
    [TestClass]
    public class DeploymentSolutionImporterExporterRequestTest : XrmModuleTest
    {
        [TestMethod]
        public void DeploymentImporterExporterRequestForSolutionExport()
        {
            PrepareTests();

            Assert.IsNotNull(TestAccount);

            var req = new XrmSolutionImporterExporterRequest();
            req.FolderPath = new Folder(TestingFolder);
            req.ImportExportTask = SolutionImportExportTask.ExportSolutions;

            var mainViewModel = new ObjectEntryViewModel(() => { }, () => { }, req, FormController.CreateForObject(req, CreateFakeApplicationController(), XrmRecordService));
            var solutionGrid = mainViewModel.SubGrids.First(r => r.ReferenceName == "SolutionExports");
            solutionGrid.AddRow();

            var row = solutionGrid.GridRecords.First();
            var connectionField = row.GetObjectFieldFieldViewModel("Connection");
            connectionField.Search();
            Assert.IsTrue(connectionField.LookupGridViewModel.DynamicGridViewModel.GridRecords.Any());
            connectionField.SetValue(connectionField.LookupGridViewModel.DynamicGridViewModel.GridRecords.First().GetRecord());

            var solutionField = row.GetLookupFieldFieldViewModel("Solution");
            solutionField.Search();
            Assert.IsTrue(solutionField.LookupGridViewModel.DynamicGridViewModel.GridRecords.Any());
            solutionField.SetValue(solutionField.LookupGridViewModel.DynamicGridViewModel.GridRecords.First().GetRecord());
        }

        [TestMethod]
        public void DeploymentImporterExporterLookupConnectionTest()
        {
            PrepareTests();

            Assert.IsNotNull(TestAccount);

            var req = new XrmSolutionImporterExporterRequest();
            req.FolderPath = new Folder(TestingFolder);
            req.ImportExportTask = SolutionImportExportTask.ExportSolutions;

            var mainViewModel = CreateObjectEntryViewModel(req, CreateFakeApplicationController());
            var solutionGrid = mainViewModel.SubGrids.First(r => r.ReferenceName == "SolutionExports");
            solutionGrid.AddRow();

            var row = solutionGrid.GridRecords.First();
            var connectionField = row.GetObjectFieldFieldViewModel("Connection");
            connectionField.Search();
            Assert.IsTrue(connectionField.LookupGridViewModel.DynamicGridViewModel.GridRecords.Any());
            connectionField.SetValue(connectionField.LookupGridViewModel.DynamicGridViewModel.GridRecords.First().GetRecord());

            var solutionField = row.GetLookupFieldFieldViewModel("Solution");
            solutionField.Search();

            var editViewModel = solutionGrid.GetEditRowViewModel(row);

            var recordsToExportGrid = editViewModel.GetSubGridViewModel("DataToExport");

            recordsToExportGrid.AddRow();
            var recordToExport = recordsToExportGrid.GridRecords.First();

            var type = recordToExport.GetRecordTypeFieldViewModel("RecordType");
            type.Value = type.ItemsSource.First();

            var editType = recordsToExportGrid.GetEditRowViewModel(recordToExport);

            var specificRecordGrid = editType.GetSubGridViewModel("OnlyExportSpecificRecords");
            specificRecordGrid.AddRow();

            var specificRecord = specificRecordGrid.GridRecords.First();

            var lookupField = specificRecord.GetLookupFieldFieldViewModel("Record");
            lookupField.Search();

        }
    }
}