// ReSharper disable CheckNamespace, InconsistentNaming

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using MusicHub;

[TestFixture]
public class Test_001_001_004
{
    private static readonly Assembly CurrentAssembly = typeof(StartUp).Assembly;
    private static Type CurrentType;

    [Test]
    public void ValidateDeveloperEntity()
    {
        CurrentType = GetType("Song");

        AssertPropertyIsOfType("Id", typeof(int));
        AssertPropertyIsOfType("Name", typeof(string));
        AssertPropertyIsOfType("Duration", typeof(TimeSpan));
        AssertPropertyIsOfType("CreatedOn", typeof(DateTime));
        AssertPropertyIsOfType("Genre", GetType("Genre"));
        AssertPropertyIsOfType("AlbumId", typeof(int?));
        AssertPropertyIsOfType("Album", GetType("Album"));
        AssertPropertyIsOfType("WriterId", typeof(int));
        AssertPropertyIsOfType("Writer", GetType("Writer"));
        AssertPropertyIsOfType("Price", typeof(decimal));
        AssertCollectionIsOfType("SongPerformers", GetCollectionType(GetType("SongPerformer")));
    }

    private static PropertyInfo GetPropertyByName(Type type, string propName)
    {
        var properties = type.GetProperties();

        var firstOrDefault = properties.FirstOrDefault(p => p.Name == propName);
        return firstOrDefault;
    }

    private static void AssertPropertyIsOfType(string propertyName, Type expectedType)
    {
        var type = CurrentType;

        var property = GetPropertyByName(type, propertyName);
        Assert.IsNotNull(property, $"{type.Name}.{propertyName} property not found.");

        var errorMessage = string.Format($"{type.Name}.{property.Name} property is not {expectedType}!");
        Assert.That(property.PropertyType, Is.EqualTo(expectedType), errorMessage);
    }

    private static void AssertCollectionIsOfType(string propertyName, Type collectionType)
    {
        var type = CurrentType;

        var ordersProperty = GetPropertyByName(type, propertyName);

        var errorMessage = string.Format($"{type.Name}.{propertyName} property does not exist!");

        Assert.IsNotNull(ordersProperty, errorMessage);

        Assert.That(collectionType.IsAssignableFrom(ordersProperty.PropertyType));
    }

    private static Type GetType(string modelName)
    {
        var modelType = CurrentAssembly
            .GetTypes()
            .FirstOrDefault(t => t.Name == modelName);

        Assert.IsNotNull(modelType, $"{modelName} model not found!");

        return modelType;
    }

    private static Type GetCollectionType(Type modelType)
    {
        var collectionType = typeof(IEnumerable<>).MakeGenericType(modelType);
        return collectionType;
    }
}