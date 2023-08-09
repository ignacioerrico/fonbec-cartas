using FluentAssertions;
using FluentAssertions.Execution;
using Fonbec.Cartas.DataAccess.DataModels.Coordinador;
using Fonbec.Cartas.DataAccess.Entities.Enums;
using Fonbec.Cartas.Logic.ViewModels.Coordinador;
using Mapster;

namespace Fonbec.Cartas.Logic.Tests.ViewModels.Coordinador
{
    public class PlannedEventsListViewModelTests : MapsterTests
    {
        private readonly DateTimeOffset _createdOnUtc = DateTimeOffset.Now;

        [Fact]
        public void Map_PlansListDataModel_To_PlannedEventsListViewModel_Success()
        {
            // Arrange
            var plansListDataModels = new List<PlansListDataModel>
            {
                CreatePlansListDataModel(planningId: 1, 2020, 12, PlanningType.PlannedEvent),
                CreatePlansListDataModel(planningId: 2, 2020, 11, PlanningType.Deadline), // -- deadline -- (index: 1)
                CreatePlansListDataModel(planningId: 3, 2020, 10, PlanningType.PlannedEvent),
                CreatePlansListDataModel(planningId: 4, 2020, 9, PlanningType.PlannedEvent),
                CreatePlansListDataModel(planningId: 5, 2020, 8, PlanningType.Deadline), // -- deadline -- (index: 4)
                CreatePlansListDataModel(planningId: 6, 2020, 7, PlanningType.PlannedEvent),
                CreatePlansListDataModel(planningId: 7, 2020, 6, PlanningType.PlannedEvent),
                CreatePlansListDataModel(planningId: 8, 2020, 5, PlanningType.PlannedEvent),
                CreatePlansListDataModel(planningId: 9, 2020, 4, PlanningType.Deadline), // -- deadline -- (index: 8)
                CreatePlansListDataModel(planningId: 10, 2020, 3, PlanningType.PlannedEvent),
                CreatePlansListDataModel(planningId: 11, 2020, 2, PlanningType.Deadline), // -- deadline -- (index: 10)
                CreatePlansListDataModel(planningId: 12, 2020, 1, PlanningType.PlannedEvent),
            };

            plansListDataModels[0].Deliveries = new()
            {
                CreateDeliveryDataModel(1, false, false),
                CreateDeliveryDataModel(2, false, true),
                CreateDeliveryDataModel(3, true, false),
                CreateDeliveryDataModel(4, false, true),
                CreateDeliveryDataModel(5, false, true),
            };
            plansListDataModels[1].Deliveries = new() // --- Deadline ---
            {
                CreateDeliveryDataModel(4, true, false),
                CreateDeliveryDataModel(5, true, true),
            };
            plansListDataModels[2].Deliveries = new()
            {
                CreateDeliveryDataModel(1, true, false),
                CreateDeliveryDataModel(2, true, true),
                CreateDeliveryDataModel(3, false, false),
                CreateDeliveryDataModel(4, false, true),
                CreateDeliveryDataModel(5, false, true),
            };
            plansListDataModels[3].Deliveries = new()
            {
                CreateDeliveryDataModel(1, false, false),
                CreateDeliveryDataModel(2, false, true),
                CreateDeliveryDataModel(3, true, false),
                CreateDeliveryDataModel(4, false, true),
                CreateDeliveryDataModel(5, false, true),
            };
            plansListDataModels[4].Deliveries = new() // --- Deadline ---
            {
                CreateDeliveryDataModel(2, true, true), // Included in total (deliveries in Deadline are always included)
            };
            plansListDataModels[5].Deliveries = new()
            {
                CreateDeliveryDataModel(1, true, true), // Included
                CreateDeliveryDataModel(2, true, true), // This one is ignored, because it's included in the Deadline above
                CreateDeliveryDataModel(3, true, false),
            };
            plansListDataModels[6].Deliveries = new()
            {
                CreateDeliveryDataModel(1, true, true), // Ignored
                CreateDeliveryDataModel(2, true, true), // Ignored
                CreateDeliveryDataModel(3, false, false),
            };
            plansListDataModels[7].Deliveries = new()
            {
                CreateDeliveryDataModel(1, true, true), // Ignored
                CreateDeliveryDataModel(2, true, true), // Ignored
                CreateDeliveryDataModel(3, false, true),
            };
            plansListDataModels[8].Deliveries = new(); // --- Deadline ---
            plansListDataModels[9].Deliveries = new()
            {
                CreateDeliveryDataModel(1, false, true),
                CreateDeliveryDataModel(2, true, true),
                CreateDeliveryDataModel(3, true, false),
            };
            plansListDataModels[10].Deliveries = new(); // --- Deadline ---
            plansListDataModels[11].Deliveries = new()
            {
                CreateDeliveryDataModel(1, false, true),
                CreateDeliveryDataModel(2, false, false),
                CreateDeliveryDataModel(3, false, false),
            };

            // Act
            var result = plansListDataModels.BuildAdapter()
                .AddParameters("dataModels", plansListDataModels)
                .AdaptToType<List<PlannedEventsListViewModel>>();

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull()
                    .And.HaveCount(12);

                result[0].PlanningId.Should().Be(1);
                result[0].Date.Should().Be(new DateTime(2020, 12, 1));
                result[0].PlanningType.Should().Be(PlanningType.PlannedEvent);
                result[0].EventName.Should().Be("Carta de diciembre de 2020");
                result[0].Description.Should().Be("La recepción de cartas comenzó el 1/12/2020");
                result[0].TotalToSend.Should().Be(5);
                result[0].AlreadySent.Should().Be(3);
                result[0].Percentage.Should().BeGreaterThan(.59)
                    .And.BeLessThan(.61);
                result[0].CreatedOnUtc.Should().Be(_createdOnUtc);
                result[0].CreatedBy.Should().Be("FirstName-1 LastName-1");
                result[0].LastUpdatedOnUtc.Should().Be(_createdOnUtc.AddDays(1));
                result[0].UpdatedBy.Should().Be("FirstName-2 LastName-2");

                // Deadline
                result[1].PlanningId.Should().Be(2);
                result[1].Date.Should().Be(new DateTime(2020, 11, 1));
                result[1].PlanningType.Should().Be(PlanningType.Deadline);
                result[1].EventName.Should().Be("Corte para recepción de notas: noviembre de 2020");
                result[1].Description.Should().Be("Se recibieron notas hasta el 1/11/2020");
                result[1].TotalToSend.Should().Be(5);
                result[1].AlreadySent.Should().Be(2);
                result[1].Percentage.Should().BeGreaterThan(.39)
                    .And.BeLessThan(.41);
                result[1].CreatedOnUtc.Should().Be(_createdOnUtc);
                result[1].CreatedBy.Should().Be("FirstName-3 LastName-3");
                result[1].LastUpdatedOnUtc.Should().Be(_createdOnUtc.AddDays(1));
                result[1].UpdatedBy.Should().Be("FirstName-4 LastName-4");

                result[2].PlanningId.Should().Be(3);
                result[2].Date.Should().Be(new DateTime(2020, 10, 1));
                result[2].PlanningType.Should().Be(PlanningType.PlannedEvent);
                result[2].EventName.Should().Be("Carta de octubre de 2020");
                result[2].Description.Should().Be("La recepción de cartas comenzó el 1/10/2020");
                result[2].TotalToSend.Should().Be(5);
                result[2].AlreadySent.Should().Be(3);
                result[2].Percentage.Should().BeGreaterThan(.59)
                    .And.BeLessThan(.61);
                result[2].CreatedOnUtc.Should().Be(_createdOnUtc);
                result[2].CreatedBy.Should().Be("FirstName-5 LastName-5");
                result[2].LastUpdatedOnUtc.Should().Be(_createdOnUtc.AddDays(1));
                result[2].UpdatedBy.Should().Be("FirstName-6 LastName-6");

                result[3].PlanningId.Should().Be(4);
                result[3].Date.Should().Be(new DateTime(2020, 9, 1));
                result[3].PlanningType.Should().Be(PlanningType.PlannedEvent);
                result[3].EventName.Should().Be("Carta de septiembre de 2020");
                result[3].Description.Should().Be("La recepción de cartas comenzó el 1/9/2020");
                result[3].TotalToSend.Should().Be(5);
                result[3].AlreadySent.Should().Be(3);
                result[3].Percentage.Should().BeGreaterThan(.59)
                    .And.BeLessThan(.61);
                result[3].CreatedOnUtc.Should().Be(_createdOnUtc);
                result[3].CreatedBy.Should().Be("FirstName-7 LastName-7");
                result[3].LastUpdatedOnUtc.Should().Be(_createdOnUtc.AddDays(1));
                result[3].UpdatedBy.Should().Be("FirstName-8 LastName-8");

                // Deadline
                result[4].PlanningId.Should().Be(5);
                result[4].Date.Should().Be(new DateTime(2020, 8, 1));
                result[4].PlanningType.Should().Be(PlanningType.Deadline);
                result[4].EventName.Should().Be("Corte para recepción de notas: agosto de 2020");
                result[4].Description.Should().Be("Se recibieron notas hasta el 1/8/2020");
                result[4].TotalToSend.Should().Be(3);
                result[4].AlreadySent.Should().Be(2);
                result[4].Percentage.Should().BeGreaterThan(.66)
                    .And.BeLessThan(.68);
                result[4].CreatedOnUtc.Should().Be(_createdOnUtc);
                result[4].CreatedBy.Should().Be("FirstName-9 LastName-9");
                result[4].LastUpdatedOnUtc.Should().Be(_createdOnUtc.AddDays(1));
                result[4].UpdatedBy.Should().Be("FirstName-10 LastName-10");

                result[5].PlanningId.Should().Be(6);
                result[5].Date.Should().Be(new DateTime(2020, 7, 1));
                result[5].PlanningType.Should().Be(PlanningType.PlannedEvent);
                result[5].EventName.Should().Be("Carta de julio de 2020");
                result[5].Description.Should().Be("La recepción de cartas comenzó el 1/7/2020");
                result[5].TotalToSend.Should().Be(3);
                result[5].AlreadySent.Should().Be(2);
                result[5].Percentage.Should().BeGreaterThan(.66)
                    .And.BeLessThan(.68);
                result[5].CreatedOnUtc.Should().Be(_createdOnUtc);
                result[5].CreatedBy.Should().Be("FirstName-11 LastName-11");
                result[5].LastUpdatedOnUtc.Should().Be(_createdOnUtc.AddDays(1));
                result[5].UpdatedBy.Should().Be("FirstName-12 LastName-12");

                result[6].PlanningId.Should().Be(7);
                result[6].Date.Should().Be(new DateTime(2020, 6, 1));
                result[6].PlanningType.Should().Be(PlanningType.PlannedEvent);
                result[6].EventName.Should().Be("Carta de junio de 2020");
                result[6].Description.Should().Be("La recepción de cartas comenzó el 1/6/2020");
                result[6].TotalToSend.Should().Be(3);
                result[6].AlreadySent.Should().Be(2);
                result[6].Percentage.Should().BeGreaterThan(.66)
                    .And.BeLessThan(.68);
                result[6].CreatedOnUtc.Should().Be(_createdOnUtc);
                result[6].CreatedBy.Should().Be("FirstName-13 LastName-13");
                result[6].LastUpdatedOnUtc.Should().Be(_createdOnUtc.AddDays(1));
                result[6].UpdatedBy.Should().Be("FirstName-14 LastName-14");

                result[7].PlanningId.Should().Be(8);
                result[7].Date.Should().Be(new DateTime(2020, 5, 1));
                result[7].PlanningType.Should().Be(PlanningType.PlannedEvent);
                result[7].EventName.Should().Be("Carta de mayo de 2020");
                result[7].Description.Should().Be("La recepción de cartas comenzó el 1/5/2020");
                result[7].TotalToSend.Should().Be(3);
                result[7].AlreadySent.Should().Be(3);
                result[7].Percentage.Should().BeGreaterThan(.99)
                    .And.BeLessThan(1.01);
                result[7].CreatedOnUtc.Should().Be(_createdOnUtc);
                result[7].CreatedBy.Should().Be("FirstName-15 LastName-15");
                result[7].LastUpdatedOnUtc.Should().Be(_createdOnUtc.AddDays(1));
                result[7].UpdatedBy.Should().Be("FirstName-16 LastName-16");

                // Deadline
                result[8].PlanningId.Should().Be(9);
                result[8].Date.Should().Be(new DateTime(2020, 4, 1));
                result[8].PlanningType.Should().Be(PlanningType.Deadline);
                result[8].EventName.Should().Be("Corte para recepción de notas: abril de 2020");
                result[8].Description.Should().Be("Se recibieron notas hasta el 1/4/2020");
                result[8].TotalToSend.Should().Be(2);
                result[8].AlreadySent.Should().Be(1);
                result[8].Percentage.Should().BeGreaterThan(.49)
                    .And.BeLessThan(.51);
                result[8].CreatedOnUtc.Should().Be(_createdOnUtc);
                result[8].CreatedBy.Should().Be("FirstName-17 LastName-17");
                result[8].LastUpdatedOnUtc.Should().Be(_createdOnUtc.AddDays(1));
                result[8].UpdatedBy.Should().Be("FirstName-18 LastName-18");

                result[9].PlanningId.Should().Be(10);
                result[9].Date.Should().Be(new DateTime(2020, 3, 1));
                result[9].PlanningType.Should().Be(PlanningType.PlannedEvent);
                result[9].EventName.Should().Be("Carta de marzo de 2020");
                result[9].Description.Should().Be("La recepción de cartas comenzó el 1/3/2020");
                result[9].TotalToSend.Should().Be(3);
                result[9].AlreadySent.Should().Be(2);
                result[9].Percentage.Should().BeGreaterThan(.66)
                    .And.BeLessThan(.67);
                result[9].CreatedOnUtc.Should().Be(_createdOnUtc);
                result[9].CreatedBy.Should().Be("FirstName-19 LastName-19");
                result[9].LastUpdatedOnUtc.Should().Be(_createdOnUtc.AddDays(1));
                result[9].UpdatedBy.Should().Be("FirstName-20 LastName-20");

                // Deadline
                result[10].PlanningId.Should().Be(11);
                result[10].Date.Should().Be(new DateTime(2020, 2, 1));
                result[10].PlanningType.Should().Be(PlanningType.Deadline);
                result[10].EventName.Should().Be("Corte para recepción de notas: febrero de 2020");
                result[10].Description.Should().Be("Se recibieron notas hasta el 1/2/2020");
                result[10].TotalToSend.Should().Be(0);
                result[10].AlreadySent.Should().Be(0);
                result[10].Percentage.Should().Be(0);
                result[10].CreatedOnUtc.Should().Be(_createdOnUtc);
                result[10].CreatedBy.Should().Be("FirstName-21 LastName-21");
                result[10].LastUpdatedOnUtc.Should().Be(_createdOnUtc.AddDays(1));
                result[10].UpdatedBy.Should().Be("FirstName-22 LastName-22");

                result[11].PlanningId.Should().Be(12);
                result[11].Date.Should().Be(new DateTime(2020, 1, 1));
                result[11].PlanningType.Should().Be(PlanningType.PlannedEvent);
                result[11].EventName.Should().Be("Carta de enero de 2020");
                result[11].Description.Should().Be("La recepción de cartas comenzó el 1/1/2020");
                result[11].TotalToSend.Should().Be(3);
                result[11].AlreadySent.Should().Be(1);
                result[11].Percentage.Should().BeGreaterThan(.33)
                    .And.BeLessThan(.34);
                result[11].CreatedOnUtc.Should().Be(_createdOnUtc);
                result[11].CreatedBy.Should().Be("FirstName-23 LastName-23");
                result[11].LastUpdatedOnUtc.Should().Be(_createdOnUtc.AddDays(1));
                result[11].UpdatedBy.Should().Be("FirstName-24 LastName-24");
            }
        }

