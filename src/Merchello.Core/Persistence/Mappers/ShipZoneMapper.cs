using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Mappers
{
    internal sealed class ShipZoneMapper : MerchelloBaseMapper
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="ShipZoneMapper"/> class.
		/// </summary>
		public ShipZoneMapper()
        {
            BuildMap();
        }

		/// <summary>
		/// Responsible for building the map between the class and the DTO.
		/// </summary>
		internal override void BuildMap()
        {
            if (!PropertyInfoCache.IsEmpty) return;

            CacheMap<ShipZone, ShipZoneDto>(src => src.Key, dto => dto.Key);
            CacheMap<ShipZone, ShipZoneDto>(src => src.CatalogKey, dto => dto.CatalogKey);
            CacheMap<ShipZone, ShipZoneDto>(src => src.ZoneCode, dto => dto.ZoneCode);
			CacheMap<ShipZone, ShipZoneDto>(src => src.Name, dto => dto.Name);
            CacheMap<ShipZone, ShipZoneDto>(src => src.UpdateDate, dto => dto.UpdateDate);
            CacheMap<ShipZone, ShipZoneDto>(src => src.CreateDate, dto => dto.CreateDate);
        }
    }
}