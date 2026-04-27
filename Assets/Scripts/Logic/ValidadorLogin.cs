using System.Text.RegularExpressions;

public class ValidadorLogin
{
    private readonly string[] palabrasProhibidas = {
        "puta", "puto", "mierda", "cabron", "pendejo", "chinga",
        "verga", "culo", "idiota", "estupido", "fuck", "shit", "bitch",
        "damn", "ass", "perra", "culero", "wey", "guey"
    };

    public bool ValidarAlias(string alias, out string error, bool esLogin = false)
    {
        if (string.IsNullOrEmpty(alias))
        { error = "El alias no puede estar vacío"; return false; }

        if (alias.Length < 1 || alias.Length > 20)
        { error = "El alias debe tener entre 1 y 20 caracteres"; return false; }

        if (!Regex.IsMatch(alias, @"^[a-zA-Z0-9_áéíóúÁÉÍÓÚñÑ]+$"))
        { error = "El alias contiene caracteres no permitidos"; return false; }

        if (!esLogin)
        {
            string aliasLower = alias.ToLower();
            foreach (var palabra in palabrasProhibidas)
                if (aliasLower.Contains(palabra))
                { error = "El alias contiene palabras no permitidas"; return false; }
        }

        error = null;
        return true;
    }

    public bool ValidarNipLogin(string nip, out string error)
    {
        if (string.IsNullOrEmpty(nip) || !Regex.IsMatch(nip, @"^\d{4}$"))
        { error = "El NIP debe tener exactamente 4 dígitos numéricos"; return false; }
        error = null;
        return true;
    }

    public bool ValidarCorreo(string correo, out string error)
    {
        if (string.IsNullOrEmpty(correo))
        { error = "El correo no puede estar vacío"; return false; }
        if (!Regex.IsMatch(correo, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        { error = "Ingresa un correo válido"; return false; }
        error = null;
        return true;
    }

    public bool ValidarAnio(string anioStr, out string error)
    {
        if (string.IsNullOrEmpty(anioStr))
        { error = "El año no puede estar vacío"; return false; }

        if (!int.TryParse(anioStr, out int anio))
        { error = "El año debe ser un número"; return false; }

        int anioActual = System.DateTime.Now.Year;
        int minimo     = anioActual - 100;
        int maximo     = anioActual - 3;

        if (anio < minimo || anio > maximo)
        { error = $"Año inválido. Debe estar entre {minimo} y {maximo}"; return false; }

        error = null;
        return true;
    }

    public int GenerarNip()
    {
        var rng = new System.Random();
        return rng.Next(1000, 10000);
    }
}
