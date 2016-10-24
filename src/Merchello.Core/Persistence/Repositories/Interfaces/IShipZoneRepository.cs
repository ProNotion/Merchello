using System;

using Umbraco.Core.Persistence.Repositories;

namespace Merchello.Core.Persistence.Repositories
{
	using Merchello.Core.Models.Interfaces;
    /// <summary>
    /// Marker interface for the ship zone repository
    /// </summary>
    internal interface IShipZoneRepository : IRepositoryQueryable<Guid, IShipZone>
    {
        bool Exists(Guid catalogKey, string zoneName);
    }
}