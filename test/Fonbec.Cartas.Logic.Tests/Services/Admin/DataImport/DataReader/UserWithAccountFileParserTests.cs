using FluentAssertions.Execution;
using FluentAssertions;
using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.DataAccess.Entities.Actors.Abstract;
using Fonbec.Cartas.DataAccess.Entities.Enums;
using Fonbec.Cartas.DataAccess.Repositories.Admin.DataImport;
using Fonbec.Cartas.Logic.Services.Admin.DataImport.DataReader;
using Fonbec.Cartas.Logic.Services.Admin.DataImport.DataReader.Payloads;
using Moq;
using System.Text;
using Fonbec.Cartas.Logic.Services.Admin;

namespace Fonbec.Cartas.Logic.Tests.Services.Admin.DataImport.DataReader
{
    public abstract class UserWithAccountFileParserTests<T> where T : UserWithAccount
    {
        private readonly UserWithAccountFileParser<T> _sut;
        
        private readonly UserWithAccountPayload<T> _padrinoPayload;

        protected UserWithAccountFileParserTests()
        {
            var userWithAccountSharedServiceMock = new Mock<IUserWithAccountSharedService>();

            userWithAccountSharedServiceMock
                .Setup(x => x.UsernameExists("usuario"))
                .Returns(false);

            userWithAccountSharedServiceMock
                .Setup(x => x.ValidatePassword("usuario", "Password1!"))
                .ReturnsAsync((true, new List<string>()));

            _padrinoPayload = new UserWithAccountPayload<T>
            {
                UserWithAccountService = userWithAccountSharedServiceMock.Object,
            };

            var createUserWithAccountRepository = new Mock<ICreateUserWithAccountRepository<T>>();

            createUserWithAccountRepository
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<UserWithAccount>());

            _sut = new UserWithAccountFileParser<T>(createUserWithAccountRepository.Object);
        }

        protected abstract string FileName { get; }

