﻿using JosephM.Application.Desktop.Module.Settings;
using JosephM.Application.ViewModel.Dialog;
using JosephM.Record.Xrm.XrmRecord;
using JosephM.Xrm.Vsix.Application;
using JosephM.Xrm.Vsix.Module.Connection;
using JosephM.XrmModule.SavedXrmConnections;
using JosephM.XrmModule.XrmConnection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JosephM.Xrm.Vsix.Module.PackageSettings
{
    public class XrmPackageSettingsDialog : AppSettingsDialog<XrmPackageSettings, XrmPackageSettings>
    {
        public IVisualStudioService VisualStudioService { get; set; }

        private bool _saveSettings = true;
        public bool SaveSettings
        {
            get
            {
                return _saveSettings;
            }
            set
            {
                _saveSettings = value;
                //if this is being set we need to ensure the connection has its equivalent
                if(SubDialogs != null && SubDialogs.First() is ConnectionEntryDialog)
                {
                    ((ConnectionEntryDialog)SubDialogs.First()).AddToSolution = SaveSettings;
                }
            }
        }

        private XrmRecordService XrmRecordService { get; set; }

        public XrmPackageSettingsDialog(IDialogController dialogController, XrmPackageSettings objectToEnter, IVisualStudioService visualStudioService, XrmRecordService xrmRecordService)
        : this(dialogController, objectToEnter, visualStudioService, xrmRecordService, saveButtonLabel: null)
        {
        }

        /// <summary>
        /// this one internal so the navigation resolver doesnt use it
        /// </summary>
        internal XrmPackageSettingsDialog(IDialogController dialogController, XrmPackageSettings objectToEnter, IVisualStudioService visualStudioService, XrmRecordService xrmRecordService, string saveButtonLabel)
            : base(dialogController, xrmRecordService, objectToEnter, saveButtonLabel: saveButtonLabel)
        {
            XrmRecordService = xrmRecordService;
            VisualStudioService = visualStudioService;

            AddRedirectToConnectionEntryIfNotConnected(visualStudioService);
        }

        /// <summary>
        /// this one internal so the navigation resolver doesnt use it
        /// </summary>
        internal XrmPackageSettingsDialog(DialogViewModel parentDialog, XrmPackageSettings objectToEnter, IVisualStudioService visualStudioService, XrmRecordService xrmRecordService)
            : base(parentDialog, xrmRecordService, objectToEnter)
        {
            XrmRecordService = xrmRecordService;
            VisualStudioService = visualStudioService;

            AddRedirectToConnectionEntryIfNotConnected(visualStudioService);
        }

        private bool RefreshActiveServiceConnection { get; set; }

        private void AddRedirectToConnectionEntryIfNotConnected(IVisualStudioService visualStudioService)
        {
            if (string.IsNullOrWhiteSpace(XrmRecordService.XrmRecordConfiguration.OrganizationUniqueName))
            {
                //if there was no connection then lets redirect to the connection entry first
                var newConnection = new SavedXrmRecordConfiguration();
                Action refreshChildDialogConnection = () =>
                {
                    newConnection.Active = true;
                    XrmRecordService.XrmRecordConfiguration = newConnection;
                    SettingsObject.Connections = new[] { newConnection };
                };
                RefreshActiveServiceConnection = true;
                var connectionEntryDialog = new ConnectionEntryDialog(this, newConnection, visualStudioService, true, doPostEntry: refreshChildDialogConnection);
                var subDialogList = new List<DialogViewModel>();
                subDialogList.Add(connectionEntryDialog);
                XrmRecordService.XrmRecordConfiguration = newConnection;
                subDialogList.AddRange(SubDialogs);
                SubDialogs = subDialogList;
            }
        }

        protected override void LoadDialogExtention()
        {
            //if we do not yet have connections added to the package settings
            //then lets add the active connection to the settings as the active connection
            if (SettingsObject.Connections == null
                || !SettingsObject.Connections.Any())
            {
                try
                {
                    var savedConnection = SavedXrmRecordConfiguration.CreateNew(XrmRecordService.XrmRecordConfiguration);
                    savedConnection.Active = true;
                    SettingsObject.Connections = new[] { savedConnection };
                }
                catch (Exception)
                {
                    //No matter if this happens user may just add it in anyway
                }
            }
            StartNextAction();
        }

        protected override void CompleteDialogExtention()
        {
            if (SaveSettings)
            {
                base.CompleteDialogExtention();
                //set the active connection to the connection selected as active
                if (SettingsObject.Connections != null)
                {
                    var activeConnections = SettingsObject.Connections.Where(c => c.Active);
                    if (activeConnections.Any())
                    {
                        var activeConnection = activeConnections.First();
                        VisualStudioService.AddSolutionItem("solution.xrmconnection", activeConnection);

                        XrmConnectionModule.RefreshXrmServices(activeConnection, ApplicationController, xrmRecordService: (RefreshActiveServiceConnection ? XrmRecordService : null));
                    }
                }
            }
            CompletionMessage = "Settings Updated";
        }
    }
}