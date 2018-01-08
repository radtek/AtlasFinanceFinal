(function(angular) {
    'use strict';
    var module = angular.module('falcon.constants', []);


    module.constant('BatchStatus', {
        1: 'label label-default',
        2: 'label label-warning',
        3: 'label label-primary',
        4: 'label label-primary',
        5: 'label label-warning',
        6: 'label label-success',
        7: 'label label-danger',
        8: 'label label-danger'
    });

    module.constant('NaedoTransactionStatus', {
        1: 'label label-default',
        2: 'label label-warning',
        3: 'label label-info',
        4: 'label label-primary',
        5: 'label label-success',
        6: 'label label-success',
        7: 'label label-danger',
        8: 'label label-danger'
    });

    module.constant('ControlStatus', {
        1: 'label label-default',
        2: 'label label-primary',
        3: 'label label-success',
        4: 'label label-warning',
        5: 'label label-warning',
        6: 'label label-danger'
    });

    module.constant('EnquiryType', {
        'Credit': 'Credit Enquiry',
        'Fraud': 'Fraud Enquiry'
    });

    module.constant('StageColour', {
        'Error': 'label label-danger',
        'Completed - Contains Errors': 'label label-danger',
        'Completed': 'label label-success',
        'Loading - Contains Errors': 'label label-info',
        'New': 'label label-primary',
        'Importing File': 'label label-default'
    });

    module.constant('StatusColour', {
        'Inactive': 'label label-default',
        'Technical Error': 'label label-danger',
        'Pending': 'label label-default',
        'Cancelled': 'label label-primary',
        'Declined': 'label label-danger',
        'Review': 'label label-info',
        'Pre-Approved': 'label label-primary',
        'Approved': 'label label-success',
        'Open': 'label label-success',
        'Legal': 'label label-warning',
        'Written Off': 'label label-danger',
        'Settled': 'label label-success',
        'Closed': 'label label-default'
    });

    module.constant('AccountDetailStatusColour', {
        'Inactive': '#999',
        'Technical Error': '#ed4e2a',
        'Pending': '#999',
        'Cancelled': '#428bca',
        'Declined': '#ed4e2a',
        'Review': '#57b5e3',
        'Pre-Approved': '#428bca',
        'Approved': '#3cc051',
        'Open': '#3cc051',
        'Legal': '#fcb322',
        'Written Off': '#ed4e2a',
        'Settled': '#3cc051',
        'Closed': '#999'
    });

})(this.angular);