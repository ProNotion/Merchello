namespace Merchello.Core.Models
{
	using System;
	using System.Reflection;
	using System.Runtime.Serialization;

	using Merchello.Core.Models.EntityBase;
	using Merchello.Core.Models.Interfaces;

	/// <summary>
	/// 
	/// </summary>
	/// <seealso cref="Merchello.Core.Models.Interfaces.IShipZone" />
	public class ShipZone : Entity, IShipZone
	{
		private Guid _catalogKey;

		private static readonly PropertyInfo CatalogKeySelector = ExpressionHelper.GetPropertyInfo<ShipCountry, Guid>(x => x.CatalogKey);

		/// <summary>
		/// The warehouse catalog key
		/// </summary>
		[DataMember]
		public Guid CatalogKey
		{
			get { return _catalogKey; }
			internal set
			{
				SetPropertyValueAndDetectChanges(value, ref _catalogKey, CatalogKeySelector);
			}
		}
	}
}
