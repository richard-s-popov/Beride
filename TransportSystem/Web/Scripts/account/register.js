$(document).ready(function () {
    var form = $('#registerForm');
    var jVal = {
        'Email': function () {
            var ele = $('#Email', form);
            var error = ele.parent().parent().children('.error');
            var patt = /^.+@.+[.].{2,}$/i;
            
            if ($.trim(ele.val()).length > 0) {
                if (!patt.test(ele.val())) {
                    jVal.errors = true;
                    error.html('Введите корректный Email').show();
                    ele.parent().css('border', '1px solid rgb(226, 79, 79)');
                } else if (ele.val().length > 50) {
                    jVal.errors = true;
                    error.html('Превышен максимальный размер 50 символов').show();
                    ele.parent().css('border', '1px solid rgb(226, 79, 79)');
                } else {
                    $.getJSON(document.EmailCheckUrl + '?email=' + $.trim(ele.val()), function (data) {
                        var el = $('#Email', form);
                        var err = el.parent().parent().children('.error');
                        
                        if (data.result == true) {
                            jVal.errors = true;
                            err.html('Пользователь с таким Email-ом уже существует').show();
                            el.parent().css('border', '1px solid rgb(226, 79, 79)');
                        } else {
                            err.hide();
                            el.parent().css('border', '1px solid #CCCCCC');
                        }
                    });
                }
            } else if ($('#Phone', form).val().length > 0) {
                error.hide();
                ele.parent().css('border', '1px solid #CCCCCC');
            } else {
                jVal.errors = true;
                error.html('Введите Email').show();
                ele.parent().css('border', '1px solid rgb(226, 79, 79)');
            }
        },
        'Phone': function () {
            var ele = $('#Phone', form);
            var error = ele.parent().parent().children('.error');
            var patt = /[0-9]$/i;

            if ($.trim(ele.val()).length > 0) {
                if (!patt.test(ele.val())) {
                    jVal.errors = true;
                    error.html('Номер телефона может содержать только цифры').show();
                    ele.parent().css('border', '1px solid rgb(226, 79, 79)');
                } else if (ele.val().length > 10) {
                    jVal.errors = true;
                    error.html('Превышен максимальный размер 10 цифр').show();
                    ele.parent().css('border', '1px solid rgb(226, 79, 79)');
                } else {
                    $.getJSON(document.PhoneCheckUrl + '?phonenumber=' + $('#countryCode :selected').val() + $.trim(ele.val()), function (data) {
                        var el = $('#Phone', form);
                        var err = el.parent().parent().children('.error');

                        if (data.result == true) {
                            jVal.errors = true;
                            err.html('Пользователь с таким телефоном уже существует').show();
                            el.parent().css('border', '1px solid rgb(226, 79, 79)');
                        } else {
                            err.hide();
                            el.parent().css('border', '1px solid #CCCCCC');
                        }
                    });
                }
            } else if ($('#Email', form).val().length > 0) {
                error.hide();
                ele.parent().css('border', '1px solid #CCCCCC');
            } else {
                jVal.errors = true;
                error.html('Введите номер телефона').show();
                ele.parent().css('border', '1px solid rgb(226, 79, 79)');
            }
        },
        'Password': function () {
            var ele = $('#Password', form);
            var error = ele.parent().parent().children('.error');
            
            if (ele.val().length == 0) {
                jVal.errors = true;
                error.html('Введите пароль').show();
                ele.parent().css('border', '1px solid rgb(226, 79, 79)');
            } else if (ele.val().length > 32 || ele.val().length < 6) {
                jVal.errors = true;
                error.html('Длина пароля должна быть от 6 до 32 символов').show();
                ele.parent().css('border', '1px solid rgb(226, 79, 79)');
            } else if ($('#ConfirmPassword').val().length > 0 && $('#ConfirmPassword').val() != ele.val()) {
                jVal.errors = true;
                error.html('Пароли не совпадают').show();
                ele.parent().css('border', '1px solid rgb(226, 79, 79)');
            } else {
                error.hide();
                ele.parent().css('border', '1px solid #CCCCCC');
            }
        },
        'ConfirmPassword': function () {
            var ele = $('#ConfirmPassword', form);
            var error = ele.parent().parent().children('.error');
            
            if (ele.val().length == 0) {
                jVal.errors = true;
                error.html('Введите пароль').show();
                ele.parent().css('border', '1px solid rgb(226, 79, 79)');
            } else if (ele.val().length > 32 || ele.val().length < 6) {
                jVal.errors = true;
                error.html('Длина пароля должна быть от 6 до 32 символов').show();
                ele.parent().css('border', '1px solid rgb(226, 79, 79)');
            } else if ($('#Password').val().length > 0 && $('#Password').val() != ele.val()) {
                jVal.errors = true;
                error.html('Пароли не совпадают').show();
                ele.parent().css('border', '1px solid rgb(226, 79, 79)');
            } else {
                if ($('#Password').val().length > 0) {
                    jVal.Password();
                }
                error.hide();
                ele.parent().css('border', '1px solid #CCCCCC');
            }
        },
        'sendIt': function() {
            if (!jVal.errors) {
                var phone = $('#Phone', form).val().length > 0 ? $('#countryCode :selected').val() + $('#Phone', form).val() : "";
                
                $.ajax({
                    type: "POST",
                    url: document.SubmitRegisterForm,
                    data: {
                        Email: $('#Email', form).val(),
                        Phone: phone,
                        Password: $('#Password', form).val(),
                        Code: $('#ConfirmCode').val()
                    },
                    dataType: "json",
                    success: function(data) {
                        data = JSON.parse(data);
                        
                        if (data) {
                            $('#registrationContainer').hide();
                            $('#loginContainer').show();
                            $('.message').show();
                        }
                    }
                });
            }
        }
    };
    
    $('#registerButton').click(function () {
        jVal.errors = false;
        jVal.Email();
        jVal.Phone();
        jVal.Password();
        jVal.ConfirmPassword();

        if ( $.trim($('#Phone').val()).length > 0 && !jVal.errors) {
            $('#displayPhoneNumber').html('+' + $('#countryCode :selected').val() + ' ' + $.trim($('#Phone').val()));
            $('#PhoneVerified').val($.trim($('#Phone').val()));

            $('.register-container').hide();
            $('.confirm-container').show();
            $.fancybox.resize();
        } else {
            jVal.sendIt();
        }
        
        return false;
    });

    $('#getVerificationCode').click(function() {
        $.ajax({
            url: document.SendVerificationCodeUrl,
            data: { phonenumber: $('#countryCode :selected').val() + $('#PhoneVerified').val() },
            success: function(data) {
                console.log(data.result);
            }
        });
    });

    $('#finishRegisterButton').click(function() {
        $.ajax({
            url: document.CheckVerificationCodeUrl,
            data: { code: $('#ConfirmCode').val() },
            success: function (data) {
                if (data.result) {
                    jVal.errors = false;
                    jVal.Email();
                    jVal.Phone();
                    jVal.Password();
                    jVal.ConfirmPassword();
                    jVal.sendIt();
                }
            }
        });
    });

    $('#changePhoneNumber').click(function() {
        $('.phone-number').hide();
        $('.phone').show();
    });
    
    $('#Password', form).change(jVal.Password);
    $('#ConfirmPassword', form).change(jVal.ConfirmPassword);
    $('#Email', form).change(jVal.Email);
    $('#Phone', form).change(jVal.Phone);
});