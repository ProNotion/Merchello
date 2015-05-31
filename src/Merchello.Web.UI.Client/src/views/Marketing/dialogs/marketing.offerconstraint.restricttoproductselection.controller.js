/**
 * @ngdoc controller
 * @name Merchello.Marketing.Dialogs.OfferConstraintPriceController
 * @function
 *
 * @description
 * The controller to configure the price component constraint
 */
angular.module('merchello').controller('Merchello.Marketing.Dialogs.OfferConstraintRestrictToProductSelectionController',
    ['$scope', 'notificationsService', 'productResource', 'settingsResource', 'productDisplayBuilder', 'queryDisplayBuilder', 'queryResultDisplayBuilder',
        function($scope, notificationsService, productResource, settingsResource, productDisplayBuilder, queryDisplayBuilder, queryResultDisplayBuilder) {

            $scope.loaded = false;
            $scope.filterText = "";
            $scope.products = [];
            $scope.filteredproducts = [];
            $scope.watchCount = 0;
            $scope.sortProperty = "name";
            $scope.sortOrder = "Ascending";
            $scope.limitAmount = 10;
            $scope.currentPage = 0;
            $scope.maxPages = 0;

            // exposed methods
            $scope.changePage = changePage;
            $scope.limitChanged = limitChanged;
            $scope.changeSortOrder = changeSortOrder;
            $scope.getFilteredProducts = getFilteredProducts;
            $scope.numberOfPages = numberOfPages;

            //--------------------------------------------------------------------------------------
            // Initialization methods
            //--------------------------------------------------------------------------------------

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description
             * Method called on intial page load.  Loads in data from server and sets up scope.
             */
            function init() {
                $scope.dialogData.component.extendedData.setValue('test', 'test');
                loadProducts();
                loadSettings();
            }

            /**
             * @ngdoc method
             * @name loadProducts
             * @function
             *
             * @description
             * Load the products from the product service, then wrap the results
             * in Merchello models and add to the scope via the products collection.
             */
            function loadProducts() {

                var page = $scope.currentPage;
                var perPage = $scope.limitAmount;
                var sortBy = $scope.sortProperty.replace("-", "");
                var sortDirection = $scope.sortOrder;

                var query = queryDisplayBuilder.createDefault();
                query.currentPage = page;
                query.itemsPerPage = perPage;
                query.sortBy = sortBy;
                query.sortDirection = sortDirection;
                query.addFilterTermParam($scope.filterText);

                var promise = productResource.searchProducts(query);
                promise.then(function (response) {
                    var queryResult = queryResultDisplayBuilder.transform(response, productDisplayBuilder);

                    $scope.products = queryResult.items;

                    $scope.maxPages = queryResult.totalPages;
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;

                }, function (reason) {
                    notificationsService.success("Products Load Failed:", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadSettings
             * @function
             *
             * @description
             * Load the settings from the settings service to get the currency symbol
             */
            function loadSettings() {
                var currencySymbolPromise = settingsResource.getCurrencySymbol();
                currencySymbolPromise.then(function (currencySymbol) {
                    $scope.currencySymbol = currencySymbol;
                }, function (reason) {
                    notificationsService.error("Settings Load Failed", reason.message);
                });
            }

            //--------------------------------------------------------------------------------------
            // Events methods
            //--------------------------------------------------------------------------------------

            /**
             * @ngdoc method
             * @name limitChanged
             * @function
             *
             * @description
             * Helper function to set the amount of items to show per page for the paging filters and calculations
             */
            function limitChanged(newVal) {
                $scope.limitAmount = newVal;
                $scope.currentPage = 0;
                loadProducts();
            }

            /**
             * @ngdoc method
             * @name changePage
             * @function
             *
             * @description
             * Helper function re-search the products after the page has changed
             */
            function changePage (newPage) {
                $scope.currentPage = newPage;
                loadProducts();
            }

            /**
             * @ngdoc method
             * @name changeSortOrder
             * @function
             *
             * @description
             * Helper function to set the current sort on the table and switch the
             * direction if the property is already the current sort column.
             */
            function changeSortOrder(propertyToSort) {

                if ($scope.sortProperty == propertyToSort) {
                    if ($scope.sortOrder == "Ascending") {
                        $scope.sortProperty = "-" + propertyToSort;
                        $scope.sortOrder = "Descending";
                    } else {
                        $scope.sortProperty = propertyToSort;
                        $scope.sortOrder = "Ascending";
                    }
                } else {
                    $scope.sortProperty = propertyToSort;
                    $scope.sortOrder = "Ascending";
                }

                loadProducts();
            }

            /**
             * @ngdoc method
             * @name getFilteredProducts
             * @function
             *
             * @description
             * Calls the product service to search for products via a string search
             * param.  This searches the Examine index in the core.
             */
            function getFilteredProducts(filter) {
                $scope.filterText = filter;
                $scope.currentPage = 0;
                loadProducts();
            }

            //--------------------------------------------------------------------------------------
            // Helper methods
            //--------------------------------------------------------------------------------------



            //--------------------------------------------------------------------------------------
            // Calculations
            //--------------------------------------------------------------------------------------

            /**
             * @ngdoc method
             * @name numberOfPages
             * @function
             *
             * @description
             * Helper function to get the amount of items to show per page for the paging
             */
            function numberOfPages() {
                return $scope.maxPages;
            }


            // Initialize the controller
            init();

        }]);
