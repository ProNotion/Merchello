namespace Merchello.Web.Models.ContentEditing
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The ship zone display.
    /// </summary>
    public class ShipZoneDisplay
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the catalog key.
        /// </summary>
        public Guid CatalogKey { get; set; }

        /// <summary>
        /// Gets or sets the countries in the zone.
        /// </summary>
        public IEnumerable<CountryDisplay> Countries { get; set; }
    }
}
