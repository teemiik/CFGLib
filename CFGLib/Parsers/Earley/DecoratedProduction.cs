﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {
	internal class DecoratedProduction {
		public Production Production { get; }
		public int CurrentPosition { get; }

		public Word NextWord {
			get {
				if (CurrentPosition >= Production.Rhs.Count) {
					return null;
				}
				return Production.Rhs[CurrentPosition];
			}
		}

		/// <summary>
		/// a, * b, c, d
		/// returns c, d
		/// </summary>
		public Sentence Tail {
			get {
				var start = CurrentPosition + 1;
				var count = Production.Rhs.Count - start;
				return Production.Rhs.GetRange(start, count);
			}
		}

		public Sentence Prefix {
			get {
				return Production.Rhs.GetRange(0, CurrentPosition - 1);
			}
		}

		/// <summary>
		/// a, * b, c, d
		/// returns b, c, d
		/// </summary>
		public Sentence Suffix {
			get {
				var start = CurrentPosition;
				var count = Production.Rhs.Count - start;
				return Production.Rhs.GetRange(start, count);
			}
		}

		public bool AtEnd {
			get {
				return CurrentPosition == Production.Rhs.Count;
			}
		}

		public DecoratedProduction(Production production, int currentPosition) {
			if (production == null) {
				throw new ArgumentNullException();
			}
			if (currentPosition < 0 || currentPosition > production.Rhs.Count) {
				throw new ArgumentOutOfRangeException();
			}
			Production = production;
			CurrentPosition = currentPosition;
		}

		public static bool operator ==(DecoratedProduction x, DecoratedProduction y) {
			if (ReferenceEquals(x, null)) {
				return ReferenceEquals(y, null);
			}
			return x.Equals(y);
		}
		public static bool operator !=(DecoratedProduction x, DecoratedProduction y) {
			return !(x == y);
		}
		public override bool Equals(object other) {
			var x = this;
			var y = other as DecoratedProduction;
			if (y == null) {
				return false;
			}

			if (x.CurrentPosition != y.CurrentPosition) {
				return false;
			}

			if (x.Production != y.Production) {
				return false;
			}

			return true;
		}

		// based on http://stackoverflow.com/a/263416/2877032
		public override int GetHashCode() {
			unchecked {
				int hash = 17;
				hash = hash * 23 + this.Production.GetHashCode();
				hash = hash * 23 + this.CurrentPosition.GetHashCode();

				return hash;
			}
		}

		public override string ToString() {
			var beforeDot = Production.Rhs.GetRange(0, CurrentPosition);
			var beforeDotString = beforeDot.Count == 0 ? "" : beforeDot.ToString();
			var afterDot = Production.Rhs.GetRange(CurrentPosition, Production.Rhs.Count - CurrentPosition);
			var afterDotString = afterDot.Count == 0 ? "" : afterDot.ToString() + " ";
			if (beforeDotString == "" && afterDotString == "") {
				beforeDotString = "ε";
			}

			// node.Label.Replace('o', '•')
			return string.Format("{0} → {1} • {2}", Production.Lhs, beforeDotString, afterDotString);
		}

		internal DecoratedProduction Increment() {
			return new DecoratedProduction(Production, CurrentPosition + 1);
		}
	}
}
