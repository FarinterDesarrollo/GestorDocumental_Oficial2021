$(document).ready(function () {

        $("#ConfCarpetaDetalleId").empty();

    $.getJSON('/MantenimientoSubniveles/GetCarpetaDetalleEncabezadoId', { idcarpeta: $('#CarpetaEncabezadoid').val() }, function (data) {
            $.each(data, function () {
                $('#ConfCarpetaDetalleId').append('<option value=' +
                    this.Value + '>' + this.Text + '</option>');
            });
        });
 
});
