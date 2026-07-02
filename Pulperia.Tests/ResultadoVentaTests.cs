using Pulperia.Services;
using Xunit;

namespace Pulperia.Tests;

/// <summary>
/// Pruebas del record <see cref="ResultadoVenta"/>, el objeto que
/// <see cref="VentaService"/> devuelve para comunicar éxito o fallo. Verifican
/// que sus fábricas <c>Ok</c> y <c>Fallo</c> dejen el objeto en un estado
/// coherente (un resultado exitoso nunca trae error, y viceversa).
/// </summary>
public class ResultadoVentaTests
{
    [Fact]
    public void Ok_marca_exito_con_id_y_sin_error()
    {
        var resultado = ResultadoVenta.Ok(42);

        Assert.True(resultado.Exito);
        Assert.Equal(42, resultado.VentaId);
        Assert.Null(resultado.Error);
    }

    [Fact]
    public void Fallo_marca_error_sin_id()
    {
        var resultado = ResultadoVenta.Fallo("Algo salió mal");

        Assert.False(resultado.Exito);
        Assert.Null(resultado.VentaId);
        Assert.Equal("Algo salió mal", resultado.Error);
    }
}
