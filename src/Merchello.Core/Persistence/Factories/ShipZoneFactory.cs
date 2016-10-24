using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Services;

namespace Merchello.Core.Persistence.Factories
{
	using Merchello.Core.Models.Interfaces;

	internal class ShipZoneFactory : IEntityFactory<IShipZone, ShipZoneDto>
	{
		private readonly IStoreSettingService _storeSettingService;

		public ShipZoneFactory(IStoreSettingService storeSettingService)
		{
			_storeSettingService = storeSettingService;
		}

		/// <summary>
		/// Builds the entity.
		/// </summary>
		/// <param name="dto">The dto object.</param>
		/// <returns>An instance of <see cref="IShipZone"/></returns>
		public IShipZone BuildEntity(ShipZoneDto dto)
		{
			var shipZone = new ShipZone(dto.CatalogKey, dto.Name)
			{
				Key = dto.Key,
				UpdateDate = dto.UpdateDate,
				CreateDate = dto.CreateDate
			};

			shipZone.ResetDirtyProperties();

			return shipZone;
		}

		public ShipZoneDto BuildDto(IShipZone entity)
		{
			return new ShipZoneDto()
			{
				Key = entity.Key,
				CatalogKey = entity.CatalogKey,
				Name = entity.Name,
				ZoneCode = entity.ZoneCode,
				UpdateDate = entity.UpdateDate,
				CreateDate = entity.CreateDate
			};

		}
	}
}