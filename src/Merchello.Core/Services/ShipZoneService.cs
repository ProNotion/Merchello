namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Threading;

    using Merchello.Core.Events;

    using Models;
    using Models.Interfaces;
    using Persistence;
    using Persistence.Querying;
    using Persistence.UnitOfWork;

    using umbraco.BusinessLogic;

    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Core.Logging;

    /// <summary>
    /// The ship zone service.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed. Suppression is OK here.")]
    public class ShipZoneService : MerchelloRepositoryService, IShipZoneService
    {
        /// <summary>
        /// The locker.
        /// </summary>
        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        /// <summary>
        /// The store setting service.
        /// </summary>
        private readonly IStoreSettingService _storeSettingService;

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ShipZoneService"/> class.
		/// </summary>
		public ShipZoneService()
            : this(LoggerResolver.Current.Logger)
        {            
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="ShipZoneService"/> class.
		/// </summary>
		/// <param name="logger">
		/// The logger.
		/// </param>
		public ShipZoneService(ILogger logger)
            : this(new RepositoryFactory(), logger, new StoreSettingService(logger))
        {
        }


		/// <summary>
		/// Initializes a new instance of the <see cref="ShipZoneService"/> class.
		/// </summary>
		/// <param name="repositoryFactory">
		/// The repository factory.
		/// </param>
		/// <param name="logger">
		/// The logger.
		/// </param>
		/// <param name="storeSettingService">
		/// The store setting service.
		/// </param>
		public ShipZoneService(RepositoryFactory repositoryFactory, ILogger logger, IStoreSettingService storeSettingService)
            : this(new PetaPocoUnitOfWorkProvider(logger), repositoryFactory, logger, storeSettingService)
        {

        }

		/// <summary>
		/// Initializes a new instance of the <see cref="ShipZoneService"/> class.
		/// </summary>
		/// <param name="provider">
		/// The provider.
		/// </param>
		/// <param name="repositoryFactory">
		/// The repository factory.
		/// </param>
		/// <param name="logger">
		/// The logger.
		/// </param>
		/// <param name="storeSettingService">
		/// The store setting service.
		/// </param>
		public ShipZoneService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, ILogger logger, IStoreSettingService storeSettingService)
            : this(provider, repositoryFactory, logger, new TransientMessageFactory(), storeSettingService)
        {
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="ShipZoneService"/> class.
		/// </summary>
		/// <param name="provider">
		/// The provider.
		/// </param>
		/// <param name="repositoryFactory">
		/// The repository factory.
		/// </param>
		/// <param name="logger">
		/// The logger.
		/// </param>
		/// <param name="eventMessagesFactory">
		/// The event messages factory.
		/// </param>
		/// <param name="storeSettingService">
		/// The store setting service.
		/// </param>
		public ShipZoneService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, ILogger logger, IEventMessagesFactory eventMessagesFactory, IStoreSettingService storeSettingService)
            : base(provider, repositoryFactory, logger, eventMessagesFactory)
        {
            Ensure.ParameterNotNull(storeSettingService, "storeSettingService");
            _storeSettingService = storeSettingService;
        }

		#endregion

		/// <summary>
		/// Create a ship zone for the warehouse catalog.
		/// </summary>
		/// <param name="warehouseCatalogKey">The warehouse catalog key.</param>
		/// <param name="zoneName">Name of the zone.</param>
		/// <param name="raiseEvents">Raise events flag.</param>
		/// <returns>
		/// The <see cref="Attempt" />.
		/// </returns>
		internal Attempt<IShipZone> CreateShipZoneWithKey(Guid warehouseCatalogKey, string zoneName, bool raiseEvents = true)
        {
			Ensure.ParameterCondition(warehouseCatalogKey != Guid.Empty, "warehouseCatalog");
            if (string.IsNullOrEmpty(zoneName)) return Attempt<IShipZone>.Fail(new ArgumentNullException("zoneName"));

            var shipZone = new ShipZone(warehouseCatalogKey, zoneName);

            if (raiseEvents)
                if (Creating.IsRaisedEventCancelled(new Events.NewEventArgs<IShipZone>(shipZone), this))
                {
                    shipZone.WasCancelled = true;
                    return Attempt<IShipZone>.Fail(shipZone);
                }

            // verify that a ShipZone does not already exist for this pair

            var sc = GetShipZonesByCatalogKey(warehouseCatalogKey).FirstOrDefault(x => x != null && x.Name.Equals(shipZone.Name));
            if (sc != null)
                return
                    Attempt<IShipZone>.Fail(
                        new ConstraintException("A ShipZone with the name '" + shipZone.Name +
                                                "' is already associated with this WarehouseCatalog"));

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateShipZoneRepository(uow, _storeSettingService))
                {
                    repository.AddOrUpdate(shipZone);
                    uow.Commit();
                }
            }

            if (raiseEvents)
                Created.RaiseEvent(new Events.NewEventArgs<IShipZone>(shipZone), this);

            return Attempt<IShipZone>.Succeed(shipZone);
        }

		/// <summary>
		/// Saves a single <see cref="IShipZone" /> instance
		/// </summary>
		/// <param name="shipZone">The ship Zone.</param>
		/// <param name="raiseEvents">The raise Events.</param>
		public void Save(IShipZone shipZone, bool raiseEvents = true)
        {
            if(raiseEvents)
            if (Saving.IsRaisedEventCancelled(new SaveEventArgs<IShipZone>(shipZone), this))
            {
                ((ShipZone)shipZone).WasCancelled = true;
                return;
            }
            
            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateShipZoneRepository(uow, _storeSettingService))
                {
                    repository.AddOrUpdate(shipZone);
                    uow.Commit();
                }
            }

            if(raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IShipZone>(shipZone), this);
        }

		/// <summary>
		/// Deletes a single <see cref="IShipZone" /> object
		/// </summary>
		/// <param name="shipZone">The ship Zone.</param>
		/// <param name="raiseEvents">Raise Events flag.</param>
		public void Delete(IShipZone shipZone, bool raiseEvents = true)
        {
            if(raiseEvents)
            if (Deleting.IsRaisedEventCancelled(new DeleteEventArgs<IShipZone>(shipZone), this))
            {
                ((ShipZone)shipZone).WasCancelled = true;
                return;
            }

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateShipZoneRepository(uow, _storeSettingService))
                {
                    repository.Delete(shipZone);
                    uow.Commit();
                }
            }
            
            if(raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IShipZone>(shipZone), this);
        }

		/// <summary>
		/// Gets a list of <see cref="IShipZone" /> objects given a <see cref="IWarehouseCatalog" /> key
		/// </summary>
		/// <param name="catalogKey">Guid</param>
		/// <returns>
		/// A collection of <see cref="IShipZone" />
		/// </returns>
		public IEnumerable<IShipZone> GetShipZonesByCatalogKey(Guid catalogKey)
        {
            using (var repository = RepositoryFactory.CreateShipZoneRepository(UowProvider.GetUnitOfWork(), _storeSettingService))
            {
                var query = Query<IShipZone>.Builder.Where(x => x.CatalogKey == catalogKey);
                return repository.GetByQuery(query);
            }
        }

		/// <summary>
		/// Gets a single <see cref="IShipZone" />
		/// </summary>
		/// <param name="catalogKey">The warehouse catalog key (guid)</param>
		/// <param name="zoneName">Name of the zone.</param>
		/// <returns></returns>
		public IShipZone GetShipZoneByName(Guid catalogKey, string zoneName)
        {
            var shipZone = GetShipZonesByCatalogKey(catalogKey).ToArray();

            if (!shipZone.Any()) return null;
            var specific = shipZone.FirstOrDefault(x => x.Name.Equals(zoneName));

            return specific;
        }


		/// <summary>
		/// Gets a single <see cref="IShipZone" /> by it's unique key (Guid pk)
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>
		/// The <see cref="IShipZone" />.
		/// </returns>
		public IShipZone GetByKey(Guid key)
        {
            using (var repository = RepositoryFactory.CreateShipZoneRepository(UowProvider.GetUnitOfWork(), _storeSettingService))
            {
                return repository.Get(key);
            }
        }

		// used for testing
		/// <summary>
		/// Gets all ship zones.
		/// </summary>
		/// <returns></returns>
		internal IEnumerable<IShipZone> GetAllShipZones()
        {
            using (var repository = RepositoryFactory.CreateShipZoneRepository(UowProvider.GetUnitOfWork(), _storeSettingService))
            {
                return repository.GetAll();
            }
        }


        #region Event Handlers

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IShipZoneService, Events.NewEventArgs<IShipZone>> Creating;


        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IShipZoneService, Events.NewEventArgs<IShipZone>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IShipZoneService, SaveEventArgs<IShipZone>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IShipZoneService, SaveEventArgs<IShipZone>> Saved;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IShipZoneService, DeleteEventArgs<IShipZone>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IShipZoneService, DeleteEventArgs<IShipZone>> Deleted;

        #endregion

    }
}