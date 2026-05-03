var Validation = {

    IsEmpty: function (ctrl) {
        var value = $.trim($(ctrl).val());
        $(ctrl).val(value);

        if ($(ctrl).attr('datarequired') == 'true' && value === "") {
            $(ctrl).removeClass('valid').addClass('required');
            $(ctrl).next('.custom-error').text("This field is required");
            return false;
        }

        $(ctrl).removeClass('required').addClass('valid');
        $(ctrl).next('.custom-error').text('');
        return true;
    },

    IsEmail: function (ctrl) {
        var value = $.trim($(ctrl).val());
        var regexp = /^[\+a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/;

        if (value === "") {
            $(ctrl).removeClass('valid').addClass('required');
            $(ctrl).next('.custom-error').text("Email is required");
            return false;
        }

        if (!regexp.test(value)) {
            $(ctrl).removeClass('valid').addClass('required');
            $(ctrl).next('.custom-error').text("Invalid email format");
            return false;
        }

        $(ctrl).removeClass('required').addClass('valid');
        $(ctrl).next('.custom-error').text('');
        return true;
    },

    IsPassword: function (ctrl) {
        var value = $(ctrl).val();
        if (value === "") {
            $(ctrl).removeClass('valid').addClass('required');
            $(ctrl).next('.custom-error').text("Password is required");
            return false;
        }

        if (value.length < 6) {
            $(ctrl).removeClass('valid').addClass('required');
            $(ctrl).next('.custom-error').text("Password must be at least 6 characters");
            return false;
        }

        $(ctrl).removeClass('required').addClass('valid');
        $(ctrl).next('.custom-error').text('');
        return true;
    },

    ForTextbox: function (ctrl) {
        if (!Validation.IsEmpty(ctrl)) return false;

        var format = $(ctrl).attr('dataformat');
        if (format === 'email') return Validation.IsEmail(ctrl);
        if (format === 'password') return Validation.IsPassword(ctrl);

        return true;
    },

    ForSelect: function (ctrl) {
        var val = $(ctrl).val();
        if ($(ctrl).attr('datarequired') == 'true' && (val === "" || val === "-1")) {
            $(ctrl).removeClass('valid').addClass('required');
            $(ctrl).next('.custom-error').text("Please select an option");
            return false;
        }
        $(ctrl).removeClass('required').addClass('valid');
        $(ctrl).next('.custom-error').text('');
        return true;
    },

    ForCheckBox: function (ctrl) {
        if ($(ctrl).attr('checkedrequired') == 'true' && !$(ctrl).is(':checked')) {
            $(ctrl).removeClass('valid').addClass('required');
            $(ctrl).next('.custom-error').text("This checkbox is required");
            return false;
        }
        $(ctrl).removeClass('required').addClass('valid');
        $(ctrl).next('.custom-error').text('');
        return true;
    }
};

// Universal form validation
var CommonUiValidation = function () {
    var result = true;
    $('input, textarea').each(function () {
        if ($(this).is(':visible') && !Validation.ForTextbox(this)) result = false;
    });
    $('select').each(function () {
        if ($(this).is(':visible') && !Validation.ForSelect(this)) result = false;
    });
    $('input[type="checkbox"]').each(function () {
        if ($(this).is(':visible') && !Validation.ForCheckBox(this)) result = false;
    });
    return result;
};

$(document).ready(function () {

    $('input, textarea').blur(function () {
        Validation.ForTextbox(this);
    });

    $('select').change(function () {
        Validation.ForSelect(this);
    });

    $('input[type="checkbox"]').change(function () {
        Validation.ForCheckBox(this);
    });

    $('#customForm').submit(function (e) {
        if (!CommonUiValidation()) {
            e.preventDefault();
            alert('Please fix errors before submitting!');
        }
    });

});
