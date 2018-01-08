(function(angular) {
    'use strict';
    var module = angular.module('Falcon.Factories.StreamResource', ['ngResource', 'ngSanitize']);

    module.value('$', $);

    module.factory('StreamResource', ['$http', 'apiBase',
        function($http, $apiBase) {
            return {
                getStaticData: function(groupType, token) {
                    return $http.post($apiBase + 'Stream/GetStaticData/', {
                        'StaticData': 0,
                        'GroupType': groupType
                    }, {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).success(function(data, status, headers) {
                        return data;
                    }).error(function(data, status, headers) {
                        return status;
                    });
                },
                getWorkItems: function (allocatedUserId, allocatedPersonId, branchId, caseId, idNumber, accountReferenceNo, startDate, endDate, streamType, groupType, completeDate, token, caseStatus, createStartDate, createEndDate) {
                    return $http.post($apiBase + 'Stream/GetWorkItems/', {
                        'AllocatedUserId': allocatedUserId,
                        'AllocatedPersonId': allocatedPersonId,
                        'BranchId': branchId,
                        'CaseId': caseId,
                        'IdNumber': idNumber,
                        'AccountReferenceNo': accountReferenceNo,
                        'ActionStartDate': startDate,
                        'ActionEndDate': endDate,
                        'StreamType': streamType,
                        'GroupType': groupType,
                        'CompleteDate': completeDate,
                        'CreateStartDate': createStartDate,
                        'CreateEndDate': createEndDate,
                        'CaseStatus': caseStatus
                    }, {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).success(function(data, status, headers) {
                        return data;
                    }).error(function(data, status, headers) {
                        return status;
                    });
                },
                getNextWorkItem: function(personId, streamType, groupType, token) {
                    return $http.post($apiBase + 'Stream/GetNextWorkItem/', {
                        'PersonId': personId,
                        'StreamType': streamType,
                        'GroupType': groupType
                    }, {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).success(function(data, status, headers) {
                        return data;
                    }).error(function(data, status, headers) {
                        return status;
                    });
                },
                getWorkItem: function (caseStreamActionId, userId, token) {
                    return $http.post($apiBase + 'Stream/GetWorkItem/', {
                        'CaseStreamActionId': caseStreamActionId,
                        'UserId': userId
                    }, {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).success(function(data, status, headers) {
                        return data;
                    }).error(function(data, status, headers) {
                        return status;
                    });
                },
                getNoteHistory: function(caseId, token) {
                    return $http.post($apiBase + 'Stream/GetNoteHistory/', {
                        'CaseId': caseId
                    }, {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).success(function(data, status, headers) {
                        return data;
                    }).error(function(data, status, headers) {
                        return status;
                    });
                },
                saveNotInterested: function(caseStreamId, userId, note, token) {
                    return $http.post($apiBase + 'Stream/SaveNotInterested/', {
                        'CaseStreamId': caseStreamId,
                        'UserId': userId,
                        'Note': note
                    }, {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).success(function(data, status, headers) {
                        return data;
                    }).error(function(data, status, headers) {
                        return status;
                    });
                },
                save: function(caseStreamId, actionId, userId, streamType, date, amount, comment, note, token) {
                    return $http.post($apiBase + 'Stream/Save/', {
                        'CaseStreamId': caseStreamId,
                        'ActionId': actionId,
                        'UserId': userId,
                        'StreamType': streamType,
                        'Amount': amount,
                        'Date': date,
                        'Comment': comment,
                        'Note': note
                    }, {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).success(function(data, status, headers) {
                        return data;
                    }).error(function(data, status, headers) {
                        return status;
                    });
                },
                saveOutcome: function(caseStreamId, userId, outcome, comment, note, token) {
                    return $http.post($apiBase + 'Stream/SaveOutcome/', {
                        'CaseStreamId': caseStreamId,
                        'UserId': userId,
                        'Outcome': outcome,
                        'Comment': comment,
                        'Note': note
                    }, {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).success(function(data, status, headers) {
                        return data;
                    }).error(function(data, status, headers) {
                        return status;
                    });
                },
                syncPayment: function(caseStreamId) {
                    return $http.post($apiBase + 'Stream/SyncPayment', {
                        'CaseStreamId': caseStreamId
                    }, {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).success(function(data, status, headers) {
                        return data;
                    }).error(function(data, status, headers) {
                        return status;
                    });
                },
                escalate: function(caseStreamId, userId, level, comment, token) {
                    return $http.post($apiBase + 'Stream/Escalate/', {
                        'CaseStreamId': caseStreamId,
                        'UserId': userId,
                        'EscalationType': level,
                        'Comment': comment
                    }, {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).success(function(data, status, headers) {
                        return data;
                    }).error(function(data, status, headers) {
                        return status;
                    });
                },
                saveContact: function(debitorId, contactType, no, userId, token) {
                    return $http.post($apiBase + 'Stream/SaveContact/', {
                        'DebitorId': debitorId,
                        'ContactType': contactType,
                        'No': no,
                        'UserId': userId
                    }, {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).success(function(data, status, headers) {
                        return data;
                    }).error(function(data, status, headers) {
                        return status;
                    });
                },
                getCaseComments: function(accountNoteId, token) {
                    return $http.post($apiBase + 'Stream/GetCaseComments/', {
                        'AccountNoteId': accountNoteId
                    }, {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).success(function(data, status, headers) {
                        return data;
                    }).error(function(data, status, headers) {
                        return status;
                    });
                },
                saveCaseComment: function(caseId, userId, note, token) {
                    return $http.post($apiBase + 'Stream/AddCaseComment/', {
                      'CaseId': caseId,
                        'UserId': userId,
                        'Note': note
                    }, {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).success(function(data, status, headers) {
                        return data;
                    }).error(function(data, status, headers) {
                        return status;
                    });
                },
                getLetterOfDemandPDF: function(caseStreamId, token) {
                    return $http.post($apiBase + 'Stream/GetLetterOfDemandPDF/', {
                        'CaseStreamId': caseStreamId
                    }, {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                },
                getEscalatedItems: function(allocatedUserId, branchId, token) {
                    return $http.post($apiBase + 'Stream/GetEscalatedItems/', {
                        'AllocatedUserId': allocatedUserId,
                        'BranchId': branchId
                    }, {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                },
                completeEscalation: function(caseStreamId, allocatedUserId, commentId, token) {
                    return $http.post($apiBase + 'Stream/EscalationReturn/', {
                        'CaseStreamId': caseStreamId,
                        'AllocatedUserId': allocatedUserId,
                        'CommentId': commentId
                    }, {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                },
                transferTo: function (caseStreamId, userId, currentUserId, newUserId, token) {
                    return $http.post($apiBase + 'Stream/TransferToUser/', {
                        'CaseStreamId': caseStreamId,
                        'UserId': userId,
                        'CurrentUserId':currentUserId,
                        'NewUserId': newUserId
                    }, {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).then(function (response) {
                        return response;
                    }, function (error) {
                        return error;
                    });
                },
                transferMultipleCaseStreamsTo: function (caseStreams, token) {
                    return $http.post($apiBase + 'Stream/TransferMultipleCaseStreamsToUser/', {
                      'ChangeAllocatedUserModels': caseStreams
                    }, {
                        headers: {
                            'Content-Type': 'application/json; charset=utf-8'
                        }
                    }).then(function(response) {
                        return response;
                    }, function(error) {
                        return error;
                    });
                }
            }
        }
    ]);

}(this.angular));