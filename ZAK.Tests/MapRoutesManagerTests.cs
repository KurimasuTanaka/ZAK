using System;
using System.Numerics;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using ZAK.DA;

namespace ZAK.Tests;

public class MapRoutesManagerTests : ZakTestBase
{
    [Fact]
    public async Task GetRoutesForBrigades()
    {
        // Arrange
        MapRoutesManager.MapRoutesManager _mapRoutesManager = new(
            @"./../../../../kyiv.osm.pbf",
            new NullLogger<MapRoutesManager.MapRoutesManager>(),
            brigadeRepository
        );

        AddressCoordinates addressCoordinates1 = new() { lat = 50.520281, lon = 30.5211 };
        AddressCoordinates addressCoordinates2 = new() { lat = 50.4941161, lon = 30.5079205 };
        AddressCoordinates addressCoordinates3 = new() { lat = 50.5162369, lon = 30.4997271 };
        AddressCoordinates addressCoordinates4 = new() { lat = 50.4633998, lon = 30.5909444 };

        Address address1 = new() { Id = 1, streetName = "ул. Приречная", building = "21-а", coordinates = addressCoordinates1 };
        Address address2 = new() { Id = 2, streetName = "ул. Иорданская (Лайоша Гавро)", building = "8", coordinates = addressCoordinates2 };
        Address address3 = new() { Id = 3, streetName = "просп. Оболонский", building = "30", coordinates = addressCoordinates3 };
        Address address4 = new() { Id = 4, streetName = "ул. Каховская", building = "62", coordinates = addressCoordinates4 };

        Application application1 = new() { address = address1 };
        Application application2 = new() { address = address2 };
        Application application3 = new() { address = address3 };
        Application application4 = new() { address = address4 };

        Brigade brigade1 = new();
        brigade1.scheduledApplications.Add(new Db.Models.ScheduledApplicationModel() { application = application1, scheduledTime = 1 });
        brigade1.scheduledApplications.Add(new Db.Models.ScheduledApplicationModel() { application = application2, scheduledTime = 5});

        Brigade brigade2 = new();
        brigade2.scheduledApplications.Add(new Db.Models.ScheduledApplicationModel() { application = application3, scheduledTime = 1  });
        brigade2.scheduledApplications.Add(new Db.Models.ScheduledApplicationModel() { application = application4, scheduledTime = 4  });

        await brigadeRepository.CreateAsync(brigade1);
        await brigadeRepository.CreateAsync(brigade2);

        List<Brigade> brigades = (await brigadeRepository.GetAllAsync()).ToList();

        //Act
        List<List<Vector2>> routes = await _mapRoutesManager.GetRoutesAsync();

        //Assert
        Assert.NotNull(routes);
        Assert.Equal(2, routes.Count);
        Assert.True(routes[0].Count > 2);
        Assert.True(routes[1].Count > 2);
    }
}
