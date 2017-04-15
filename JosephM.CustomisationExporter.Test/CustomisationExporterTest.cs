﻿using System.Linq;
using System.Threading;
using JosephM.Application.ViewModel.SettingTypes;
using JosephM.Core.FieldType;
using JosephM.Core.Utility;
using JosephM.CustomisationExporter.Exporter;
using JosephM.Prism.XrmModule.Test;
using JosephM.Xrm.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JosephM.CustomisationExporter.Test
{
    [TestClass]
    public class CustomisationExporterTest : XrmModuleTest
    {
        [TestMethod]
        public void CustomisationExporterTestExport()
        {
            FileUtility.DeleteFiles(TestingFolder);
            Assert.IsFalse(FileUtility.GetFiles(TestingFolder).Any());

            //okay script through generation of the three types

            //create test application with module loaded
            var testApplication = CreateAndLoadTestApplication<CustomisationExporterModule>();

            //first script generation of C# entities and fields
            var request = new CustomisationExporterRequest();
            request.IncludeAllRecordTypes = true;
            request.DuplicateManyToManyRelationshipSides = true;
            request.Entities = true;
            request.Fields = true;
            request.FieldOptionSets = true;
            request.Relationships = true;
            request.SharedOptionSets = true;
            request.IncludeOneToManyRelationships = true;
            request.SaveToFolder = new Folder(TestingFolder);

            testApplication.NavigateAndProcessDialog<CustomisationExporterModule, CustomisationExporterDialog>(request);
            Assert.IsTrue(FileUtility.GetFiles(TestingFolder).Any());

            request.IncludeAllRecordTypes = true;
            request.DuplicateManyToManyRelationshipSides = false;
            request.Entities = true;
            request.Fields = false;
            request.FieldOptionSets = false;
            request.Relationships = true;
            request.SharedOptionSets = false;
            request.IncludeOneToManyRelationships = false;

            Thread.Sleep(1000);
            FileUtility.DeleteFiles(TestingFolder);
            
            testApplication.NavigateAndProcessDialog<CustomisationExporterModule, CustomisationExporterDialog>(request);
            Assert.IsTrue(FileUtility.GetFiles(TestingFolder).Any());

            request.IncludeAllRecordTypes = false;
            request.DuplicateManyToManyRelationshipSides = true;
            request.Entities = true;
            request.Fields = true;
            request.FieldOptionSets = true;
            request.Relationships = true;
            request.SharedOptionSets = true;
            request.IncludeOneToManyRelationships = true;
            request.RecordTypes = new[]
            {
                new RecordTypeSetting(Entities.account, Entities.account),
                new RecordTypeSetting(Entities.contact, Entities.contact)
            };

            Thread.Sleep(1000);
            FileUtility.DeleteFiles(TestingFolder);
        }
    }
}