        [Fact]
        public async Task ConvertToObjects_Success_AllFieldsSpecified()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("nombre,apellido,apodo,sexo,email,teléfono,usuario,clave");
            fileContents.AppendLine("CoordinadorNombre, CoordinadorApellido, CoordinadorApodo, F, coordinador@email.com, Phone, usuario, Password1!");

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), _padrinoPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().BeEmpty();

                result.Should().ContainSingle();

                result.Single().FirstName.Should().Be("CoordinadorNombre");
                result.Single().LastName.Should().Be("CoordinadorApellido");
                result.Single().NickName.Should().Be("CoordinadorApodo");
                result.Single().Gender.Should().Be(Gender.Female);
                result.Single().Email.Should().Be("coordinador@email.com");
                result.Single().Phone.Should().Be("Phone");
                result.Single().Username.Should().Be("usuario");
                result.Single().Password.Should().Be("Password1!");
            }
        }

        [Fact]
        public async Task ConvertToObjects_Success_OnlyRequiredFieldsAreSpecified()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("nombre,apellido,apodo,sexo,email,teléfono,usuario,clave");
            fileContents.AppendLine("CoordinadorNombre, , , F, coordinador@email.com, , usuario, Password1!");

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), _padrinoPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().BeEmpty();

                result.Should().ContainSingle();

                result.Single().FirstName.Should().Be("CoordinadorNombre");
                result.Single().LastName.Should().BeNull();
                result.Single().NickName.Should().BeNull();
                result.Single().Gender.Should().Be(Gender.Female);
                result.Single().Email.Should().Be("coordinador@email.com");
                result.Single().Phone.Should().BeNull();
                result.Single().Username.Should().Be("usuario");
                result.Single().Password.Should().Be("Password1!");
            }
        }

        [Fact]
        public async Task ConvertToObjects_FirstNameCannotBeEmpty()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("nombre,apellido,apodo,sexo,email,teléfono,usuario,clave");
            fileContents.AppendLine(" , CoordinadorApellido, CoordinadorApodo, F, coordinador@email.com, Phone, usuario, Password1!");

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), _padrinoPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().ContainSingle();

                errors.Single().Should().Be($"{FileName}: line 2: first name cannot be empty");

                result.Should().ContainSingle();

                result.Single().FirstName.Should().BeEmpty();
                result.Single().LastName.Should().Be("CoordinadorApellido");
                result.Single().NickName.Should().Be("CoordinadorApodo");
                result.Single().Gender.Should().Be(Gender.Female);
                result.Single().Email.Should().Be("coordinador@email.com");
                result.Single().Phone.Should().Be("Phone");
                result.Single().Username.Should().Be("usuario");
                result.Single().Password.Should().Be("Password1!");
            }
        }

        [Fact]
        public async Task ConvertToObjects_GenderMustBeEitherMOrF()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("nombre,apellido,apodo,sexo,email,teléfono,usuario,clave");
            fileContents.AppendLine("CoordinadorNombre, CoordinadorApellido, CoordinadorApodo, X, coordinador@email.com, Phone, usuario, Password1!");

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), _padrinoPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().ContainSingle();

                errors.Single().Should().Be($"{FileName}: line 2: gender must be either 'M' or 'F'; 'X' was found");

                result.Should().ContainSingle();

                result.Single().FirstName.Should().Be("CoordinadorNombre");
                result.Single().LastName.Should().Be("CoordinadorApellido");
                result.Single().NickName.Should().Be("CoordinadorApodo");
                result.Single().Gender.Should().Be(Gender.Unknown);
                result.Single().Email.Should().Be("coordinador@email.com");
                result.Single().Phone.Should().Be("Phone");
                result.Single().Username.Should().Be("usuario");
                result.Single().Password.Should().Be("Password1!");
            }
        }

        [Fact]
        public async Task ConvertToObjects_EmailCannotBeEmpty()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("nombre,apellido,apodo,sexo,email,teléfono,usuario,clave");
            fileContents.AppendLine("CoordinadorNombre, CoordinadorApellido, CoordinadorApodo, F, , Phone, usuario, Password1!");

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), _padrinoPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().ContainSingle();

                errors.Single().Should().Be($"{FileName}: line 2: email cannot be empty");

                result.Should().ContainSingle();

                result.Single().FirstName.Should().Be("CoordinadorNombre");
                result.Single().LastName.Should().Be("CoordinadorApellido");
                result.Single().NickName.Should().Be("CoordinadorApodo");
                result.Single().Gender.Should().Be(Gender.Female);
                result.Single().Email.Should().BeEmpty();
                result.Single().Phone.Should().Be("Phone");
                result.Single().Username.Should().Be("usuario");
                result.Single().Password.Should().Be("Password1!");
            }
        }

        [Fact]
        public async Task ConvertToObjects_EmailMustBeAValidEmailAddress()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("nombre,apellido,apodo,sexo,email,teléfono,usuario,clave");
            fileContents.AppendLine("CoordinadorNombre, CoordinadorApellido, CoordinadorApodo, F, NotAValidEmailAddress, Phone, usuario, Password1!");

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), _padrinoPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().ContainSingle();

                errors.Single().Should().Be($"{FileName}: line 2: 'NotAValidEmailAddress' is not a valid email address");

                result.Should().ContainSingle();

                result.Single().FirstName.Should().Be("CoordinadorNombre");
                result.Single().LastName.Should().Be("CoordinadorApellido");
                result.Single().NickName.Should().Be("CoordinadorApodo");
                result.Single().Gender.Should().Be(Gender.Female);
                result.Single().Email.Should().Be("NotAValidEmailAddress");
                result.Single().Phone.Should().Be("Phone");
                result.Single().Username.Should().Be("usuario");
                result.Single().Password.Should().Be("Password1!");
            }
        }

        [Fact]
        public async Task ConvertToObjects_UsuarioCannotBeEmpty()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("nombre,apellido,apodo,sexo,email,teléfono,usuario,clave");
            fileContents.AppendLine("CoordinadorNombre, CoordinadorApellido, CoordinadorApodo, F, coordinador@email.com, Phone, , Password1!");

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), _padrinoPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().ContainSingle();

                errors.Single().Should().Be($"{FileName}: line 2: username cannot be empty");

                result.Should().ContainSingle();

                result.Single().FirstName.Should().Be("CoordinadorNombre");
                result.Single().LastName.Should().Be("CoordinadorApellido");
                result.Single().NickName.Should().Be("CoordinadorApodo");
                result.Single().Gender.Should().Be(Gender.Female);
                result.Single().Email.Should().Be("coordinador@email.com");
                result.Single().Phone.Should().Be("Phone");
                result.Single().Username.Should().BeEmpty();
                result.Single().Password.Should().Be("Password1!");
            }
        }

        [Fact]
        public async Task ConvertToObjects_UsuarioAlreadyExists()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("nombre,apellido,apodo,sexo,email,teléfono,usuario,clave");
            fileContents.AppendLine("CoordinadorNombre, CoordinadorApellido, CoordinadorApodo, F, coordinador@email.com, Phone, usuario, Password1!");

            var errors = new List<string>();

            var userWithAccountSharedServiceMock = new Mock<IUserWithAccountSharedService>();

            userWithAccountSharedServiceMock
                .Setup(x => x.UsernameExists("usuario"))
                .Returns(true);

            userWithAccountSharedServiceMock
                .Setup(x => x.ValidatePassword("usuario", "Password1!"))
                .ReturnsAsync((true, new List<string>()));

            var padrinoPayload = new UserWithAccountPayload<T>
            {
                UserWithAccountService = userWithAccountSharedServiceMock.Object,
            };

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), padrinoPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().ContainSingle();

                errors.Single().Should().Be($"{FileName}: line 2: username 'usuario' already exists");

                result.Should().ContainSingle();

                result.Single().FirstName.Should().Be("CoordinadorNombre");
                result.Single().LastName.Should().Be("CoordinadorApellido");
                result.Single().NickName.Should().Be("CoordinadorApodo");
                result.Single().Gender.Should().Be(Gender.Female);
                result.Single().Email.Should().Be("coordinador@email.com");
                result.Single().Phone.Should().Be("Phone");
                result.Single().Username.Should().Be("usuario");
                result.Single().Password.Should().Be("Password1!");
            }
        }

        [Fact]
        public async Task ConvertToObjects_PasswordCannotBeEmpty()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("nombre,apellido,apodo,sexo,email,teléfono,usuario,clave");
            fileContents.AppendLine("CoordinadorNombre, CoordinadorApellido, CoordinadorApodo, F, coordinador@email.com, Phone, usuario, ");

            var errors = new List<string>();

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), _padrinoPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().ContainSingle();

                errors.Single().Should().Be($"{FileName}: line 2: password cannot be empty");

                result.Should().ContainSingle();

                result.Single().FirstName.Should().Be("CoordinadorNombre");
                result.Single().LastName.Should().Be("CoordinadorApellido");
                result.Single().NickName.Should().Be("CoordinadorApodo");
                result.Single().Gender.Should().Be(Gender.Female);
                result.Single().Email.Should().Be("coordinador@email.com");
                result.Single().Phone.Should().Be("Phone");
                result.Single().Username.Should().Be("usuario");
                result.Single().Password.Should().BeEmpty();
            }
        }

        [Fact]
        public async Task ConvertToObjects_PasswordAlreadyExists()
        {
            // Arrange
            var fileContents = new StringBuilder();
            fileContents.AppendLine("nombre,apellido,apodo,sexo,email,teléfono,usuario,clave");
            fileContents.AppendLine("CoordinadorNombre, CoordinadorApellido, CoordinadorApodo, F, coordinador@email.com, Phone, usuario, Password1!");

            var errors = new List<string>();

            var userWithAccountSharedServiceMock = new Mock<IUserWithAccountSharedService>();

            userWithAccountSharedServiceMock
                .Setup(x => x.UsernameExists("usuario"))
                .Returns(false);

            userWithAccountSharedServiceMock
                .Setup(x => x.ValidatePassword("usuario", "Password1!"))
                .ReturnsAsync((false, new List<string> { "Password is not valid", "Second error message" }));

            var padrinoPayload = new UserWithAccountPayload<T>
            {
                UserWithAccountService = userWithAccountSharedServiceMock.Object,
            };

            // Act
            var result = await _sut.ConvertToObjects(fileContents.ToString(), padrinoPayload, errors);

            // Assert
            using (new AssertionScope())
            {
                errors.Should().ContainSingle();

                errors.Single().Should().Be($"{FileName}: line 2: password validation error: Password is not valid, Second error message");

                result.Should().ContainSingle();

                result.Single().FirstName.Should().Be("CoordinadorNombre");
                result.Single().LastName.Should().Be("CoordinadorApellido");
                result.Single().NickName.Should().Be("CoordinadorApodo");
                result.Single().Gender.Should().Be(Gender.Female);
                result.Single().Email.Should().Be("coordinador@email.com");
                result.Single().Phone.Should().Be("Phone");
                result.Single().Username.Should().Be("usuario");
                result.Single().Password.Should().Be("Password1!");
            }
        }
    }

    public class CoordinadorFileParserTests : UserWithAccountFileParserTests<Coordinador>
    {
        protected override string FileName => "coordinadores.csv";
    }

    public class MediadorFileParserTests : UserWithAccountFileParserTests<Mediador>
    {
        protected override string FileName => "mediadores.csv";
    }

    public class RevisorFileParserTests : UserWithAccountFileParserTests<Revisor>
    {
        protected override string FileName => "revisores.csv";
    }
}
