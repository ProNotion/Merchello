namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models.Interfaces;

    using Models;
    using Umbraco.Core.Services;

    /// <summary>
    /// Defines the shipZoneServcie
    /// </summary>
    public interface IShipZoneService : IService
    {
        /// <summary>
        /// Saves a single <see cref="ShipZone"/>
        /// </summary>
        /// <param name="shipZone">
        /// The ship Zone.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise Events.
        /// </param>
        void Save(IShipZone shipZone, bool raiseEvents = true);

        /// <summary>
        /// Deletes a single <see cref="IShipZone"/> object
        /// </summary>
        /// <param name="shipZone">
        /// The ship Zone.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise Events.
        /// </param>
        void Delete(IShipZone shipZone, bool raiseEvents = true);

        /// <summary>
        /// Gets a single <see cref="IShipZone"/> by it's unique key (Guid pk)
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IShipZone"/>.
        /// </returns>
        IShipZone GetByKey(Guid key);

        /// <summary>
        /// Gets a list of <see cref="IShipZone"/> objects given a <see cref="IWarehouseCatalog"/> key
        /// </summary>
        /// <param name="catalogKey">The catalog key</param>
        /// <returns>A collection of <see cref="IShipZone"/></returns>
        IEnumerable<IShipZone> GetShipZonesByCatalogKey(Guid catalogKey);
    }
}