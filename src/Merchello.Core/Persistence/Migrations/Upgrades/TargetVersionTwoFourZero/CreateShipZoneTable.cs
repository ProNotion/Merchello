namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionTwoFourZero
{
	using Merchello.Core.Configuration;
	using Merchello.Core.Models.Rdbms;

	using Umbraco.Core;
	using Umbraco.Core.Logging;
	using Umbraco.Core.Persistence;
	using Umbraco.Core.Persistence.Migrations;

	//using DatabaseSchemaHelper = Merchello.Core.Persistence.Migrations.DatabaseSchemaHelper;

    /// <summary>
    /// The create product collection table.
    /// </summary>
    [Migration("2.4.0", 1, MerchelloConfiguration.MerchelloMigrationName)]
    public class CreateShipZoneTable : IMerchelloMigration
    {
        /// <summary>
        /// The schema helper.
        /// </summary>
        private readonly DatabaseSchemaHelper _schemaHelper;

		/// <summary>
		/// Initializes a new instance of the <see cref="CreateShipZoneTable"/> class.
		/// </summary>
		public CreateShipZoneTable()
        {
            var dbContext = ApplicationContext.Current.DatabaseContext;
            this._schemaHelper = new DatabaseSchemaHelper(dbContext.Database, LoggerResolver.Current.Logger, dbContext.SqlSyntax);
        }

        /// <summary>
        /// Adds the merchProductCollection table to the database.
        /// </summary>
        public void Up()
        {
            if (!this._schemaHelper.TableExist("merchShipZone"))
            {
                this._schemaHelper.CreateTable(false, typeof(ShipZoneDto));
            }
        }

        /// <summary>
        /// The down.
        /// </summary>
        /// <exception cref="DataLossException">
        /// Throws a data loss exception if a downgrade is attempted
        /// </exception>
        public void Down()
        {
            throw new DataLossException("Cannot downgrade from a version 2.4.0 database to a prior version, the database schema has already been modified");
        }
    }
}