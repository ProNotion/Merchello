namespace Merchello.Core.Models.Rdbms
{
	using System;
	using Umbraco.Core.Persistence;
	using Umbraco.Core.Persistence.DatabaseAnnotations;

	[TableName("merchShipZone")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    internal class ShipZoneDto
    {
		/// <summary>
		/// Gets or sets the primary key.
		/// </summary>
		/// <value>
		/// The primary key.
		/// </value>
		[Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Constraint(Default = "newid()")]
        public Guid Key { get; set; }

		/// <summary>
		/// Gets or sets the catalog key.
		/// </summary>
		/// <value>
		/// The catalog key.
		/// </value>
		[Column("catalogKey")]
        [ForeignKey(typeof(WarehouseCatalogDto), Name = "FK_merchCatalogShipZone_merchWarehouseCatalog", Column = "pk")]
        public Guid CatalogKey { get; set; }

		/// <summary>
		/// Gets or sets the zone code pretty much like a country code.
		/// </summary>
		/// <value>
		/// The zone code.
		/// </value>
		[Column("zoneCode")]
        public string ZoneCode { get; set; }

		/// <summary>
		/// Gets or sets the name of the shipping zone.
		/// </summary>
		/// <value>
		/// The name of the shipping zone.
		/// </value>
		[Column("name")]
        public string Name { get; set; }


		/// <summary>
		/// Gets or sets the update date.
		/// </summary>
		/// <value>
		/// The update date.
		/// </value>
		[Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

		/// <summary>
		/// Gets or sets the create date.
		/// </summary>
		/// <value>
		/// The create date.
		/// </value>
		[Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }
    }
}