﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGLib.Parsers.Earley {
	internal class Item {
		public readonly Production Production;

		// where the dot is in relation to the RHS of the production
		public readonly int CurrentPosition;

		// where this production starts matching in relation to the input string
		public int StartPosition;

		// where this production finishes matching in relation to the input string
		// used only in the tree reconstruction phase
		public int EndPosition;

		// TODO: maybe should keep these out of the item and therefore avoid the messy equality stuff
		public readonly HashSet<Pointer> Predecessors;
		public readonly HashSet<Pointer> Reductions;
		
		// public bool Processed = false;

		public Word PrevWord {
			get {
				if (CurrentPosition - 1 < 0) {
					return null;
				}
				if (Production.Rhs.Count == 0) {
					return null;
				}
				return Production.Rhs[CurrentPosition - 1];
			}
		}
		public Word NextWord {
			get {
				if (CurrentPosition >= Production.Rhs.Count) {
					return null;
				}
				return Production.Rhs[CurrentPosition];
			}
		}


		public Item(Production production, int currentPosition, int startPosition, int endPosition) {
			if (currentPosition < 0) {
				throw new ArgumentOutOfRangeException();
			}
			if (startPosition < 0) {
				throw new ArgumentOutOfRangeException();
			}
			Production = production;
			CurrentPosition = currentPosition;
			StartPosition = startPosition;
			EndPosition = endPosition;
			Predecessors = new HashSet<Pointer>();
			Reductions = new HashSet<Pointer>();
		}

		internal string ProductionToString() {
			var beforeDot = Production.Rhs.GetRange(0, CurrentPosition);
			var beforeDotString = beforeDot.Count == 0 ? "" : beforeDot.ToString();
			var afterDot = Production.Rhs.GetRange(CurrentPosition, Production.Rhs.Count - CurrentPosition);
			var afterDotString = afterDot.Count == 0 ? "" : afterDot.ToString() + " ";
			if (beforeDotString == "" && afterDotString == "") {
				beforeDotString = "ε";
			}

			return string.Format("{0} → {1} o {2}", Production.Lhs, beforeDotString, afterDotString);
		}

		public override string ToString() {
			return string.Format("{0}[{1}] ({2}--{3})", this.ProductionToString(), Production.Weight, StartPosition, EndPosition);
		}

		internal Item Increment() {
			var copy = new Item(Production, CurrentPosition + 1, StartPosition, EndPosition);
			return copy;
		}
		
		internal Item Decrement() {
			var copy = new Item(Production, CurrentPosition - 1, StartPosition, EndPosition);
			return copy;
		}

		internal bool IsComplete() {
			return this.NextWord == null;
		}

		internal void AddPredecessor(int label, Item item) {
			var predp = new Pointer(label, item);
			Predecessors.Add(predp);
		}

		internal void AddReduction(int label, Item item) {
			var reductionp = new Pointer(label, item);
			Reductions.Add(reductionp);
		}
	}

	internal class ItemComparer : IEqualityComparer<Item> {
		public bool Equals(Item x, Item y) {
			if (x == null && y == null) {
				return true;
			}
			if (x == null) {
				return false;
			}
			if (y == null) {
				return false;
			}
			
			if (x.Production != y.Production) {
				return false;
			}
			if (x.CurrentPosition != y.CurrentPosition) {
				return false;
			}
			if (x.StartPosition != y.StartPosition) {
				return false;
			}
			if (x.EndPosition != y.EndPosition) {
				return false;
			}

			return true;
		}

		public int GetHashCode(Item obj) {
			//return new {
			//	obj.Production,
			//	obj.CurrentPosition,
			//	obj.StartPosition,
			//	obj.EndPosition,
			//}.GetHashCode();

			// based on http://stackoverflow.com/a/263416/2877032
			unchecked {
				int hash = 17;
				hash = hash * 23 + obj.Production.GetHashCode();
				hash = hash * 23 + obj.CurrentPosition.GetHashCode();
				hash = hash * 23 + obj.StartPosition.GetHashCode();
				return hash;
			}
		}
	}
}
