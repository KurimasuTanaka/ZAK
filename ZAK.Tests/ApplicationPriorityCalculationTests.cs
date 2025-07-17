using System;
using Xunit;
using ZAK.DA;
using ZAK.Db.Models;

namespace ZAK.Tests;

public class ApplicationPriorityCalculationTests
{
    [Fact]
    public void CalculatePriorityByDistance()
    {
        // Arrange
        Application application1 = new();
        Application application2 = new();
        Application application3 = new();

        application1.address = new AddressModel();
        application1.address.coordinates = new AddressCoordinatesModel { lat = 50.53184, lon = 30.51289 }; //50.531844824813426, 30.512894712067872

        application2.address = new AddressModel();
        application2.address.coordinates = new AddressCoordinatesModel { lat = 50.5179, lon = 30.5109 }; //50.517985003588876, 30.510920606380573

        application3.address = new AddressModel();
        application3.address.coordinates = new AddressCoordinatesModel { lat = 50.49210, lon = 30.4788 }; //50.492109725173655, 30.478819930084466

        Dictionary<string, double> coefficients = new()
        {
            { "distance", 10 },
            { "housePriority", 1 },
            { "urgency", 1 },
            { "statusCheck", 1 },
            { "freeCable", 1 },
            { "tarrifeChangeApplication", 1 },
            { "deadline", 1 }
        };

        // Act
        application2.CalculateApplicationPriorityLevel(
            coefficients,
            new List<Application> { application1}
        );

        double application2PriorityNearApp1 = application2.priority;

        application2.CalculateApplicationPriorityLevel(
            coefficients,
            new List<Application> { application3}
        );

        double application2PriorityNearApp3 = application2.priority;

        // Assert
        Assert.True(application2PriorityNearApp1 > application2PriorityNearApp3);
    }
}
