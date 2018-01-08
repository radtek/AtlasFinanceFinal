(function(angular) {
    'use strict';
    var module = angular.module('Falcon.Factories.AccountResource', ['ngResource', 'ngSanitize']);

    module.value('$', $);
    
    module.factory('ContactResource', ['$http',
        function($http) {
            return {
                getTypes: function(token) {
                    return $http({
                        method: 'GET',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/general/?getContactTypes=0')
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                },
                add: function(personId, contactTypeId, value, token) {
                    return $http({
                        method: 'POST',
                        headers: {
                            'forge_token': token
                        },
                        url: ("/api/loan/ContactAdd/?personId=" + personId + "&contactTypeId=" + contactTypeId + "&value=" + value)
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                },
                disable: function(personId, contactId, token) {
                    return $http({
                        method: 'POST',
                        headers: {
                            'forge_token': token
                        },
                        url: ("/api/loan/ContactDisable/?personId=" + personId + "&contactId=" + contactId)
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                }
            };
        }
    ]);


    module.factory('AddressResource', ['$http',
        function($http) {
            return {
                getTypes: function(token) {
                    return $http({
                        method: 'GET',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/general/?getAddressTypes=0')
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                },
                add: function(personId, userPersonId, addressTypeId, provinceId, line1, line2, line3, line4, postalCode, token) {
                    return $http({
                        method: 'POST',
                        headers: {
                            'forge_token': token
                        },
                        url: ("/api/loan/AddressAdd/?personId=" + personId + "&userPersonId=" + userPersonId + "&addressTypeId=" + addressTypeId +
                            "&provinceId=" + provinceId + "&line1=" + line1 + "&line2=" + line2 + "&line3=" + line3 + "&line4=" + line4 + "&postalCode=" + postalCode)
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                },
                disable: function(personId, addressId, token) {
                    return $http({
                        method: 'POST',
                        headers: {
                            'forge_token': token
                        },
                        url: ("/api/loan/AddressDisable/?personId=" + personId + "&addressId=" + addressId)
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                }
            };
        }
    ]);


    module.factory('RelationResource', ['$http',
        function($http) {
            return {
                getTypes: function(token) {
                    return $http({
                        method: 'GET',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/loan/?getRelationTypes=0')
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                },
                add: function(personId, userPersonId, firstname, lastname, cellNo, relationTypeId, token) {
                    return $http({
                        method: 'POST',
                        headers: {
                            'forge_token': token
                        },
                        url: ("/api/loan/?addRelation=0&personId=" + personId + "&userPersonId=" + userPersonId + "&firstname=" + firstname + "&lastname=" + lastname + "&cellNo=" + cellNo + "&relationTypeId=" + relationTypeId)
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                },
                update: function(personId, relationPersonId, firstname, lastname, cellNo, relationTypeId, token) {
                    return $http({
                        method: 'POST',
                        headers: {
                            'forge_token': token
                        },
                        url: ("/api/loan/?updateRelation=0&personId=" + personId + "&relationPersonId=" + relationPersonId + "&firstname=" + firstname + "&lastname=" + lastname + "&cellNo=" + cellNo + "&relationTypeId=" + relationTypeId)
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                }
            };
        }
    ]);


    module.factory('WorkflowResource', ['$http',
        function($http) {
            return {
                getWorkflows: function(hostId, branchId, accountNo, startDate, endDate) {
                    return $http({
                        method: 'GET',
                        headers: {},
                        url: ('/api/Workflow/?hostId=' + hostId + '&branchId=' + branchId + '&accountNo=' + accountNo + '&startDate=' + startDate + '&endDate=' + endDate)
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                },
                redirectProcess: function(processStepJobAccountId, userId) {
                    return $http({
                        method: 'POST',
                        headers: {},
                        url: ("/api/Workflow/RedirectAccountToProcessStep/?processStepJobAccountId=" + processStepJobAccountId + "&userId=" + userId)
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                }
            };
        }
    ]);

    module.factory('QuotationResource', ['$http',
        function($http) {
            return {
                accept: function(accountId, quotationId, token) {
                    return $http({
                        method: 'POST',
                        headers: {
                            'forge_token': token
                        },
                        url: ("/api/quotation/?acceptQuotation=0&accountId=" + accountId + "&quotationId=" + quotationId)
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                },
                reject: function(accountId, quotationId, token) {
                    return $http({
                        method: 'POST',
                        headers: {
                            'forge_token': token
                        },
                        url: ("/api/quotation/?rejectQuotation=0&accountId=" + accountId + "&quotationId=" + quotationId)
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                }
            };
        }
    ]);

    module.factory('AffordabilityResource', ['$http',
        function($http) {
            return {
                getCategories: function(token) {
                    return $http({
                        method: 'GET',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/loan/?getAffordabiltiyCategories=0')
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                },
                acceptOption: function(accountId, affordabilityOptionId, token) {
                    return $http({
                        method: 'POST',
                        headers: {
                            'forge_token': token
                        },
                        url: ("/api/loan/?accountId=" + accountId + "&affordabilityOptionId=" + affordabilityOptionId)
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                },
                addItem: function(accountId, affordabilityCategoryId, amount, personId, token) {
                    return $http({
                        method: 'POST',
                        headers: {
                            'forge_token': token
                        },
                        url: ("/api/loan/?accountId=" + accountId + "&affordabilityCategoryId=" + affordabilityCategoryId + "&amount=" + amount + "&personId=" + personId)
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                },
                deleteItem: function(accountId, affordabilityId, personId, token) {
                    return $http({
                        method: 'POST',
                        headers: {
                            'forge_token': token
                        },
                        url: ("/api/loan/?accountId=" + accountId + "&affordabilityId=" + affordabilityId + "&personId=" + personId)
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                }
            };
        }
    ]);

    module.factory('AccountResource', ['$http', 'StatusColour', '$q',
        function($http, StatusColour, $q) {
            return {
                search: function(query, token) {
                    return $http({
                        method: 'GET',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/loan/?query=' + query)
                    }).then(function(response) {
                        for (var i = response.data.SearchResults.length - 1; i >= 0; i--) {
                            /*jshint -W069 */
                            response.data.SearchResults[i]["StatusColour"] = StatusColour[response.data.SearchResults[i]["Status"]];
                        }
                        return response.data.SearchResults;
                    }, function(error) {
                        return error;
                    });
                },
                get: function(id, token) {
                    return $http({
                        method: 'GET',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/loan/' + id)
                    }).then(function(response) {
                            /*jshint -W069 */
                            response.data.Result["StatusColour"] = StatusColour[response.data.Result.Status];
                            return response.data.Result;
                        },
                        function(error) {
                            return error;
                        });
                },
                overrideFraudScore: function(id, uid, token, reason) {
                    return $http({
                        method: 'POST',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/loan/?fid=' + id + '&fUid=' + uid + '&reason=' + reason)
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                },
                addNote: function(accountId, personId, token, note) {
                    return $http({
                        method: 'POST',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/loan/?accountId=' + accountId + '&personId=' + personId + '&note=' + note)
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                },
                authenticationFn: function(authenticationId, reason, token, oId, fn) {
                    return $http({
                        method: 'POST',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/loan/?authenticationId=' + authenticationId + '&fn=' + fn + '&reason=' + reason + '&oId=' + oId)
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                },
                adjustAccount: function(accountId, loanAmount, period, token) {
                    return $http({
                        method: 'POST',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/loan/?accountId=' + accountId + '&loanAmount=' + loanAmount + '&period=' + period)
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                }
            };
        }
    ]);
}(this.angular));