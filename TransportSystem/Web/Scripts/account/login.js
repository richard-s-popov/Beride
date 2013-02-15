$(document).ready(function () {
    var form = $('#loginForm');

    var jVal = {
        'Login': function() {
            var ele = $('#Login', form);
            var error = ele.parent().parent().children('.error');

            if (ele.val().length == 0) {
                jVal.errors = true;
                error.html('Введите логин').show();
                ele.parent().css('border', '1px solid rgb(226, 79, 79)');
            } else if (ele.val().length > 30 || ele.val().length < 6) {
                jVal.errors = true;
                error.html('Длина логина может быть не больше 30 символов').show();
                ele.parent().css('border', '1px solid rgb(226, 79, 79)');
            } else {
                error.hide();
                ele.parent().css('border', '1px solid #CCCCCC');
            }
        },
        'Password': function() {
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
            } else {
                error.hide();
                ele.parent().css('border', '1px solid #CCCCCC');
            }
        },
        'sendIt': function () {
            if (!jVal.errors) {
                $.ajax({
                    type: "POST",
                    url: document.SubmitLoginForm,
                    data: {
                        Login: $('#Login', form).val(),
                        Password: $('#Password', form).val()
                    },
                    dataType: "json",
                    success: function (data) {
                        data = JSON.parse(data);

                        if (data) {
                            $.fancybox.close();
                            $('#signIn').hide();
                            $('#logOut').show();
                            
                            if (typeof(window.callbackCabinet) == "function") {
                                window.callbackCabinet();
                            }
                            
                            if (typeof createAfterLogin === 'undefined') {
                                $('#createThis').click();
                            }
                        }
                    }
                });
            }
        }
    };

    $('#login').click(function () {
        jVal.errors = false;
        jVal.Login();
        jVal.Password();
        jVal.sendIt();

        return false;
    });
    
    $('#Password', form).change(jVal.Password);
    $('#Login', form).change(jVal.Login);
});