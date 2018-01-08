(function(angular, $) {
  'use strict';

  var module = angular.module('atlas.factories', [
    'ngResource',

    'atlas.config'
  ]);

  module.constant('atlasEnums', {
    APP_STATUS: {
      Inactive: 0,
      Technical_Error: 5,
      Pending: 10,
      Cancelled: 15,
      Declined: 20,
      Review: 25,
      PreApproved: 30,
      Approved: 35,
      Open: 40,
      Legal: 50,
      WrittenOff: 60,
      Settled: 70,
      Closed: 80
    },
    AVS_RESULT: {
      PASSED: 1,
      PASSEDWARNINGS: 2,
      FAILED: 3,
      NORESULT: 4,
      LOCKED: 5
    },
    MARITAL_STATUS: {
      NotSet: 0,
      Single: 1,
      Married: 2,
      Divorced: 3,
      Widowed: 4
    }
  });

  module.value('$', $);

  module.factory('$localStorage', ['$window',
    function($window) {
      return $window.localStorage || {
        _data: {},
        setItem: function(id, val) {
          return this._data[id] = String(val);
        },
        getItem: function(id) {
          return this._data.hasOwnProperty(id) ? this._data[id] : undefined;
        },
        removeItem: function(id) {
          return delete this._data[id];
        },
        clear: function() {
          return this._data = {};
        }
      };
    }
  ]);

  // Redirect interceptor
  module.config(['$httpProvider',
    function($httpProvider) {
      $httpProvider.responseInterceptors.push(['$q', '$window', 'atlasConfig',
        function($q, $window, config) {
          return function(promise) {
            return promise.then(function(resp) {
              var location = resp.headers(config.redirectHeader);
              if (location) {
                var origin = window.location.protocol + '//' + window.location.hostname;
                if (location.indexOf(origin) != 0) {
                  throw 'Attempt to redirect to different origin ' + location + ' not from ' + origin + '.';
                }

                $window.location = location;
                return false;
              }

              return resp;
            });
          };
        }
      ]);
    }
  ]);

  // Atlas API Resource
  // Adds a base url from config to all api resources
  // Poller() will poll resource until predicate returns true or false
  module.factory('$apiResource', ['$resource', '$timeout', '$q', '$rootScope', 'atlasConfig', '$',
    function($resource, $timeout, $q, $rootScope, config, $) {
      var base = config.apiBase.replace(/:/g, '\\:');
      return function(url, paramDefaults, actions) {
        var resource = $resource(base + url, paramDefaults, actions);

        resource.poller = function(options, data, predicate) {
          var interval,
            timeout = false;

          if (angular.isObject(options)) {
            interval = options.interval;
            angular.isDefined(options.timeout) && (timeout = options.timeout);
          } else {
            interval = options;
            options = {
              interval: interval
            };
          }

          if (angular.isFunction(data)) {
            predicate = data;
            data = {};
          }

          function Poller(interval, data, predicate) {
            var self = this,
              ticks = 0,
              intervalPromise = null,
              intervalCancelled = false,
              defer = $q.defer();

            this.poll = function() {
              ticks++;
              if (options.includeTick) {
                data.tick = ticks;
              }

              resource.get(data, function() {
                var result = predicate.apply(this, arguments);
                if (result === true) {
                  defer.resolve.apply(this, arguments);
                  intervalPromise = null;
                  return;
                } else if (result === false) {
                  defer.reject.apply(this, arguments);
                  intervalPromise = null;
                  return;
                }

                // Timed out?
                if (timeout && timeout < interval * ticks) {
                  $(self).trigger('_apiResourceTimeout', [arguments[0], ticks]);
                  return;
                }

                if (!intervalCancelled) {
                  intervalPromise = $timeout(self.poll, interval);
                }
              }, function(e) {
                $(self).trigger('_apiResourceError', [e]);
              });

              return this;
            };

            this.cancel = function() {
              intervalPromise && $interval.cancel(intervalPromise)
              intervalCancelled = true;
              return this;
            };

            this.timeout = function(callback) {
              if (callback) {
                $(self).on('_apiResourceTimeout', callback);
              } else if (callback === false) {
                $(self).off('_apiResourceTimeout');
              }

              return this;
            };

            this.error = function(callback) {
              if (callback) {
                $(self).on('_apiResourceError', callback);
              } else if (callback === false) {
                $(self).off('_apiResourceError');
              }

              return this;
            };

            this.done = function(callback) {
              defer.then(callback);
              return this;
            };

            this.fail = function(callback) {
              defer.then(null, callback);
              return this;
            };
          }

          return new Poller(interval, data, predicate);
        };

        return resource;
      }
    }
  ]);

  module.factory('AvsResource', ['$apiResource',
    function($apiResource) {
      return $apiResource('/Avs/:action', {}, {
        get: {
          action: 'Status',
          method: 'GET'
        },
        submit: {
          action: 'Submit',
          method: 'POST',
          params: {
            ApplicationId: '@ApplicationId'
          }
        }
      });
    }
  ]);

  // Loan calculator
  module.service('LoanCalculator', function() {
    this.capital = null;
    this.period = null;

    // lee: added INSURANCE_PERC
    this.CONSTANTS = {
      VAT: 0.14,
      MAX_DAYS: 60,
      SERVICE_FEE: 50,
      INSURANCE_PERC: 0.5
    };

    this.CONSTANTS.PERIOD = this.CONSTANTS.MAX_DAYS / 365;

    this.initialize = function(params) {
      var p;
      for (p in params) {
        this[p] = params[p];
      }

      this._totalFee = null;
      this._interest = null;
      this._initFee = null;
      this._vat = null;
    };

    this.initiationFee = function() {
      if (this._initFee) {
        return this._initFee;
      }

      var capital = this.capital;

      return this.initFee = (capital <= 1000) ?
        (capital * 0.15) :
        (150 + ((capital - 1000) * 0.1));
    };

    this.interest = function() {
      if (this._interest) {
        return this._interest;
      }

      var initFee = this.initiationFee(),
        CONST = this.CONSTANTS;

      return this._interest = ((this.capital + initFee + CONST.SERVICE_FEE + ((initFee + CONST.SERVICE_FEE) * CONST.VAT)) *
        (CONST.PERIOD / 100)) * this.period;
    };

    this.totalFee = function() {
      if (this._totalFee) {
        return this._totalFee;
      }

      return this._totalFee = (this.initiationFee() + this.interest() + this.CONSTANTS.SERVICE_FEE + this.vat());
    };

    this.vat = function() {
      if (this._vat) {
        return this._vat
      };

      var initFee = this.initiationFee();
      return this._vat = (initFee + this.CONSTANTS.SERVICE_FEE) * this.CONSTANTS.VAT;
    };

    // lee: added to calc insurance premium
    this.insurancePremium = function(repayment) {
      var insuranceAmount = repayment * this.CONSTANTS.INSURANCE_PERC / 100;
      return ((insuranceAmount * this.CONSTANTS.VAT) + insuranceAmount);
    };

    // lee: modified to include insurance premium
    this.repaymentAmount = function() {
      //return this.capital + this.totalFee(); 
      var repayment = this.capital + this.totalFee();
      return (this.insurancePremium(repayment) + repayment);
    };
  });

  module.factory('DateExtension', function() {
    return {
      getTimeZoneDate: function(data) {
        var customDate = new Date();
        var timeZone = (customDate.getTimezoneOffset() / 60) * (-1);
        customDate = new Date(data);
        customDate.setHours(customDate.getHours() + timeZone);
        return customDate;
      }
    }
  });

  // Public holiday factory
  module.factory('Holidays', function() {
    var holidays = [
      '1/1/2013',
      '21/3/2013',
      '29/3/2013',
      '1/4/2013',
      '27/4/2013',
      '1/5/2013',
      '16/6/2013',
      '9/8/2013',
      '24/9/2013',
      '16/12/2013',
      '25/12/2013',
      '26/12/2013',

      '1/1/2014',
      '2/1/2014',
      '21/3/2014',
      '18/4/2014',
      '21/4/2014',
      '27/4/2014',
      '1/5/2014',
      '16/6/2014',
      '9/8/2014',
      '24/9/2014',
      '16/12/2014',
      '25/12/2014',
      '26/12/2014',
    ];

    return {
      holidays: holidays,
      // Maybe for future if we want to load holidays through ajax
      setHolidayData: function(data) {
        holidays = data;
      },
      isWorkingDay: function(date) {
        // Sunday?
        if (date.getDay() === 0) {
          return false;
        }

        // Public holiday?
        var dateStr = date.getDate() + '/' + (date.getMonth() + 1) + '/' + date.getFullYear();
        if (holidays.indexOf(dateStr) >= 0) {
          return false;
        }

        return true;
      },

      getNextWorkingDay: function(period) {
        var nextWorkingDay = false;

        while (!nextWorkingDay) {
          var date = new Date();
          date.setDate(date.getDate() + period);
          if (!this.isWorkingDay(date)) {
            period--;
            nextWorkingDay = true;
          }
        }
        return period;
      }
    }
  });

  // Slider API resource
  module.factory('LoanResource', ['$apiResource',
    function($apiResource) {
      return $apiResource("/loan/:method", {}, {
        get: {
          method: 'GET',
          params: {
            method: 'get'
          }
        },
        save: {
          method: 'POST',
          params: {
            method: 'post'
          }
        }
      });
    }
  ]);

}(this.angular, this.jQuery));