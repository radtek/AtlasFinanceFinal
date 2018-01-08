(function(angular) {
    'use strict';
    var module = angular.module('Falcon.Factories', ['ngResource', 'ngSanitize']);

    module.value('$', $);

    module.factory('AvsAnalyticsResource', ['$apiResource',
        function($apiResource) {
            return $apiResource('/AccountVerificationAnalyticsResource/:action', {}, {
                totalCount: {
                    method: 'GET',
                    params: {
                        method: 'GetTransactionTotal'
                    }
                }
            });
        }
    ]);

    module.factory('BatchResource', ['$apiResource',
        function($apiResource) {
            return $apiResource('/BatchResource/:action', {}, {
                getJobs: {
                    method: 'GET',
                    params: {
                        method: 'GetJobs'
                    }
                }
            });
        }
    ]);

    module.factory('CreditResource', ['$http',
        function($http) {
            return {
                reSubmit: function(enquiryId, token) {
                    return $http({
                        method: 'POST',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/credit/resubmit?enquiryId=' + enquiryId)
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                },
                report: function(reportId, token) {
                    return $http({
                        method: 'GET',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/credit/report?reportId=' + reportId)
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

    /*module.factory('AvsResource', ['$http',
        function($http) {
            return {
                getTransactions: function(branchId, startDate, endDate, transactionId, idNumber, bankId, token) {
                    return $http({
                        method: 'GET',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/Avs/GetTransactions/?branchId=' + branchId + '&startDate=' + startDate + '&endDate=' + endDate + '&transactionId=' + transactionId + '&idNumber=' + idNumber + '&bankId=' + bankId)
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                },
                getBanks: function(token) {
                    return $http({
                        method: 'GET',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/Avs/GetBanks/?getBanks=0')
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                },
                cancelAVS: function(transactionId, token) {
                    return $http({
                        method: 'POST',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/Avs/Cancel/?cancelTransactionId=' + transactionId)
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                },
                resendAVS: function(transactionId, token) {
                    return $http({
                        method: 'POST',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/Avs/Resend/?resendTransactionId=' + transactionId)
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                },
                saveServiceSchedule: function(services, serviceBanks, token) {
                    return $http({
                        method: 'POST',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/Avs/SaveServiceSettings/?servicesString=' + services + '&serviceBanksString=' + serviceBanks)
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                },
                getServiceSchedule: function(token) {
                    return $http({
                        method: 'GET',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/Avs/GetServiceSchedules/?getServiceSchedules=0')
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                }
            };
        }
    ]);*/

    /*module.factory('PayoutResource', ['$http',
        function($http) {
            var CONSTANTS = {
                New: [1, 'New', 'label label-info'],
                Cancelled: [2, 'Cancelled', 'label label-warning'],
                OnHold: [3, 'On Hold', 'label label-info'],
                Batched: [4, 'Batched', 'label label-default'],
                Submitted: [5, 'Submitted', 'label label-default'],
                Successful: [6, 'Successful', 'label label-success'],
                Failed: [7, 'Failed', 'label label-danger'],
                Removed: [8, 'Removed', 'label label-warning']
            };

            var _updateStatus = function(payout, payoutStatus) {
                payout.PayoutStatusId = payoutStatus[0];
                payout.PayoutStatus = payoutStatus[1];
                payout.PayoutStatusColor = payoutStatus[2];
                return payout;
            };

            return {
                constants: CONSTANTS,
                getTransactions: function(branchId, startRangeActionDate, endRangeActionDate, payoutId, idNumber, bankId, token) {
                    return $http({
                        method: 'GET',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/Payout/GetTransactions/?branchId=' + branchId + '&startRangeActionDate=' + startRangeActionDate + '&endRangeActionDate=' + endRangeActionDate + '&payoutId=' + payoutId + '&idNumber=' + idNumber + '&bankId=' + bankId)
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                },
                getBanks: function(token) {
                    return $http({
                        method: 'GET',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/Payout/GetBanks/?getBanks=0')
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                },
                holdPayment: function(payout, token) {
                    return $http({
                        method: 'POST',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/Payout/PlaceOnHold/?payoutToHold=' + payout.PayoutId)
                    }).then(function(response) {
                        return _updateStatus(payout, CONSTANTS.OnHold);
                    }, function() {
                        return "Error placing payout on hold!";
                    });
                },
                releasePayment: function(payout, token) {
                    return $http({
                        method: 'POST',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/Payout/RemoveHoldFromPayout/?payoutToRemoveHold=' + payout.PayoutId)
                    }).then(function(response) {
                        return _updateStatus(payout, CONSTANTS.New);
                    }, function() {
                        return "Error removing hold from payout!";
                    });
                },
                getAlerts: function(token) {
                    return $http({
                        method: 'GET',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/Payout/GetAlerts/?getAlerts=0')
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                }
            };
        }
    ]);*/

    /*module.factory('NaedoResource', ['$http',
        function($http) {
            return {
                getBatches: function(branchId, batchId, batchStatus, startRange, endRange, token) {
                    return $http({
                        method: 'GET',
                        headers: {
                            'forge_token': token
                        },
                        url: ("/api/Naedo/?branchId=" + branchId + "&batchId=" + batchId + "&batchStatus=" + batchStatus + "&startRange=" + startRange + "&endRange=" + endRange)
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                },
                getTransaction: function(batchId, token) {
                    return $http({
                        method: 'GET',
                        headers: {
                            'forge_token': token
                        },
                        url: ("/api/Naedo/?batchId=" + batchId)
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                },
                getControl: function(controlId, token) {
                    return $http({
                        method: 'GET',
                        headers: {
                            'forge_token': token
                        },
                        url: ("/api/Naedo/?ControlId=" + controlId)
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                },
                addAdditionalDebit: function(controlId, amount, actionDate, token) {
                    return $http({
                        method: 'POST',
                        headers: {
                            'forge_token': token
                        },
                        url: ("/api/Naedo/AddAdditionalDebitOrder/?controlId=" + controlId + "&instalment=" + amount + "&actionDate=" + actionDate)
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                },
                canAdditionalDebit: function(controlId, transactionId, token) {
                    return $http({
                        method: 'POST',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/Naedo/CancelAdditionalDebitOrder/?controlId=' + controlId + '&transactionId=' + transactionId)
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                },
                saveServiceSchedule: function(services, serviceBanks, token) {
                    return $http({
                        method: 'POST',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/Naedo/SaveServiceSettings/?servicesString=' + services + '&serviceBanksString=' + serviceBanks)
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                },
                getServiceSchedule: function(token) {
                    return $http({
                        method: 'GET',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/Naedo/GetServiceSchedules/?getServiceSchedules=0')
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                },
                getControls: function(host, branchId, startDate, endDate, controlOnly, token) {
                    return $http({
                        method: 'GET',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/Naedo/?host=' + host + '&branchId=' + branchId + '&startDate=' + startDate + '&endDate=' + endDate + '&controlOnly=' + controlOnly)
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                }
            };
        }
    ]);*/

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

    module.factory('UniversalResource', ['$http',
        function($http) {
            return {
                getHostsLinkedToPerson: function(personId) {
                    return $http({
                        method: 'GET',
                        headers: {},
                        url: ('/api/User/?getAccessibleHostsPersonId=' + personId)
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                },
                getProvinces: function(token) {
                    return $http({
                        method: 'GET',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/general/?getProvinces=0')
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                },
                getPublicHolidaysFromToday: function(token) {
                    return $http({
                        method: 'GET',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/general/?getPublicHolidaysFromToday=0')
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

    module.factory('Transactions', ['$http',
        function($http) {

        }
    ]);

    /* module.factory('FalconTable', ['ngTableParams', '$filter',
        function(ngTableParams, $filter) {
            return {
                new: function($scope, obj, pageLength, pageCount) {
                    /*jshint -W055 */
    /* return new ngTableParams({
                        page: 1,
                        count: pageLength,
                        sorting: {
                            name: 'asc'
                        }
                    }, {
                        total: $scope[obj].length, // length of data
                        getData: function($defer, params) {

                            var filteredData = params.filter() ? $filter('filter')($scope[obj], params.filter()) : $scope[obj];
                            var orderedData = params.sorting() ? $filter('orderBy')(filteredData, params.orderBy()) : $scope[obj];
                            params.total(orderedData.length);
                            $defer.resolve(orderedData.slice((params.page() - 1) * params.count(), params.page() * params.count()));
                        }
                    });
                }
            };
        }
    ]);*/

    module.factory('SocketWrapper', ['$rootScope', 'xSocketServer', '$q',
        function($rootScope, xSocketServer, $q) {

            var Listener = (function() {
                function Listener(fn) {
                    this._a = fn;
                }
                Listener.prototype.process = function(fn) {
                    this._a = fn;
                    return this;
                };

                Listener.prototype.invoke = function(a) {
                    this._a(a);
                    return this;
                };
                return Listener;
            })();

            function Bind(sock, eventName, listeners, fn) {
                if (!listeners.hasOwnProperty(eventName)) {
                    listeners[eventName] = new Listener();
                    sock.on(eventName, function(fn) {
                        $rootScope.$apply(function() {
                            listeners[eventName].invoke(fn);
                        });
                    });
                    return listeners;
                } else {
                    return listeners;
                }
            }

            function Publish(connected, sock, eventName, data, queued) {
                if (connected && typeof(sock) != "undefined") {
                    try {
                        sock.publish(eventName, data);
                    } catch (error) {
                        console.log("caught error: " + error);
                        queued.push({
                            t: eventName,
                            d: data || {}
                        });
                    }
                } else {
                    queued.push({
                        t: eventName,
                        d: data || {}
                    });
                }
                return queued;
            }

            function CreateNewSocket(url) {
                return new XSockets.WebSocket(xSocketServer + url);
            }

            var Connect = function(sock) {
                var def = $q.defer();
                sock.on(XSockets.Events.open, function(conn) {
                    $rootScope.$apply(function() {
                        def.resolve(conn);
                    });
                });
                return def.promise;
            };

            return {
                Listener: Listener,
                Bind: Bind,
                Publish: Publish,
                CreateNewSocket: CreateNewSocket,
                Connect: Connect
            };
        }
    ]);

    module.factory('xSocketsProxy', ['$rootScope', 'SocketWrapper',
        function($rootScope, SocketWrapper) {
            var _connected, _listeners = {},
                _sock, _queued = [];

            function publish(topic, data) {
                _queued = SocketWrapper.Publish(_connected, _sock, topic, data, _queued);
            }

            function subscribe(topic) {
                return bind(topic, {});
            }

            function subscribeToSpecific(topic, data) {
                return bind(topic, data);
            }

            function bind(topic, data) {
                publish(topic, data);
                _listeners = SocketWrapper.Bind(_sock, topic, _listeners);
                return _listeners[topic];
            }

            function connection(url) {
                _sock = SocketWrapper.CreateNewSocket(url);
                SocketWrapper.Connect(_sock).then(function(ctx) {
                    _connected = true;
                    _queued.forEach(function(msg, i) {
                        publish(msg.t, msg.d);
                    });
                    _queued = [];
                });
            }

            function connectionFP(url, returnPromise) {
                _sock = SocketWrapper.CreateNewSocket(url);

                if (returnPromise)
                    return SocketWrapper.Connect(_sock);
                else
                    connection(url);
            }

            function setProperty(name, newValue) {
                publish('set_' + name, {
                    value: newValue
                });
            }

            return {
                isConnected: _connected,
                connection: connection,
                subscribe: subscribe,
                subscribeToSpecific: subscribeToSpecific,
                publish: publish,
                setProperty: setProperty,
            };
        }
    ]);

    module.factory('NotificationService', ['$rootScope', 'SocketWrapper',
        function($rootScope, SocketWrapper) {
            var _connected, _listeners = {},
                _sock, _queued = [],
                _notes = [],
                _noteCount;

            function publish(topic, data) {
                _queued = SocketWrapper.Publish(_connected, _sock, topic, data, _queued);
            }

            function subscribe(topic, data) {
                publish(topic, data);
                _listeners = SocketWrapper.Bind(_sock, topic, _listeners);
                return _listeners[topic];
            }

            var notes = function() {
                try {
                    _notes = JSON.parse(localStorage.getItem('Falcon_Notifications')).Notes || [];
                } catch (e) {
                    _notes = [];
                }
                return _notes;
            };

            var noteCount = function() {
                try {
                    _noteCount = JSON.parse(localStorage.getItem('Falcon_Notifications')).NoteCount;
                } catch (e) {
                    _noteCount = 0;
                }
                return _noteCount;
            };

            function saveNotification(notification) {
                localStorage.setItem('Falcon_Notifications', JSON.stringify(notification));
            }

            function connection(url) {
                _sock = SocketWrapper.CreateNewSocket(url);
                SocketWrapper.Connect(_sock).then(function(ctx) {
                    _connected = true;
                    _queued.forEach(function(msg, i) {
                        publish(msg.t, msg.d);
                    });
                    _queued = [];
                });
            }

            function setProperty(name, newValue) {
                publish('set_' + name, {
                    value: newValue
                });
            }

            return {
                connection: connection,
                subscribe: subscribe,
                setProperty: setProperty,
                notes: notes,
                noteCount: noteCount,
                saveNotification: saveNotification
            };
        }
    ]);


    module.factory('$localStorage', ['$window',
        function($window) {
            return $window.localStorage || {
                _data: {},
                setItem: function(id, val) {
                    this._data[id] = String(val);
                    return this._data[id];
                },
                getItem: function(id) {
                    return this._data.hasOwnProperty(id) ? this._data[id] : undefined;
                },
                removeItem: function(id) {
                    return delete this._data[id];
                },
                clear: function() {
                    this._data = {};
                    return this._data;
                }
            };
        }
    ]);

    module.factory('Xsrf', ['$document',
        function($document) {
            return {
                get: function() {
                    return angular.element($document[0].querySelector(".token")).attr("request-verification-token");
                }
            };
        }
    ]);

    module.factory('OTPResource', ['$http',
        function($http) {
            return {
                verify: function(hash, otp, token) {
                    return $http({
                        method: 'POST',
                        headers: {
                            'forge_token': token
                        },
                        data: {
                            hash: hash,
                            OTP: otp
                        },
                        url: ('/api/otp/')
                    });
                }
            }
        }
    ]);

    module.factory('ForgotPasswordResource', ['$http',
        function($http) {
            return {
                verify: function(dt, token) {
                    return $http({
                        method: 'POST',
                        url: '/api/forgot/',
                        headers: {
                            'forge_token': token
                        },
                        data: dt,
                    })
                }
            }
        }
    ]);

    module.factory('ResetPasswordResource', ['$http',
        function($http) {
            return {
                reset: function(hash, password, token) {
                    return $http({
                        method: 'POST',
                        headers: {
                            'forge_token': token
                        },
                        data: {
                            password: password,
                            hash: hash

                        },
                        url: ('/api/password/')
                    });
                }
            }
        }
    ]);

    module.factory('GuidResource', ['$http',
        function($http) {
            return {
                guid: function(token) {
                    return $http({
                        method: 'GET',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/guid/')
                    });
                }
            }
        }
    ]);

    module.factory('ApplicationResource', ['$http',
        function($http) {
            return {
                checkClient: function(idNo, token) {
                    return $http({
                        method: 'GET',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/application?idNo=' + idNo)
                    });
                }
            }
        }
    ]);

    module.factory('UserManagementResource', ['$http',
        function($http) {
            return {
                list: function(token) {
                    return $http({
                        method: 'GET',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/user/getuserlist')
                    });
                },
                revoke: function(id, token) {
                    return $http({
                        method: 'POST',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/user/?method=revoke&id=' + id)
                    });
                },
                authorise: function(id, token) {
                    return $http({
                        method: 'POST',
                        headers: {
                            'forge_token': token
                        },
                        url: ('/api/user/?method=authorise&id=' + id)
                    });
                }
            }

        }
    ]);

}(this.angular));