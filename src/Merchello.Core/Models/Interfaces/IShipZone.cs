
namespace Merchello.Core.Models.Interfaces
{
	using System;

	using Merchello.Core.Models.EntityBase;

	/// <summary>
	/// An interface to use for any shipping zones
	/// </summary>
	/// <seealso cref="IShipZone" />
	public interface IShipZone : IEntity
	{
		/// <summary>
		/// Gets or sets the name of the shipping zone.
		/// </summary>
		/// <value>
		/// The name of the shipping zone.
		/// </value>
		string Name { get; set; }

		/// <summary>
		/// Gets or sets the catalog key.
		/// </summary>
		/// <value>
		/// The catalog key.
		/// </value>
		Guid CatalogKey { get; }

		/// <summary>
		/// Gets or sets the unique zone code.
		/// </summary>
		/// <value>
		/// The zone code.
		/// </value>
		string ZoneCode { get; set; }
	}
}
