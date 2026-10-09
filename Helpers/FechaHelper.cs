using System.Globalization;

namespace UsuariosApi.Helpers;

public static class FechaHelper
{
    public const string Formato = "yyyy-MM-dd";

    /// <summary>Edad exacta considerando año, mes y día.</summary>
    public static int CalcularEdad(DateOnly nacimiento, DateOnly hoy)
    {
        var edad = hoy.Year - nacimiento.Year;

        // Si todavía no ha cumplido años este año, se resta uno
        if (hoy < nacimiento.AddYears(edad))
        {
            edad--;
        }

        return edad;
    }

    /// <summary>
    /// Convierte el texto yyyy-MM-dd en fecha y valida que sea coherente.
    /// Lanza InvalidOperationException (→ HTTP 400 con { mensaje }) si no lo es.
    /// </summary>
    public static DateOnly ParsearFechaNacimiento(string texto)
    {
        if (!DateOnly.TryParseExact(
                texto?.Trim(),
                Formato,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var fecha))
        {
            throw new InvalidOperationException(
                "La fecha de nacimiento no es válida. Use el formato yyyy-MM-dd.");
        }

        var hoy = DateOnly.FromDateTime(DateTime.Today);

        if (fecha > hoy)
        {
            throw new InvalidOperationException(
                "La fecha de nacimiento no puede ser futura.");
        }

        if (CalcularEdad(fecha, hoy) > 120)
        {
            throw new InvalidOperationException(
                "La fecha de nacimiento no es válida.");
        }

        return fecha;
    }
}
