using Fonbec.Cartas.DataAccess.Entities;
using Fonbec.Cartas.DataAccess.Entities.Actors;

namespace Fonbec.Cartas.Logic.Constants
{
    public static class ImportFileNameConstants
    {
        private static readonly Dictionary<Type, string> FileNames = new()
        {
            { typeof(Apadrinamiento), "apadrinamientos.csv" },
            { typeof(Becario), "becarios.csv" },
            { typeof(Coordinador), "coordinadores.csv" },
            { typeof(SendAlsoTo), "enviar-copia.csv" },
            { typeof(Mediador), "mediadores.csv" },
            { typeof(Padrino), "padrinos.csv" },
            { typeof(Revisor), "revisores.csv" },
        };

        private static readonly Dictionary<Type, string> Headers = new()
        {
            { typeof(Apadrinamiento), "becario,padrino,desde,hasta" },
            { typeof(Becario), "nombre,apellido,apodo,sexo,nivel,email,teléfono,mediador" },
            { typeof(Coordinador), "nombre,apellido,apodo,sexo,email,teléfono,usuario,clave" },
            { typeof(SendAlsoTo), "padrino,nombre-completo,email,bcc" },
            { typeof(Mediador), "nombre,apellido,apodo,sexo,email,teléfono,usuario,clave" },
            { typeof(Padrino), "nombre,apellido,apodo,sexo,email,teléfono" },
            { typeof(Revisor), "nombre,apellido,apodo,sexo,email,teléfono,usuario,clave" },
        };

        public static string FileNameOf(Type t)
        {
            if (!FileNames.TryGetValue(t, out var value))
            {
                throw new InvalidOperationException($"{typeof(ImportFileNameConstants)}: Invalid type {t.Name} in {nameof(FileNameOf)}");
            }

            return value;
        }

        public static string HeaderOf(Type t)
        {
            if (!Headers.TryGetValue(t, out var value))
            {
                throw new InvalidOperationException($"{typeof(ImportFileNameConstants)}: Invalid type {t.Name} in {nameof(HeaderOf)}");
            }

            return value;
        }

        public static readonly Type[] Types = FileNames.Keys.ToArray();

        public static readonly string[] ExpectedFileNames = FileNames.Values.ToArray();

        public static string FilesList() => string.Join(", ", ExpectedFileNames.OrderBy(f => f));
    }
}
