/**
    * @ngdoc service
    * @name merchello.models.shipZoneDisplayBuilder
    *
    * @description
    * A utility service that builds ShipZoneDisplay models
    */
angular.module('merchello.models')
	.factory('shipZoneDisplayBuilder',
	[
		'genericModelBuilder', 'shipCountryDisplayBuilder', 'ShipZoneDisplay',
		function(genericModelBuilder, shipCountryDisplayBuilder, ShipZoneDisplay) {
			var Constructor = ShipZoneDisplay;
			return {
				createDefault: function() {
					return new Constructor();
				},
				transform: function(jsonResult) {
					if (jsonResult === undefined || jsonResult === null) {
						return;
					}
					var zones = genericModelBuilder.transform(jsonResult, Constructor);
					if (angular.isArray(jsonResult)) {
						for (var i = 0; i < jsonResult.length; i++) {
							zones[i].countries = shipCountryDisplayBuilder.transform(jsonResult[i].countries);
						}
					} else {
						zones.countries = shipCountryDisplayBuilder.transform(jsonResult.countries);
					}
					return zones;
				}
			};
		}
	]);