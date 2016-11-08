using System;
using System.Linq;

namespace Merchello.Core.Services
{
	using System.Data;
	using System.Threading;

	using Merchello.Core.Events;
	using Merchello.Core.Models;
	using Merchello.Core.Persistence;
	using Merchello.Core.Persistence.UnitOfWork;
	using Merchello.Core.Services.Interfaces;

	using Umbraco.Core;
	using Umbraco.Core.Events;
	using Umbraco.Core.Logging;

	public class ShipZoneCountryService : ShipCountryService, IShipZoneCountryService
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
		/// Initializes a new instance of the <see cref="ShipCountryService"/> class.
		/// </summary>
		public ShipZoneCountryService()
            : this(LoggerResolver.Current.Logger)
        {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ShipCountryService"/> class.
		/// </summary>
		/// <param name="logger">
		/// The logger.
		/// </param>
		public ShipZoneCountryService(ILogger logger)
            : this(new RepositoryFactory(), logger, new StoreSettingService(logger))
        {
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="ShipCountryService"/> class.
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
		public ShipZoneCountryService(RepositoryFactory repositoryFactory, ILogger logger, IStoreSettingService storeSettingService)
            : this(new PetaPocoUnitOfWorkProvider(logger), repositoryFactory, logger, storeSettingService)
        {

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ShipCountryService"/> class.
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
		public ShipZoneCountryService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, ILogger logger, IStoreSettingService storeSettingService)
            : this(provider, repositoryFactory, logger, new TransientMessageFactory(), storeSettingService)
        {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ShipCountryService"/> class.
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
		public ShipZoneCountryService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, ILogger logger, IEventMessagesFactory eventMessagesFactory, IStoreSettingService storeSettingService)
            : base(provider, repositoryFactory, logger, storeSettingService)
        {
			Mandate.ParameterNotNull(storeSettingService, "storeSettingService");
			_storeSettingService = storeSettingService;
		}

		#endregion


		internal Attempt<IShipCountry> CreateShipCountryWithKey(Guid warehouseCatalogKey, Guid zoneKey, ICountry country, bool raiseEvents = true)
		{
			Ensure.ParameterCondition(warehouseCatalogKey != Guid.Empty, "warehouseCatalog");
			Ensure.ParameterCondition(zoneKey != Guid.Empty, "zoneKey");

			if (country == null) return Attempt<IShipCountry>.Fail(new ArgumentNullException("country"));

			var shipCountry = new ShipCountry(warehouseCatalogKey, zoneKey, country);

			if (raiseEvents)
				if (Creating.IsRaisedEventCancelled(new Events.NewEventArgs<IShipCountry>(shipCountry), this))
				{
					shipCountry.WasCancelled = true;
					return Attempt<IShipCountry>.Fail(shipCountry);
				}

			// verify that a ShipCountry does not already exist for this pair

			var sc = GetShipCountriesByCatalogKey(warehouseCatalogKey).FirstOrDefault(x => x.CountryCode.Equals(country.CountryCode));
			if (sc != null)
				return
					Attempt<IShipCountry>.Fail(
						new ConstraintException("A ShipCountry with CountryCode '" + country.CountryCode +
												"' is already associate with this WarehouseCatalog"));

			using (new WriteLock(Locker))
			{
				var uow = UowProvider.GetUnitOfWork();
				using (var repository = RepositoryFactory.CreateShipCountryRepository(uow, _storeSettingService))
				{
					repository.AddOrUpdate(shipCountry);
					uow.Commit();
				}
			}

			if (raiseEvents)
				Created.RaiseEvent(new Events.NewEventArgs<IShipCountry>(shipCountry), this);

			return Attempt<IShipCountry>.Succeed(shipCountry);
		}

        internal Attempt<IShipCountry> CreateShipCountryWithKey(Guid warehouseCatalogKey, Guid zoneKey, string countryCode, bool raiseEvents = true)
        {
            Ensure.ParameterCondition(warehouseCatalogKey != Guid.Empty, "warehouseCatalog");
            Ensure.ParameterCondition(zoneKey != Guid.Empty, "zoneKey");
            Ensure.ParameterCondition(!string.IsNullOrEmpty(countryCode), "countryCode");

            var country = _storeSettingService.GetCountryByCode(countryCode);

            if (country == null) return Attempt<IShipCountry>.Fail(new ArgumentNullException("country"));

            var shipCountry = new ShipCountry(warehouseCatalogKey, zoneKey, country);

            if (raiseEvents)
                if (Creating.IsRaisedEventCancelled(new Events.NewEventArgs<IShipCountry>(shipCountry), this))
                {
                    shipCountry.WasCancelled = true;
                    return Attempt<IShipCountry>.Fail(shipCountry);
                }

            // verify that a ShipCountry does not already exist for this pair

            var sc = GetShipCountriesByCatalogKey(warehouseCatalogKey).FirstOrDefault(x => x.CountryCode.Equals(country.CountryCode));
            if (sc != null)
                return
                    Attempt<IShipCountry>.Fail(
                        new ConstraintException("A ShipCountry with CountryCode '" + country.CountryCode +
                                                "' is already associate with this WarehouseCatalog"));

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateShipCountryRepository(uow, _storeSettingService))
                {
                    repository.AddOrUpdate(shipCountry);
                    uow.Commit();
                }
            }

            if (raiseEvents)
                Created.RaiseEvent(new Events.NewEventArgs<IShipCountry>(shipCountry), this);

            return Attempt<IShipCountry>.Succeed(shipCountry);
        }

        #region Event Handlers

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IShipCountryService, Events.NewEventArgs<IShipCountry>> Creating;


		/// <summary>
		/// Occurs after Create
		/// </summary>
		public static event TypedEventHandler<IShipCountryService, Events.NewEventArgs<IShipCountry>> Created;

		/// <summary>
		/// Occurs before Save
		/// </summary>
		public static event TypedEventHandler<IShipCountryService, SaveEventArgs<IShipCountry>> Saving;

		/// <summary>
		/// Occurs after Save
		/// </summary>
		public static event TypedEventHandler<IShipCountryService, SaveEventArgs<IShipCountry>> Saved;

		/// <summary>
		/// Occurs before Delete
		/// </summary>		
		public static event TypedEventHandler<IShipCountryService, DeleteEventArgs<IShipCountry>> Deleting;

		/// <summary>
		/// Occurs after Delete
		/// </summary>
		public static event TypedEventHandler<IShipCountryService, DeleteEventArgs<IShipCountry>> Deleted;

		#endregion

	}
}
