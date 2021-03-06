#region

using JosephM.Core.Extentions;
using JosephM.Core.FieldType;
using JosephM.Core.Log;
using JosephM.Core.Service;
using JosephM.Core.Utility;
using JosephM.Deployment.DataImport;
using JosephM.Deployment.ExportXml;
using JosephM.Deployment.ImportXml;
using JosephM.Record.Extentions;
using JosephM.Record.IService;
using JosephM.Record.Xrm.XrmRecord;
using JosephM.Xrm;
using JosephM.Xrm.Schema;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

#endregion

namespace JosephM.Deployment.MigrateRecords
{
    public class MigrateRecordsService :
        ServiceBase<MigrateRecordsRequest, MigrateRecordsResponse, DataImportResponseItem>
    {
        public MigrateRecordsService()
        {
        }

        public override void ExecuteExtention(MigrateRecordsRequest request, MigrateRecordsResponse response,
            ServiceRequestController controller)
        {
            var exportService = new ExportXmlService(new XrmRecordService(request.SourceConnection));

            var exportedEntities = new List<Entity>();

            exportService.ProcessExport(request.RecordTypesToMigrate, request.IncludeNotes, request.IncludeNNRelationshipsBetweenEntities, controller.Controller
                , (entity) => exportedEntities.Add(entity)
                , (entity) => exportedEntities.Add(entity));

            var removeDuplicates = new List<Entity>();
            foreach(var entity in exportedEntities)
            {
                if(!removeDuplicates.Any(e => e.Id == entity.Id && e.LogicalName == entity.LogicalName))
                {
                    removeDuplicates.Add(entity);
                }
            }

            var importService = new DataImportService(new XrmRecordService(request.TargetConnection));
            var matchOption = request.MatchByName ? DataImportService.MatchOption.PrimaryKeyThenName : DataImportService.MatchOption.PrimaryKeyOnly;
            var dataImportResponse = importService.DoImport(removeDuplicates, controller, request.MaskEmails, matchOption: matchOption, includeOwner: request.IncludeOwner);
            response.LoadDataImport(dataImportResponse);
        }
    }
}