        [Fact]
        public void Map_PlansListDataModel_To_PlannedEventsListViewModel_UpdatedByIsNullWhenUpdatedByIsNull()
        {
            // Arrange
            var plansListDataModels = new List<PlansListDataModel>
            {
                new()
                {
                    Date = new DateTime(2020, 6, 1),
                    CreatedBy = new(),
                    LastUpdatedOnUtc = null,
                    UpdatedBy = null,
                }
            };

            // Act
            var result = plansListDataModels.BuildAdapter()
                .AddParameters("dataModels", plansListDataModels)
                .AdaptToType<List<PlannedEventsListViewModel>>();

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull()
                    .And.HaveCount(1);

                result[0].LastUpdatedOnUtc.Should().BeNull();
                result[0].UpdatedBy.Should().BeNull();
            }
        }

        [Theory]
        [InlineData(PlanningType.PlannedEvent, "Carta de junio de 2020")]
        [InlineData(PlanningType.Deadline, "Corte para recepción de notas: junio de 2020")]
        public void Map_PlansListDataModel_To_PlannedEventsListViewModel_EventName(PlanningType planningType, string expectedEventName)
        {
            // Arrange
            var plansListDataModels = new List<PlansListDataModel>
            {
                new()
                {
                    Date = new DateTime(2020, 6, 1),
                    PlanningType = planningType,
                    CreatedBy = new(),
                }
            };

            // Act
            var result = plansListDataModels.BuildAdapter()
                .AddParameters("dataModels", plansListDataModels)
                .AdaptToType<List<PlannedEventsListViewModel>>();

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull()
                    .And.HaveCount(1);

                result[0].PlanningType.Should().Be(planningType);
                result[0].EventName.Should().Be(expectedEventName);
            }
        }

        [Theory]
        [InlineData(PlanningType.PlannedEvent, "La recepción de cartas comenzó el")]
        [InlineData(PlanningType.Deadline, "Se recibieron notas hasta el")]
        public void Map_PlansListDataModel_To_PlannedEventsListViewModel_Description_PastEvents(PlanningType planningType, string expectedDescription)
        {
            // Arrange
            var pastDate = DateTime.Today.AddMonths(-1);

            var plansListDataModels = new List<PlansListDataModel>
            {
                new()
                {
                    Date = pastDate,
                    PlanningType = planningType,
                    CreatedBy = new(),
                }
            };

            // Act
            var result = plansListDataModels.BuildAdapter()
                .AddParameters("dataModels", plansListDataModels)
                .AdaptToType<List<PlannedEventsListViewModel>>();

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull()
                    .And.HaveCount(1);

                result[0].PlanningType.Should().Be(planningType);
                result[0].Description.Should().Be($"{expectedDescription} {pastDate.Date:d/M/yyyy}");
            }
        }

        [Theory]
        [InlineData(PlanningType.PlannedEvent, "La recepción de cartas se habilitará el")]
        [InlineData(PlanningType.Deadline, "Se recibirán notas hasta el")]
        public void Map_PlansListDataModel_To_PlannedEventsListViewModel_Description_FutureEvents(PlanningType planningType, string expectedDescription)
        {
            // Arrange
            var futureDate = DateTime.Today.AddMonths(1);

            var plansListDataModels = new List<PlansListDataModel>
            {
                new()
                {
                    Date = futureDate,
                    PlanningType = planningType,
                    CreatedBy = new(),
                }
            };

            // Act
            var result = plansListDataModels.BuildAdapter()
                .AddParameters("dataModels", plansListDataModels)
                .AdaptToType<List<PlannedEventsListViewModel>>();

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull()
                    .And.HaveCount(1);

                result[0].PlanningType.Should().Be(planningType);
                result[0].Description.Should().Be($"{expectedDescription} {futureDate.Date:d/M/yyyy}");
            }
        }

        [Theory]
        [InlineData(PlanningType.PlannedEvent)]
        [InlineData(PlanningType.Deadline)]
        public void Map_PlansListDataModel_To_PlannedEventsListViewModel_PercentageShoudBeZero_WhenNoDeliveries(PlanningType planningType)
        {
            // Arrange
            var plansListDataModels = new List<PlansListDataModel>
            {
                new()
                {
                    Date = new DateTime(2020, 6, 1),
                    PlanningType = planningType,
                    CreatedBy = new(),
                }
            };

            // Act
            var result = plansListDataModels.BuildAdapter()
                .AddParameters("dataModels", plansListDataModels)
                .AdaptToType<List<PlannedEventsListViewModel>>();

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull()
                    .And.HaveCount(1);

                result[0].TotalToSend.Should().Be(0);
                result[0].AlreadySent.Should().Be(0);
                result[0].Percentage.Should().Be(0);
            }
        }

        private PlansListDataModel CreatePlansListDataModel(int planningId, int year, int month, PlanningType planningType)
        {
            var plansListDataModel = new PlansListDataModel
            {
                PlanningId = planningId,
                Date = new DateTime(year, month, 1),
                PlanningType = planningType,
                CreatedOnUtc = _createdOnUtc,
                CreatedBy = new DataAccess.Entities.Actors.Coordinador
                {
                    FirstName = $"FirstName-{2 * planningId - 1}",
                    LastName = $"LastName-{2 * planningId - 1}"
                },
                LastUpdatedOnUtc = _createdOnUtc.AddDays(1),
                UpdatedBy = new DataAccess.Entities.Actors.Coordinador
                {
                    FirstName = $"FirstName-{2 * planningId}",
                    LastName = $"LastName-{2 * planningId}"
                },
            };

            return plansListDataModel;
        }

        private PlansListDeliveriesDataModel CreateDeliveryDataModel(int apadrinamientoId, bool includesBoletínOrLibUniv, bool hasBeenSent)
        {
            var plansListDeliveriesDataModel = new PlansListDeliveriesDataModel
            {
                Apadrinamiento = new()
                {
                    Id = apadrinamientoId,
                },
                IncludesBoletínOrLibUniv = includesBoletínOrLibUniv,
                HasBeenSent = hasBeenSent,
            };

            return plansListDeliveriesDataModel;
        }
    }
}
