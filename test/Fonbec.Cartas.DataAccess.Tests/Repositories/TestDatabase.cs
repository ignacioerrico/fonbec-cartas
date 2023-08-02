using Fonbec.Cartas.DataAccess.Entities;
using Fonbec.Cartas.DataAccess.Entities.Actors;
using Fonbec.Cartas.DataAccess.Entities.Actors.Abstract;
using Fonbec.Cartas.DataAccess.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Fonbec.Cartas.DataAccess.Tests.Repositories
{
    public class TestDatabase
    {
        private const string ConnectionString = @"Server=(localdb)\Fonbec;Database=Fonbec.Cartas.Testing.Db;Trusted_Connection=True;MultipleActiveResultSets=true";

        public TestDatabase()
        {
            using var context = GetAppDbContext();
            
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var filiales = GetFiliales();
            context.Filiales.AddRange(filiales);

            // First of all, we need Filiales
            context.SaveChanges();
            
            var coordinadores = GetUsersWithAccount<Entities.Actors.Coordinador>();
            context.Coordinadores.AddRange(coordinadores);

            var mediadores = GetUsersWithAccount<Mediador>();
            context.Mediadores.AddRange(mediadores);

            var revisores = GetUsersWithAccount<Revisor>();
            context.Revisores.AddRange(revisores);

            // Then, we need the users with an account
            context.SaveChanges();

            var padrinos = GetPadrinos();
            context.Padrinos.AddRange(padrinos);

            var becarios = GetBecarios();
            context.Becarios.AddRange(becarios);
            
            // Then, we need the padrinos and becarios
            context.SaveChanges();

            var apadrinamientos = GetApadrinamientos();
            context.Apadrinamientos.AddRange(apadrinamientos);
            
            // Save their relationships
            context.SaveChanges();
        }

        public IDbContextFactory<ApplicationDbContext> GetAppDbContextFactory()
        {
            var appDbContextFactoryMock = new Mock<IDbContextFactory<ApplicationDbContext>>();

            var appDbContext = GetAppDbContext();

            appDbContextFactoryMock
                .Setup(x => x.CreateDbContextAsync(default))
                .ReturnsAsync(appDbContext);

            return appDbContextFactoryMock.Object;
        }

        private static ApplicationDbContext GetAppDbContext()
        {
            var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(ConnectionString)
                .Options;

            return new ApplicationDbContext(dbContextOptions);
        }

        // Id | Name
        //  1 | Default
        //  2 | Córdoba
        private static IEnumerable<Filial> GetFiliales()
        {
            yield return new Filial
            {
                Name = "Córdoba",
            };
        }

        // COORDINADORES
        // Id | FilialId | FirstName              | LastName              | NickName | Gender | Email                  | Phone
        //  1 |        1 | Coordinador3-FirstName | Coordinador3-LastName | Coo3     | F      | coordinador3@email.com | Coordinador3-Phone
        //  2 |        1 | Coordinador2-FirstName | Coordinador2-LastName | Coo2     | M      | coordinador2@email.com | Coordinador2-Phone
        //  3 |        2 | Coordinador1-FirstName | Coordinador1-LastName | Coo1     | F      | coordinador1@email.com | Coordinador1-Phone
        // MEDIADORES
        //  1 |        1 | Mediador3-FirstName    | Mediador3-LastName    | Med3     | F      | mediador3@email.com    | Mediador3-Phone
        //  2 |        1 | Mediador2-FirstName    | Mediador2-LastName    | Med2     | M      | mediador2@email.com    | Mediador2-Phone
        //  3 |        2 | Mediador1-FirstName    | Mediador1-LastName    | Med1     | F      | mediador1@email.com    | Mediador1-Phone
        // REVISORES
        //  1 |        1 | Revisor3-FirstName     | Revisor3-LastName     | Rev3     | F      | revisor3@email.com     | Revisor3-Phone
        //  2 |        1 | Revisor2-FirstName     | Revisor2-LastName     | Rev2     | M      | revisor2@email.com     | Revisor2-Phone
        //  3 |        2 | Revisor1-FirstName     | Revisor1-LastName     | Rev1     | F      | revisor1@email.com     | Revisor1-Phone
        private static IEnumerable<T> GetUsersWithAccount<T>()
            where T : UserWithAccount, new()
        {
            string Truncate(string value, int maxLength) =>
                value.Length <= maxLength
                    ? value
                    : value[..maxLength];

            for (var index = 0; index < 3; index++)
            {
                var typeName = typeof(T).Name;
                var number = 3 - index;
                yield return new T
                {
                    FilialId = index is 0 or 1 ? 1 : 2,
                    FirstName = $"{typeName}{number}-FirstName",
                    LastName = $"{typeName}{number}-LastName",
                    NickName = $"{Truncate(typeName, 3)}{number}",
                    Gender = index is 0 or 2 ? Gender.Female : Gender.Male,
                    Email = $"{typeName.ToLower()}{number}@email.com",
                    Phone = $"{typeName}{number}-Phone",
                };
            }
        }

        // Id | FilialId | FirstName          | LastName              | NickName | Gender | Email              | Phone              | CreatedByCoordinadorId
        //  1 |        1 | Padrino3-FirstName | Coordinador3-LastName | Coo3     | F      | padrino3@email.com | Coordinador3-Phone | 1
        //  2 |        1 | Padrino2-FirstName | Coordinador2-LastName | Coo2     | M      | padrino2@email.com | Coordinador2-Phone | 2
        //  3 |        2 | Padrino1-FirstName | Coordinador1-LastName | Coo1     | F      | padrino1@email.com | Coordinador1-Phone | 3
        private static IEnumerable<Padrino> GetPadrinos()
        {
            for (var index = 0; index < 3; index++)
            {
                var number = 3 - index;
                yield return new Padrino
                {
                    FilialId = index is 0 or 1 ? 1 : 2,
                    FirstName = $"Padrino{number}-FirstName",
                    LastName = $"Padrino{number}-LastName",
                    NickName = $"Pad{number}",
                    Gender = index is 0 or 2 ? Gender.Female : Gender.Male,
                    Email = $"padrino{number}@email.com",
                    Phone = $"Padrino{number}-Phone",
                    SendAlsoTo = index is 0
                        ? new()
                        {
                            new()
                            {
                                RecipientFullName = $"Padrino{number}-SendAlsoTo-RecipientFullName",
                                RecipientEmail = $"Padrino{number}-SendAlsoTo-RecipientEmail",
                                SendAsBcc = true,
                            },
                        }
                        : null,
                    CreatedByCoordinadorId = index + 1,
                };
            }
        }

        // Id | FilialId | FirstName          | LastName          | NickName | NivelDeEstudio | Gender | Email              | Phone          | CreatedByCoordinadorId | MediadorId
        //  1 |        1 | Becario5-FirstName | Becario5-LastName | Bec5     | Primario       | F      | becario5@email.com | Becario5-Phone | 1                      | 1
        //  2 |        1 | Becario4-FirstName | Becario4-LastName | Bec4     | Secundario     | M      | becario4@email.com | Becario4-Phone | 1                      | 1
        //  3 |        1 | Becario3-FirstName | Becario3-LastName | Bec3     | Universitario  | F      | becario3@email.com | Becario3-Phone | 2                      | 2
        //  4 |        1 | Becario2-FirstName | Becario2-LastName | Bec2     | Primario       | M      | becario2@email.com | Becario2-Phone | 2                      | 2
        //  5 |        2 | Becario1-FirstName | Becario1-LastName | Bec1     | Secundario     | F      | becario1@email.com | Becario1-Phone | 3                      | 3
        private static IEnumerable<Becario> GetBecarios()
        {
            for (var index = 0; index < 5; index++)
            {
                var number = 5 - index;
                yield return new Becario
                {
                    FilialId = index < 4 ? 1 : 2,
                    FirstName = $"Becario{number}-FirstName",
                    LastName = $"Becario{number}-LastName",
                    NickName = $"Bec{number}",
                    NivelDeEstudio = index switch
                    {
                        0 or 3 => NivelDeEstudio.Primario,
                        1 or 4 => NivelDeEstudio.Secundario,
                        _ => NivelDeEstudio.Universitario
                    },
                    Gender = index % 2 == 0 ? Gender.Female : Gender.Male,
                    Email = $"becario{number}@email.com",
                    Phone = $"Becario{number}-Phone",
                    CreatedByCoordinadorId = index switch
                    {
                        0 or 1 => 1,
                        2 or 3 => 2,
                        _ => 3
                    },
                    MediadorId = index switch
                    {
                        0 or 1 => 1,
                        2 or 3 => 2,
                        _ => 3
                    },
                };
            }
        }

        // Id | PadrinoId | BecarioId | From       | To         | CreatedByCoordinadorId
        //  1 |         1 |         1 | 2020-01-01 | 2020-05-30 | 1
        //  2 |         1 |         2 | 2020-01-01 | 2020-05-30 | 1
        //  3 |         2 |         3 | 2020-06-01 | 2020-09-30 | 1
        //  4 |         3 |         4 | 2020-01-01 | 2020-09-30 | 3
        private static IEnumerable<Apadrinamiento> GetApadrinamientos()
        {
            yield return new Apadrinamiento
            {
                PadrinoId = 1,
                BecarioId = 1,
                From = new DateTime(2020, 1, 1),
                To = new DateTime(2020, 5, 30),
                CreatedByCoordinadorId = 1,
            };
            yield return new Apadrinamiento
            {
                PadrinoId = 1,
                BecarioId = 2,
                From = new DateTime(2020, 1, 1),
                To = new DateTime(2020, 5, 30),
                CreatedByCoordinadorId = 1,
            };
            yield return new Apadrinamiento
            {
                PadrinoId = 2,
                BecarioId = 3,
                From = new DateTime(2020, 6, 1),
                To = new DateTime(2020, 9, 30),
                CreatedByCoordinadorId = 1,
            };
            yield return new Apadrinamiento
            {
                PadrinoId = 3,
                BecarioId = 4,
                From = new DateTime(2020, 1, 1),
                To = new DateTime(2020, 9, 30),
                CreatedByCoordinadorId = 3,
            };
        }



    }
}
