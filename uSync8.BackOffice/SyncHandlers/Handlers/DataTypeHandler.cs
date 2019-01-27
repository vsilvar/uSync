﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Entities;
using Umbraco.Core.Services;
using Umbraco.Core.Services.Implement;
using uSync8.BackOffice.Services;
using uSync8.Core;
using uSync8.Core.Serialization;
using uSync8.Core.Tracking;

namespace uSync8.BackOffice.SyncHandlers.Handlers
{
    [SyncHandler("dataTypeHandler", "Datatypes", "DataTypes", uSyncBackOfficeConstants.Priorites.DataTypes, Icon = "icon-autofill")]
    public class DataTypeHandler : SyncHandlerTreeBase<IDataType, IDataTypeService>, ISyncHandler, ISyncPostImportHandler
    {
        private readonly IDataTypeService dataTypeService;

        public DataTypeHandler(
            IEntityService entityService,
            IDataTypeService dataTypeService,
            IProfilingLogger logger, 
            ISyncSerializer<IDataType> serializer, 
            ISyncTracker<IDataType> tracker,
            SyncFileService syncFileService, 
            uSyncBackOfficeSettings settings) 
            : base(entityService, logger, serializer, tracker, syncFileService, settings)
        {
            this.dataTypeService = dataTypeService;
            this.itemObjectType = UmbracoObjectTypes.DataType;
        }

        protected override IDataType GetFromService(int id)
            => dataTypeService.GetDataType(id);

        public void InitializeEvents()
        {
            DataTypeService.Saved += ItemSavedEvent;
            DataTypeService.Deleted += ItemDeletedEvent;
        }

        protected override string GetItemFileName(IUmbracoEntity item)
            => item.Name.ToSafeFileName();

        protected override void DeleteFolder(int id)
            => dataTypeService.DeleteContainer(id);

        public IEnumerable<uSyncAction> ProcessPostImport(string folder, IEnumerable<uSyncAction> actions)
        {
            if (actions == null || !actions.Any())
                return null;

            foreach (var action in actions)
            {
                var attempt = Import(action.FileName);
                if (attempt.Success)
                {
                    ImportSecondPass(action.FileName, attempt.Item);
                }
            }

            return CleanFolders(folder, -1);
        }

    }
}
