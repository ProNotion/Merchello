    /**
     * @ngdoc model
     * @name ProcessorArgumentsCollectionDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's ProcessVoidPaymentDialogData object
     */
    var ProcessVoidPaymentDialogData = function() {
        var self = this;
        self.invoiceKey = '';
        self.paymentMethodKey = '';
        self.paymentKey = '';
        self.processorArgumentCollectionDisplay = new ProcessorArgumentCollectionDisplay();
        self.warning = '';
    };

    ProcessVoidPaymentDialogData.prototype = (function() {

        function toPaymentRequestDisplay() {
            var paymentRequest = angular.extend(this, PaymentRequestDisplay);
            paymentRequest.processorArgs = this.processorArgumentCollectionDisplay.toArray();
            return paymentRequest;
        }

        return {
            toPaymentRequestDisplay : toPaymentRequestDisplay
        }
    }());

    angular.module('merchello.models').constant('ProcessVoidPaymentDialogData', ProcessVoidPaymentDialogData);
