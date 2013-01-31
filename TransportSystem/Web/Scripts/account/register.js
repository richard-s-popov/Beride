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
                    // ele.removeClass('normal').addClass('wrong').css({ 'font-weight': 'normal' });
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
            } else {
                jVal.errors = true;
                error.html('Введите Email').show();
                ele.parent().css('border', '1px solid rgb(226, 79, 79)');
            }
        },
        'Phone': function () {

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
                $.ajax({
                    type: "POST",
                    url: document.SubmitRegisterForm,
                    data: {
                        Email: $('#Email', form).val(),
                        Phone: $('#Phone', form).val(),
                        Password: $('#Password', form).val()
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
        jVal.sendIt();
        
        return false;
    });
    
    $('#Password', form).change(jVal.Password);
    $('#ConfirmPassword', form).change(jVal.ConfirmPassword);
    $('#Email', form).change(jVal.Email);
    $('#Phone', form).change(jVal.Phone);
});