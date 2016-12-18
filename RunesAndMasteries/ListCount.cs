using System;

namespace RunesAndMasteries {

    /// <summary>
    /// Helper class to help organize the results of the Champion.GG query.</summary>
    public class ListCount : IComparable<ListCount> {

        /// <summary>
        /// The number of times the particular page appears.</summary>
        public int Count { get; set; }

        /// <summary>
        /// The actual list of runes/masteries.</summary>
        public string List { get; set; }

        /// <summary>
        /// Compares the current instance with a specified <c>ListCount</c> object and returns an integer that indicates whether 
        /// the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(ListCount other) {
            if (Count == other.Count) {
                return List.CompareTo(other.List);
            } else {
                return other.Count - Count;
            }
        }
    }
}
