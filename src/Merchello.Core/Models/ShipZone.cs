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
		/// <summary>
		/// The property selectors.
		/// </summary>
		private static readonly Lazy<PropertySelectors> _ps = new Lazy<PropertySelectors>();

		/// <summary>
		/// The catalog key
		/// </summary>
		private Guid _catalogKey;

		/// <summary>
		/// The warehouse catalog key
		/// </summary>
		[DataMember]
		public Guid CatalogKey
		{
			get { return _catalogKey; }
			internal set
			{
				SetPropertyValueAndDetectChanges(value, ref _catalogKey, _ps.Value.CatalogKeySelector);
			}
		}

		/// <summary>
		/// Gets or sets the unique zone code.
		/// </summary>
		/// <value>
		/// The zone code.
		/// </value>
		public string ZoneCode { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public string Name { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ShipZone" /> class.
		/// </summary>
		/// <param name="catalogKey">The catalog key.</param>
		/// <param name="zoneName">Name of the zone.</param>
		public ShipZone(Guid catalogKey, string zoneName)
		{
			this.CatalogKey = catalogKey;
			this.Name = zoneName;
			this.ZoneCode = Guid.NewGuid().ToString();
		}

		private class PropertySelectors
		{
			/// <summary>
			/// The catalog key selector.
			/// </summary>
			public readonly PropertyInfo CatalogKeySelector = ExpressionHelper.GetPropertyInfo<ShipZone, Guid>(x => x.CatalogKey);
		}
	}
}
