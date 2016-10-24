    /**
     * @ngdoc model
     * @name ShipZoneDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's ShipCountryDisplay object
     */
var ShipZoneDisplay = function () {
        var self = this;
        self.key = '';
        self.catalogKey = '';
        self.zoneCode = '';
        self.name = '';
        self.countries = [];
    };

    angular.module('merchello.models').constant('ShipZoneDisplay', ShipZoneDisplay);