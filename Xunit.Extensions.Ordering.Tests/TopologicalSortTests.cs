using System.Collections.Generic;

namespace Xunit.Extensions.Ordering.Tests
{
    public class Sort
    {
        [Fact]
        [Trait("Order", "TopologicalSort")]
        // ReSharper disable once InconsistentNaming
        public void TopologicalSortTest()
        {
            var t = typeof(Xunit.Extensions.Ordering.TestDependencyOrderer);
            var a = new Item("A");
            var b = new Item("B", "C", "E");
            var c = new Item("C");
            var d = new Item("D", "A");
            var e = new Item("E", "D", "G");
            var f = new Item("F");
            var g = new Item("G", "F", "H");
            var h = new Item("H");

            var unsorted = new[] { a, b, c, d, e, f, g, h };
            var expected = new[] { a, c, d, f, h, g, e, b };
            var sorted = unsorted.TSort(x => x.Dependencies, y => y.DisplayName);
            Assert.Equal(expected, sorted);
        }
    }

    public class Item
    {
        public IEnumerable<string> Dependencies { get; private set; }
        public string DisplayName { get; }
        public Item(string name, params string[] dependencies)
        {
            DisplayName = name;
            Dependencies = dependencies;
        }
    }
}
