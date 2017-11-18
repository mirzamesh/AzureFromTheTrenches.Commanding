﻿using AccidentalFish.Commanding.Abstractions.Model;
using AccidentalFish.Commanding.Model;

namespace AccidentalFish.Commanding.AzureEventHub
{
    /// <summary>
    /// Interface for an event hub seriailzer. The default serializer uses Json.
    /// </summary>
    public interface IEventHubSerializer
    {
        /// <summary>
        /// Return a serialized verison of an audit item
        /// </summary>
        /// <param name="auditItem">The audit item to serialize</param>
        /// <returns>Serialized audit item</returns>
        string Serialize(AuditItem auditItem);
    }
}
