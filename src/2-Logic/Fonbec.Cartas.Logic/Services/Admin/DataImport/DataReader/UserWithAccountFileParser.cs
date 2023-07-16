using Fonbec.Cartas.DataAccess.Entities.Actors.Abstract;
using Fonbec.Cartas.DataAccess.Repositories.Admin.DataImport;
using Fonbec.Cartas.Logic.Models.Admin.DataImport;
using Fonbec.Cartas.Logic.Services.Admin.DataImport.DataReader.Payloads;
using Mapster;

namespace Fonbec.Cartas.Logic.Services.Admin.DataImport.DataReader
{
    public class UserWithAccountFileParser<T> : FileParserBase<T, UserWithAccountToCreate>
        where T : UserWithAccount
    {
        private readonly ICreateUserWithAccountRepository<T> _createUserWithAccountRepository;

        public UserWithAccountFileParser(ICreateUserWithAccountRepository<T> createUserWithAccountRepository)
        {
            _createUserWithAccountRepository = createUserWithAccountRepository;
        }

        protected override UserWithAccountToCreate CreateObjectFromFields(List<string> fields, IDataReaderPayload<T> payload, int lineNumber, List<string> errors)
        {
            var userWithAccountPayload = (UserWithAccountPayload<T>)payload;

            var firstName = fields[0];
            var lastName = string.IsNullOrEmpty(fields[1]) ? null : fields[1];
            var nickName = string.IsNullOrEmpty(fields[2]) ? null : fields[2];
            var genderString = fields[3];
            var email = fields[4];
            var phone = string.IsNullOrEmpty(fields[5]) ? null : fields[5];
            var username = fields[6];
            var password = fields[7];

            ValidateFirstName(firstName, errors, lineNumber);

            var gender = ValidateGender(genderString, errors, lineNumber);

            ValidateEmail(email, errors, lineNumber, isRequired: true);

            if (string.IsNullOrEmpty(username))
            {
                AddError(errors, lineNumber, "username cannot be empty");
            }
            else if (userWithAccountPayload.UserWithAccountService.UsernameExists(username))
            {
                AddError(errors, lineNumber, $"username '{username}' already exists");
            }

            if (string.IsNullOrEmpty(password))
            {
                AddError(errors, lineNumber, "password cannot be empty");
            }
            else if (!string.IsNullOrEmpty(username))
            {
                var (isPasswordValid, passwordValidationErrors) = userWithAccountPayload.UserWithAccountService.ValidatePassword(username, password).Result;
                if (!isPasswordValid)
                {
                    AddError(errors, lineNumber, $"password validation error: {string.Join(", ", passwordValidationErrors)}");
                }
            }

            var user = new UserWithAccountToCreate
            {
                FirstName = firstName,
                LastName = lastName,
                NickName = nickName,
                Gender = gender,
                Email = email,
                Phone = phone,
                Username = username,
                Password = password,
            };

            return user;
        }

        protected override async Task<IEnumerable<UserWithAccountToCreate>> GetExistingObjectsAsync()
        {
            var usersWithAccount = await _createUserWithAccountRepository.GetAllAsync();
            return usersWithAccount.Adapt<IEnumerable<UserWithAccountToCreate>>();
        }

        protected override string GetKey(UserWithAccountToCreate t) =>
            string.IsNullOrWhiteSpace(t.LastName)
                ? t.FirstName
                : $"{t.FirstName} {t.LastName}";
    }
}
