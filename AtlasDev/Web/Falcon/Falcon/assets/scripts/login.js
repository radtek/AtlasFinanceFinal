var Login = function() {

    var handleLogin = function() {

        $('.login-form').validate({
            errorElement: 'span', //default input error message container
            errorClass: 'help-block', // default input error message class
            focusInvalid: false, // do not focus the last invalid input
            rules: {
                username: {
                    required: true
                },
                password: {
                    required: true
                },
                remember: {
                    required: false
                }
            },

            messages: {
                username: {
                    required: "Username is required."
                },
                password: {
                    required: "Password is required."
                }
            },

            invalidHandler: function(event, validator) { //display error alert on form submit   
                $('.alert-danger', $('.login-form')).show();
            },

            highlight: function(element) { // hightlight error inputs
                $(element)
                    .closest('.form-group').addClass('has-error'); // set error class to the control group
            },

            success: function(label) {
                label.closest('.form-group').removeClass('has-error');
                label.remove();
            },

            errorPlacement: function(error, element) {
                error.insertAfter(element.closest('.input-icon'));
            }
        });

        $('.login-form input').keypress(function(e) {
            if (e.which === 13) {
                if ($('.login-form').validate().form()) {
                    $('#login').click(); //form validation success, call ajax form submit
                }
                return false;
            }
        });
    }

    var handleAssociate = function() {

        $('.login-form').validate({
            errorElement: 'span', //default input error message container
            errorClass: 'help-block', // default input error message class
            focusInvalid: false, // do not focus the last invalid input
            rules: {
                idnumber: {
                    required: true
                }
            },

            messages: {
                idnumber: {
                    required: "ID Number is required."
                }
            },

            invalidHandler: function(event, validator) { //display error alert on form submit   
                $('.alert-danger', $('.login-form')).show();
            },

            highlight: function(element) { // hightlight error inputs
                $(element)
                    .closest('.form-group').addClass('has-error'); // set error class to the control group
            },

            success: function(label) {
                label.closest('.form-group').removeClass('has-error');
                label.remove();
            },

            errorPlacement: function(error, element) {
                error.insertAfter(element.closest('.input-icon'));
            }
        });

        $('.login-form input').keypress(function(e) {
            if (e.which === 13) {
                if ($('.login-form').validate().form()) {
                    $('#login').click(); //form validation success, call ajax form submit
                }
                return false;
            }
        });
    }

    var handleForgetPassword = function() {
        $('.forget-form').validate({
            errorElement: 'span', //default input error message container
            errorClass: 'help-block', // default input error message class
            focusInvalid: false, // do not focus the last invalid input
            ignore: "",
            rules: {
                IDNo: {
                    required: true
                },
                CellNo: {
                    required: true
                },
                OTP: {
                    required: true
                }
            },

            messages: {
                IDNo: {
                    required: "ID No is required."
                },
                CellNo: {
                    required: "Cell No is required."
                },
                OTP: {
                    required: "OTP is required."
                }
            },

            invalidHandler: function(event, validator) { //display error alert on form submit   

            },

            highlight: function(element) { // hightlight error inputs
                $(element)
                    .closest('.form-group').addClass('has-error'); // set error class to the control group
            },

            success: function(label) {
                label.closest('.form-group').removeClass('has-error');
                label.remove();
            },

            errorPlacement: function(error, element) {
                error.insertAfter(element.closest('.input-icon'));
            },

            //!!!!!!!!!!!!!!!!! BREAKS EVERYTHING !!!!!!!!!!!!!!!\\

            /* submitHandler: function(form) {
                form.submit();
            }*/
        });

        $('.forget-form input').keypress(function(e) {
            if (e.which == 13) {
                $('.forget-form').validate().form();
                return false;
            }
        });

        jQuery('#forget-password').click(function() {
            jQuery('.login-form').hide();
            jQuery('.forget-form').show();
        });

        jQuery('#back-btn').click(function() {
            jQuery('.login-form').show();
            jQuery('.forget-form').hide();
        });

    }

    var handleResetOTP = function() {
        $('.login-form').validate({
            errorElement: 'span', //default input error message container
            errorClass: 'help-block', // default input error message class
            focusInvalid: false, // do not focus the last invalid input
            ignore: "",
            rules: {
                OTP: {
                    required: true
                }
            },

            messages: {
                OTP: {
                    required: "OTP is required."
                }
            },

            invalidHandler: function(event, validator) { //display error alert on form submit   

            },

            highlight: function(element) { // hightlight error inputs
                $(element)
                    .closest('.form-group').addClass('has-error'); // set error class to the control group
            },

            success: function(label) {
                label.closest('.form-group').removeClass('has-error');
                label.remove();
            },

            errorPlacement: function(error, element) {
                error.insertAfter(element.closest('.input-icon'));
            },
        });

        $('.login-form input').keypress(function(e) {
            if (e.which == 13) {
                $('.login-form').validate().form();
                return false;
            }
        });


    };

    var handleRegister = function() {

        function format(state) {
            if (!state.id) return state.text; // optgroup
            return "<img class='flag' src='../../assets/global/img/flags/" + state.id.toLowerCase() + ".png'/>&nbsp;&nbsp;" + state.text;
        }

        if (jQuery().select2) {
            $("#select2_sample4").select2({
                placeholder: '<i class="fa fa-map-marker"></i>&nbsp;Select a Country',
                allowClear: true,
                formatResult: format,
                formatSelection: format,
                escapeMarkup: function(m) {
                    return m;
                }
            });


            $('#select2_sample4').change(function() {
                $('.register-form').validate().element($(this)); //revalidate the chosen dropdown value and show error or success message for the input
            });
        }

        $('.register-form').validate({
            errorElement: 'span', //default input error message container
            errorClass: 'help-block', // default input error message class
            focusInvalid: false, // do not focus the last invalid input
            ignore: "",
            rules: {

                fullname: {
                    required: true
                },
                email: {
                    required: true,
                    email: true
                },
                address: {
                    required: true
                },
                city: {
                    required: true
                },
                country: {
                    required: true
                },

                username: {
                    required: true
                },
                password: {
                    required: true
                },
                rpassword: {
                    equalTo: "#register_password"
                },

                tnc: {
                    required: true
                }
            },

            messages: { // custom messages for radio buttons and checkboxes
                tnc: {
                    required: "Please accept TNC first."
                }
            },

            invalidHandler: function(event, validator) { //display error alert on form submit   

            },

            highlight: function(element) { // hightlight error inputs
                $(element)
                    .closest('.form-group').addClass('has-error'); // set error class to the control group
            },

            success: function(label) {
                label.closest('.form-group').removeClass('has-error');
                label.remove();
            },

            errorPlacement: function(error, element) {
                if (element.attr("name") == "tnc") { // insert checkbox errors after the container                  
                    error.insertAfter($('#register_tnc_error'));
                } else if (element.closest('.input-icon').size() === 1) {
                    error.insertAfter(element.closest('.input-icon'));
                } else {
                    error.insertAfter(element);
                }
            },

            submitHandler: function(form) {
                form.submit();
            }
        });

        $('.register-form input').keypress(function(e) {
            if (e.which == 13) {
                if ($('.register-form').validate().form()) {
                    $('.register-form').submit();
                }
                return false;
            }
        });

        jQuery('#register-btn').click(function() {
            jQuery('.login-form').hide();
            jQuery('.register-form').show();
        });

        jQuery('#register-back-btn').click(function() {
            jQuery('.login-form').show();
            jQuery('.register-form').hide();
        });
    }

    var handlePasswordChange = function () {
      $('#changePasswordForm').validate({
        errorElement: 'span', //default input error message container
        errorClass: 'help-block', // default input error message class
        focusInvalid: true, // do not focus the last invalid input
        rules: {
          username: {
            required: true
          },
          currentPassword: {
            required: true
          },
          newPassword: {
            required: true,
            minlength: 6
          },
          confirmNewPassword: {
            required: true,
            minlength: 6,
            equalTo: "#newPassword"
          }
        },

        messages: {
          username: {
            required: "Username is required."
          },
          currentPassword: {
            required: "Current Password is required."
          },
          newPassword: {
            required: "New Password is required",
            minlength: "Password too short. Min 6 characters"
          },
          confirmNewPassword: {
            required: "Confirm New Password is required",
            minlength: "Password too short. Min 6 characters",
            equalTo: "Passwords does not match"
          }
        },

        invalidHandler: function (event, validator) { //display error alert on form submit   
          $('.alert-danger', $('#changePasswordForm')).show();
        },

        highlight: function (element) { // hightlight error inputs
          $(element)
              .closest('.form-group').addClass('has-error'); // set error class to the control group
        },

        success: function (label) {
          label.closest('.form-group').removeClass('has-error');
          label.remove();
        },

        errorPlacement: function (error, element) {
          error.insertAfter(element.closest('.input-icon'));
        },
        
        submitHandler: function (form) {
          debugger;
          form.submit();
        }
      });

      $('#changePasswordForm .input').keypress(function (e) {
        if (e.which === 13) {
          if ($('#changePasswordForm').validate().form()) {
            $('#btnSubmit').click(); //form validation success, call ajax form submit
          }
          return false;
        }
      });
    
  };
  return {
        initForgetPassword: function() {
            handleForgetPassword();
        },
        initLogin: function() {
            handleLogin();
        },
        initOTP: function() {
            handleResetOTP();
        },
        initAssociate: function() {
            handleAssociate();
        },
      initChangePassword:function() {
        handlePasswordChange();
      }
    };
}();