using ArgonUI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgonUI.Test;

[TestClass]
public class ObservableStringSetTests
{
    public ObservableStringSet set;

    public ObservableStringSetTests()
    {
        set = [];
    }

    [TestMethod]
    public void TestAdd()
    {
        Assert.AreEqual(0, set.Count);

        set.Add("Hello");
        set.Add("Hello");
        set.Add("Hi");

        Assert.AreEqual(2, set.Count);
        string[] strings = set.ToArray();
        Assert.IsTrue(strings.Contains("Hello"));
        Assert.IsTrue(strings.Contains("Hi"));

        set = [];
        int itemsToAdd = 10000;
        for (int i = 0; i < itemsToAdd; i++)
            set.Add($"ExampleItem_{i}");

        Assert.AreEqual(itemsToAdd, set.Count);

        set = [];

        set.AddRange(["Test1", "Test2", "Test3"]);
        strings = set.ToArray();
        Assert.IsTrue(strings.Contains("Test1"));
        Assert.IsTrue(strings.Contains("Test2"));
        Assert.IsTrue(strings.Contains("Test3"));
    }

    [TestMethod]
    public void TestRemove()
    {
        set.Add("Hello");
        set.Add("Hello");
        set.Add("Hi");

        Assert.AreEqual(2, set.Count);

        set.Remove("Hello");

        string[] strings = set.ToArray();
        Assert.IsFalse(strings.Contains("Hello"));
        Assert.IsTrue(strings.Contains("Hi"));

        Assert.IsFalse(set.Remove("Greetings"));

        set = [];
        int itemsToAdd = 10000;
        int itemsToRemove = itemsToAdd / 2;
        for (int i = 0; i < itemsToAdd; i++)
            set.Add($"ExampleItem_{i}");
        for (int i = 0; i < itemsToRemove; i++)
            Assert.IsTrue(set.Remove($"ExampleItem_{i}"));

        Assert.AreEqual(itemsToAdd - itemsToRemove, set.Count);

        set.Clear();

        Assert.AreEqual(0, set.Count);

        set.AddRange(["Test1", "Test2", "Test3"]);
        set.RemoveRange(["Test1", "Test2", "Test3"]);
        strings = set.ToArray();
        Assert.IsFalse(strings.Contains("Test1"));
        Assert.IsFalse(strings.Contains("Test2"));
        Assert.IsFalse(strings.Contains("Test3"));
    }

    [TestMethod]
    public void TestContains()
    {
        Assert.IsFalse(set.Contains("Hello"));

        set.Add("Hello");
        set.Add("Hello");
        set.Add("Hi");

        Assert.IsTrue(set.Contains("Hello"));
        Assert.IsTrue(set.Contains("Hello".AsSpan()));
    }

    [TestMethod]
    public void TestConstructors()
    {
        set = new();
        Assert.AreEqual(0, set.Count);

        set = new(10);
        Assert.IsTrue(set.Capacity >= 10);

        set = new("Test1", "Test2", "Test3");
        Assert.IsTrue(set.Contains("Test1"));
        Assert.IsTrue(set.Contains("Test2"));
        Assert.IsTrue(set.Contains("Test3"));

        set = new(["Test1", "Test2", "Test3"]);
        Assert.IsTrue(set.Contains("Test1"));
        Assert.IsTrue(set.Contains("Test2"));
        Assert.IsTrue(set.Contains("Test3"));
    }

    [TestMethod]
    public void TestBooleanTests()
    {
        set = new("a", "b", "c");
        string[] otherEq = ["a", "b", "c"];
        string[] otherSup = ["a", "b"]; // For clarity, sup and sub here, refer to set being a superset or a subset of other (not the other way around)
        string[] otherSub = ["a", "b", "c", "d"];
        string[] otherOverlap = ["a", "b", "d"];
        // For now, we can ignore testing with these, internally the the tested methods convert other to an ObservableStringSet
        ObservableStringSet otherSetEq = new(otherEq);
        ObservableStringSet otherSetSup = new(otherSup);
        ObservableStringSet otherSetSub = new(otherSub);
        ObservableStringSet otherSetOverlap = new(otherOverlap);

        Assert.IsFalse(set.IsProperSubsetOf(otherEq));
        Assert.IsTrue(set.IsProperSubsetOf(otherSub));
        Assert.IsFalse(set.IsProperSubsetOf(otherSup));
        Assert.IsFalse(set.IsProperSubsetOf(otherOverlap));

        Assert.IsTrue(set.IsSubsetOf(otherEq));
        Assert.IsTrue(set.IsSubsetOf(otherSub));
        Assert.IsFalse(set.IsSubsetOf(otherSup));
        Assert.IsFalse(set.IsSubsetOf(otherOverlap));

        Assert.IsFalse(set.IsProperSupersetOf(otherEq));
        Assert.IsFalse(set.IsProperSupersetOf(otherSub));
        Assert.IsTrue(set.IsProperSupersetOf(otherSup));
        Assert.IsFalse(set.IsProperSupersetOf(otherOverlap));

        Assert.IsTrue(set.IsSupersetOf(otherEq));
        Assert.IsFalse(set.IsSupersetOf(otherSub));
        Assert.IsTrue(set.IsSupersetOf(otherSup));
        Assert.IsFalse(set.IsSupersetOf(otherOverlap));

        Assert.IsTrue(set.Overlaps(otherEq));
        Assert.IsTrue(set.Overlaps(otherSub));
        Assert.IsTrue(set.Overlaps(otherSup));
        Assert.IsTrue(set.Overlaps(otherOverlap));
        Assert.IsFalse(set.Overlaps(["f"]));

        Assert.IsTrue(set.SetEquals(otherEq));
        Assert.IsFalse(set.SetEquals(otherSub));
        Assert.IsFalse(set.SetEquals(otherSup));
        Assert.IsFalse(set.SetEquals(otherOverlap));
    }

    [TestMethod]
    public void TestBooleanOperations()
    {
        set = new("a", "b", "c");
        string[] otherEq = ["a", "b", "c"];
        string[] otherSup = ["a", "b"]; // For clarity, sup and sub here, refer to set being a superset or a subset of other (not the other way around)
        string[] otherSub = ["a", "b", "c", "d"];
        string[] otherOverlap = ["a", "b", "d"];

        set.ExceptWith(otherSup);
        Assert.IsTrue(set.Contains("c"));
        Assert.AreEqual(1, set.Count);

        set = new(otherEq);
        set.IntersectWith(otherSup);
        Assert.IsTrue(set.Contains("a"));
        Assert.IsTrue(set.Contains("b"));
        Assert.AreEqual(2, set.Count);

        set = new(otherEq);
        set.SymmetricExceptWith(otherOverlap);
        Assert.IsTrue(set.Contains("c"));
        Assert.IsTrue(set.Contains("d"));
        Assert.AreEqual(2, set.Count);

        set = new(otherEq);
        set.UnionWith(otherOverlap);
        Assert.IsTrue(set.Contains("a"));
        Assert.IsTrue(set.Contains("b"));
        Assert.IsTrue(set.Contains("c"));
        Assert.IsTrue(set.Contains("d"));
        Assert.AreEqual(4, set.Count);
    }
}
