$(function () {
    $('.sb-dropdown > a').on('click', function () {
        $(this).parent().find('ul').toggleClass('d-none');
        $(this).find('.arrow').toggleClass('arrow-rotar');
    });
}, jQuery);