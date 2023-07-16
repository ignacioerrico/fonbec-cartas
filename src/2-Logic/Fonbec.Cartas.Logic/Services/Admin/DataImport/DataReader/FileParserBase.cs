using Fonbec.Cartas.DataAccess.Entities.Enums;
using Fonbec.Cartas.Logic.Constants;
using Fonbec.Cartas.Logic.ExtensionMethods;
using Fonbec.Cartas.Logic.Services.Admin.DataImport.DataReader.Payloads;
using System.ComponentModel.DataAnnotations;

namespace Fonbec.Cartas.Logic.Services.Admin.DataImport.DataReader
{
    public abstract class FileParserBase<T, TOut>
    {
        protected string FileName;

        protected string Header;

        protected int NumberOfFields;

        protected FileParserBase()
        {
            FileName = ImportFileNameConstants.FileNameOf(typeof(T));
            Header = ImportFileNameConstants.HeaderOf(typeof(T));
            NumberOfFields = Header.Split(',').Length;
        }

        public virtual async Task<List<TOut>> ConvertToObjects(string fileContents, IDataReaderPayload<T> payload, List<string> errors)
        {
            List<TOut> ts = new();

            var existing = await GetExistingObjectsAsync();
            List<TOut> existingTs = new(existing);

            var lineNumber = 1;
            var headerHasBeenProcessed = false;

            using var stringReader = new StringReader(fileContents);
            while (await stringReader.ReadLineAsync() is { } line)
            {
                if (string.IsNullOrEmpty(line))
                {
                    lineNumber++;
                    continue;
                }

                if (!headerHasBeenProcessed)
                {
                    if (line != Header)
                    {
                        AddError(errors, lineNumber, "wrong header or header missing");
                    }

                    lineNumber++;
                    headerHasBeenProcessed = true;
                    continue;
                }

                var fields = line.Split(',')
                    .Select(f => f.Trim())
                    .ToList();

                if (fields.Count != NumberOfFields)
                {
                    AddError(errors, lineNumber, $"wrong number of fields ({fields.Count} instead of the expected {NumberOfFields})");
                    lineNumber++;
                    continue;
                }

                var t = CreateObjectFromFields(fields, payload, lineNumber, errors);

                var (isDuplicate, duplicateInImportFile) = IsElementInCollection(t, ts);
                var (alreadyExisted, duplicateInDatabase) = IsElementInCollection(t, existingTs);
                if (isDuplicate || alreadyExisted)
                {
                    if (duplicateInImportFile is not null)
                    {
                        AddError(errors, lineNumber, $"'{GetKey(t)}' is a duplicate of '{duplicateInImportFile}' in {FileName}");
                    }
                    
                    if (duplicateInDatabase is not null)
                    {
                        AddError(errors, lineNumber, $"'{GetKey(t)}' is a duplicate of '{duplicateInDatabase}' in the database");
                    }
                }
                else
                {
                    ts.Add(t);
                }

                lineNumber++;
            }

            return ts;
        }

        protected abstract TOut CreateObjectFromFields(List<string> fields, IDataReaderPayload<T> payload, int lineNumber, List<string> errors);

        protected abstract Task<IEnumerable<TOut>> GetExistingObjectsAsync();

        protected abstract string GetKey(TOut t);

        protected (bool, string?) IsElementInCollection(TOut element, List<TOut> list)
        {
            var elementConsideredFound = false;

            var elementKey = GetKey(element);
            string? duplicateItem = null;

            foreach (var listKey in list.Select(GetKey))
            {
                elementConsideredFound = elementKey.Length > listKey.Length
                    ? elementKey.ContainsIgnoringAccents(listKey)
                    : listKey.ContainsIgnoringAccents(elementKey);

                if (elementConsideredFound)
                {
                    duplicateItem = listKey;
                    break;
                }
            }

            return (elementConsideredFound, duplicateItem);
        }

        protected void AddError(ICollection<string> errors,
            int lineNumber,
            string message) =>
            errors.Add($"{FileName}: line {lineNumber}: {message}");

        protected void ValidateFirstName(string firstName, List<string> errors, int lineNumber)
        {
            if (string.IsNullOrEmpty(firstName))
            {
                AddError(errors, lineNumber, "first name cannot be empty");
            }
        }

        protected Gender ValidateGender(string genderString, List<string> errors, int lineNumber)
        {
            Gender gender;
            if (genderString is "M" or "F")
            {
                gender = genderString == "M"
                    ? Gender.Male
                    : Gender.Female;
            }
            else
            {
                AddError(errors, lineNumber, $"gender must be either 'M' or 'F'; '{genderString}' was found");
                gender = Gender.Unknown;
            }

            return gender;
        }

        protected void ValidateEmail(string? email, List<string> errors, int lineNumber, bool isRequired)
        {
            if (isRequired && string.IsNullOrEmpty(email))
            {
                AddError(errors, lineNumber, "email cannot be empty");
            }
            else if (!new EmailAddressAttribute().IsValid(email))
            {
                AddError(errors, lineNumber, $"'{email}' is not a valid email address");
            }
        }
    }
}
