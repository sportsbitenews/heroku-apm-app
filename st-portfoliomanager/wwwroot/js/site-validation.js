$.validator.setDefaults({
    highlight: function (element) {
        $(element).closest('.form-group').addClass('has-error');
        $("span[data-valmsg-for=" + element.name + "]").addClass('help-block');
    },
    unhighlight: function (element) {
        $(element).closest('.form-group').removeClass('has-error');
        $("span[data-valmsg-for=" + element.name + "]").removeClass('help-block');
    },
    success: function (element) {
        $(element).closest('.form-group').addClass('has-success');
        $("span[data-valmsg-for=" + element.name + "]").addClass('help-block');
    },
    errorClass: 'form-control-error',
    errorPlacement: function (error, element) {
    },
    ignore: ''
});

$(function () {
    $('.input-validation-error').parents('.form-group').addClass('has-error');
    $('.field-validation-error').addClass('text-danger');

    $('.input-validation-error').on('focus', function () {
        $(this).parents('.form-group').removeClass('has-error');
        $(this).attr('class', 'form-control');
        $(this).next('span.field-validation-error').attr('class', 'field-validation-valid').html('');
    })
});