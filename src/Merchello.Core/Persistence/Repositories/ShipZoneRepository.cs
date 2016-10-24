namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using Merchello.Core.Models;
    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Models.Rdbms;
    using Merchello.Core.Persistence.Factories;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Persistence.UnitOfWork;
    using Merchello.Core.Services;

    using Umbraco.Core;
    using Umbraco.Core.Cache;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Querying;
    using Umbraco.Core.Persistence.SqlSyntax;

    /// <summary>
    /// The ship country repository.
    /// </summary>
    internal class ShipZoneRepository : MerchelloPetaPocoRepositoryBase<IShipZone>, IShipZoneRepository
    {
        /// <summary>
        /// The store setting service.
        /// </summary>
        private readonly IStoreSettingService _storeSettingService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShipCountryRepository"/> class.
        /// </summary>
        /// <param name="work">
        /// The work.
        /// </param>
        /// <param name="cache">
        /// The cache.
        /// </param>
        /// <param name="storeSettingService">
        /// The store setting service.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="sqlSyntax">
        /// The SQL syntax.
        /// </param>
        public ShipZoneRepository(IDatabaseUnitOfWork work, CacheHelper cache, IStoreSettingService storeSettingService, ILogger logger, ISqlSyntaxProvider sqlSyntax) 
            : base(work, cache, logger, sqlSyntax)
        {
            Mandate.ParameterNotNull(storeSettingService, "settingsService");

            _storeSettingService = storeSettingService;
        }

		/// <summary>
		/// Determines if a ship zone exists for a catalog.
		/// </summary>
		/// <param name="catalogKey">The catalog key.</param>
		/// <param name="zoneName">Name of the zone.</param>
		/// <returns>True if a zone with the name exists for the catalog, otherwise returns false</returns>
		public bool Exists(Guid catalogKey, string zoneName)
        {
            var sql = new Sql();
            sql.Select("*")
                .From<ShipZoneDto>(SqlSyntax)
                .Where<ShipZoneDto>(x => x.CatalogKey == catalogKey && x.Name == zoneName);

            return Database.Fetch<ShipZoneDto>(sql).Any();
        }

        /// <summary>
        /// Gets a <see cref="IShipZone"/> by it's key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IShipZone"/>.
        /// </returns>
        protected override IShipZone PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<ShipZoneDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new ShipZoneFactory(_storeSettingService);
            return factory.BuildEntity(dto);
        }

        /// <summary>
        /// Gets all <see cref="IShipZone"/>.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IShipZone}"/>.
        /// </returns>
        protected override IEnumerable<IShipZone> PerformGetAll(params Guid[] keys)
        {
            if (keys.Any())
            {
                foreach (var key in keys)
                {
                    yield return Get(key);
                }
            }
            else
            {
                var factory = new ShipZoneFactory(_storeSettingService);
                var dtos = Database.Fetch<ShipZoneDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IShipZone"/> by query.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IShipZone}"/>.
        /// </returns>
        protected override IEnumerable<IShipZone> PerformGetByQuery(IQuery<IShipZone> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IShipZone>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<ShipZoneDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key)) ?? Enumerable.Empty<IShipZone>();
        }

        /// <summary>
        /// Gets the base query.
        /// </summary>
        /// <param name="isCount">
        /// The is count.
        /// </param>
        /// <returns>
        /// The <see cref="Sql"/>.
        /// </returns>
        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
                .From<ShipZoneDto>(SqlSyntax);

            return sql;
        }

        /// <summary>
        /// Gets the base where clause.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected override string GetBaseWhereClause()
        {
            return "merchShipZone.pk = @Key";
        }

        /// <summary>
        /// Gets the delete clauses.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/>.
        /// </returns>
        protected override IEnumerable<string> GetDeleteClauses()
        {
			// TODO: Implement delete clauses
            // TODO : RSS - The update in the middle of these delete clauses needs to be refactored - just a quick fix for now
            var list = new List<string>
            {
                //"DELETE FROM merchShipRateTier WHERE shipMethodKey IN (SELECT pk FROM merchShipMethod WHERE shipCountryKey = @Key)",                
                //"UPDATE merchShipment SET shipMethodKey = NULL WHERE shipMethodKey IN (SELECT pk FROM merchShipMethod WHERE shipCountryKey = @Key)",
                //"DELETE FROM merchShipMethod WHERE shipCountryKey = @Key",
                //"DELETE FROM merchShipCountry WHERE pk = @Key"
            };

            return list;
        }

        /// <summary>
        /// Saves a new item to the database.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistNewItem(IShipZone entity)
        {
            // TODO : revisit how this constraint is implemented
            // Assert that a ShipCountry for a given WarehouseCatalog does not already exist with this country code
            if(Exists(entity.CatalogKey, entity.Name)) throw new ConstraintException("A merchShipZone record already exists with the CatalogKey and Zone name");

            ((Entity)entity).AddingEntity();

            var factory = new ShipZoneFactory(_storeSettingService);
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);

            entity.Key = dto.Key;

            entity.ResetDirtyProperties();
        }

        /// <summary>
        /// Updates an existing item in the database.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistUpdatedItem(IShipZone entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new ShipZoneFactory(_storeSettingService);

            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }       
    }
}