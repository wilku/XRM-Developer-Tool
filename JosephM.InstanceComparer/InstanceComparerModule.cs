﻿using JosephM.Application.Desktop.Module.ServiceRequest;
using JosephM.Application.Modules;
using JosephM.Application.ViewModel.Dialog;
using JosephM.Application.ViewModel.Extentions;
using JosephM.Application.ViewModel.Grid;
using JosephM.Application.ViewModel.RecordEntry.Form;
using JosephM.Core.Attributes;
using JosephM.Core.FieldType;
using JosephM.InstanceComparer.AddToSolution;
using JosephM.Record.Service;
using JosephM.XrmModule.SavedXrmConnections;
using System;
using System.Linq;
using System.Threading;

namespace JosephM.InstanceComparer
{
    [MyDescription("Compare The State Of Customisations And Data Between 2 CRM Instances. Note This Is Not A Complete Comparison")]
    [DependantModule(typeof(SavedXrmConnectionsModule))]
    public class InstanceComparerModule :
        ServiceRequestModule
            <InstanceComparerDialog, InstanceComparerService, InstanceComparerRequest, InstanceComparerResponse, InstanceComparerResponseItem>
    {
        public override string MainOperationName
        {
            get { return "Instance Compare"; }
        }

        public override void InitialiseModule()
        {
            base.InitialiseModule();
            AddPortalDataButtonToRequestFormGrid();
            AddComponentsToSolutionButtonInSummaryGrid();
        }

        private void AddComponentsToSolutionButtonInSummaryGrid()
        {
            var customGridFunction = new CustomGridFunction("ADDTOSOLUTION", "Add To Solution", new[] {
                new CustomGridFunction("ADDTOSOLUTIONC1",
                (g) =>
                {
                    try
                    {
                        var response = GetResponse(g);
                        return response.ServiceOne?.XrmRecordConfiguration?.ToString() ?? "Instance 1";
                    }
                    catch (Exception ex)
                    {
                        g.ApplicationController.ThrowException(ex);
                        return "Instance 1";
                    }
                },
                (g) =>
                {
                    g.ApplicationController.DoOnAsyncThread(() =>
                    {
                        try
                        {
                            g.ParentForm.LoadingViewModel.IsLoading = true;
                            var response = GetResponse(g);
                            var service = response.ServiceOne;
                            var dialogController = new DialogController(g.ApplicationController);
                                                    var items = response.AllDifferences
                                .Where(d => d.ComponentTypeForSolution.HasValue && d.IdForSolution1 != null)
                                .Select(d => new AddToSolutionItem(d.ComponentTypeForSolution.Value, d.IdForSolution1))
                                .ToArray();
                            var request = new AddToSolutionRequest(items, service);
                            var dialog = new AddToSolutionDialog(service, dialogController, request: request, onClose: g.RemoveParentDialog);
                            g.LoadDialog(dialog);
                        }
                        catch (Exception ex)
                        {
                            g.ApplicationController.ThrowException(ex);
                        }
                        finally
                        {
                            g.ParentForm.LoadingViewModel.IsLoading = false;
                        }
                    });
                }, (g) =>
                {
                    try
                    {
                        var response = GetResponse(g);
                        var items = response.AllDifferences
                            .Where(d => d.ComponentTypeForSolution.HasValue && d.IdForSolution1 != null)
                            .Select(d => new AddToSolutionItem(d.ComponentTypeForSolution.Value, d.IdForSolution1))
                            .ToArray();
                        return items.Any();
                    }
                    catch (Exception ex)
                    {
                        g.ApplicationController.ThrowException(ex);
                    }
                    return false;
                }
                ),
                new CustomGridFunction("ADDTOSOLUTIONC2",
                (g) =>
                {
                    try
                    {
                        var response = GetResponse(g);
                        return response.ServiceTwo?.XrmRecordConfiguration?.ToString() ?? "Instance 2";
                    }
                    catch (Exception ex)
                    {
                        g.ApplicationController.ThrowException(ex);
                        return "Instance 2";
                    }
                },
                (g) =>
                {
                    g.ApplicationController.DoOnAsyncThread(() =>
                    {
                        try
                        {
                            g.ParentForm.LoadingViewModel.IsLoading = true;
                            var response = GetResponse(g);
                            var service = response.ServiceTwo;
                            var dialogController = new DialogController(g.ApplicationController);
                            var items = response.AllDifferences
                                .Where(d => d.ComponentTypeForSolution.HasValue && d.IdForSolution2 != null)
                                .Select(d => new AddToSolutionItem(d.ComponentTypeForSolution.Value, d.IdForSolution2))
                                .ToArray();
                            var request = new AddToSolutionRequest(items, service);
                            var dialog = new AddToSolutionDialog(service, dialogController, request: request, onClose: g.RemoveParentDialog);
                            g.LoadDialog(dialog);
                        }
                        catch (Exception ex)
                        {
                            g.ApplicationController.ThrowException(ex);
                        }
                        finally
                        {
                            g.ParentForm.LoadingViewModel.IsLoading = false;
                        }
                    });
                },
                (g) =>
                {
                    try
                    {
                        var response = GetResponse(g);
                        var items = response.AllDifferences
                            .Where(d => d.ComponentTypeForSolution.HasValue && d.IdForSolution2 != null)
                            .Select(d => new AddToSolutionItem(d.ComponentTypeForSolution.Value, d.IdForSolution2))
                            .ToArray();
                        return items.Any();
                    }
                    catch (Exception ex)
                    {
                        g.ApplicationController.ThrowException(ex);
                    }
                    return false;
                })
            });
            this.AddCustomGridFunction(customGridFunction, typeof(InstanceComparerTypeSummary));
        }

        private InstanceComparerResponse GetResponse(DynamicGridViewModel g)
        {
            var record = g.ParentForm.GetRecord();
            var objectRecord = record as ObjectRecord;
            if (objectRecord == null)
                throw new NullReferenceException($"Error expected response record of type {typeof(ObjectRecord).Name}. Actual type is {record.GetType().Name}");
            var instance = objectRecord.Instance;
            var response = objectRecord.Instance as InstanceComparerResponse;
            if (response == null)
                throw new NullReferenceException($"Error expected response object of type {typeof(InstanceComparerResponse).Name}. Actual type is {instance.GetType().Name}");
            return response;
        }

        private void AddPortalDataButtonToRequestFormGrid()
        {
            var customFormFunction = new CustomFormFunction("ADDPORTALDATA", "Add Portal Types", (r) =>
            {
                try
                {
                    r.GetBooleanFieldFieldViewModel(nameof(InstanceComparerRequest.Data)).Value = true;
                    var typesGrid = r.GetEnumerableFieldViewModel(nameof(InstanceComparerRequest.DataComparisons));
                    var typesToAdd = new[]
                    {
                        "adx_contentsnippet",
                        "adx_entityform",
                        "adx_entityformmetadata",
                        "adx_entitylist",
                        "adx_entitypermission",
                        "adx_pagetemplate",
                        "adx_publishingstate",
                        "adx_sitemarker",
                        "adx_sitesetting",
                        "adx_webfile",
                        "adx_weblink",
                        "adx_weblinkset",
                        "adx_webpage",
                        "adx_webpageaccesscontrolrule",
                        "adx_webrole",
                        "adx_webtemplate",
                    };
                    var typesGridService = typesGrid.GetRecordService();
                    foreach (var item in typesToAdd.Reverse())
                    {
                        var newRecord = typesGridService.NewRecord(typeof(InstanceComparerRequest.InstanceCompareDataCompare).AssemblyQualifiedName);
                        newRecord.SetField(nameof(InstanceComparerRequest.InstanceCompareDataCompare.RecordType), new RecordType(item, item), typesGridService);
                        typesGrid.InsertRecord(newRecord, 0);
                    }
                }
                catch(Exception ex)
                {
                    r.ApplicationController.ThrowException(ex);
                }
            }, (r) => true);
            this.AddCustomFormFunction(customFormFunction, typeof(InstanceComparerRequest));
        }
    }
}