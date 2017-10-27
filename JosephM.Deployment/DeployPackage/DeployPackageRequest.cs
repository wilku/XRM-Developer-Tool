﻿#region

using JosephM.Core.Attributes;
using JosephM.Core.Constants;
using JosephM.Core.FieldType;
using JosephM.Core.Service;
using JosephM.Core.Utility;
using JosephM.Deployment.ExportXml;
using JosephM.Prism.XrmModule.SavedXrmConnections;
using JosephM.Record.Attributes;
using JosephM.Xrm.Schema;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#endregion

namespace JosephM.Deployment.DeployPackage
{
    [AllowSaveAndLoad]
    [Group(Sections.Main, true, 10)]
    [Group(Sections.Connection, true, 20)]
    public class DeployPackageRequest : ServiceRequestBase
    {
        public static DeployPackageRequest CreateForDeployPackage(string folder)
        {
            return new DeployPackageRequest()
            {
                FolderContainingPackage = new Folder(folder),
                HideTypeAndFolder = true
            };
        }

        [Group(Sections.Connection)]
        [DisplayOrder(100)]
        [DisplayName("Saved Connection To Import Into")]
        [RequiredProperty]
        [SettingsLookup(typeof(ISavedXrmConnections), nameof(ISavedXrmConnections.Connections))]
        public SavedXrmRecordConfiguration Connection { get; set; }


        [Group(Sections.Main)]
        [DisplayOrder(20)]
        [RequiredProperty]
        [PropertyInContextByPropertyValue(nameof(HideTypeAndFolder), false)]
        public Folder FolderContainingPackage { get; set; }

        [Hidden]
        public bool HideTypeAndFolder { get; set; }

        private static class Sections
        {
            public const string Main = "Main";
            public const string Connection = "Connection";
        }
    }
}