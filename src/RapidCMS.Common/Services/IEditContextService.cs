﻿using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Forms;
using RapidCMS.Common.Models.Commands;

namespace RapidCMS.Common.Services
{
    public interface IEditContextService
    {
        Task<EditContext> GetEntityAsync(UsageType usageType, string collectionAlias, string? variantAlias, ParentPath? parentPath, string? id);
        Task<ViewCommand> ProcessEntityActionAsync(UsageType usageType, string collectionAlias, ParentPath? parentPath, string? id, EditContext editContext, string actionId, object? customData);

        Task<EditContext> GetRootAsync(UsageType usageType, string collectionAlias, ParentPath? parentPath);

        Task<List<EditContext>> GetEntitiesAsync(UsageType usageType, string collectionAlias, ParentPath? parentPath, Query query);
        Task<ViewCommand> ProcessListActionAsync(UsageType usageType, string collectionAlias, ParentPath? parentPath, IEnumerable<EditContext> editContexts, string actionId, object? customData);
        Task<ViewCommand> ProcessListActionAsync(UsageType usageType, string collectionAlias, ParentPath? parentPath, string id, EditContext editContext, string actionId, object? customData);

        Task<List<EditContext>> GetRelatedEntitiesAsync(UsageType usageType, string collectionAlias, IEntity relatedEntity, Query query);
        Task<ViewCommand> ProcessRelationActionAsync(UsageType usageType, string collectionAlias, IEntity relatedEntity, IEnumerable<EditContext> editContexts, string actionId, object? customData);
        Task<ViewCommand> ProcessRelationActionAsync(UsageType usageType, string collectionAlias, IEntity relatedEntity, string id, EditContext editContext, string actionId, object? customData);
    }
}