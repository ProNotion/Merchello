using System;

namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionTwoFourZero
{
	using System.Linq;

	using Merchello.Core.Configuration;

	using Umbraco.Core;
	using Umbraco.Core.Persistence.Migrations;

	/// <summary>
	/// Migration to add a new foreignkey to the merchShipCountry table
	/// </summary>
	/// <seealso cref="Merchello.Core.Persistence.Migrations.MerchelloMigrationBase" />
	/// <seealso cref="Merchello.Core.Persistence.Migrations.IMerchelloMigration" />
	[Migration("2.4.0", 1, MerchelloConfiguration.MerchelloMigrationName)]
	public class AddShipZoneKeyColumn : MerchelloMigrationBase, IMerchelloMigration
	{
		public override void Up()
		{
			var database = ApplicationContext.Current.DatabaseContext.Database;
			var columns = SqlSyntax.GetColumnsInSchema(database).ToArray();

			// 'shared' column
			if (columns.Any(
				  x => x.TableName.InvariantEquals("merchShipCountry") && x.ColumnName.InvariantEquals("shipZoneKey"))
			  == false)
			{
				Logger.Info(typeof(AddShipZoneKeyColumn), "Adding shipZoneKey column to merchShipCountry table.");

				//// Add the new 'shared' column
				Create.Column("shipZoneKey").OnTable("merchShipCountry").AsGuid().Nullable().ForeignKey("FK_merchCatalogCountry_merchShipZone", "merchShipZone", "pk");
			}
		}

		public override void Down()
		{
			throw new NotImplementedException();
		}
	}
}
