using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stebs5.Tests
{
    public static class Helper
    {
        /// <summary>
        /// Helper which asserts, that two enumerables of generic elements are equal.
        /// The element count and the Equals method of the individual elements are used.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        public static void EnumerableEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual, string message = "Enumerables are not equal")
        {
            Assert.AreEqual(expected.Count(), actual.Count(), $"{message}. Not the same count.");
            IEnumerator<T> e = expected.GetEnumerator(), a = actual.GetEnumerator();
            while(e.MoveNext() && a.MoveNext())
            {
                Assert.AreEqual(e.Current, a.Current, message);
            }
        }

        /// <summary>
        /// Helper which asserts, that two dictionaries of generic elements are equal.
        /// The element count and the Equals method of the individual elements are used.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        public static void DictionaryEqual<K, T>(IDictionary<K, T> expected, IDictionary<K, T> actual, string message = "Dictionaries are not equal")
        {
            Assert.AreEqual(expected.Count(), actual.Count(), $"{message}. Not the same count.");
            foreach (var key in expected.Keys)
            {
                Assert.IsTrue(actual.ContainsKey(key), $"{message}. Actual result does not contain key '{key}'");
                Assert.AreEqual(expected[key], actual[key], message);
            }
        }
    }
}
